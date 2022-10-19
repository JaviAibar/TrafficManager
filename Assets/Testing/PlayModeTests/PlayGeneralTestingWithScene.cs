using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using BezierSolution;

public class PlayGeneralTestingWithScene : MonoBehaviour
{

    bool sceneLoaded;
    bool referencesSetup;
    Transform pedestrianTransform;
    TrafficLightController trafficLight;


    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("TestingScene", LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneLoaded = true;
    }

    void SetupReferences()
    {
        if (referencesSetup)
        {
            return;
        }

        Transform[] objects = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in objects)
        {
            if (t.name == "PedestrianRoot") pedestrianTransform = t;
            if (t.name == "TrafficLight") trafficLight = t.GetComponent<TrafficLightController>();
            
        }

        referencesSetup = true;
    }

    [UnityTest]
    public IEnumerator TestReferencesNotNullAfterLoad()
    {
        
        //Add all other references as well for quick nullref testing
        yield return null;
    }


    [UnityTest]
    public IEnumerator TrafficLightsTiming()
    {
        Debug.LogWarning("Testing Traffic Timming, this test takes up to 13 seconds, please be patient :)");
        Instantiate<GameObject>((GameObject)Resources.Load("Prefabs/Logic/Game Engine"));
        var gameObject = new GameObject();
        GameObject trafficLightPanelGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLightPanel"));
        TrafficLightUIController trafficLightUI = trafficLightPanelGO.GetComponent<TrafficLightUIController>();

        var trafficLight = gameObject.AddComponent<TrafficLightController>();
        //trafficLight.trafficLightPanel = trafficLightPanelGO;
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

    [UnityTest]
    public IEnumerator PedestrianInteractionTest()
    {
       // SceneManager.LoadScene("TestingScene", LoadSceneMode.Single);
        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences(); 
        yield return new WaitForSeconds(0.2f);
        Assert.IsNotNull(pedestrianTransform);
        var pedestrian = pedestrianTransform.GetComponentInChildren<PedestrianController>();
        var pedestrianBezier = pedestrianTransform.GetComponent<BezierWalkerWithSpeed>();
        Assert.IsNotNull(pedestrian);
        Assert.IsNotNull(pedestrianBezier);
        Assert.IsNotNull(trafficLight);
        
        var gameObject = new GameObject();
        var gameObject2 = new GameObject();

        // var trafficLight = GameObject.Find("TrafficLight").GetComponent<TrafficLightController>();

        trafficLight.timeOffset = 1;
        trafficLight.SetValues(0, 8, 1);
        // These are only so to fulfill the Pedestrian requisites but not used in the test
        //      gameObject2.AddComponent<BoxCollider2D>();
        //      gameObject2.AddComponent<Rigidbody2D>();
        // var pedestrian = GameObject.Find("PedestrianRoot").GetComponent<PedestrianController>();

        Assert.IsTrue(pedestrian.respectsTheRules);

        Assert.AreEqual(pedestrian.normalSpeed, pedestrianBezier.speed);
        yield return new WaitForSeconds(1);
        Assert.AreEqual(expected: 0f, pedestrianBezier.speed);

        //Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLightUI.GetType());
        //Assert.AreEqual(2, privateTypeMyClass.GetStaticField("red"));
    }
}
// Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLight.GetType());

//((SpriteRenderer)privateTypeMyClass.("spriteRenderer")).sprite