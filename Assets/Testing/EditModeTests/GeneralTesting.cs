using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BezierSolution;
using Level;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using static Level.GameEngine;

public class GeneralTesting : MonoBehaviour
{
    // A Test behaves as an ordinary method
    [Test]
    public void SetValuesInTrafficLight()
    {
        //var trafficLight = gameObject.AddComponent<TrafficLightController>();

        var trafficLightGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLight"));
        var trafficLight = trafficLightGO.GetComponent<TrafficLightController>();

        trafficLight.SetValues(4, 6, 7);

        Assert.AreEqual(4, trafficLight.TimeRed);
        Assert.AreEqual(6, trafficLight.TimeYellow);
        Assert.AreEqual(7, trafficLight.TimeGreen);
    }

    [Test]
    public void PassValuesToTrafficLightUI()
    {
        PrepareScene(true);
        trafficLight.SetValues(3, 8, 2);
        trafficLight.OpenMenu();
        TrafficLightUIController tfuic = gameKernel.GetComponentInChildren<TrafficLightUIController>();
        Assert.AreEqual(tfuic.Red, 3);
        Assert.AreEqual(tfuic.Yellow, 8);
        Assert.AreEqual(tfuic.Green, 2);
    }

    [Test]
    public void CancelInTrafficLightUI()
    {
        var gameObject = new GameObject();

        
        GameObject trafficLightPanelGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLightPanel"));
        TrafficLightUIController trafficLightUI = trafficLightPanelGO.GetComponent<TrafficLightUIController>();
        var trafficLight = gameObject.AddComponent<TrafficLightController>();
        //trafficLight.trafficLightPanel = trafficLightPanelGO;
        trafficLight.TrafficLightUIPanel = trafficLightUI;
        trafficLight.SetValues(2, 8, 3);
        trafficLightUI.SetValues(2, 8, 3);
        //trafficLightUI.SetSender(trafficLight);
        //trafficLightUI.SetRed(6);
        ///trafficLightUI.Cancel();
        Assert.AreEqual(2, trafficLight.timeAmounts[0]);

        //Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLightUI.GetType());
        //Assert.AreEqual(2, privateTypeMyClass.GetStaticField("red"));
    }

    [Test]
    public void AcceptInTrafficLightUI()
    {
        var gameObject = new GameObject();
        GameObject trafficLightPanelGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLightPanel"));
        TrafficLightUIController trafficLightUI = trafficLightPanelGO.GetComponent<TrafficLightUIController>();
        var trafficLight = gameObject.AddComponent<TrafficLightController>();
        //trafficLight.trafficLightPanel = trafficLightPanelGO;
        trafficLight.TrafficLightUIPanel = trafficLightUI;
        trafficLight.SetValues(2, 8, 3);
        trafficLightUI.SetValues(2, 8, 3);
        trafficLightUI.Sender = trafficLight;
        trafficLightUI.Red = 6;
        trafficLightUI.Accept();
        Assert.AreEqual(6, trafficLight.TimeRed);
        trafficLightUI.Green = 4;
        trafficLightUI.Accept();
        Assert.AreEqual(4, trafficLight.TimeGreen);
        trafficLightUI.Yellow = 7;
        trafficLightUI.Accept();

        Assert.AreEqual(6, trafficLight.TimeRed);
        Assert.AreEqual(4, trafficLight.TimeGreen);
        Assert.AreEqual(7, trafficLight.TimeYellow);

        for (int i = 1; i <= 10; i++)
        {
            trafficLightUI.Red = i;
            trafficLightUI.Accept();
            Assert.AreEqual(i, trafficLight.TimeRed);
            trafficLightUI.Green = i;
            trafficLightUI.Accept();
            Assert.AreEqual(i, trafficLight.TimeGreen);
            trafficLightUI.Yellow = i;
            trafficLightUI.Accept();
            Assert.AreEqual(i, trafficLight.TimeYellow);

            var redRandom = Random.Range(1, 10);
            var greenRandom = Random.Range(1, 10);
            var yellowRandom = Random.Range(1, 10);
            trafficLightUI.Red = redRandom;
            trafficLightUI.Accept();
            Assert.AreEqual(redRandom, trafficLight.TimeRed);
            trafficLightUI.Green = greenRandom;
            trafficLightUI.Accept();
            Assert.AreEqual(greenRandom, trafficLight.TimeGreen);
            trafficLightUI.Yellow = yellowRandom;
            trafficLightUI.Accept();
            Assert.AreEqual(yellowRandom, trafficLight.TimeYellow);
        }

        //Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLightUI.GetType());
        //Assert.AreEqual(2, privateTypeMyClass.GetStaticField("red"));
    }


    [Test]
    public void AcceptInTrafficLightUI2()
    {
        var gameObject = new GameObject();
        GameObject trafficLightPanelGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLightPanel"));
        TrafficLightUIController trafficLightUI = trafficLightPanelGO.GetComponent<TrafficLightUIController>();
        var trafficLight = gameObject.AddComponent<TrafficLightController>();
        //trafficLight.trafficLightPanel = trafficLightPanelGO;
        trafficLight.TrafficLightUIPanel = trafficLightUI;
        trafficLight.SetValues(2, 8, 3);
        trafficLightUI.SetValues(5, 1, 9);
        trafficLightUI.Sender = trafficLight;
        //trafficLightUI.SetRed(6);
       // trafficLightUI.Accept();
    //    Assert.AreEqual(6, trafficLight.timeAmounts[0]);
      //  trafficLightUI.SetGreen(4);
     //   trafficLightUI.Accept();
       // Assert.AreEqual(4, trafficLight.timeAmounts[1]);
       // trafficLightUI.SetYellow(7);

        trafficLightUI.Accept();

        Assert.AreEqual(5, trafficLight.TimeRed);
        Assert.AreEqual(1, trafficLight.TimeYellow);
        Assert.AreEqual(9, trafficLight.TimeGreen);

        for (int i = 1; i <= 10; i++)
        {
            trafficLightUI.SetValues(i, i, i);
            trafficLightUI.Accept();
            Assert.AreEqual(i, trafficLight.TimeRed);
            Assert.AreEqual(i, trafficLight.TimeGreen);
            Assert.AreEqual(i, trafficLight.TimeYellow);

            var redRandom = Random.Range(1, 10);
            var greenRandom = Random.Range(1, 10);
            var yellowRandom = Random.Range(1, 10);
            trafficLightUI.SetValues(redRandom, yellowRandom, greenRandom);
            trafficLightUI.Accept();
            Assert.AreEqual(redRandom, trafficLight.TimeRed);
            Assert.AreEqual(greenRandom, trafficLight.TimeGreen);
            Assert.AreEqual(yellowRandom, trafficLight.TimeYellow);
        }

        //Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLightUI.GetType());
        //Assert.AreEqual(2, privateTypeMyClass.GetStaticField("red"));
    }

    [Test]
    public void Vector3ToDirectionTest()
    {
        // Easy ones
        Assert.AreEqual(Direction.Up, GameEngine.Vector3ToDirection(Vector3.up));
        Assert.AreEqual(Direction.Right, GameEngine.Vector3ToDirection(Vector3.right));
        Assert.AreEqual(Direction.Left, GameEngine.Vector3ToDirection(Vector3.left));
        Assert.AreEqual(Direction.Down, GameEngine.Vector3ToDirection(Vector3.down));

        Assert.AreEqual(Direction.UpRight, GameEngine.Vector3ToDirection(Vector3.one));
        Assert.AreEqual(Direction.DownRight, GameEngine.Vector3ToDirection(new Vector3(1, -1, 0)));
        Assert.AreEqual(Direction.DownLeft, GameEngine.Vector3ToDirection(new Vector3(-1, -1, 0)));
        Assert.AreEqual(Direction.UpLeft, GameEngine.Vector3ToDirection(new Vector3(-1, 1, 0)));

        // Tricky ones
        //Up
        Assert.AreEqual(Direction.Up, GameEngine.Vector3ToDirection(new Vector3(-0.5f, 2, 0)));
        Assert.AreEqual(Direction.Up, GameEngine.Vector3ToDirection(new Vector3(0.5f, 2, 0)));

        // Right
        Assert.AreEqual(Direction.Right, GameEngine.Vector3ToDirection(new Vector3(1.73f, 0.52f, 0)));
        Assert.AreEqual(Direction.Right, GameEngine.Vector3ToDirection(new Vector3(4.85f, -0.21f, 0)));

        // Down
        Assert.AreEqual(Direction.Down, GameEngine.Vector3ToDirection(new Vector3(4.85f, -25.69f, 0)));
        Assert.AreEqual(Direction.Down, GameEngine.Vector3ToDirection(new Vector3(-3.01f, -25.69f, 0)));

        // Left
        Assert.AreEqual(Direction.Left, GameEngine.Vector3ToDirection(new Vector3(-2.9f, -0.6f, 0)));
        Assert.AreEqual(Direction.Left, GameEngine.Vector3ToDirection(new Vector3(-2.9f, 0.49f, 0)));


        // UpRight
        Assert.AreEqual(Direction.UpRight, GameEngine.Vector3ToDirection(new Vector3(0.41f, 0.49f, 0)));
        Assert.AreEqual(Direction.UpRight, GameEngine.Vector3ToDirection(new Vector3(0.66f, 0.49f, 0)));

        // DownRight
        Assert.AreEqual(Direction.DownRight, GameEngine.Vector3ToDirection(new Vector3(0.99f, -0.6f, 0)));
        Assert.AreEqual(Direction.DownRight, GameEngine.Vector3ToDirection(new Vector3(0.99f, -1.25f, 0)));

        // DownLeft
        Assert.AreEqual(Direction.DownLeft, GameEngine.Vector3ToDirection(new Vector3(-0.81f, -1.25f, 0f)));
        Assert.AreEqual(Direction.DownLeft, GameEngine.Vector3ToDirection(new Vector3(-0.79f, -0.58f, 0f)));

        // UpLeft
        Assert.AreEqual(Direction.UpLeft, GameEngine.Vector3ToDirection(new Vector3(-0.79f, 0.49f, 0)));
        Assert.AreEqual(Direction.UpLeft, GameEngine.Vector3ToDirection(new Vector3(-0.79f, 1, 0)));

        // Specific cases
        Assert.AreEqual(Direction.Right, GameEngine.Vector3ToDirection(new Vector3(0.99f, -0.16f, 0)));
    }

    GameObject gameKernel;
    TrafficLightController trafficLight;
    GameEngine gameEngine;
    GameObject roadUsersGO;
    GameObject blackCar;
    VehicleController vehicle;
    LevelManager levelManager;
    private void PrepareScene(bool trafficAreasInteraction)
    {
        float tooLongTime = 200;
        gameKernel = Instantiate((GameObject)Resources.Load("Prefabs/Game Kernel"));
        if (!trafficAreasInteraction)
        {
            gameKernel.GetComponentsInChildren<TrafficLightController>().ToList().ForEach(e => e.gameObject.SetActive(false));
            gameKernel.GetComponentsInChildren<TrafficArea>().ToList().ForEach(e => e.gameObject.SetActive(false));
        }
        else
        {
            trafficLight = gameKernel.GetComponentInChildren<TrafficLightController>();
            trafficLight.SetValues(0, 0, (int)tooLongTime); // Traffic light always green
        }
        gameEngine = gameKernel.GetComponentInChildren<GameEngine>();
        roadUsersGO = GameObject.Find("RoadUsers");
        // Avoid an exception in Vehicle caused because it expects to have BezierSpline on Awake (selected in Inspector)
       // LogAssert.Expect(LogType.Exception, @"Exception: Root of Black Car(Clone)'s Bezier needs a reference to a BezierSpline component");
        blackCar = Instantiate((GameObject)Resources.Load("Prefabs/RoadUsers/Black Car"), roadUsersGO.transform);

        vehicle = blackCar?.GetComponent<VehicleController>();
        vehicle.bezier = blackCar.AddComponent<BezierWalkerWithSpeedVariant>();
        vehicle.Spline = gameKernel.GetComponentsInChildren<BezierSpline>()[2]; // Asign the Spline 2 because we want it to go left
        vehicle.enabled = true; // Set enable because it disables automatically due to the aforementioned Exception
        vehicle.TimeToLoop = tooLongTime;
        levelManager = gameKernel.GetComponentInChildren<LevelManager>();
        levelManager.TimeToSolve = tooLongTime; // Make level not solvable
        levelManager.TimeToLoop = tooLongTime;  // Make level not loopable

    }



    /*public void AcceptCancelTrafficLightUI()
    {
        var gameobject = new GameObject();

        var trafficLightUI = gameobject.AddComponent<TrafficLightUIController>();

        trafficLightUI.SetValues(2, 8, 3);

        yield return null;
    }*/
}