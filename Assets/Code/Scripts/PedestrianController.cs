using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianController : RoadUser
{
    public override void CheckMovingConditions()
    {
        if (trafficArea != null && trafficArea.stopArea && GameEngine.Vector3ToDirection(transform.forward) == trafficArea.direction && trafficLight.GetState() == TrafficLightController.TrafficLightColour.Green)
        {
            bezier.speed = stopSpeed;
        }
        else
        {
            bezier.speed = speed;
        }
    }
}
