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
        private int CAR_LAYER;

        protected override void Awake()
        {
            base.Awake();
            InitAudio();
            carDetector = GetComponents<BoxCollider2D>().First(e => e.isTrigger);
            CAR_LAYER = LayerMask.NameToLayer("Car");
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
            carDetector.enabled = false; // Temp disabled to avoid unintentional accidents
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

            PrintInfoCheckMovingConditions();
            if (!MovingConditions()) return; // If moving conditions not fulfilled
            if (MustStop())
                Stop();

            else if (MustRun())
                Run();

            else if (MustReduce())
                Reduce();

            else
                NormalMove();
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            base.OnTriggerEnter2D(collider);
            if (!IsAccident(collider) || !IsCar(collider)) return;

            VehicleController vehicle = collider.GetComponent<VehicleController>();
            if (vehicle == null)
                Debug.LogWarning($"{collider.name} has LayerMask Car but has no VehicleController");

            if (!CompareDirections(RoadUserDir, vehicle.RoadUserDir)) return;

            Accident(vehicle);
        }

        protected override void OnTriggerExit2D(Collider2D collider)
        {
            base.OnTriggerExit2D(collider);
            if (ColliderOnExitNotRelevant(collider)) return;
            CopySpeedOf(vehicleAhead);
            //Debug.Break();
            //print($"Como OTHER era {other.name} hemos puesto vehicleAhead a null? {vehicleAhead} y comprobamos condiciones de movimiento");
            // CheckMovingConditions();
        }

        #region Condition checks
        public bool MustStop() => trafficArea.StopArea && trafficLight.IsRed && trafficArea.SameDirection(RoadUserDir);

        public bool MustRun() =>
            (trafficLight.IsYellow && (trafficArea.IsCenter || trafficArea.SameDirection(RoadUserDir))) ||
            (trafficLight.IsRed && trafficArea.IsCenter);

        public bool MustReduce() => trafficLight.IsRed && !trafficArea.StopArea && trafficArea.SameDirection(RoadUserDir);

        public bool MovingConditions() =>
            trafficArea && respectsTheRules && !inAnEmergency && hasStartedMoving && !vehicleAhead;
        public bool MovingAgainAfterVehicleAheadConditions => hasStartedMoving && hadVehicleAhead;

        #endregion

        #region Movement methods
        public void Reduce()
        {
            speedController.ChangeSpeed(5f);
        }

        public void Run()
        {
            if (CurrentSpeed == 0) AudioFadeIn();
            speedController.ChangeSpeed(runningSpeed);
            Moving(true);
        }

        public void Stop()
        {
            if (CurrentSpeed > 0) AudioFadeOut();
            speedController.ChangeSpeedImmediately(0);
            Moving(false);
        }
        public void NormalMove()
        {
            if (CurrentSpeed == 0) AudioFadeIn();
            speedController.ChangeSpeed(normalSpeed);
            Moving(true);
        }

        public void CopySpeedOf(VehicleController otherVehicle)
        {
            speedController.ChangeSpeed(vehicleAhead.CurrentSpeed, vehicleAhead.Acceleration);
            if (otherVehicle == vehicleAhead)
                vehicleAhead = null;
        }
        #endregion

        #region Audio controls
        private void AudioFadeIn()
        {
            StartCoroutine(AudioFade(0.4f));
        }

        private void AudioFadeOut()
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

        private void InitAudio()
        {
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
        }
        #endregion

        public void Accident(VehicleController vehicle)
        {
            speedController.ChangeSpeedImmediately(0);
            Print($"{name} blocked due to {vehicle.name}", VerboseEnum.Speed);
            vehicleAhead = vehicle;
            hadVehicleAhead = true;
        }

        private bool ColliderOnExitNotRelevant(Collider2D collider) => !vehicleAhead || !IsCar(collider) || !SameVehicleAhead(collider);
        public bool IsAccident(Collider2D collider) => !collider.isTrigger;
        public bool SameVehicleAhead(Collider2D other) => other.GetComponent<VehicleController>() != vehicleAhead;
        public bool IsCar(Collider2D collider) => collider.gameObject.layer == CAR_LAYER;

        private void PrintInfoCheckMovingConditions()
        {
            string moreInfo = (trafficArea
                ? $"After the method the vehicle will be {(MustStop() ? "Stopped" : MustRun() ? "Running" : MustReduce() ? "Reducing" : "at Walking speed")}"
                  + $"\nExplanation:\nIs an stop area ({trafficArea.StopArea}) affecting us (same dir)({trafficArea.SameDirection(RoadUserDir)}) in red ({trafficLight.IsRed})(then must stop)? -> {MustStop()}.\n"
                  + $"Is yellow ({trafficLight.IsYellow}) and middle ({trafficArea.IsCenter}) OR before ({trafficArea.SameDirection(RoadUserDir)}) a cross OR IsRed ({trafficLight.IsRed} and Center ({trafficArea.IsCenter})? -> {MustRun()}.\n"
                  + $"Is red? {trafficLight.IsRed} and is NOT a stop area {!trafficArea.StopArea} and affecting us (same dir) {trafficArea.SameDirection(RoadUserDir)}\n"
                  + $"Otherwise? {!MustStop() && !MustRun() && !MustReduce()}"
                : "");
            Print($"[{name}] [CheckMovingConditions] MovingConditions? {MovingConditions()}: "
                  + $"(hasStartedMoving?: {hasStartedMoving} respectsRules?: {respectsTheRules} inAnEmergency?: {inAnEmergency} {(trafficArea ? $"has ({trafficArea.name})" : "doesn't have ")}a traffic area)\n"
                  + moreInfo, VerboseEnum.Speed);
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
         } */
    }
}