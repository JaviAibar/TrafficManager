using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;
using System;
using System.Linq;
using static GameEngine;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(BezierWalkerWithSpeed))]
[System.Serializable]
public abstract class RoadUser : MonoBehaviour
{
    protected Vector2 InitPos => new Vector2(1000, 1000);
    [SerializeField] protected float Acceleration = 5f;
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
    public float Speed => bezier.speed;
    public BezierSpline Spline { get => spline; set { spline = value; bezier.spline = value; } }
    protected Rigidbody2D rb;
    public float normalSpeed = 10;
    public float runningSpeed = 15; // This is used when the vehicles are crossing in yellow or when a pedestrian is stuck in green at a crossroad
    public bool IsWalking => baseSpeed == normalSpeed;
    public bool IsRunning => baseSpeed == runningSpeed;
    public bool respectsTheRules = true;
    protected TrafficLightController trafficLight;
    protected TrafficArea trafficArea;
    protected float baseSpeed; // This value is used when the GameSpeed is changed, because the car could be stopped while running for instance
    protected bool hasStartedMoving;
    public bool HasStartedMoving => hasStartedMoving;
    public bool HasWaitedEnough => timer >= timeOffset && timer <= timeOffset + 0.1f;
    public bool TimeToRepeat => timer >= timeToLoop && timer <= timeToLoop + 0.5f;
    protected new Collider2D collider;
    protected bool colliding;
    protected Vector3 collisionDirection;
    protected Vector3 endOfCollision;
    private float otherMass;
    protected bool accelerating;
    public bool Accelerating => accelerating;
    public Vector3 UserDir => transform.up;

    void OnEnable()
    {
        AddOnPathCompleteListener();
        EventManager.OnTrafficLightChanged += TrafficLightChanged;
        EventManager.OnLoopStarted += LoopStarted;
        EventManager.OnGameSpeedChanged += GameSpeedChanged;
    }

    void OnDisable()
    {
        RemoveOnOnPathCompleteListener();
        EventManager.OnTrafficLightChanged -= TrafficLightChanged;
        EventManager.OnLoopStarted -= LoopStarted;
        EventManager.OnGameSpeedChanged -= GameSpeedChanged;
    }

#if UNITY_EDITOR
    private void OnValidate() => TimeToLoop = timeToLoop;
#endif

    void AddOnPathCompleteListener()
    {
        bezier?.onPathCompleted.RemoveListener(LoopStarted);
        if (timeToLoop <= 0) bezier?.onPathCompleted.AddListener(LoopStarted);
    }

    void RemoveOnOnPathCompleteListener() => bezier?.onPathCompleted.RemoveListener(LoopStarted);

    protected virtual void Awake()
    {
        collider = GetComponent<Collider2D>();
        bezier = (BezierWalkerWithSpeedVariant)gameObject.AddComponent(typeof(BezierWalkerWithSpeedVariant));
        // bezier.spline = Spline ?? FindObjectOfType<BezierSpline>();
        bezier.spline = Spline ?? GetComponent<BezierSpline>();
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
        timer += Time.deltaTime * (int)GameEngine.instance.Speed;
        if (timeToLoop > 0 && TimeToRepeat)
            LoopStarted();
        if (colliding)
            MoveCollision();
        if (HasWaitedEnough) // Start moving after timeOffset time (limited to a couple of frames)
        {
            hasStartedMoving = true;
            collider.enabled = true;
            ChangeSpeed(normalSpeed);
        }
        if (accelerating)
            Accelerate();
    }

    private void MoveCollision()
    {
        transform.position = Vector3.Lerp(transform.position, endOfCollision, 5 * Time.deltaTime);
        colliding = Vector3.Distance(transform.position, endOfCollision) >= 2f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Print($"Accident between {name} and {collision.transform.name}");
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        trafficArea = other.GetComponent<TrafficArea>();
        trafficLight = trafficArea.TrafficLight;
        CheckMovingConditions();
        //Print(name + " reaches " + other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RaycastHit2D raycast = Physics2D.Linecast(transform.position, transform.position + transform.up * 0.02f, 1 << 9);

        Print($"{transform.name} {(raycast ? "" : " ya no")} está {(raycast ? "todavía" : "")} colisionando {(raycast ? "con " + raycast.collider.name : "")}");
        // Debug.Break();
        if (raycast) return;
        trafficLight = null;
        trafficArea = null;
    }


    public void TrafficLightChanged(TrafficLightController.TrafficLightColour colour)
    {
        Print("Traffic light changed to " + colour);
        if (trafficArea != null && trafficLight != null)
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
        //      collider.enabled = false;
        //      transform.position = InitPos;
        collider.enabled = false;
        hasStartedMoving = false;
        bezier.enabled = true;
        bezier.NormalizedT = 0;

        /*
         * This speed to 1 and the fact that this method is a coroutine is because in order to not hit anybody while backing
         * to the start, I need to disable the collider, but bezier won't move back to the start until we have a couple of
         * frames moving.
         * 
         * */
        bezier.speed = 1f;
        yield return new WaitWhile(() => bezier.NormalizedT <= 0f);
        accelerating = false;
        bezier.speed = 0;
        baseSpeed = 0;
        timer = 0;
    }

    /*
     * Executed when Game changes speed. Changes to speed are applied instantly
     * because in a Game speed change makes no sense an acceleration effect
     */
    public virtual void GameSpeedChanged(GameSpeed state)
    {
        if (state == GameSpeed.Paused) bezier.speed = 0;
        else if (!accelerating) bezier.speed = baseSpeed * (int)state;
    }

    public void ChangeSpeed(float newSpeed)
    {
       if (newSpeed != baseSpeed/* && !accelerating*/)
        {
            Moving(newSpeed != 0); // If new speed is diff of 0, then Moving(true)
            baseSpeed = newSpeed;
            accelerating = true;
            initialSpeed = bezier.speed;
            timeAccelerating = 0;
            counterAccelIterations = 0;
            targetSpeed = newSpeed;
         }
    }
    private float initialSpeed;
    private float timeAccelerating = 0;
    private int counterAccelIterations = 0;
    private float targetSpeed;
    public void Accelerate()
    {
        accelerating = Mathf.Abs(bezier.speed - targetSpeed) > Double.Epsilon;
        if (accelerating)
        {
            counterAccelIterations++;
            timeAccelerating += /*Time.deltaTime*/ 0.1f * (int)instance.Speed; // We multiply with Speed in this moment because if game is stop
                                                                               // print($"Increasing {name} speed to { Mathf.Lerp(initialSpeed, targetSpeed, Acceleration * timeAccelerating)} t={ Acceleration * (int)instance.Speed * timeAccelerating} (delta={timeAccelerating} * gameSpeed={instance.Speed} * Accl={Acceleration})");
            bezier.speed = Mathf.Lerp(initialSpeed, targetSpeed, Acceleration * timeAccelerating) * (int)instance.Speed;
        }
        else
        {
            //      Print($"Velocidad objetivo de {name} ({targetSpeed * (int)instance.Speed}) alcanzada: actual {bezier.speed} in {counterAccelIterations} iterations");
        }
    }

    public abstract void CheckMovingConditions();

}
