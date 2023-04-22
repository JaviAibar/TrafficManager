using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using static Level.GameEngine;
using Random = UnityEngine.Random;

namespace Level
{
    [System.Serializable]
    public class VehicleController : RoadUser
    {
        public bool inAnEmergency;
        protected Animator anim;
        protected BoxCollider2D carDetector;
        protected VehicleController vehicleAhead;
        protected bool hadVehicleAhead = false;
        private static readonly int EMERGENCY = Animator.StringToHash("Emergency");
        private AudioSource audio;
        public AudioClip motorClip;
        public AudioClip emergencyClip;
        public AudioMixerGroup fxMixer;
        private int CAR;

        protected override void Awake()
        {
            base.Awake();
            audio = gameObject.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.loop = true;
            audio.pitch = Random.Range(0.8f, 1.2f);
            audio.volume = 0;
            audio.outputAudioMixerGroup = fxMixer;
            if (inAnEmergency)
                audio.clip = emergencyClip;
            else
                audio.clip = motorClip;
            carDetector = GetComponents<BoxCollider2D>().First(e => e.isTrigger);
            CAR = LayerMask.NameToLayer("Car");
            anim = GetComponent<Animator>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            // EventManager.OnRoadUserMoving += RoadUserMoving;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            //   EventManager.OnRoadUserMoving -= RoadUserMoving;
        }

        /* private void RoadUserMoving(RoadUser responsible)
         {
             Print($"Vehicle moving again is {responsible}", VerboseEnum.GameTrace);
             if (vehicleAhead && vehicleAhead.GetInstanceID() == responsible.GetInstanceID())
             {
                 Print($"The vehicle ahead of {name} is moving again, then {name} can also start moving",
                     VerboseEnum.SolutionConditions);
                 speedController.ChangeSpeed(vehicleAhead.BaseSpeed, vehicleAhead.Acceleration);
                 vehicleAhead = null;
                 // CheckMovingConditions();
             }
         }
 */
        protected override void Start()
        {
            base.Start();
            if (inAnEmergency) anim.SetTrigger(EMERGENCY);
        }

        protected override void StartMoving()
        {
            base.StartMoving();
            AudioFadeIn();
            carDetector.enabled = true;
        }

        public override void LoopStarted()
        {
            base.LoopStarted();
            audio.volume = 0;
            carDetector.enabled = false;
        }

        public override void CheckMovingConditions()
        {
            if (MovingAgainAfterVehicleAheadConditions)
            {
                Print("Moving again after stopped due to vehicle ahead", VerboseEnum.Speed);
                speedController.ChangeSpeed(normalSpeed);
                Moving(true);
                hadVehicleAhead = false;
                return;
            }

            string moreInfo = (trafficArea
                ? $"After the method the vehicle will be {(MustStop() ? "Stopped" : MustRun() ? "Running" : MustReduce() ? "Reducing" : "at Walking speed")}"
                  + $"\nExplanation:\nIs an stop area ({trafficArea.StopArea}) affecting us (same dir)({trafficArea.SameDirection(UserDir)}) in red ({trafficLight.IsRed})(then must stop)? -> {MustStop()}.\n"
                  + $"Is yellow ({trafficLight.IsYellow}) and middle ({trafficArea.IsCenter}) OR before ({trafficArea.SameDirection(UserDir)}) a cross OR IsRed ({trafficLight.IsRed} and Center ({trafficArea.IsCenter})? -> {MustRun()}.\n"
                  + $"Is red? {trafficLight.IsRed} and is NOT a stop area {!trafficArea.StopArea} and affecting us (same dir) {trafficArea.SameDirection(UserDir)}\n"
                  + $"Otherwise? {!MustStop() && !MustRun() && !MustReduce()}"
                : "");
            Print($"[{name}] [CheckMovingConditions] MovingConditions? {MovingConditions()}: "
                  + $"(hasStartedMoving?: {hasStartedMoving} respectsRules?: {respectsTheRules} inAnEmergency?: {inAnEmergency} {(trafficArea ? $"has ({trafficArea.name})" : "doesn't have ")}a traffic area)\n"
                  + moreInfo, VerboseEnum.Speed);
            if (!MovingConditions()) return; // If moving conditions not fulfilled
            if (MustStop())
            {
                // bool worthCallMoving = SwitchStopped(0);
                if (CurrentSpeed > 0) AudioFadeOut();

                speedController.ChangeSpeedImmediately(0);
                /*if (worthCallMoving) */
                Moving(false);
            }
            // Yellow and before or middle cross, then boost
            // or Red but in middle cross, then boost
            else if (MustRun())
            {
                if (CurrentSpeed == 0) AudioFadeIn();
                //bool worthCallMoving = SwitchStopped(runningSpeed);
                speedController.ChangeSpeed(runningSpeed);
                /*if (worthCallMoving)*/
                Moving(true);
            }
            else if (MustReduce())
            {
                Reduce();
            }
            else
            {
                if (CurrentSpeed == 0) AudioFadeIn();

                //bool worthCallMoving = SwitchStopped(normalSpeed);
                speedController.ChangeSpeed(normalSpeed);
                /*if (worthCallMoving) */
                Moving(true);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            // print($"[{collision.name}]");
            if (collision.isTrigger || collision.gameObject.layer != /*LayerMask.NameToLayer("Car")*/ CAR) return;
            Print($"[{name}] Detected other car ahead: {collision.name}", VerboseEnum.Physics);
            VehicleController vehicle = collision.GetComponent<VehicleController>();
            if (vehicle == null) Debug.LogWarning($"{collision.name} has LayerMask Car but has no VehicleController");
            if (Vector3ToDirection(UserDir) != Vector3ToDirection(vehicle.UserDir)) return;
            //bezier.speed = 0;
            //Moving(false);
            speedController.ChangeSpeedImmediately(0);
            /* Acceleration = vehicle.Acceleration;
                ChangeSpeed(vehicle.Speed);*/
            Print($"{name} blocked due to {collision.name}", VerboseEnum.Speed);
            vehicleAhead = vehicle;
            hadVehicleAhead = true;
            //Debug.Break();
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            base.OnTriggerExit2D(other);
            if (!vehicleAhead) return;
            if (other.gameObject.layer != CAR || other.GetComponent<VehicleController>() != vehicleAhead) return;
            speedController.ChangeSpeed(vehicleAhead.CurrentSpeed, vehicleAhead.Acceleration);
            vehicleAhead = null;
            //Debug.Break();
            //print($"Como OTHER era {other.name} hemos puesto vehicleAhead a null? {vehicleAhead} y comprobamos condiciones de movimiento");
            // CheckMovingConditions();
        }

        public bool MustStop() => trafficArea.StopArea && trafficLight.IsRed && trafficArea.SameDirection(UserDir);

        public bool MustRun() =>
            (trafficLight.IsYellow && (trafficArea.IsCenter || trafficArea.SameDirection(UserDir))) ||
            (trafficLight.IsRed && trafficArea.IsCenter);

        public bool MustReduce() => trafficLight.IsRed && !trafficArea.StopArea && trafficArea.SameDirection(UserDir);

        public bool MovingConditions() =>
            trafficArea && respectsTheRules && !inAnEmergency && hasStartedMoving && !vehicleAhead;

        public bool MovingAgainAfterVehicleAheadConditions => hasStartedMoving && hadVehicleAhead;

        public void Reduce()
        {
            speedController.ChangeSpeed(5f);
        }

        public void AudioFadeIn()
        {
            StartCoroutine(AudioFade(0.4f));
        }


        public void AudioFadeOut()
        {
            StartCoroutine(AudioFade(0));
        }


        private IEnumerator AudioFade(float targetVolume)
        {
            audio.Play();
            float lerpSmooth = 0;
            float initialVolume = audio.volume;
          //  print($"[{name}] Queremos cambiar vol de {initialVolume} a {targetVolume}");
            while (lerpSmooth < 1)
            {
              //  print($"lerpSmooth: {lerpSmooth}");
                audio.volume = Mathf.Lerp(initialVolume, targetVolume, lerpSmooth);
                lerpSmooth += Time.deltaTime;
                yield return null;
            }
        }
    }
}