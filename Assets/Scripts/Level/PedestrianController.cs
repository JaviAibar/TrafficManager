using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEngine;

[System.Serializable]
public class PedestrianController : RoadUser
{
    private Animator anim;
    private SpriteRenderer rend;
    public Sprite ranOverSprite;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
    }
    // Called both when the pedestrian reaches a collider or when the trafficlight changes
    public override void CheckMovingConditions()
    {
        if (hasStartedMoving && trafficArea) // This flag takes offsetTime condition into account
        {
            if (trafficArea.StopArea && trafficArea.SameDirection(transform.up) && trafficLight.IsGreen)
            {
                Moving(false);
                StartCoroutine(Accelerate(0));

                baseSpeed = 0;
                anim.SetBool("isWalking", false);
            }
            else if (trafficLight.IsGreen && trafficArea.IsCenter)
            {
                Moving(true);
                StartCoroutine(Accelerate(runningSpeed * (int)GameEngine.instance.Speed));
                anim.speed = 1.7f;
                baseSpeed = runningSpeed;
                anim.SetBool("isWalking", true);
            }
            else
            {
                Moving(true);
                StartCoroutine(Accelerate(normalSpeed * (int)GameEngine.instance.Speed));
                anim.speed = 1f;

                baseSpeed = normalSpeed;
                anim.SetBool("isWalking", true);
            }
        }
    }

    public override void GameSpeedChanged(GameSpeed state)
    {
        base.GameSpeedChanged(state);
        anim.speed = (float)state;
    }

    internal void BeRanOver()
    {
        anim.enabled = false;
        rend.sprite = ranOverSprite;
    }

    public override void LoopStarted()
    {
        base.LoopStarted();
        anim.enabled = true;
    }
}
