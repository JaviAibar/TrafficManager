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
        #region Variables

        [Min(0)] public float timeOffset = 0;
        [Min(0)] [SerializeField] private float timeToLoop = 10;
        public float timer = 0;
        [HideInInspector] public BezierWalkerWithSpeed bezier;
        [SerializeField] private BezierSpline spline;
        protected Rigidbody2D rb;
        public bool respectsTheRules = true;
        protected TrafficLightController trafficLight;
        protected TrafficArea trafficArea;
        protected bool hasStartedMoving;
        protected new Collider2D collider;
        protected bool colliding;
        protected Vector3 collisionDirection;
        protected Vector3 endOfCollision;
        private float otherMass;
        private bool looping;
        private int ROAD_LAYER;
        [SerializeField] protected SpeedController speedController;

        public float normalSpeed = 10;

        // Used eg. when the vehicles are crossing in yellow or when a pedestrian is stuck in green at a crossroad
        public float runningSpeed = 15;

        #endregion

        #region Accessors

        public int GameSpeedInt => (int)Instance.Speed;

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

        public BezierSpline Spline
        {
            get => spline;
            set
            {
                spline = value;
                bezier.spline = value;
            }
        }

        public bool HasStartedMoving => hasStartedMoving;

        /// <summary>
        /// Is not looping, time offset is over and didn't started yet
        /// </summary>
        public bool CanStartMoving => timer >= timeOffset && !hasStartedMoving && !looping;


        public bool CanMove => timer >= timeOffset && !looping;

        /// <summary>
        /// If time set for the road user to loop is over
        /// </summary>
        public bool TimeToRepeat => timer >= timeToLoop && timer <= timeToLoop + 0.5f;

        public Vector3 RoadUserDir => transform.up;
        public bool Looping => looping;
        public float Acceleration => speedController.Acceleration;

        /// <summary>
        /// Speed in this precise moment
        /// </summary>
        public float CurrentSpeed => bezier.speed;

        /// <summary>
        /// Speed without GameSpeed alterations
        /// Used as a reference when changing game speed
        /// actualSpeed = baseSpeed * GameSpeed
        /// </summary>
        public float BaseSpeed => speedController.BaseSpeed;

        //public float Speed => bezier.speed;
        public bool IsWalking => speedController.BaseSpeed == normalSpeed;
        public bool IsRunning => speedController.BaseSpeed == runningSpeed;
        public bool IsStopped => BaseSpeed == 0;
        public bool IsMoving => !IsStopped;
        public bool Accelerating => speedController.CanAccelerate;

        /// <summary>
        /// If Game is not stopped, road user is not looping and should be accelerating
        /// </summary>
        public bool MustAccelerate => Accelerating && !Looping && Instance.IsRunning;

        private bool IsOnRoad => trafficArea && trafficLight;

        #endregion

        protected virtual void OnEnable()
        {
            AddOnPathCompleteListener();
            EventManager.OnTrafficLightChanged += TrafficLightChanged;
            EventManager.OnLoopStarted += LoopStarted;
            EventManager.OnGameSpeedChanged += GameSpeedChanged;
            bezier.onPathCompleted.AddListener(PathFinished);
        }

        protected virtual void OnDisable()
        {
            RemoveOnOnPathCompleteListener();
            EventManager.OnTrafficLightChanged -= TrafficLightChanged;
            EventManager.OnLoopStarted -= LoopStarted;
            EventManager.OnGameSpeedChanged -= GameSpeedChanged;
            bezier?.onPathCompleted.RemoveListener(PathFinished);
        }

        #region Debug

        [ContextMenu("Change speed to Normal Speed")]
        public void DEBUGChangeSpeedToNormalSpeed()
        {
            ChangeSpeed(normalSpeed);
        }

        [ContextMenu("Check bezier.speed")]
        public void DEBUGCheckBezierSpeed()
        {
            print($"bezier.speed:{bezier.speed}");
        }

        #endregion


        void AddOnPathCompleteListener()
        {
            print("Add  OnPathCompleteListener");
            bezier?.onPathCompleted.RemoveListener(LoopStarted);
            if (timeToLoop <= 0) bezier?.onPathCompleted.AddListener(LoopStarted);
        }

        void RemoveOnOnPathCompleteListener()
        {
            print("Remove OnPathCompleteListener");
            bezier?.onPathCompleted.RemoveListener(LoopStarted);
        }

        protected virtual void Awake()
        {
            collider = GetComponents<Collider2D>().First(e => !e.isTrigger); // Should only be one
            bezier = (BezierWalkerWithSpeedVariant)gameObject.AddComponent(typeof(BezierWalkerWithSpeedVariant));
            // bezier.spline = Spline ?? FindObjectOfType<BezierSpline>();
            bezier.spline = Spline ? Spline : GetComponent<BezierSpline>();
            ROAD_LAYER = LayerMask.NameToLayer("Road");
            rb = GetComponentInParent<Rigidbody2D>();
            speedController = new SpeedController(name, bezier);
            if (!bezier)
                throw new NullReferenceException($"Root of {name} needs a Bezier Walker With Speed component");
            if (!spline)
                throw new NullReferenceException($"Root of {name}'s Bezier needs a reference to a BezierSpline component");
        }

        protected virtual void Start()
        {
            LoopStarted();
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime * GameSpeedInt;
            if (timeToLoop > 0 && TimeToRepeat)
                LoopStarted();
            if (colliding)
                MoveByCollision();
            if (CanStartMoving) // Start moving after timeOffset
                StartMoving();
            if (MustAccelerate)
                speedController.Accelerate();
        }

        public virtual void LoopStarted()
        {
            if (bezier.NormalizedT != 0) PathFinished();
            Print(
                $"[{name}] began its looping coroutine (lopping true, collider false, statedMoving false, bezier true,"+
                "normalizedT 0, baseSpeed 0 bezier.speed 1)", VerboseEnum.GameTrace);
            looping = true; // Flag to lock other actions while looping
            speedController.LoopStarted();
            EnableColliderAndStartedMoving(false);
            RecoverFromCollision();
            timer = 0;
            looping = false;
            Print($"[{name}] finished looping coroutine (timer 0, looping false)", VerboseEnum.GameTrace);
        }
        protected virtual void StartMoving()
        {
            Print($"[{name}] waited its offset time ({timeOffset} seconds)", VerboseEnum.Speed);
            // Accelerating is only true when speedController is given a speed;
            // Normally shouldn't be true, as it (StartMoving) should be called only once, whenever level loops
            // and is responsible of begin moving. It can be, though, for debug or testing purposes
            if (!Accelerating) 
            {
                EnableColliderAndStartedMoving(true);
                ChangeSpeed(normalSpeed);
            }
            else Print($"[{name}] was accelerating before CanStartMoving was true", VerboseEnum.Speed);
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            Print($"Accident between {name} and {collision.transform.name}", VerboseEnum.Physics);
            EventManager.RaiseOnRoadUserCollision(collision.transform.GetComponent<RoadUser>(), this);
            //print($"I'm {name} and crashed with dir  {Vector2.Angle(collision.contacts[0].normal, transform.up)} with a power of {power}");

            // I don't use RigidBody because it offsets the collider and the position
            //rb.constraints = RigidbodyConstraints2D.None;
            //rb.AddForce(collision.contacts[0].normal * power);
            collisionDirection = collision.contacts[0].normal;
            otherMass = collision.rigidbody.mass;
            endOfCollision = transform.position + collisionDirection * otherMass;
            bezier.enabled = false;
            colliding = true;
        }

        protected virtual void OnTriggerEnter2D(Collider2D otherCollider)
        {
            if (otherCollider.gameObject.layer == ROAD_LAYER)
            {
                trafficArea = otherCollider.GetComponent<TrafficArea>();
                trafficLight = trafficArea.TrafficLight;
                CheckMovingConditions();
            }
            else
                Print(
                    $"[{name}] is not entering OnTriggerEnter because {otherCollider.name} is not tagged as Road",
                    VerboseEnum.Physics);
        }


        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (StillOnRoad()) return;
            trafficLight = null;
            trafficArea = null;
        }

        private void MoveByCollision()
        {
            Transform transform1;
            (transform1 = transform).position =
                Vector3.Lerp(transform.position, endOfCollision, 5 * Time.fixedDeltaTime);
            colliding = Vector3.Distance(transform1.position, endOfCollision) >= 2f;
        }

        public void TrafficLightChanged(TrafficLightController.TrafficLightColour colour)
        {
            Print($"[{name}] Traffic light changed to {colour} has{(trafficArea ? "" : "n't")} " +
                  $"trafficArea and has{(trafficLight ? "" : "n't")} trafficLight", VerboseEnum.TrafficLightChanges);
            if (IsOnRoad)
                CheckMovingConditions();
        }

        public void Moving(bool isMoving)
        {
            if (isMoving)
                EventManager.RaiseOnRoadUserMoving(this);
            else
                EventManager.RaiseOnRoadUserStopped(this);
        }

        protected void PathFinished()
        {
            collider.enabled = false;
            bezier.NormalizedT = 0;
            bezier.SetAtStart();
            Print($"[{name}] reached finish line (collider disabled)", VerboseEnum.GameTrace);
            ChangeSpeedImmediately(0);
        }
        
        /// <summary>
        /// Changes speed to value given with acceleration specified (recommended between 0.01 and 2)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="acceleration"></param>
        public void ChangeSpeed(float value, float? acceleration = null)
        {
            //bool worthCallMoving = SwitchStopped(value);
            bool? moving = speedController.ChangeSpeed(value, acceleration);
            // if (worthCallMoving && moving != null) Moving(moving.Value);
        }

        /// <summary>
        /// Changes speed to value given with acceleration specified (recommended between 0.01 and 2)
        /// </summary>
        /// <param name="value"></param>
        public void ChangeSpeedImmediately(float value)
        {
            //  bool worthCallMoving = SwitchStopped(value);

            bool? moving = speedController.ChangeSpeedImmediately(value);
            //if (worthCallMoving && moving != null) Moving(moving.Value);
        }

        /// <summary>
        /// Executed when Game changes speed. Changes to speed are applied instantly
        /// because in a Game speed change makes no sense an acceleration effect
        /// </summary>
        /// <param name="state"></param>
        public virtual void GameSpeedChanged(GameSpeed state)
        {
            speedController.GameSpeedChanged(state);
        }

        public bool SwitchStopped(float newSpeed) => newSpeed == 0 && IsMoving || IsStopped && newSpeed > 0;

        public void EnableColliderAndStartedMoving(bool value)
        {
            hasStartedMoving = value;
            collider.enabled = value;
        }

        public void RecoverFromCollision()
        {
            bezier.enabled = true;
        }

        private bool StillOnRoad()
        {
            var position = transform.position;
            RaycastHit2D raycast =
                Physics2D.Linecast(position, position + transform.up * 0.02f, 1 << 9);

            Print(
                $"{transform.name} is {(raycast ? "still" : "no longer")} colliding" + 
                $"{(raycast ? $"with {raycast.collider.name}" : "")}", VerboseEnum.Physics);
            return raycast;
        }

        public abstract void CheckMovingConditions();


        /* public virtual IEnumerator LoopStartedCoroutine()
         {
             /*
              * Disabling collider and moving to a different position were intended to fix a visual bug:
              * When there's an accident with a pedestrian, it changes its sprite to ranOver and apparently
              * animator works sooner than Bezier, therefore there are some frames in which the pedestrian recovers in place.
              * 
              * This solution though working, created accidents at Start, because if more than one RoadUsers have TimeOffset
              * different of zero, then all of them have an accident, opted then not to fix this little bug
              * to avoid correcting a bigger one.
              * 
              * transform.position = InitPosition;
              * collider.enabled = false;
              */


        //  }

        /*
        #if UNITY_EDITOR
                private void OnValidate() => TimeToLoop = timeToLoop;
        #endif
        */
    }
}