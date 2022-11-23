using System;
using System.Collections;
using System.Linq;
using BezierSolution;
using UnityEngine;
using static Level.GameEngine;

namespace Level
{
    [RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(BezierWalkerWithSpeed))]
    [System.Serializable]
    public abstract class RoadUser : MonoBehaviour
    {
        public int GameSpeedInt => (int) instance.Speed;

        [Min(0)] public float timeOffset = 0;
        [Min(0)] [SerializeField] private float timeToLoop = 10;
        protected float Epsilon => 0.01f;

        public float TimeToLoop
        {
            get => timeToLoop;
            set
            {
                if (value <= 0)
                    AddOnPathCompleteListener();
                else
                    RemoveOnOnPathCompleteListener();
                timeToLoop = value;
            }
        }

        public float timer = 0;
        [HideInInspector] public BezierWalkerWithSpeed bezier;
        [SerializeField] private BezierSpline spline;

        public BezierSpline Spline
        {
            get => spline;
            set
            {
                spline = value;
                bezier.spline = value;
            }
        }

        protected Rigidbody2D rb;
        public bool respectsTheRules = true;
        protected TrafficLightController trafficLight;
        protected TrafficArea trafficArea;
        
        protected bool hasStartedMoving;
        public bool HasStartedMoving => hasStartedMoving;

        // public bool HasWaitedEnough => timer >= timeOffset && timer <= timeOffset +0.1f/* && !hasStartedMoving*/;
        public bool HasWaitedEnough => timer >= timeOffset && !hasStartedMoving && !looping;
        public bool TimeToRepeat => timer >= timeToLoop && timer <= timeToLoop + 0.5f;
        protected new Collider2D collider;
        protected bool colliding;
        protected Vector3 collisionDirection;
        protected Vector3 endOfCollision;
        private float otherMass;
        public Vector3 UserDir => transform.up;
        private bool looping;
        public bool Looping => looping;
        
        //     Speed variables    //
        public float Acceleration => acceleration;
        public float BaseAcceleration => 0.5f;
        private float acceleration = 1f;
        /// <summary>
        /// Speed in this precise moment
        /// </summary>
        public float ActualSpeed => bezier.speed;

        //private float targetSpeed;
        
        /// <summary>
        /// Speed reached or in process of reaching
        /// </summary>
        public float Speed => baseSpeed;
        protected float
            baseSpeed; // This value is used when the GameSpeed is changed, because the car could be stopped while running for instance

        //public float Speed => bezier.speed;
        public float normalSpeed = 10;

        public float
            runningSpeed =
                15; // This is used when the vehicles are crossing in yellow or when a pedestrian is stuck in green at a crossroad

        public bool IsWalking => baseSpeed == normalSpeed;
        public bool IsRunning => baseSpeed == runningSpeed;
        
        protected bool accelerating;
        public bool Accelerating => accelerating;
        public float TargetSpeed => baseSpeed * GameSpeedInt;
        private float initialSpeed;
        private float accelerationStep = 0;
        private int counterAccelIterations = 0;

        protected virtual void OnEnable()
        {
            AddOnPathCompleteListener();
            EventManager.OnTrafficLightChanged += TrafficLightChanged;
            EventManager.OnLoopStarted += LoopStarted;
            EventManager.OnGameSpeedChanged += GameSpeedChanged;
        }

        protected virtual void OnDisable()
        {
            RemoveOnOnPathCompleteListener();
            EventManager.OnTrafficLightChanged -= TrafficLightChanged;
            EventManager.OnLoopStarted -= LoopStarted;
            EventManager.OnGameSpeedChanged -= GameSpeedChanged;
        }

#if UNITY_EDITOR
        private void OnValidate() => TimeToLoop = timeToLoop;
#endif
        [ContextMenu("Mi prueba")]
        public void MiPrueba()
        {
            ChangeSpeed(normalSpeed);
        }

        [ContextMenu("Printeame")]
        public void PrintMe()
        {
            print($"bezier.speed:{bezier.speed}");
        }

        void AddOnPathCompleteListener()
        {
            bezier?.onPathCompleted.RemoveListener(LoopStarted);
            if (timeToLoop <= 0) bezier?.onPathCompleted.AddListener(LoopStarted);
            bezier?.onPathCompleted.AddListener(PathFinished);
        }

        void RemoveOnOnPathCompleteListener()
        {
            bezier?.onPathCompleted.RemoveListener(LoopStarted);
            bezier?.onPathCompleted.RemoveListener(PathFinished);
        }

        protected virtual void Awake()
        {
            collider = GetComponents<Collider2D>().First(e => !e.isTrigger); // Should only be one
            bezier = (BezierWalkerWithSpeedVariant)gameObject.AddComponent(typeof(BezierWalkerWithSpeedVariant));
            // bezier.spline = Spline ?? FindObjectOfType<BezierSpline>();
            bezier.spline = Spline ? Spline : GetComponent<BezierSpline>();
            rb = GetComponentInParent<Rigidbody2D>();
            if (!bezier)
                throw new Exception($"Root of {name} needs a Bezier Walker With Speed component");
            else if (!spline)
                throw new Exception($"Root of {name}'s Bezier needs a reference to a BezierSpline component");
        }

        protected virtual void Start()
        {
            LoopStarted();
        }

        private void Update()
        {
            timer += Time.deltaTime * GameSpeedInt;
            if (timeToLoop > 0 && TimeToRepeat)
                LoopStarted();
            if (colliding)
                MoveCollision();
            if (HasWaitedEnough) // Start moving after timeOffset
            {
                Print($"[{name}] waited its offset time ({timeOffset} seconds)", VerboseEnum.Speed);
                hasStartedMoving = true;
                collider.enabled = true;
                if (!accelerating) ChangeSpeed(normalSpeed);
            }

            if (accelerating && !looping && instance.IsPlayed)
                Accelerate();
        }

        private void MoveCollision()
        {
            Transform transform1;
            (transform1 = transform).position = Vector3.Lerp(transform.position, endOfCollision, 5 * Time.deltaTime);
            colliding = Vector3.Distance(transform1.position, endOfCollision) >= 2f;
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            Print($"Accident between {name} and {collision.transform.name}", VerboseEnum.Physics);
            EventManager.RaiseOnRoadUserCollision(collision.transform.GetComponent<RoadUser>(), this);
            //  rb.constraints = RigidbodyConstraints2D.None;
            //print($"I'm {name} and crashed with dir  {Vector2.Angle(collision.contacts[0].normal, transform.up)} with a power of {power}");

            // I don't use RigidBody because it offsets the collider and the position
            //rb.AddForce(collision.contacts[0].normal * power);
            collisionDirection = collision.contacts[0].normal;
            otherMass = collision.rigidbody.mass;
            endOfCollision = transform.position + collisionDirection * otherMass;
            bezier.enabled = false;
            colliding = true;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Road"))
            {
                trafficArea = other.GetComponent<TrafficArea>();
                trafficLight = trafficArea.TrafficLight;
                CheckMovingConditions();
            }
            else
            {
                Print(
                    $"[{name}] is not entering OnTriggerEnter because {other.name} is not tagged as Road",
                    VerboseEnum.Physics);
            }
            //Print(name + " reaches " + other.name);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            var position = transform.position;
            RaycastHit2D raycast =
                Physics2D.Linecast(position, position + transform.up * 0.02f, 1 << 9);

            Print(
                $"{transform.name} is {(raycast ? "still" : "no longer")} colliding {(raycast ? $"with {raycast.collider.name}" : "")}",
                VerboseEnum.Physics);
            // Debug.Break();
            if (raycast) return;
            trafficLight = null;
            trafficArea = null;
        }


        public void TrafficLightChanged(TrafficLightController.TrafficLightColour colour)
        {
            Print(
                $"[{name}] Traffic light changed to {colour} has{(trafficArea ? "" : "n't")} trafficArea and has{(trafficLight ? "" : "n't")} trafficLight",
                VerboseEnum.TrafficLightChanges);
            if (trafficArea && trafficLight)
                CheckMovingConditions();
        }

        public void Moving(bool isMoving)
        {
            if (isMoving)
                EventManager.RaiseOnRoadUserMoving(this);
            else
                EventManager.RaiseOnRoadUserStopped(this);
        }

        public virtual void LoopStarted()
        {
            StartCoroutine(LoopStartedCoroutine());
        }

        public virtual IEnumerator LoopStartedCoroutine()
        {
            /*
         * Disabling collider and moving to a different position were intented to fix a visual bug:
         * When there's an accident with a pedestrian, it changes its sprite to ranOver and aparently
         * animator works sooner than Bezier, therefore there are some frames in which the pedestrian recovers in place.
         * 
         * This solution though working, created accidents at Start, because if more than one RoadUsers have TimeOffset
         * different of zero, then all of them have an accident, opted then not to fix this little bug
         * to avoid correcting a bigger one.
         */
            Print(
                $"[{name}] began its looping coroutine (lopping true, collider false, statedMoving false, bezier true, normalizedT 0, baseSpeed 0 bezier.speed 1)",
                VerboseEnum.GameTrace);
            //      collider.enabled = false;
            //      transform.position = InitPos;
            looping = true;
            accelerating = false;
            collider.enabled = false;
            hasStartedMoving = false;
            bezier.enabled = true;
            bezier.NormalizedT = 0;
            baseSpeed = 1;
            /*
         * This speed to 1 and the fact that this method is a coroutine is because in order to not hit anybody while backing
         * to the start, I need to disable the collider, but bezier won't move back to the start until we have a couple of
         * frames moving.
         * 
         * */
            bezier.speed = 1f;
            yield return new WaitWhile(() => bezier.NormalizedT <= 0f);
            // while (bezier.NormalizedT <= 0f)
            // {
            //     yield return null;
            // }
            //
            // if (!accelerating)
            // {
            //     bezier.speed = 0;
            //     baseSpeed = 0;
            // }
            bezier.speed = 0;
            baseSpeed = 0;
            timer = 0;
            looping = false;
            Print($"[{name}] finished looping coroutine (timer 0, looping false)", VerboseEnum.GameTrace);
        }

        /// <summary>
        /// Executed when Game changes speed. Changes to speed are applied instantly
        /// because in a Game speed change makes no sense an acceleration effect
        /// </summary>
        /// <param name="state"></param>
        public virtual void GameSpeedChanged(GameSpeed state)
        {
            Print(
                $"Speed of {name} {(state == GameSpeed.Paused || !accelerating ? $"changed to {(state == GameSpeed.Paused ? "0" : (bezier.speed = baseSpeed * (int)state))} due to GameSpeed changed to {state}" : "didn't change")}",
                VerboseEnum.Speed);
            if (state == GameSpeed.Paused) bezier.speed = 0;
            else if (!accelerating) bezier.speed = baseSpeed * (int)state;
        }

        /// <summary>
        /// Change the speed of the user for the given amount
        /// </summary>
        /// <param name="newSpeed"></param>
        /// <param name="newAcceleration">default is base acceleration</param>
        public void ChangeSpeed(float newSpeed, float newAcceleration=-1)
        {
            Print(
                $"Asked {name} to change its speed to {newSpeed} {(Mathf.Approximately(newSpeed, baseSpeed) ? "but it was already" : $"target was {baseSpeed} and current {ActualSpeed}")} btw its {(accelerating ? "" : "NOT")} accelerating already",
                VerboseEnum.Speed);
            if (!hasStartedMoving) Debug.LogWarning($"[{name}] asked to move before its hasStartedMoving was completed!");
            if (hasStartedMoving && Mathf.Approximately(newSpeed, baseSpeed)) return;
            Moving(newSpeed != 0); // If new speed is diff of 0, then Moving(true)
            baseSpeed = newSpeed;
            accelerating = true;
            acceleration = newAcceleration != -1 ? newAcceleration : BaseAcceleration; 
            initialSpeed = bezier.speed;
            accelerationStep = 0;
            counterAccelIterations = 0;
        }

        /// <summary>
        /// Changes speed of user almost instantly
        /// </summary>
        /// <param name="newSpeed"></param>
        public void ChangeSpeedImmediately(float newSpeed)
        {
            Print($"[{name}] forced to change its speed quickly to {newSpeed}", VerboseEnum.Speed);
            ChangeSpeed(newSpeed, 100);
        }

        protected void PathFinished()
        {
            collider.enabled = false;
            Print($"[{name}] reached finish line (collider disabled)", VerboseEnum.GameTrace);
        }

        public void Accelerate()
        {
            accelerating = Mathf.Abs(bezier.speed - TargetSpeed) > Epsilon;
            if (accelerating)
            {
                var increment = bezier.speed;
                Print(
                    $"[{name}] {(Mathf.Approximately(bezier.speed, initialSpeed) ? "started" : "still")} {(bezier.speed > TargetSpeed ? "de" : "ac")}celerating from {bezier.speed} to {TargetSpeed}",
                    VerboseEnum.Speed);
                counterAccelIterations++;
                accelerationStep += 0.1f; // We multiply with Speed in this moment because if game is stop
                // print($"Increasing {name} speed to { Mathf.Lerp(initialSpeed, targetSpeed, Acceleration * timeAccelerating)} t={ Acceleration * (int)instance.Speed * timeAccelerating} (delta={timeAccelerating} * gameSpeed={instance.Speed} * Accl={Acceleration})");
                bezier.speed = Mathf.Lerp(initialSpeed, TargetSpeed, Acceleration * accelerationStep/* * GameSpeedInt*/) ;
                Print(
                    $"[{name}] acceleration iteration ended up with bezier.speed={bezier.speed}.   {bezier.speed - increment} was added with Lerp(a:{initialSpeed}, b:{TargetSpeed}, t:{Acceleration * accelerationStep * GameSpeedInt}) t = Acceleration:{Acceleration} * accelerationStep:{accelerationStep}  * GameSpeed:{GameSpeedInt}",
                    VerboseEnum.Speed);
            }
            else
            {
                Print(
                    $"Target speed of {name} ({baseSpeed * GameSpeedInt}) reached: current {bezier.speed} in {counterAccelIterations} iterations",
                    VerboseEnum.Speed);
            }
        }

        public abstract void CheckMovingConditions();
    }
}