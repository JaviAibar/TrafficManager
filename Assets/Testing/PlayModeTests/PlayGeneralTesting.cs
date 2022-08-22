using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayGeneralTesting : MonoBehaviour
{
    [UnityTest]
    public IEnumerator TrafficLightsTiming()
    {
        Debug.LogWarning("Testing Traffic Timming, this test takes up to 13 seconds, please be patient :)");
        Instantiate<GameObject>((GameObject)Resources.Load("Prefabs/Logic/Game Engine"));
        var gameObject = new GameObject();
        GameObject trafficLightPanelGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLightPanel"));
        TrafficLightUIController trafficLightUI = trafficLightPanelGO.GetComponent<TrafficLightUIController>();

        var trafficLight = gameObject.AddComponent<TrafficLightController>();
        trafficLight.trafficLightPanel = trafficLightPanelGO;
        trafficLight.trafficLightUIPanel = trafficLightUI;
        trafficLight.SetValues(2, 8, 3);

        yield return new WaitForSeconds(1);
        Assert.AreEqual("Red", trafficLight.image.sprite.name);

        Assert.AreNotEqual("Green", trafficLight.image.sprite.name);
        Assert.AreNotEqual("Yellow", trafficLight.image.sprite.name);

        yield return new WaitForSeconds(1);
        Assert.AreEqual("Green", trafficLight.image.sprite.name);

        Assert.AreNotEqual("Red", trafficLight.image.sprite.name);
        Assert.AreNotEqual("Yellow", trafficLight.image.sprite.name);

        yield return new WaitForSeconds(3);
        Assert.AreEqual("Yellow", trafficLight.image.sprite.name);

        Assert.AreNotEqual("Green", trafficLight.image.sprite.name);
        Assert.AreNotEqual("red", trafficLight.image.sprite.name);

        yield return new WaitForSeconds(8);
        Assert.AreEqual("Red", trafficLight.image.sprite.name);

        Assert.AreNotEqual("Green", trafficLight.image.sprite.name);
        Assert.AreNotEqual("Yellow", trafficLight.image.sprite.name);
    }
}
// Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLight.GetType());

//((SpriteRenderer)privateTypeMyClass.("spriteRenderer")).sprite