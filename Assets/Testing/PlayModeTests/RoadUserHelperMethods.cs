using BezierSolution;
using Level;
using System.Collections;
using UnityEngine;

public class RoadUserHelperMethods
{
    public static PedestrianController CreateDefaultPedestrian(GameEngineFaker gameEngineFaker)
    {
        float TOO_LONG_TIME = 200;

        var pedestrian1GO = MonoBehaviour.Instantiate((GameObject)Resources.Load("Prefabs/RoadUsers/Pedestrian1"), gameEngineFaker.RoadUsersGO.transform);

        var pedestrianController = pedestrian1GO.GetComponent<PedestrianController>();
        pedestrianController.Spline = gameEngineFaker.SelectSpline(1); // Asign the Spline 1 because we want it to go down
        pedestrianController.TimeToLoop = TOO_LONG_TIME;

        return pedestrianController;
    }

    public static VehicleController CreateDefaultVehicle(GameEngineFaker gameEngineFaker)
    {
        float TOO_LONG_TIME = 200;

        var blackCarGO = MonoBehaviour.Instantiate((GameObject)Resources.Load("Prefabs/RoadUsers/Black Car"), gameEngineFaker.RoadUsersGO.transform);

        var vehicleController = blackCarGO.GetComponent<VehicleController>();
        vehicleController.Spline = gameEngineFaker.SelectSpline(2); // Asign the Spline 2 because we want it to go left
        vehicleController.TimeToLoop = TOO_LONG_TIME;

        return vehicleController;
    }

    public static IEnumerator CalculateTimesSpeedRoadUser_NormalSpeed(RoadUser roadUser, GameEngineFaker gameEngineFaker, float speed, TestingDurations durations)
    {
        gameEngineFaker.GameEngine.Speed = GameEngine.GameSpeed.Normal;
        gameEngineFaker.GameEngine.Verbose = GameEngine.VerboseEnum.GameTrace;
        yield return MakeRoadUserStartMoving(roadUser, speed, durations);
        durations.normalDuration = Time.time - durations.t;
    }

    public static IEnumerator CalculateTimesSpeedRoadUser_FastSpeed(RoadUser roadUser, GameEngineFaker gameEngineFaker, float speed, TestingDurations durations)
    {
        gameEngineFaker.GameEngine.Speed = GameEngine.GameSpeed.Fast;
        gameEngineFaker.GameEngine.Verbose = GameEngine.VerboseEnum.GameTrace;
        yield return MakeRoadUserStartMoving(roadUser, speed, durations);
        durations.fastDuration = Time.time - durations.t;
    }

    public static IEnumerator CalculateTimesSpeedRoadUser_FastestSpeed(RoadUser roadUser, GameEngineFaker gameEngineFaker, float speed, TestingDurations durations)
    {
        gameEngineFaker.GameEngine.Speed = GameEngine.GameSpeed.SuperFast;
        gameEngineFaker.GameEngine.Verbose = GameEngine.VerboseEnum.GameTrace;
        yield return MakeRoadUserStartMoving(roadUser, speed, durations);
        durations.fastestDuration = Time.time - durations.t;
    }

    private static IEnumerator MakeRoadUserStartMoving(RoadUser roadUser, float speed, TestingDurations durations)
    {
        roadUser.StartLoop();
        yield return WaitWhileLooping(roadUser);
       // yield return WaitWhileCantStartMoving(roadUser);
        SetRoadUserAtStart(roadUser.Bezier);
        durations.t = Time.time;
        roadUser.ChangeSpeed(speed); // Acceleration is included in this test
        yield return WaitWhileFinishLineNotReached(roadUser.Bezier);
        yield return WaitWhileOnFinishLine(roadUser.Bezier);
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