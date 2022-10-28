using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VehicleController : RoadUser
{
    public bool inAnEmergency;
    private Animator anim;
    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        if (inAnEmergency) anim.SetTrigger("Emergency");
    }
    public override void CheckMovingConditions()
    {
        if (trafficArea != null && respectsTheRules && !inAnEmergency && canMove)
        {
            if (trafficArea.stopArea && trafficLight.GetState() == TrafficLightController.TrafficLightColour.Red && GameEngine.Vector3ToDirection(transform.parent.up) == trafficArea.direction)
            {
                Moving(false);
                bezier.speed = 0;
                baseSpeed = 0;
            }
            // Yellow and before or middle cross, then boost
            // or Red but in middle cross, then boost
            else if (trafficLight.GetState() == TrafficLightController.TrafficLightColour.Yellow && (GameEngine.Vector3ToDirection(transform.forward) == trafficArea.direction || GameEngine.Direction.Center == trafficArea.direction) ||
                trafficLight.GetState() == TrafficLightController.TrafficLightColour.Yellow && GameEngine.Direction.Center == trafficArea.direction)
            {
                Moving(true);
                bezier.speed = runningSpeed * (int)GameEngine.instance.GetGameSpeed();
                baseSpeed = runningSpeed;

            }
            else
            {
                Moving(true);
                bezier.speed = normalSpeed * (int)GameEngine.instance.GetGameSpeed();
                baseSpeed = normalSpeed;
            }
        }
    }
}
