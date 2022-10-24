using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
        if (canMove) // This flag takes offsetTime condition into account
        {
            if (trafficArea != null && trafficArea.stopArea && GameEngine.Vector3ToDirection(transform.parent.up) == trafficArea.direction && trafficLight.GetState() == TrafficLightController.TrafficLightColour.Green)
            {
                Moving(false);
                bezier.speed = 0;
                anim.SetBool("isWalking", false);
                baseSpeed = 0;
            }
            else if (trafficArea != null && trafficLight.GetState() == TrafficLightController.TrafficLightColour.Green && trafficArea.direction == GameEngine.Direction.Center)
            {

                Moving(true);
                anim.speed = 1.7f;
                bezier.speed = runningSpeed * (int)GameEngine.instance.GetGameSpeed();
                anim.SetBool("isWalking", true);
                baseSpeed = runningSpeed;
            }
            else
            {
                Moving(true);
                bezier.speed = normalSpeed * (int)GameEngine.instance.GetGameSpeed();
                anim.SetBool("isWalking", true);
                anim.speed = 1f;

                baseSpeed = normalSpeed;
            }
        }
    }

    public override void SpeedChanged(GameEngine.GameSpeed state)
    {
        base.SpeedChanged(state);
        anim.speed = (float)state;
    }
}
