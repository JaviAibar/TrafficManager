using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class GeneralTesting : MonoBehaviour
{
    // A Test behaves as an ordinary method
    [Test]
    public void SetValuesInTrafficLight()
    {
        var gameObject = new GameObject();

        var trafficLight = gameObject.AddComponent<TrafficLightController>();

        trafficLight.SetValues(4, 6, 7);

        Assert.AreEqual(4, trafficLight.trafficLightTimeAmounts[0]);
        Assert.AreEqual(6, trafficLight.trafficLightTimeAmounts[1]);
        Assert.AreEqual(7, trafficLight.trafficLightTimeAmounts[2]);
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
        Assert.AreEqual(2, trafficLight.trafficLightTimeAmounts[0]);

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
        Assert.AreEqual(6, trafficLight.trafficLightTimeAmounts[0]);

        //Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLightUI.GetType());
        //Assert.AreEqual(2, privateTypeMyClass.GetStaticField("red"));
    }

    [Test]
    public void Vector3ToDirectionTest()
    {
        print(Quaternion.LookRotation(new Vector3(3.2f - 0.1f, 2)).eulerAngles);

        // Easy ones
        Assert.AreEqual(GameEngine.Direction.Up, GameEngine.Vector3ToDirection(Vector3.up));
        Assert.AreEqual(GameEngine.Direction.Right, GameEngine.Vector3ToDirection(Vector3.right));
        Assert.AreEqual(GameEngine.Direction.Left, GameEngine.Vector3ToDirection(Vector3.left));
        Assert.AreEqual(GameEngine.Direction.Down, GameEngine.Vector3ToDirection(Vector3.down));

        Assert.AreEqual(GameEngine.Direction.UpRight, GameEngine.Vector3ToDirection(Vector3.one));
        Assert.AreEqual(GameEngine.Direction.DownRight, GameEngine.Vector3ToDirection(new Vector3(1, -1, 0)));
        Assert.AreEqual(GameEngine.Direction.DownLeft, GameEngine.Vector3ToDirection(new Vector3(-1, -1, 0)));
        Assert.AreEqual(GameEngine.Direction.UpLeft, GameEngine.Vector3ToDirection(new Vector3(-1, 1, 0)));

        // Tricky ones
        //Up
        Assert.AreEqual(GameEngine.Direction.Up, GameEngine.Vector3ToDirection(new Vector3(-0.5f, 2, 0)));
        Assert.AreEqual(GameEngine.Direction.Up, GameEngine.Vector3ToDirection(new Vector3(0.5f, 2, 0)));

        // Right
        Assert.AreEqual(GameEngine.Direction.Right, GameEngine.Vector3ToDirection(new Vector3(1.73f, 0.52f, 0)));
        Assert.AreEqual(GameEngine.Direction.Right, GameEngine.Vector3ToDirection(new Vector3(4.85f, -0.21f, 0)));

        // Down
        Assert.AreEqual(GameEngine.Direction.Down, GameEngine.Vector3ToDirection(new Vector3(4.85f, -25.69f, 0)));
        Assert.AreEqual(GameEngine.Direction.Down, GameEngine.Vector3ToDirection(new Vector3(-3.01f, -25.69f, 0)));

        // Left
        Assert.AreEqual(GameEngine.Direction.Left, GameEngine.Vector3ToDirection(new Vector3(-2.9f, -0.6f, 0)));
        Assert.AreEqual(GameEngine.Direction.Left, GameEngine.Vector3ToDirection(new Vector3(-2.9f, 0.49f, 0)));


        // UpRight
        Assert.AreEqual(GameEngine.Direction.UpRight, GameEngine.Vector3ToDirection(new Vector3(0.41f, 0.49f, 0)));
        Assert.AreEqual(GameEngine.Direction.UpRight, GameEngine.Vector3ToDirection(new Vector3(0.66f, 0.49f, 0)));

        // DownRight
        Assert.AreEqual(GameEngine.Direction.DownRight, GameEngine.Vector3ToDirection(new Vector3(0.99f, -0.6f, 0)));
        Assert.AreEqual(GameEngine.Direction.DownRight, GameEngine.Vector3ToDirection(new Vector3(0.99f, -1.25f, 0)));

        // DownLeft
        Assert.AreEqual(GameEngine.Direction.DownLeft, GameEngine.Vector3ToDirection(new Vector3(-0.81f, -1.25f, 0f)));
        Assert.AreEqual(GameEngine.Direction.DownLeft, GameEngine.Vector3ToDirection(new Vector3(-0.79f, -0.58f, 0f)));

        // UpLeft
        Assert.AreEqual(GameEngine.Direction.UpLeft, GameEngine.Vector3ToDirection(new Vector3(-0.79f, 0.49f, 0)));
        Assert.AreEqual(GameEngine.Direction.UpLeft, GameEngine.Vector3ToDirection(new Vector3(-0.79f, 1, 0)));
    }
    

    /*public void AcceptCancelTrafficLightUI()
    {
        var gameobject = new GameObject();

        var trafficLightUI = gameobject.AddComponent<TrafficLightUIController>();

        trafficLightUI.SetValues(2, 8, 3);

        yield return null;
    }*/
}