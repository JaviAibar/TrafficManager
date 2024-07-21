using System;
using System.Collections;
using System.Collections.Generic;
using BezierSolution;
using Level;
using UnityEngine;
using static Level.GameEngine;

[Serializable]
public class SpeedController : MonoBehaviour
{
    #region Variables
    private float initialSpeed;
    private float accelerationStep = 0;
    private int counterAccelIterations = 0;
    // This value is used when the GameSpeed is changed, because the car could be stopped while running for instance
    private BezierWalkerWithSpeed bezier;
    private float baseSpeedBeforeHalt = -1;

    #endregion

    #region Accessors

    /// <summary>
    /// Variation of the speed per unit time without taking into account any variation, e.g. game speed, running, slowing down...
    /// This will be used when returning to an unaltered acceleration
    /// </summary>
    public float BaseAcceleration { get; private set; } = 0.2f;
    private float Epsilon => 0.01f;
    public bool TargetSpeedReached => Mathf.Abs(CurrentSpeed - TargetSpeed) < Epsilon;
    public bool CanAccelerate => EngineRunning && Instance.IsRunning;
    /// <summary>
    /// Current acceleration with all variations already calculated
    /// </summary>
    public float Acceleration { get; private set; } = 1f;
    private float TargetSpeed => BaseSpeed * GameSpeedInt;
    public int GameSpeedInt => (int)Instance.Speed;
    /// <summary>
    /// Normal speed without taking into account any variation, e.g. game speed, running, slowing down...
    /// This will be used when returning to an unaltered speed
    /// </summary>
    public float BaseSpeed { get; private set; }
    /// <summary>
    /// Actual speed in this specific moment with all possible alterations already calculate
    /// </summary>
    public float CurrentSpeed => bezier.speed;
    public bool IsAccelerating => !TargetSpeedReached && CanAccelerate;

    /// <summary>
    /// Default true. 
    /// If Halt() executed, will be false.
    /// If Resume() executed, will be true.
    /// </summary>
    public bool EngineRunning { get; private set; } = false;

    #endregion

    private void Awake()
    {
        if (!TryGetComponent<RoadUser>(out var roadUser))
            throw new NullReferenceException($"RoadUser not found along with SpeedController in GameObject {name}");
        bezier = roadUser.Bezier;
    }

    private void Update()
    {
        if (IsAccelerating) Accelerate();
    }

    private float Accelerate()
    {
        if (!TargetSpeedReached)
        {
            var increment = CurrentSpeed;
            Print(
                $"[{name}] {(Mathf.Approximately(CurrentSpeed, initialSpeed) ? "started" : "still")} {(CurrentSpeed > TargetSpeed ? "de" : "ac")}celerating from {CurrentSpeed} to {TargetSpeed}",
                VerboseEnum.SpeedDetail);
            counterAccelIterations++;
            accelerationStep += 0.1f; // It will take 1 second to reach accelerationStep=1
            
            bezier.speed = Mathf.Lerp(initialSpeed, TargetSpeed, Acceleration * accelerationStep * GameSpeedInt);
            Print(
                $"[{name}] acceleration iteration ended up with bezier.speed={CurrentSpeed}.   {CurrentSpeed - increment} was added with Lerp(a:{initialSpeed}, b:{TargetSpeed}, t:{Acceleration * accelerationStep}) t = Acceleration:{Acceleration} * accelerationStep:{accelerationStep}  ",
                VerboseEnum.SpeedDetail);
        }
        else
        {
            Print(
                $"Target speed of {name} ({TargetSpeed}) reached: current {CurrentSpeed} in {counterAccelIterations} iterations",
                VerboseEnum.Speed);
        }

        return CurrentSpeed;
    }

    /// <summary>
    /// Change the speed of the user for the given amount
    /// </summary>
    /// <param name="newSpeed"></param>
    /// <param name="speedChanged"></param>
    /// <param name="newAcceleration">default is base acceleration</param>
    public void ChangeSpeed(float newSpeed, float? newAcceleration = null)
    {
        if (newAcceleration == 0 || Acceleration == 0)
        {
            ChangeSpeedImmediately(newSpeed);
            return;
        }

        GameEngine.Print(
            $"Asked {name} to change its speed to {newSpeed} {(Mathf.Approximately(newSpeed, BaseSpeed) ? "but it was already" : $"target was {BaseSpeed} and current {initialSpeed}")} btw its {(TargetSpeedReached ? "" : "NOT")} accelerating already",
            VerboseEnum.Speed);
        
        if (Mathf.Approximately(newSpeed, BaseSpeed)) return;
        initialSpeed = CurrentSpeed;

        BaseSpeed = newSpeed;

        Acceleration = newAcceleration ?? BaseAcceleration;
        accelerationStep = 0;
        counterAccelIterations = 0;
    }

    /// <summary>
    /// Executed when Game changes speed. Changes to speed are applied instantly
    /// because in a Game speed change makes no sense an acceleration effect
    /// </summary>
    /// <param name="state"></param>
   /* public virtual void GameSpeedChanged(GameSpeed state)
    {
        Print(
            $"Speed of {name} {(state == GameSpeed.Paused || !TargetSpeedReached ? $"changed to {(state == GameSpeed.Paused ? "0" : (bezier.speed = BaseSpeed * (int)state))} due to GameSpeed changed to {state}" : "didn't change")}",
            VerboseEnum.Speed);
        if (state == GameSpeed.Paused) bezier.speed = 0;
        else if (!TargetSpeedReached) bezier.speed = BaseSpeed * (int)state;
    }*/

    /// <summary>
    /// Changes speed of user almost instantly
    /// </summary>
    /// <param name="newSpeed"></param>
    public void ChangeSpeedImmediately(float newSpeed)
    {
        Print($"[{name}] forced to change its speed quickly to {newSpeed}", VerboseEnum.Speed);
        BaseSpeed = newSpeed;
        bezier.speed = newSpeed;
        //return ChangeSpeed(newSpeed, 100);
    }

    public void LoopStarted()
    {
    }

   /* public void SetInitValues(RoadUser roadUser)
    {
        this.name = roadUser.name;
        this.bezier = roadUser.Bezier;
    }

    public void SetInitValues(string name, BezierWalkerWithSpeedVariant bezier)
    {
        this.name = name;
        this.bezier = bezier;
    }*/

    /// <summary>
    /// You can execute this method if some condition is not met. It will save the state of the last speed set and stop immediately, i.e. CurrentSpeed = 0.
    /// When Resume() is executed and if the rest of the conditions are met, it will accelerate until last speed set was reached
    /// </summary>
    public void Halt()
    {
        baseSpeedBeforeHalt = BaseSpeed;
        ChangeSpeedImmediately(0);
        EngineRunning = false;
    }

    /// <summary>
    /// This will revert Halt() method i.e. it will restore last speed set by accelerating
    /// </summary>
    public void Resume()
    {
        if (EngineRunning) return;
        ChangeSpeed(baseSpeedBeforeHalt);
        EngineRunning = true;
    }
}
