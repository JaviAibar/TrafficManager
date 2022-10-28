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

    [UnityTest]
    public IEnumerator ScenesHistoryTesting()
    {
        // expected values
        string mainMenuSceneName = "Main Menu";
        string levelsSceneName = "Levels Scene";
        string level1SceneName = "Level001";
        string tutorialSceneName = "Tutorial Scene";
        Instantiate((GameObject)Resources.Load("Prefabs/UI/Menu controller"));
        yield return new WaitForSeconds(1);
        // Initial values
        Assert.AreEqual(-1, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(0, MenuController.instance.GetSavedScenes());

        // Loading Main Menu (default scene)
        // Current state index = 0 | [0] Main menu (meaning after scene loaded)
        MenuController.instance.LoadMainMenuScene();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(0, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(1, MenuController.instance.GetSavedScenes());
        Assert.AreEqual(mainMenuSceneName, SceneManager.GetActiveScene().name);

        // Loading Levels scene
        // Saving Main Menu at index 0
        // Current state index = 1 | [0] Main menu, [1] Levels
        MenuController.instance.LoadLevelsScene();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(1, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(2, MenuController.instance.GetSavedScenes());
        Assert.AreEqual(levelsSceneName, SceneManager.GetActiveScene().name);


        // Loading New Game again (Tutorial)
        // Current state index = 2 | [0] Main menu, [1] Levels, [2] Tutorial
        MenuController.instance.NewGame();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(2, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(3, MenuController.instance.GetSavedScenes());
        Assert.AreEqual(tutorialSceneName, SceneManager.GetActiveScene().name);

        // Loading Level001
        // Current state index = 3 | [0] Main menu, [1] Levels, [2] Tutorial, [3] Level001
        MenuController.instance.LoadLevel(1);
        yield return new WaitForSeconds(1);
        Assert.AreEqual(3, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(4, MenuController.instance.GetSavedScenes());
        Assert.AreEqual(level1SceneName, SceneManager.GetActiveScene().name);

        // Go to previous scene (Tutorial)
        // Current state index = 2 | [0] Main menu, [1] Levels, [2] Tutorial, [3] Level001
        MenuController.instance.LoadPreviousScene();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(2, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(4, MenuController.instance.GetSavedScenes());
        Assert.AreEqual(tutorialSceneName, SceneManager.GetActiveScene().name);

        // Go to previous scene (Levels)
        // Current state index = 1 | [0] Main menu, [1] Levels, [2] Tutorial, [3] Level001
        MenuController.instance.LoadPreviousScene();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(1, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(4, MenuController.instance.GetSavedScenes());
        Assert.AreEqual(levelsSceneName, SceneManager.GetActiveScene().name);


        // Go to previous scene (Levels)
        // Current state index = 0 | [0] Main menu, [1] Levels, [2] Tutorial, [3] Level001
        MenuController.instance.LoadPreviousScene();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(0, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(4, MenuController.instance.GetSavedScenes());
        Assert.AreEqual(mainMenuSceneName, SceneManager.GetActiveScene().name);

        // Go to next scene (Levels)
        // Current state index = 1 | [0] Main menu, [1] Levels, [2] Tutorial, [3] Level001
        MenuController.instance.LoadNextScene();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(1, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(4, MenuController.instance.GetSavedScenes());
        Assert.AreEqual(levelsSceneName, SceneManager.GetActiveScene().name);


        // Load Main menu Scene (Main menu)
        // Current state index = 2 | [0] Main menu, [1] Levels, [2] Main menu (should replace history with the Main menu in position 2)
        MenuController.instance.LoadMainMenuScene();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(2, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(3, MenuController.instance.GetSavedScenes());
        Assert.AreEqual(levelsSceneName, SceneManager.GetActiveScene().name);


        /*  var gameObject = new GameObject();
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
          Assert.AreNotEqual(expectedNameYellow, trafficLight.image.sprite.name);*/
    }

    [UnityTest]
    public IEnumerator MINIScenesHistoryTesting()
    {
        // expected values
        string mainMenuSceneName = "Main Menu";
        string levelsSceneName = "Levels Scene";
        string level1SceneName = "Level001";
        string tutorialSceneName = "Tutorial Scene";
        Instantiate((GameObject)Resources.Load("Prefabs/UI/Menu controller"));
        yield return new WaitForSeconds(1);
        // Initial values
        Assert.AreEqual(-1, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(0, MenuController.instance.GetSavedScenes());


        MenuController.instance.LoadMainMenuScene();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(0, MenuController.instance.GetHistoryIndex());
        Assert.AreEqual(1, MenuController.instance.GetSavedScenes());
        Assert.AreEqual(mainMenuSceneName, SceneManager.GetActiveScene().name);

        MenuController.instance.LoadLevel(1);
        yield return new WaitForSeconds(1);


        MenuController.instance.LoadLevel(1);
        yield return new WaitForSeconds(1);

    }


    // Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLight.GetType());

    //((SpriteRenderer)privateTypeMyClass.("spriteRenderer")).sprite
}