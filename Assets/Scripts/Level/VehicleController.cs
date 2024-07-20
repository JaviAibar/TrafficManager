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
        [SerializeField] private bool inAnEmergency;
        [SerializeField] private AudioClip motorClip;
        [SerializeField] private AudioClip emergencyClip;
        [SerializeField] private AudioMixerGroup fxMixer;
     
        protected Animator anim;
        protected BoxCollider2D carDetector;
        protected VehicleController vehicleAhead;
        protected bool hadVehicleAhead = false;
        
        private static readonly int EMERGENCY = Animator.StringToHash("Emergency");
        private AudioSource audioSource;
        private int CAR_LAYER;

        protected override void Awake()
        {
            base.Awake();
            InitAudio();
            carDetector = GetComponents<BoxCollider2D>().First(e => e.isTrigger);
            CAR_LAYER = LayerMask.NameToLayer("Car");
            anim = GetComponent<Animator>();
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

        public override void StartLoop()
        {
            base.StartLoop();
            audioSource.volume = 0;
            carDetector.enabled = false; // Temp disabled to avoid unintentional accidents
        }

        public override void CheckMovingConditions()
        {
            if (MovingAgainAfterVehicleAheadConditions)
            {
                Print("Moving again after stopped due to vehicle ahead", VerboseEnum.Speed);
                speedController.ChangeSpeed(NormalSpeed);
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

            if (!collider.TryGetComponent<VehicleController>(out var vehicle))
                Debug.LogWarning($"{collider.name} has LayerMask Car but has no VehicleController");

            if (!CompareDirections(RoadUserDir, vehicle.RoadUserDir)) return;

            Accident(vehicle);
        }

        protected override void OnTriggerExit2D(Collider2D collider)
        {
            base.OnTriggerExit2D(collider);
            if (ColliderOnExitNotRelevant(collider)) return;
            CopySpeedOf(vehicleAhead);
        }

        #region Condition checks
        public bool MustStop() => trafficArea.StopArea && trafficLight.IsRed && trafficArea.SameDirection(RoadUserDir);

        public bool MustRun() =>
            (trafficLight.IsYellow && (trafficArea.IsCenter || trafficArea.SameDirection(RoadUserDir))) ||
            (trafficLight.IsRed && trafficArea.IsCenter);

        public bool MustReduce() => trafficLight.IsRed && !trafficArea.StopArea && trafficArea.SameDirection(RoadUserDir);

        public bool MovingConditions() =>
            trafficArea && RespectsTheRules && !inAnEmergency && !vehicleAhead;
        public bool MovingAgainAfterVehicleAheadConditions => hadVehicleAhead;

        #endregion

        #region Movement methods
        public void Reduce()
        {
            speedController.ChangeSpeed(5f);
        }

        public void Run()
        {
            if (CurrentSpeed == 0) AudioFadeIn();
            speedController.ChangeSpeed(RunningSpeed);
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
            speedController.ChangeSpeed(NormalSpeed);
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
            audioSource.Play();
            float lerpSmooth = 0;
            float initialVolume = audioSource.volume;
            while (lerpSmooth < 1)
            {
                audioSource.volume = Mathf.Lerp(initialVolume, targetVolume, lerpSmooth);
                lerpSmooth += Time.deltaTime;
                yield return null;
            }
        }

        private void InitAudio()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.volume = 0;
            audioSource.outputAudioMixerGroup = fxMixer;
            if (inAnEmergency)
                audioSource.clip = emergencyClip;
            else
                audioSource.clip = motorClip;
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
                  + $"(respectsRules?: {RespectsTheRules} inAnEmergency?: {inAnEmergency} {(trafficArea ? $"has ({trafficArea.name})" : "doesn't have ")}a traffic area)\n"
                  + moreInfo, VerboseEnum.Speed);
        }
    }
}