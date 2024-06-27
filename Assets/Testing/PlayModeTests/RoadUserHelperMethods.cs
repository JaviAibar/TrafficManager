using BezierSolution;
using Level;
using System.Collections;
using UnityEngine;

public class RoadUserHelperMethods
{

    public static IEnumerator CalculateTimesSpeedRoadUser_NormalSpeed(RoadUser roadUser, GameEngineFaker gameEngineFaker, float speed, TestingDurations durations)
    {
        gameEngineFaker.GameEngine.Speed = GameEngine.GameSpeed.Normal;
        gameEngineFaker.GameEngine.Verbose = GameEngine.VerboseEnum.GameTrace;
        yield return MakeRoadUserStartMoving(roadUser, speed, gameEngineFaker, durations);
        durations.normalDuration = Time.time - durations.t;
    }

    public static IEnumerator CalculateTimesSpeedRoadUser_FastSpeed(RoadUser roadUser, GameEngineFaker gameEngineFaker, float speed, TestingDurations durations)
    {
        gameEngineFaker.GameEngine.Speed = GameEngine.GameSpeed.Fast;
        gameEngineFaker.GameEngine.Verbose = GameEngine.VerboseEnum.GameTrace;
        yield return MakeRoadUserStartMoving(roadUser, speed, gameEngineFaker, durations);
        durations.fastDuration = Time.time - durations.t;
    }

    public static IEnumerator CalculateTimesSpeedRoadUser_FastestSpeed(RoadUser roadUser, GameEngineFaker gameEngineFaker, float speed, TestingDurations durations)
    {
        gameEngineFaker.GameEngine.Speed = GameEngine.GameSpeed.SuperFast;
        gameEngineFaker.GameEngine.Verbose = GameEngine.VerboseEnum.GameTrace;
        yield return MakeRoadUserStartMoving(roadUser, speed, gameEngineFaker, durations);
        durations.fastestDuration = Time.time - durations.t;
    }

    private static IEnumerator MakeRoadUserStartMoving(RoadUser roadUser, float speed, GameEngineFaker gameEngineFaker, TestingDurations durations)
    {
        roadUser.LoopStarted();
        yield return WaitWhileLooping(roadUser);
        yield return WaitWhileCantStartMoving(roadUser);
        SetRoadUserAtStart(roadUser.bezier);
        durations.t = Time.time;
        roadUser.ChangeSpeed(speed); // Acceleration is included in this test
        yield return WaitWhileFinishLineNotReached(gameEngineFaker.Bezier);
        yield return WaitWhileOnFinishLine(gameEngineFaker.Bezier);
    }

    private static void SetRoadUserAtStart(BezierWalkerWithSpeed bezier)
    {
        bezier.NormalizedT = 0;
    }

    public static IEnumerator WaitWhileFinishLineNotReached(BezierWalkerWithSpeedVariant bezier)
    {
        while (1 - bezier.NormalizedT > HelperUtilities.Epsilon)
        {
            yield return null;
        }
    }

    public static IEnumerator WaitWhileOnFinishLine(BezierWalkerWithSpeedVariant bezier)
    {
        while (1 - bezier.NormalizedT <= HelperUtilities.Epsilon)
        {
            yield return null;
        }
    }

    public static IEnumerator WaitWhileCantStartMoving(RoadUser roadUser)
    {
        while (!roadUser.HasStartedMoving)
        {
            yield return null;
        }
    }

    private static IEnumerator WaitWhileLooping(RoadUser roadUser)
    {
        while (roadUser.Looping)
        {
            yield return null;
        }
    }

    /*public static IEnumerator WaitWhileAccelerating()
    {
        while (Accelerating)
        {
            Debug.Log("Not yet accelerated");
            yield return null;
        }
    }*/

    /* public bool Accelerating => vehicle ? vehicle.Accelerating : pedestrian.Accelerating;
     public bool CantMove => vehicle ? !vehicle.CanMove : !pedestrian.CanMove;*/
}

// A class and not a struct because object values change needed in coroutine
public class TestingDurations
{
    public float normalDuration, fastDuration, fastestDuration, t;
}