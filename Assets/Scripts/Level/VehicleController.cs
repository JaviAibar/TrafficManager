using BezierSolution;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEngine;

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
        string moreInfo = (trafficArea ? $": {(MustStop() ? "Stopped" : MustRun() ? "Running" : "Walking speed")} Explanation:\nIs an stop area ({(trafficArea.StopArea)}) affecting us (same dir)({(trafficArea.SameDirection(UserDir))}) in red ({(trafficLight.IsRed)})(then must stop)? -> {MustStop()}.\nIs yellow ({(trafficLight.IsYellow)}) and middle ({(trafficArea.IsCenter)}) OR before ({(trafficArea.SameDirection(UserDir))}) a cross? -> {MustRun()}.\nOtherwise? {!MustStop() && !MustRun()}" : "");
        Print($"{name} {(trafficArea ? "has ":"doesn't have ")} a traffic area"+ moreInfo);
        if (trafficArea && respectsTheRules && !inAnEmergency && hasStartedMoving)
        {
            if (MustStop())
            {
                Moving(false);
                StartCoroutine(Accelerate(0));

                baseSpeed = 0;
            }
            // Yellow and before or middle cross, then boost
            // or Red but in middle cross, then boost
            else if (MustRun())
            {
                Moving(true);
                StartCoroutine(Accelerate(runningSpeed * (int)GameEngine.instance.Speed));
                baseSpeed = runningSpeed;

            }
            else
            {
                Moving(true);
                StartCoroutine(Accelerate(normalSpeed * (int)GameEngine.instance.Speed));
                baseSpeed = normalSpeed;
            }
        }
    }

    public bool MustStop() => trafficArea.StopArea && trafficLight.IsRed && trafficArea.SameDirection(UserDir);
    public bool MustRun() => trafficLight.IsYellow && (trafficArea.IsCenter || trafficArea.SameDirection(UserDir));
}
