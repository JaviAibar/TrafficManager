using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(BezierWalkerWithSpeed))]
public abstract class RoadUser : MonoBehaviour
{
    protected const float stopSpeed = 0.00000000000000000000001f;
    protected BezierWalkerWithSpeed bezier;
    public float speed = 10;
    public float speedBoost = 15; // This is used when the vehicles are crossing in yellow or when a pedestrian is stuck in red at a crossroad
    public bool respectsTheRules = true;
    protected TrafficLightController trafficLight;
    protected TrafficArea trafficArea;

    private void Awake()
    {
        bezier = GetComponentInParent<BezierWalkerWithSpeed>();
        if (bezier == null)
            throw new System.Exception("Root of " + name + " needs a Bezier Walker With Speed component");
    }

    private void Start()
    {
        bezier.speed = speed;
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
        print("Car crash with "+collision.transform.name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        trafficArea = other.GetComponent<TrafficArea>();
        trafficLight = trafficArea.GetTrafficLight();
        CheckMovingConditions();
        print("Trigger enter 2D de " + name + " con "+other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        trafficLight = null;
        trafficArea = null;
    }
    public void TrafficLightChanged(TrafficLightController.TrafficLightColour colour)
    {
        if (trafficArea != null && trafficLight != null)
            CheckMovingConditions();
    }

    public abstract void CheckMovingConditions();

}
