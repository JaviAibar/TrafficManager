using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(BezierWalkerWithSpeed))]
public abstract class RoadUser : MonoBehaviour
{
    [Min(0)] public float timeOffset = 0;
    [Min(0)] public float timeToLoop = 10;
    public float timer = 0;
    protected BezierWalkerWithSpeed bezier;
    public float speed = 10;
    public float runningSpeed = 15; // This is used when the vehicles are crossing in yellow or when a pedestrian is stuck in green at a crossroad
    public bool respectsTheRules = true;
    public TrafficLightController trafficLight;
    public TrafficArea trafficArea;

    protected virtual void Awake()
    {
        bezier = GetComponentInParent<BezierWalkerWithSpeed>();
        if (bezier == null)
            throw new System.Exception("Root of " + name + " needs a Bezier Walker With Speed component");
    }

    protected virtual void Start()
    {
        bezier.speed = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeOffset && timer <= timeOffset + 0.5f)
        {
            bezier.speed = speed;
        } else if (timer >= timeToLoop && timer <= timeToLoop + 0.5f)
        {
            bezier.NormalizedT = 0;
            timer = 0;
        }
    }

    void OnEnable()
    {
        EventManager.OnTrafficLightChanged += TrafficLightChanged;
    }

    void OnDisable()
    {
        EventManager.OnTrafficLightChanged -= TrafficLightChanged;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameEngine.instance.Print("Car crash with " + collision.transform.name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        trafficArea = other.GetComponent<TrafficArea>();
        trafficLight = trafficArea.GetTrafficLight();
        CheckMovingConditions();
        GameEngine.instance.Print("Trigger enter 2D de " + name + " con " + other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        bool stillColliding = Physics2D.CircleCast(transform.position, 0.02f, Vector2.right);

        if (stillColliding) return;
        trafficLight = null;
        trafficArea = null;
    }


    public void TrafficLightChanged(TrafficLightController.TrafficLightColour colour)
    {
        GameEngine.instance.Print("Traffic light changed to " + colour);
        if (trafficArea != null && trafficLight != null)
            CheckMovingConditions();
    }

    public abstract void CheckMovingConditions();

}
