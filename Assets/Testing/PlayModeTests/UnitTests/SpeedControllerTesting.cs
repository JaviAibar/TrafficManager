using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnitTests
{
    public class SpeedControllerTesting
    {
        public SpeedController CreateDefaultSpeedController()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var vehicle = RoadUserHelperMethods.CreateDefaultVehicle(gameEngineFaker);
            SpeedController speedController = new(vehicle);

            return speedController;
        }

        [Test]
        public void _00_DefaultSpeedControl_IsAcceleratingAtStartTest()
        {
            var speedController = CreateDefaultSpeedController();
            Assert.IsFalse(speedController.IsAccelerating);
        }

        [Test]
        public void _01_DefaultSpeedControl_IsAcceleratingAfterSpeedChangedTest()
        {
            var speedController = CreateDefaultSpeedController();
            speedController.ChangeSpeed(0);
            Assert.IsTrue(speedController.IsAccelerating);
        }

        [Test]
        public void _02_DefaultSpeedControl_IsAcceleratingAfterSpeedChangedTest()
        {
            var speedController = CreateDefaultSpeedController();
            speedController.ChangeSpeed(int.MaxValue);
            Assert.IsTrue(speedController.IsAccelerating);
        }

        [Test]
        public void _03_DefaultSpeedControl_IsAcceleratingAfterSpeedImmediatelyChangedTest()
        {
            var speedController = CreateDefaultSpeedController();
            speedController.ChangeSpeedImmediately(0);
            Assert.IsTrue(speedController.IsAccelerating);
        }

        [Test]
        public void _04_DefaultSpeedControl_IsAcceleratingAfterSpeedImmediatelyChangedTest()
        {
            var speedController = CreateDefaultSpeedController();
            speedController.ChangeSpeedImmediately(int.MaxValue);
            Assert.IsTrue(speedController.IsAccelerating);

        }
        [UnityTest]
        public IEnumerator _10_DefaultSpeedControl_SetSpeedImmediately()
        {
            var speedController = CreateDefaultSpeedController();
            var ARBITRARY_SPEED = int.MaxValue;
            speedController.ChangeSpeedImmediately(ARBITRARY_SPEED);
            speedController.Accelerate();
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(ARBITRARY_SPEED, speedController.CurrentSpeed);
        }

        [UnityTest]
        public IEnumerator _20_DefaultSpeedControl_SetSpeed()
        {
            var speedController = CreateDefaultSpeedController();
            var ARBITRARY_SPEED = int.MaxValue;
            speedController.ChangeSpeedImmediately(ARBITRARY_SPEED);
            speedController.Accelerate();
            yield return new WaitWhile(() => speedController.IsAccelerating);
            Assert.AreEqual(ARBITRARY_SPEED, speedController.CurrentSpeed);
        }
    }
}
