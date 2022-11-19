using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;
using System;
using System.Linq;
using static GameEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(BezierWalkerWithSpeed))]
[System.Serializable]
public abstract class RoadUser : MonoBehaviour
{
    public AnimationCurve curve = new AnimationCurve();
    protected Vector2 InitPos => new Vector2(1000, 1000);
    [SerializeField] protected float Acceleration = 5f;
    [Min(0)] public float timeOffset = 0;
    [Min(0)] [SerializeField] private float timeToLoop = 10;
    protected float Epsilon => 0.1f;
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
    protected Collider2D collider;
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
            //Debug.LogWarning($"{name}'s Bezier is taking a random reference to a BezierSpline component");
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
        if (HasWaitedEnough) // Start moving after timeOffset time
        {
            hasStartedMoving = true;
            /*bezier.speed = */StartCoroutine(Accelerate(normalSpeed));//normalSpeed * (int)GameEngine.instance.Speed;
            baseSpeed = normalSpeed;
            // collider.enabled = true;
        }
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
        //        Debug.Break();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //   print($"trigger culpable {other.name}");
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
        //  print("Semaforo culpable");
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
        hasStartedMoving = false;
        // ResetSpline();
        bezier.enabled = true;
        bezier.NormalizedT = 0;
        bezier.speed = 0;

        timer = 0;
        baseSpeed = 0;
    }

    public virtual void GameSpeedChanged(GameSpeed state)
    {
        if (state == GameSpeed.Paused) bezier.speed = 0;
        else if (!accelerating) bezier.speed = baseSpeed * (int)state;
    }

    
    /* public void ResetSpline()
     {
         bezier.spline = spline;
     }*/

    public IEnumerator Accelerate(float newSpeed)
    {
        accelerating = true;
        print($"{name} Accelerating from {bezier.speed} to {newSpeed} with an acceleration of {Acceleration} and game speed of {instance.Speed} ({(int)instance.Speed})");
        //curve.AddKey(Time.realtimeSinceStartup, 0);
        float initialSpeed = bezier.speed;
        float time = 0;
        int counter = 0;
        while (Mathf.Abs(bezier.speed - newSpeed) > Epsilon)
        {
            counter++;
            time += Time.deltaTime;
           // print($"Increasing {name} speed to {Mathf.Lerp(bezier.speed, newSpeed, Acceleration * (int)instance.Speed * time)} t={ Acceleration * (int)instance.Speed * time}");
            bezier.speed = Mathf.Lerp(initialSpeed, newSpeed, Acceleration * (int)instance.Speed * time);
            yield return null;
        }
        Print($"Velocidad objetivo de {name} ({newSpeed}) alcanzada: actual {bezier.speed} in {counter} iterations");
        accelerating = false;
    }

    public abstract void CheckMovingConditions();

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, endOfCollision);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, sizeOfSphere);
        // Gizmos.color = Color.magenta;
        // Gizmos.DrawLine(origin, destinys);
        // Gizmos.color = Color.cyan;
        //normals?.ForEach(e => Gizmos.DrawLine(transform.position, e*));
        // Gizmos.DrawLine(origin+Vector3.up, Vector3.Lerp(origin, destinys+Vector3.up, 5));
    }*/
}
