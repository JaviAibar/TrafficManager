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

        Assert.AreEqual(4, trafficLight.red);
        Assert.AreEqual(6, trafficLight.yellow);
        Assert.AreEqual(7, trafficLight.green);
    }

    

    [Test]
    public void AccumulativeCalculationInTrafficLight()
    {
        var gameObject = new GameObject();

        var trafficLight = gameObject.AddComponent<TrafficLightController>();

        trafficLight.SetValues(4, 6, 7);

        Assert.AreEqual(4, trafficLight.red);
        Assert.AreEqual(11, trafficLight.accumulativeGreen);
        Assert.AreEqual(17, trafficLight.accumulativeYellow);


        /*Assert.AreEqual(10, privateTypeMyClass.);
        Assert.AreEqual(13, privateTypeMyClass.green);*/

    }
    [Test]
    public void CancelInTrafficLightUI()
    {
        var gameObject = new GameObject();

        
        GameObject trafficLightPanelGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLightPanel"));
        TrafficLightUIController trafficLightUI = trafficLightPanelGO.GetComponent<TrafficLightUIController>();
        var trafficLight = gameObject.AddComponent<TrafficLightController>();
        trafficLight.trafficLightPanel = trafficLightPanelGO;
        trafficLight.trafficLightUIPanel = trafficLightUI;
        trafficLight.SetValues(2, 8, 3);
        trafficLightUI.SetValues(2, 8, 3);
        //trafficLightUI.SetSender(trafficLight);
        //trafficLightUI.SetRed(6);
        ///trafficLightUI.Cancel();
        Assert.AreEqual(2, trafficLight.red);

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
        trafficLight.trafficLightPanel = trafficLightPanelGO;
        trafficLight.trafficLightUIPanel = trafficLightUI;
        trafficLight.SetValues(2, 8, 3);
        trafficLightUI.SetValues(2, 8, 3);
        trafficLightUI.SetSender(trafficLight);
        trafficLightUI.SetRed(6);
        trafficLightUI.Accept();
        Assert.AreEqual(6, trafficLight.red);

        //Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLightUI.GetType());
        //Assert.AreEqual(2, privateTypeMyClass.GetStaticField("red"));
    }

    
    /*public void AcceptCancelTrafficLightUI()
    {
        var gameobject = new GameObject();

        var trafficLightUI = gameobject.AddComponent<TrafficLightUIController>();

        trafficLightUI.SetValues(2, 8, 3);

        yield return null;
    }*/
}