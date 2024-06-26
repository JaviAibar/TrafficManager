using BezierSolution;
using Level;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class PedestrianTesting : MonoBehaviour
{
    /*private float normalDuration = 0;
    private float fastDuration = 0;
    private float fastestDuration = 0;*/

    private PedestrianController CreateDefaultPedestrian(GameEngineFaker gameEngineFaker)
    {
        float TOO_LONG_TIME = 200;
        // HelperUtilities.CreateDefaultPlayground(out gameEngine, out BezierSpline bezierSpline);
        /*if (gameKernel != null) Destroy(gameKernel);*/


        //   var roadUsersGO = GameObject.Find("RoadUsers");
        // Avoid an exception in Vehicle caused because it expects to have BezierSpline on Awake (selected in Inspector)
        GameObject roadUsersGO = new();
        /*      LogAssert.Expect(LogType.Exception,
                @"Exception: Root of Pedestrian1(Clone)'s Bezier needs a reference to a BezierSpline component");*/
        LogAssert.Expect(LogType.Exception,
             @"NullReferenceException: Root of Pedestrian1(Clone)'s Bezier needs a reference to a BezierSpline component");

        var pedestrian1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Prefabs/RoadUsers/Pedestrian1"), roadUsersGO.transform);
        var pedestrian = pedestrian1.GetComponent<PedestrianController>();
        gameEngineFaker.SetBezier(pedestrian);
        pedestrian.Spline = gameEngineFaker.BezierSpline;
           // gameKernel.GetComponentsInChildren<BezierSpline>()[1]; // Asign the Spline 2 because we want it to go left
        pedestrian.enabled = true; // Set enable because it gets automatically disabled due to the aforementioned Exception
        pedestrian.TimeToLoop = TOO_LONG_TIME;

        return pedestrian;
    }


    [UnityTest]
    public IEnumerator _00_GameSpeedChangingTest_FastIsProportional([Values(40, 30, 20)] float speed)
    {
        GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
       // yield return new WaitForSeconds(1);
        //Debug.Log(gameEngineFaker.GameEngine);
        PedestrianController pedestrian = CreateDefaultPedestrian(gameEngineFaker);
        //yield return PrepareScene(false, RoadUserType.Pedestrian);
        //gameEngine.Verbose = GameEngine.VerboseEnum.Speed | GameEngine.VerboseEnum.GameTrace;
        TestingDurations durations = new();

        yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser_NormalSpeed(pedestrian, gameEngineFaker, speed, durations);
        yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser_FastSpeed(pedestrian, gameEngineFaker, speed, durations);

        var expected = durations.normalDuration / 2;

        if (!HelperUtilities.Approx(expected, durations.fastDuration))
        {
            Debug.Log($"Expected {expected} but was {durations.fastDuration}");
        }
        Destroy(pedestrian.gameObject);
        Assert.IsTrue(HelperUtilities.Approx(expected, durations.fastDuration));
    }

    [UnityTest]
    public IEnumerator _00_GameSpeedChangingTest_FastestIsProportional([Values(100, 40, 30, 20)] float speed)
    {
        GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
        PedestrianController pedestrian = CreateDefaultPedestrian(gameEngineFaker);

        TestingDurations durations = new();
        yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser_NormalSpeed(pedestrian, gameEngineFaker, speed, durations);
        yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser_FastestSpeed(pedestrian, gameEngineFaker, speed, durations);
        //  HelperUtilities.PrintTimesSpeed(normalDuration, fastDuration, fastestDuration);
        var expected = durations.normalDuration / 3;

        if (!HelperUtilities.Approx(expected, durations.fastestDuration))
        {
            Debug.Log($"Expected {expected} but was {durations.fastestDuration}"); 
        }

        Destroy(pedestrian.gameObject);
        Assert.IsTrue(HelperUtilities.Approx(expected, durations.fastestDuration));
    }

    /* [UnityTest]
     public IEnumerator _01_GameSpeedChangingPedestrianTest2(
         [Values(10, 3, 30, 50, 80)] float speedToTest)
     {
         //float normalizedTCompletedLoop = 0.95f;
         //yield return PrepareScene(trafficAreasInteraction: false, RoadUserType.Pedestrian);
         PedestrianController pedestrian = CreateDefaultPedestrian();
         yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser(pedestrian, GameEngine.GameSpeed.Normal, speedToTest);
         yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser(pedestrian, GameEngine.GameSpeed.Fast, speedToTest);
         yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser(pedestrian, GameEngine.GameSpeed.SuperFast, speedToTest);

         HelperUtilities.PrintTimesSpeed(normalDuration, fastDuration, fastestDuration);

         Assert.IsTrue(HelperUtilities.Approx(normalDuration / 2, fastDuration));
         Assert.IsTrue(HelperUtilities.Approx(normalDuration / 3, fastestDuration));
     }


     [UnityTest]
     public IEnumerator _02_SpeedChangingPedestrianTest(
         [Values(1, 10, 5, 30, 0, 50)] float speedsToTest)
     {
        // yield return PrepareScene(false, RoadUserType.Pedestrian);

        // gameEngine.Verbose = GameEngine.VerboseEnum.GameTrace | GameEngine.VerboseEnum.Speed;

         gameEngine.Speed = GameEngine.GameSpeed.Normal;
         //yield return new WaitForEndOfFrame(); // Wait for all the corresponding Awake and Start

         yield return WaitWhileCantMove();
         yield return WaitWhileAccelerating();
         bezier.NormalizedT = 0;
         foreach (float s in speedsToTest)
         {
             pedestrian.ChangeSpeed(s, 0.2f);
             yield return WaitWhileAccelerating();
             print($"{s} ahora {pedestrian.CurrentSpeed} {Mathf.Approximately(s, pedestrian.CurrentSpeed)}");
             Assert.True(Approx(s, pedestrian.CurrentSpeed));
         }
     }

     [UnityTest]
     public IEnumerator _03_LoopPedestrianTest()
     {
         yield return PrepareScene(false, RoadUserType.Pedestrian);
         //gameEngine.verbose = GameEngine.VerboseEnum.GameTrace | GameEngine.VerboseEnum.Speed;
         Vector3 initPos = new Vector3(-4.62f, 33.00f, 0.00f);
         yield return WaitWhileFinishLineNotReached();
         yield return WaitWhileOnFinishLine();
         // yield return new WaitForSeconds(1);
         print(
             $"Expected pos: {initPos}, Actual pos: {pedestrian.transform.position}, diff: {(initPos - pedestrian.transform.position).magnitude}");
         Assert.IsTrue((initPos - pedestrian.transform.position).magnitude < Epsilon);
     }*/
}