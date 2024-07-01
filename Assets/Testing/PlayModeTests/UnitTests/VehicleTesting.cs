using BezierSolution;
using Level;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnitTests
{
    public class VehicleTesting
    {
        BezierWalkerWithSpeedVariant bezier;
        private VehicleController CreateDefaultVehicle(GameEngineFaker gameEngineFaker)
        {
            float TOO_LONG_TIME = 200;

            GameObject roadUsersGO = new();

            var blackCar = MonoBehaviour.Instantiate((GameObject)Resources.Load("Prefabs/RoadUsers/Black Car"), roadUsersGO.transform);

            var vehicle = blackCar.GetComponent<VehicleController>();
            gameEngineFaker.SetBezier(vehicle);
            vehicle.Spline = gameEngineFaker.SelectSpline(2); // Asign the Spline 2 because we want it to go left
            vehicle.enabled = true; // Set enable because it disables automatically due to the aforementioned Exception
            vehicle.TimeToLoop = TOO_LONG_TIME;
            bezier = blackCar.GetComponent<BezierWalkerWithSpeedVariant>();

            return vehicle;
        }


        [UnityTest]
        public IEnumerator _00_GameSpeedChangingVehicleTest_FastIsProportional([Values(40, 30, 20)] float speed)
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            VehicleController vehicle = CreateDefaultVehicle(gameEngineFaker);
            TestingDurations durations = new();

            yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser_NormalSpeed(vehicle, gameEngineFaker, speed, durations);
            yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser_FastSpeed(vehicle, gameEngineFaker, speed, durations);

            var expected = durations.normalDuration / 2;

            if (!HelperUtilities.Approx(expected, durations.fastDuration))
            {
                Debug.Log($"Expected {expected} but was {durations.fastDuration}");
            }
            MonoBehaviour.Destroy(vehicle.gameObject);
            Assert.IsTrue(HelperUtilities.Approx(expected, durations.fastDuration));
        }

        [UnityTest]
        public IEnumerator _01_GameSpeedChangingVehicleTest_FastestIsProportional([Values(40, 30, 20)] float speed)
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            VehicleController vehicle = CreateDefaultVehicle(gameEngineFaker);
            TestingDurations durations = new();

            yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser_NormalSpeed(vehicle, gameEngineFaker, speed, durations);
            yield return RoadUserHelperMethods.CalculateTimesSpeedRoadUser_FastestSpeed(vehicle, gameEngineFaker, speed, durations);

            var expected = durations.normalDuration / 3;

            if (!HelperUtilities.Approx(expected, durations.fastestDuration))
            {
                Debug.Log($"Expected {expected} but was {durations.fastestDuration}");
            }
            MonoBehaviour.Destroy(vehicle.gameObject);
            Assert.IsTrue(HelperUtilities.Approx(expected, durations.fastestDuration));
        }

        /* [UnityTest]
         public IEnumerator SpeedChangingVehicleTesting()
         {
             float[] speedsToTest = new float[] { 1, 10, 5, 30, 0, 50 };
             yield return PrepareScene(false, RoadUserType.Vehicle);

             gameEngine.Verbose = GameEngine.VerboseEnum.GameTrace | GameEngine.VerboseEnum.Speed;

             gameEngine.Speed = GameEngine.GameSpeed.Normal;
             //yield return new WaitForEndOfFrame(); // Wait for all the corresponding Awake and Start

             yield return WaitWhileCantMove();
             yield return WaitWhileAccelerating();
             bezier.NormalizedT = 0;
             foreach (float s in speedsToTest)
             {
                 vehicle.ChangeSpeed(s, 0.2f);
                 yield return WaitWhileAccelerating();
                 print($"{s} ahora {vehicle.CurrentSpeed} {Mathf.Approximately(s, vehicle.CurrentSpeed)}");
                 Assert.True(Approx(s, vehicle.CurrentSpeed));
             }
         }

         [UnityTest]
         public IEnumerator LoopVehicleTest()
         {
             yield return PrepareScene(false, RoadUserType.Vehicle);
             //gameEngine.verbose = GameEngine.VerboseEnum.GameTrace | GameEngine.VerboseEnum.Speed;
             Vector3 initPos = new Vector3(49.38f, 4.31f, 0.00f);
             yield return WaitWhileFinishLineNotReached();
             yield return WaitWhileOnFinishLine();
             // yield return new WaitForSeconds(1);
             print(
                 $"Expected pos: {initPos}, Actual pos: {vehicle.transform.position}, diff: {(initPos - vehicle.transform.position).magnitude}");
             Assert.IsTrue((initPos - vehicle.transform.position).magnitude < Epsilon);
         }*/

    }
}