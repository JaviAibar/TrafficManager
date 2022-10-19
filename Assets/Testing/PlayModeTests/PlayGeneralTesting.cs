using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using BezierSolution;
using UnityEngine.UI;

public class PlayGeneralTesting : MonoBehaviour
{

    bool sceneLoaded;
    bool referencesSetup;
    Transform pedestrianTransform;
    TrafficLightController trafficLight;


 

    [UnityTest]
    public IEnumerator TrafficLightsTiming()
    {
        var expectedNameRed = "TrafficLight - Red";
        var expectedNameYellow = "TrafficLight - Yellow";
        var expectedNameGreen = "TrafficLight - Green";
        Debug.LogWarning("Testing Traffic Timming, this test takes up to 13 seconds, please be patient :)");
        Instantiate((GameObject)Resources.Load("Prefabs/Logic/Game Engine"));
        var gameObject = new GameObject();
        GameObject trafficLightPanelGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLightPanel"));
        TrafficLightUIController trafficLightUI = trafficLightPanelGO.GetComponent<TrafficLightUIController>();

       
        var trafficLightGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLight"));
        var trafficLight = trafficLightGO.GetComponent<TrafficLightController>();
        //trafficLight.trafficLightPanel = trafficLightPanelGO;
        trafficLight.trafficLightUIPanel = trafficLightUI;
        trafficLight.SetValues(2, 8, 3);

        yield return new WaitForSeconds(1);
        Assert.AreEqual(expectedNameRed, trafficLight.image.sprite.name);

        Assert.AreNotEqual(expectedNameGreen, trafficLight.image.sprite.name);
        Assert.AreNotEqual(expectedNameYellow, trafficLight.image.sprite.name);

        yield return new WaitForSeconds(1);
        Assert.AreEqual(expectedNameGreen, trafficLight.image.sprite.name);

        Assert.AreNotEqual(expectedNameRed, trafficLight.image.sprite.name);
        Assert.AreNotEqual(expectedNameYellow, trafficLight.image.sprite.name);

        yield return new WaitForSeconds(3);
        Assert.AreEqual(expectedNameYellow, trafficLight.image.sprite.name);

        Assert.AreNotEqual(expectedNameGreen, trafficLight.image.sprite.name);
        Assert.AreNotEqual(expectedNameRed, trafficLight.image.sprite.name);

        yield return new WaitForSeconds(8);
        Assert.AreEqual(expectedNameRed, trafficLight.image.sprite.name);

        Assert.AreNotEqual(expectedNameGreen, trafficLight.image.sprite.name);
        Assert.AreNotEqual(expectedNameYellow, trafficLight.image.sprite.name);
    }

}
// Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLight.GetType());

//((SpriteRenderer)privateTypeMyClass.("spriteRenderer")).sprite