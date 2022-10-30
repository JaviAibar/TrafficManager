using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(BezierWalkerWithSpeed))]
[System.Serializable]
public abstract class RoadUser : MonoBehaviour
{
    [Min(0)] public float timeOffset = 0;
    [Min(0)] public float timeToLoop = 10;
    public float timer = 0;
    public BezierWalkerWithSpeed bezier;
    protected Rigidbody2D rb;
    public float normalSpeed = 10;
    public float runningSpeed = 15; // This is used when the vehicles are crossing in yellow or when a pedestrian is stuck in green at a crossroad
    public bool respectsTheRules = true;
   /* [HideInInspector] */public TrafficLightController trafficLight;
   /* [HideInInspector] */public TrafficArea trafficArea;
    public float baseSpeed; // This value is used when the GameSpeed is changed, because the car could be stopped while running for instance
    public bool canMove;
    protected virtual void Awake()
    {
        bezier = GetComponentInParent<BezierWalkerWithSpeed>();
        rb = GetComponentInParent<Rigidbody2D>();
        if (bezier == null)
            throw new System.Exception("Root of " + name + " needs a Bezier Walker With Speed component");
    }

    protected virtual void Start()
    {
        LoopEnded();
    }

    private void Update()
    {
        timer += Time.deltaTime * (int)GameEngine.instance.GetGameSpeed();
        if (timer >= timeOffset && timer <= timeOffset + 0.1f) // Start moving after timeOffset time
        {
            canMove = true;
            bezier.speed = normalSpeed * (int)GameEngine.instance.GetGameSpeed();
            baseSpeed = normalSpeed;
        }
        if (timeToLoop > 0 && timer >= timeToLoop && timer <= timeToLoop + 0.5f)
        {
            LoopEnded();
        }
    }

    void OnEnable()
    {
        EventManager.OnTrafficLightChanged += TrafficLightChanged;
        EventManager.OnLoopEnded += LoopEnded;
        EventManager.OnGameSpeedChanged += SpeedChanged;
    }

    void OnDisable()
    {
        EventManager.OnTrafficLightChanged -= TrafficLightChanged;
        EventManager.OnLoopEnded -= LoopEnded;
        EventManager.OnGameSpeedChanged -= SpeedChanged;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EventManager.RaiseOnRoadUserCollision(collision.transform.GetComponent<RoadUser>(), this);
        /*  GameEngine.instance.Print("Car crash with " + collision.transform.name);
          rb.constraints = RigidbodyConstraints2D.None;
          print(Vector2.Angle(collision.contacts[0].normal, transform.up));

          rb.AddForce(collision.contacts[0].normal * 1000);
          bezier.speed = 0;*/
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        trafficArea = other.GetComponent<TrafficArea>();
        trafficLight = trafficArea.GetTrafficLight();
        CheckMovingConditions();
        GameEngine.instance.Print(name + " reaches " + other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RaycastHit2D raycast = Physics2D.Linecast(transform.position, transform.position + transform.up * 0.02f, 1 << 9);

        GameEngine.instance.Print(transform.name + (raycast ?  "":" no")+ " está todavía colisionando con " +(raycast ? raycast.collider.name: ""));
       // Debug.Break();
        if (raycast) return;
        trafficLight = null;
        trafficArea = null;
    }


    public void TrafficLightChanged(TrafficLightController.TrafficLightColour colour)
    {
        GameEngine.instance.Print("Traffic light changed to " + colour);
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

    public void LoopEnded()
    {
        bezier.speed = 0;
        canMove = false;

        timer = 0;
        bezier.NormalizedT = 0;
        baseSpeed = 0;
    }

    public virtual void SpeedChanged(GameEngine.GameSpeed state)
    {
        if (state == GameEngine.GameSpeed.Paused) bezier.speed = 0;
        else bezier.speed = baseSpeed * (int)state;
    }

    public void SetSpline(BezierSpline spline)
    {
        bezier.spline = spline;
    }

    public abstract void CheckMovingConditions();

}
