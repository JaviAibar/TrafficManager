using System;
using System.Collections;
using System.Collections.Generic;
using BezierSolution;
using Level;
using UnityEngine;
using static Level.GameEngine;

[Serializable]
public class SpeedController
{
    #region Variables
    private float initialSpeed;
    private string name;
    private float accelerationStep = 0;
    private int counterAccelIterations = 0;
    // This value is used when the GameSpeed is changed, because the car could be stopped while running for instance
    private float baseSpeed;
    private bool _accelerating;
    private float acceleration = 1f;
    private BezierWalkerWithSpeed bezier;
    private bool speedChanged;

    #endregion
    
    #region Accessors

    public float baseAcceleration = 0.2f;
    public float BaseAcceleration => baseAcceleration;
    private float Epsilon => 0.01f;
    public bool TargetSpeedReached => Mathf.Abs(CurrentSpeed - TargetSpeed) < Epsilon;
    public bool CanAccelerate => speedChanged && !TargetSpeedReached;
    public float Acceleration => acceleration;
    private float TargetSpeed => BaseSpeed * GameSpeedInt;
    public int GameSpeedInt => (int) instance.Speed;
    public float BaseSpeed => baseSpeed;
    public bool IsOnStartingLine => bezier.NormalizedT <= 0f;
    public float CurrentSpeed
    {
        get => bezier.speed;
        set => bezier.speed = value;
    }
#endregion
    public SpeedController(string name, BezierWalkerWithSpeed bezier)
    {
        this.name = name;
        this.bezier = bezier;
    }
    public float Accelerate()
    {
        if (!TargetSpeedReached)
        {
            var increment = CurrentSpeed;
            Print(
                $"[{name}] {(Mathf.Approximately(CurrentSpeed, initialSpeed) ? "started" : "still")} {(CurrentSpeed > TargetSpeed ? "de" : "ac")}celerating from {CurrentSpeed} to {TargetSpeed}",
                VerboseEnum.SpeedDetail);
            counterAccelIterations++;
            accelerationStep += 0.1f; // It will take 1 second to reach accelerationStep=1
            // print($"Increasing {name} speed to { Mathf.Lerp(initialSpeed, targetSpeed, Acceleration * timeAccelerating)} t={ Acceleration * (int)instance.Speed * timeAccelerating} (delta={timeAccelerating} * gameSpeed={instance.Speed} * Accl={Acceleration})");
            CurrentSpeed = Mathf.Lerp(initialSpeed, TargetSpeed, Acceleration * accelerationStep * GameSpeedInt);
            Print(
                $"[{name}] acceleration iteration ended up with bezier.speed={CurrentSpeed}.   {CurrentSpeed - increment} was added with Lerp(a:{initialSpeed}, b:{TargetSpeed}, t:{Acceleration * accelerationStep }) t = Acceleration:{Acceleration} * accelerationStep:{accelerationStep}  ",
                VerboseEnum.SpeedDetail);
            CheckSpeedChangedReached();
        }
        else
        {
            Print(
                $"Target speed of {name} ({TargetSpeed}) reached: current {CurrentSpeed} in {counterAccelIterations} iterations",
                VerboseEnum.Speed);
        }

        return CurrentSpeed;
    }

    private void CheckSpeedChangedReached()
    {
        speedChanged = !TargetSpeedReached;
    }

    /// <summary>
    /// Change the speed of the user for the given amount
    /// </summary>
    /// <param name="newSpeed"></param>
    /// <param name="speedChanged"></param>
    /// <param name="newAcceleration">default is base acceleration</param>
    public bool? ChangeSpeed(float newSpeed, float? newAcceleration = null)
    {
        speedChanged = true;
        initialSpeed = CurrentSpeed;
        GameEngine.Print(
            $"Asked {name} to change its speed to {newSpeed} {(Mathf.Approximately(newSpeed, baseSpeed) ? "but it was already" : $"target was {baseSpeed} and current {initialSpeed}")} btw its {(TargetSpeedReached ? "" : "NOT")} accelerating already",
            VerboseEnum.Speed);
        /*if (!hasStartedMoving) Debug.LogWarning($"[{name}] asked to move before its hasStartedMoving was completed!");*/
        if (/*hasStartedMoving && */Mathf.Approximately(newSpeed, baseSpeed)) return null;
        //Moving(newSpeed != 0); // If new speed is diff of 0, then Moving(true)
        baseSpeed = newSpeed;
       // Moving(newSpeed != 0);
        acceleration = newAcceleration ?? BaseAcceleration; 
        accelerationStep = 0;
        counterAccelIterations = 0;
        return newSpeed != 0;
    }

    /// <summary>
    /// Executed when Game changes speed. Changes to speed are applied instantly
    /// because in a Game speed change makes no sense an acceleration effect
    /// </summary>
    /// <param name="state"></param>
    public virtual void GameSpeedChanged(GameSpeed state)
    {
        Print(
            $"Speed of {name} {(state == GameSpeed.Paused || !TargetSpeedReached ? $"changed to {(state == GameSpeed.Paused ? "0" : (bezier.speed = baseSpeed * (int)state))} due to GameSpeed changed to {state}" : "didn't change")}",
            VerboseEnum.Speed);
        if (state == GameSpeed.Paused) bezier.speed = 0;
        else if (!TargetSpeedReached) bezier.speed = baseSpeed * (int)state;
    }
    
    /// <summary>
    /// Changes speed of user almost instantly
    /// </summary>
    /// <param name="newSpeed"></param>
    public void ChangeSpeedImmediately(float newSpeed)
    {
        Print($"[{name}] forced to change its speed quickly to {newSpeed}", VerboseEnum.Speed);
        ChangeSpeed(newSpeed, 100);
    }

    public void LoopStarted()
    {
    }
    
  //  public IEnumerator LocateOnStartLine()
  //  {
      //  bezier.NormalizedT = 0;
        //baseSpeed = 1;
        /*
         * This speed to 1 and the fact that this method is a coroutine is because in order to not hit anybody while backing
         * to the start, I need to disable the collider, but bezier won't move back to the start until we have a couple of
         * frames moving.
         * 
         */
       // bezier.speed = 1f;
      //  yield return new WaitWhile(() => IsOnStartingLine);
      //  bezier.speed = 0;
     // yield return null;
   // }
}
