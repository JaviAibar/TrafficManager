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
using static Level.TrafficLightController;

namespace UnitTests
{
    public class TrafficLightsTesting
    {
        /*  private void DeactivateTrafficLightsAndAreas()
       {
           trafficLight = gameKernel.GetComponentInChildren<TrafficLightController>();
           trafficLight.SetValues(0, 0, (int)tooLongTime); // Traffic light always green
           gameKernel.GetComponentsInChildren<TrafficArea>().ToList().ForEach(e => e.gameObject.SetActive(false));
       }*/


        [Test]
        public void _00_TrafficLightsDefaultStateTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            Assert.AreEqual(TrafficLightColour.Off, trafficLight.State);
        }

        [Test]
        public void _01_TrafficLightsSettingStateRedDefaultTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            trafficLight.State = TrafficLightColour.Red;

            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);
        }

        [Test]
        public void _02_TrafficLightsSettingStateYellowDefaultTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            trafficLight.State = TrafficLightColour.Yellow;

            Assert.AreEqual(TrafficLightColour.Yellow, trafficLight.State);
        }

        [Test]
        public void _03_TrafficLightsSettingStateGreenDefaultTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            trafficLight.State = TrafficLightColour.Green;

            Assert.AreEqual(TrafficLightColour.Green, trafficLight.State);
        }



        [UnityTest]
        public IEnumerator _10_TrafficLightsOffToRedTimingDefaultTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            Assert.AreEqual(TrafficLightColour.Off, trafficLight.State);
            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);

        }

        [UnityTest]
        public IEnumerator _11_TrafficLightsRedToGreenTimingDefaultTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            trafficLight.State = TrafficLightColour.Red;

            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);
            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Green, trafficLight.State);

        }

        [UnityTest]
        public IEnumerator _12_TrafficLightsGreenToYellowTimingDefaultTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            trafficLight.State = TrafficLightColour.Green;

            Assert.AreEqual(TrafficLightColour.Green, trafficLight.State);
            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Yellow, trafficLight.State);
        }



        [UnityTest]
        public IEnumerator _13_TrafficLightsYellowToToRedTimingDefaultTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            trafficLight.State = TrafficLightColour.Yellow;


            Assert.AreEqual(TrafficLightColour.Yellow, trafficLight.State);
            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);
        }


        [UnityTest]
        public IEnumerator _20_TrafficLightsCompleteCycleTimingDefaultTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);
            /*gameKernel = Instantiate((GameObject)Resources.Load("Prefabs/Game Kernel"));
            LevelManager lm = gameKernel.GetComponentInChildren<LevelManager>();
            lm.timeToSolve = 100;
            lm.timeToLoop = 100;
            print(gameKernel.GetComponentInChildren<GameEngine>());
            //trafficLight.trafficLightUIPanel = trafficLightUI;
            trafficLight = gameKernel.GetComponentInChildren<TrafficLightController>();*/
            //TrafficLightUIController TLUIController = GameKernel.GetComponentInChildren<TrafficLightUIController>();
            //trafficLight.SetValues(2, 8, 3);

            /* var expectedNameRed = "TrafficLight - Red";
             var expectedNameYellow = "TrafficLight - Yellow";
             var expectedNameGreen = "TrafficLight - Green";*/

            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);

            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Green, trafficLight.State);

            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Yellow, trafficLight.State);

            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);

            /*yield return WaitAndCheckTrafficLight(1, expectedNameRed);
            yield return WaitAndCheckTrafficLight(1, expectedNameGreen);
            yield return WaitAndCheckTrafficLight(3, expectedNameYellow);
            yield return WaitAndCheckTrafficLight(8, expectedNameRed);*/
        }

        [UnityTest]
        public IEnumerator _21_TrafficLightsCompleteCycleTimingSetting0SecondsTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            trafficLight.SetValues(0, 0, 0);


            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);

            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Green, trafficLight.State);

            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Yellow, trafficLight.State);

            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);

        }

        [UnityTest]
        public IEnumerator _22_TrafficLightsCompleteCycleTimingSetting1SecondTest()
        {
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            trafficLight.SetValues(1, 1, 1);

            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);

            yield return new WaitForSeconds(2);
            Assert.AreEqual(TrafficLightColour.Green, trafficLight.State);

            yield return new WaitForSeconds(2);
            Assert.AreEqual(TrafficLightColour.Yellow, trafficLight.State);

            yield return new WaitForSeconds(2);
            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);
        }

        [UnityTest]
        public IEnumerator _23_TrafficLightsCompleteCycleTimingSettingArbitrarySecondsTest([Values(new int[] { 1, 2, 3 }, new int[] { 3, 2, 1 }, new int[] { 1, 3, 1 }, new int[] { 3, 1, 3 })] int[] speeds)
        {
            Debug.LogWarning("Testing Traffic Timing, this test takes up some time, please be patient :)");
            GameEngineFaker gameEngineFaker = GameEngineFaker.CreateDefaultPlayground();
            var trafficLight = CreateDefaultTrafficLight(gameEngineFaker);

            trafficLight.SetValues(speeds[0], speeds[1], speeds[2]);

            var firstAndLastWaiting = speeds[0] + 1;
            var secondsWaiting = speeds[1] + 1;
            var thirdWaiting = speeds[2] + 1;

            yield return new WaitForSeconds(1);
            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);

            yield return new WaitForSeconds(secondsWaiting);
            Assert.AreEqual(TrafficLightColour.Green, trafficLight.State);

            yield return new WaitForSeconds(thirdWaiting);
            Assert.AreEqual(TrafficLightColour.Yellow, trafficLight.State);

            yield return new WaitForSeconds(firstAndLastWaiting);
            Assert.AreEqual(TrafficLightColour.Red, trafficLight.State);
        }

        private TrafficLightController CreateDefaultTrafficLight(in GameEngineFaker gameEngineFaker)
        {
            var trafficLight = gameEngineFaker.GameKernel.GetComponentInChildren<TrafficLightController>();
            return trafficLight;
        }

        /*   private IEnumerator WaitAndCheckTrafficLight(float timeToWait, string expectedName)
           {
               yield return new WaitForSeconds(timeToWait);

               //Assert.AreEqual(expectedName, trafficLight.image.sprite.name);
           }*/
    }
}
