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

            return vehicle.GetComponent<SpeedController>();
        }

        // SECTION 0
        [Test]
        public void _00_DefaultSpeedControl_IsNotAcceleratingAtStartTest()
        {

            var speedController = CreateDefaultSpeedController();
            Assert.IsFalse(speedController.IsAccelerating);

            MonoBehaviour.Destroy(speedController.gameObject);
        }

        [Test]
        public void _01_DefaultSpeedControl_CanNotAccelerateAtStartTest()
        {
            var speedController = CreateDefaultSpeedController();
            // A default speed controller is set CanAccelerate to false because at start
            // RoadUsers are in start process (Looping)
            Assert.IsFalse(speedController.CanAccelerate);
            MonoBehaviour.Destroy(speedController.gameObject);
        }
        [Test]
        public void _02_DefaultSpeedControl_CurrentSpeed0Test()
        {
            var speedController = CreateDefaultSpeedController();
            Assert.AreEqual(0f, speedController.CurrentSpeed);
            MonoBehaviour.Destroy(speedController.gameObject);
        }


        [Test]
        public void _03_ResumeMakesCanAccelerateTest()
        {
            var speedController = CreateDefaultSpeedController();
            speedController.Resume();
            Assert.IsTrue(speedController.CanAccelerate);
            MonoBehaviour.Destroy(speedController.gameObject);
        }


        // SECTION 1
        [Test]
        public void _10_CanNotAccelerateAfterHalt()
        {
            var speedController = CreateDefaultSpeedController();
            speedController.Halt();
            Assert.IsFalse(speedController.CanAccelerate);
            MonoBehaviour.Destroy(speedController.gameObject);
        }

        [Test]
        public void _11_CanAccelerateAfterHaltAndResume()
        {
            var speedController = CreateDefaultSpeedController();
            speedController.Halt();
            speedController.Resume();
            Assert.IsTrue(speedController.CanAccelerate);
            MonoBehaviour.Destroy(speedController.gameObject);
        }

        // SECTION 2
        [Test]
        public void _20_ChangeSpeedEqualsBaseSpeedTest()
        {
            var speedController = CreateDefaultSpeedController();
            speedController.ChangeSpeed(0);
            Assert.AreEqual(0f, speedController.BaseSpeed);
            MonoBehaviour.Destroy(speedController.gameObject);
        }


        [Test]
        public void _22_IsAcceleratingAfterSpeedChangedTest()
        {
            var speedController = CreateDefaultSpeedController();
            speedController.Resume();
            speedController.ChangeSpeed(int.MaxValue);
            Assert.IsTrue(speedController.IsAccelerating);
            MonoBehaviour.Destroy(speedController.gameObject);
        }

        [Test]
        public void _23_ChangeSpeedEqualsBaseSpeedTest()
        {
            var speedController = CreateDefaultSpeedController();
            var ARBITRARY_SPEED = float.MaxValue;
            speedController.ChangeSpeed(ARBITRARY_SPEED);
            Assert.AreEqual(ARBITRARY_SPEED, speedController.BaseSpeed);
            MonoBehaviour.Destroy(speedController.gameObject);
        }

        [Test]
        public void _24_ChangeSpeedWithAcceleration()
        {
            var speedController = CreateDefaultSpeedController();
            var ARBITRARY_SPEED = float.MaxValue;
            speedController.ChangeSpeed(ARBITRARY_SPEED, 0);
            Assert.AreEqual(ARBITRARY_SPEED, speedController.BaseSpeed);
            MonoBehaviour.Destroy(speedController.gameObject);
        }

        // SECTION 3

        [Test]
        public void _31_IsAcceleratingAfterSpeedImmediatelyChangedTest()
        {
            var speedController = CreateDefaultSpeedController();
            var ARBITRARY_SPEED = float.MaxValue;
            speedController.ChangeSpeedImmediately(ARBITRARY_SPEED);
            // Is it intended that IsAccelerating is always false after ChangeSpeedImmidiately() is executed
            // because it is reached immediatelly, so no acceleration involved
            Assert.IsFalse(speedController.IsAccelerating);
            MonoBehaviour.Destroy(speedController.gameObject);
        }

        [Test]
        public void _32_ChangeSpeedImmediateEqualsBaseSpeedTest()
        {
            var speedController = CreateDefaultSpeedController();
            speedController.ChangeSpeedImmediately(0);
            Assert.AreEqual(0f, speedController.BaseSpeed);
            MonoBehaviour.Destroy(speedController.gameObject);
        }

        [Test]
        public void _33_ChangeSpeedImmediateEqualsBaseSpeedTest()
        {
            var speedController = CreateDefaultSpeedController();
            var ARBITRARY_SPEED = float.MaxValue;
            speedController.ChangeSpeedImmediately(ARBITRARY_SPEED);
            Assert.AreEqual(ARBITRARY_SPEED, speedController.BaseSpeed);
            MonoBehaviour.Destroy(speedController.gameObject);
        }


        // SECTION 4

        [UnityTest]
        public IEnumerator _40_SetSpeedImmediatelyTest()
        {
            var speedController = CreateDefaultSpeedController();
            var ARBITRARY_SPEED = float.MaxValue;
            speedController.ChangeSpeedImmediately(ARBITRARY_SPEED);
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(ARBITRARY_SPEED, speedController.CurrentSpeed);
            MonoBehaviour.Destroy(speedController.gameObject);
        }

        [UnityTest]
        public IEnumerator _41_SetSpeedArbitraryWaitUntilReachedTest()
        {
            var speedController = CreateDefaultSpeedController();
            var ARBITRARY_SPEED = float.MaxValue;
            speedController.ChangeSpeed(ARBITRARY_SPEED);
            Assert.AreEqual(ARBITRARY_SPEED, speedController.BaseSpeed);
            yield return new WaitWhile(() => speedController.IsAccelerating);
            Assert.IsFalse(speedController.IsAccelerating);
            Assert.AreEqual(ARBITRARY_SPEED, speedController.BaseSpeed);
            Assert.IsTrue(speedController.TargetSpeedReached);
            Assert.AreEqual(ARBITRARY_SPEED, speedController.CurrentSpeed);
            MonoBehaviour.Destroy(speedController.gameObject);
        }

        [UnityTest]
        public IEnumerator _42_SetSpeedArbitraryTo0_IsAcceleratingTest()
        {
            var speedController = CreateDefaultSpeedController();
            var ARBITRARY_SPEED = float.MaxValue;
            speedController.ChangeSpeed(ARBITRARY_SPEED);
            yield return new WaitForEndOfFrame();

            Assert.AreNotEqual(0f, speedController.CurrentSpeed);

            speedController.ChangeSpeed(0);
            Assert.IsTrue(speedController.IsAccelerating);
            MonoBehaviour.Destroy(speedController.gameObject);
        }
    }
}
