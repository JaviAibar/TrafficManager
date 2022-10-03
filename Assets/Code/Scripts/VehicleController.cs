using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : RoadUser
{
    public override void CheckMovingConditions()
    {
        if (trafficArea != null) {
            if (trafficArea.stopArea && trafficLight.GetState() == TrafficLightController.TrafficLightColour.Red && GameEngine.Vector3ToDirection(transform.forward) == trafficArea.direction)
            {
                bezier.speed = stopSpeed;
            }
            // Yellow and before or middle cross, then boost
            // or Red but in middle cross, then boost
            else if (trafficLight.GetState() == TrafficLightController.TrafficLightColour.Yellow && (GameEngine.Vector3ToDirection(transform.forward) == trafficArea.direction || GameEngine.Direction.Center == trafficArea.direction) ||
                trafficLight.GetState() == TrafficLightController.TrafficLightColour.Yellow && GameEngine.Direction.Center == trafficArea.direction)
            {
                bezier.speed = speed + speedBoost;
                print("Boost de " + name);
            }
            else
            {
                bezier.speed = speed;
            }
        }
    }
}
