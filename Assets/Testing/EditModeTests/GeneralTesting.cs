using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using static GameEngine;

public class GeneralTesting : MonoBehaviour
{
    // A Test behaves as an ordinary method
    [Test]
    public void SetValuesInTrafficLight()
    {
        var gameObject = new GameObject();

        //var trafficLight = gameObject.AddComponent<TrafficLightController>();

        var trafficLightGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLight"));
        var trafficLight = trafficLightGO.GetComponent<TrafficLightController>();

        trafficLight.SetValues(4, 6, 7);

        Assert.AreEqual(4, trafficLight.timeAmounts[0]);
        Assert.AreEqual(6, trafficLight.timeAmounts[1]);
        Assert.AreEqual(7, trafficLight.timeAmounts[2]);
    }

    [Test]
    public void CancelInTrafficLightUI()
    {
        var gameObject = new GameObject();

        
        GameObject trafficLightPanelGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLightPanel"));
        TrafficLightUIController trafficLightUI = trafficLightPanelGO.GetComponent<TrafficLightUIController>();
        var trafficLight = gameObject.AddComponent<TrafficLightController>();
        //trafficLight.trafficLightPanel = trafficLightPanelGO;
        trafficLight.trafficLightUIPanel = trafficLightUI;
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
        trafficLight.trafficLightUIPanel = trafficLightUI;
        trafficLight.SetValues(2, 8, 3);
        trafficLightUI.SetValues(2, 8, 3);
        trafficLightUI.SetSender(trafficLight);
        trafficLightUI.SetRed(6);
        trafficLightUI.Accept();
        Assert.AreEqual(6, trafficLight.timeAmounts[0]);
        trafficLightUI.SetGreen(4);
        trafficLightUI.Accept();
        Assert.AreEqual(4, trafficLight.timeAmounts[1]);
        trafficLightUI.SetYellow(7);
        trafficLightUI.Accept();

        Assert.AreEqual(6, trafficLight.timeAmounts[0]);
        Assert.AreEqual(4, trafficLight.timeAmounts[1]);
        Assert.AreEqual(7, trafficLight.timeAmounts[2]);

        for (int i = 1; i <= 10; i++)
        {
            trafficLightUI.SetRed(i);
            trafficLightUI.Accept();
            Assert.AreEqual(i, trafficLight.timeAmounts[0]);
            trafficLightUI.SetGreen(i);
            trafficLightUI.Accept();
            Assert.AreEqual(i, trafficLight.timeAmounts[1]);
            trafficLightUI.SetYellow(i);
            trafficLightUI.Accept();
            Assert.AreEqual(i, trafficLight.timeAmounts[2]);

            var redRandom = Random.Range(1, 10);
            var greenRandom = Random.Range(1, 10);
            var yellowRandom = Random.Range(1, 10);
            trafficLightUI.SetRed(redRandom);
            trafficLightUI.Accept();
            Assert.AreEqual(redRandom, trafficLight.timeAmounts[0]);
            trafficLightUI.SetGreen(greenRandom);
            trafficLightUI.Accept();
            Assert.AreEqual(greenRandom, trafficLight.timeAmounts[1]);
            trafficLightUI.SetYellow(yellowRandom);
            trafficLightUI.Accept();
            Assert.AreEqual(yellowRandom, trafficLight.timeAmounts[2]);
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

    

    /*public void AcceptCancelTrafficLightUI()
    {
        var gameobject = new GameObject();

        var trafficLightUI = gameobject.AddComponent<TrafficLightUIController>();

        trafficLightUI.SetValues(2, 8, 3);

        yield return null;
    }*/
}