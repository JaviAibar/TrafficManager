using System;
using System.Linq;
using UnityEngine;
using static Level.GameEngine;

namespace Level
{
    [System.Serializable]
    public class VehicleController : RoadUser
    {
        public bool inAnEmergency;
        protected Animator anim;
        protected BoxCollider2D carDetector;
        protected VehicleController vehicleAhead;
        private static readonly int EMERGENCY = Animator.StringToHash("Emergency");

        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EventManager.OnRoadUserMoving += RoadUserMoving;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventManager.OnRoadUserMoving -= RoadUserMoving;
        }

        private void RoadUserMoving(RoadUser responsible)
        {
            if (vehicleAhead?.GetInstanceID() == responsible.GetInstanceID())
            {
                Print($"The vehicle ahead of {name} is moving again, then {name} can also start moving",
                    VerboseEnum.SolutionConditions);
                ChangeSpeed(vehicleAhead.Speed, vehicleAhead.Acceleration);
                vehicleAhead = null;
                // CheckMovingConditions();
            }
        }

        protected override void Start()
        {
            base.Start();
            carDetector = GetComponents<BoxCollider2D>().First(e => e.isTrigger);
            if (inAnEmergency) anim.SetTrigger(EMERGENCY);
        }

        public override void CheckMovingConditions()
        {
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
                ChangeSpeed(0);
            }
            // Yellow and before or middle cross, then boost
            // or Red but in middle cross, then boost
            else if (MustRun())
            {
                ChangeSpeed(runningSpeed);
            }
            else if (MustReduce())
            {
                Reduce();
            }
            else
            {
                ChangeSpeed(normalSpeed);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            if (collision.isTrigger || collision.gameObject.layer != LayerMask.NameToLayer("Car")) return;
            else Print($"[{name}] Detected other car ahead: {collision.name}", VerboseEnum.Physics);
            VehicleController vehicle = collision.GetComponent<VehicleController>();
            if (vehicle == null) Debug.LogWarning($"{collision.name} has LayerMask Car but has no VehicleController");
            if (Vector3ToDirection(UserDir) != Vector3ToDirection(vehicle.UserDir)) return;
            //bezier.speed = 0;
            //Moving(false);
            ChangeSpeedImmediately(0);
            /* Acceleration = vehicle.Acceleration;
                ChangeSpeed(vehicle.Speed);*/
            Print($"{name} blocked due to {collision.name}", VerboseEnum.Speed);
            vehicleAhead = vehicle;
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            base.OnTriggerExit2D(other);
            if (other != vehicleAhead) return;
            vehicleAhead = null;
            CheckMovingConditions();
        }

        public bool MustStop() => trafficArea.StopArea && trafficLight.IsRed && trafficArea.SameDirection(UserDir);
        public bool MustRun() => (trafficLight.IsYellow && (trafficArea.IsCenter || trafficArea.SameDirection(UserDir)))|| 
                                 (trafficLight.IsRed && trafficArea.IsCenter);
        public bool MustReduce() => trafficLight.IsRed && !trafficArea.StopArea && trafficArea.SameDirection(UserDir);
        public bool MovingConditions() => trafficArea && respectsTheRules && !inAnEmergency && hasStartedMoving && !vehicleAhead;

        public void Reduce()
        {
            ChangeSpeed(5f, 0.03f);
        }
    }
}