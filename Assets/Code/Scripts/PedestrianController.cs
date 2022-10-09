using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianController : RoadUser
{
    public Animator anim;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }
    // Called both when the pedestrian reaches a collider or when the trafficlight changes
    public override void CheckMovingConditions()
    {
        if (trafficArea != null && trafficArea.stopArea && GameEngine.Vector3ToDirection(transform.parent.up) == trafficArea.direction && trafficLight.GetState() == TrafficLightController.TrafficLightColour.Green)
        {
            bezier.speed = 0f;
            anim.SetBool("isWalking", false);
        }
        else if (trafficArea != null && trafficLight.GetState() == TrafficLightController.TrafficLightColour.Green && trafficArea.direction == GameEngine.Direction.Center) {
            anim.speed = 1.7f;
            bezier.speed = runningSpeed;
            anim.SetBool("isWalking", true);
        }
        else {
            bezier.speed = speed;
            anim.SetBool("isWalking", true);
            anim.speed = 1f;

        }
    }
}
