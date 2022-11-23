using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using BezierSolution;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.InteropServices;
using Level;

public class PlayGeneralTesting : MonoBehaviour
{
    bool sceneLoaded;
    bool referencesSetup;
    Transform pedestrianTransform;
    TrafficLightController trafficLight;
    bool waiting;
    float Epsilon => 0.13f;
    
    private float t = 0;
    private float normalDuration = 0;
    private float fastDuration = 0;
    private float fastestDuration = 0;

    [UnityTest]
    public IEnumerator ScenesHistoryTesting()
    {
        // expected values
        string mainMenuSceneName = "Main Menu";
        string levelsSceneName = "Levels Scene";
        string level1SceneName = "Level001";
        string tutorialSceneName = "Tutorial 1";
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


        // Loading Tutorial
        // Current state index = 2 | [0] Main menu, [1] Levels, [2] Tutorial
        MenuController.instance.LoadTutorial();
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

    [UnityTest]
    public IEnumerator TrafficLightsTiming()
    {
        Debug.LogWarning("Testing Traffic Timming, this test takes up to 13 seconds, please be patient :)");
        /*GameEngine engine = (Instantiate((GameObject)Resources.Load("Prefabs/Logic/Game Engine"))).GetComponent<GameEngine>();
        var gameObject = new GameObject();
        GameObject trafficLightPanelGO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/TrafficLightPanel"));
        TrafficLightUIController trafficLightUI = trafficLightPanelGO.GetComponent<TrafficLightUIController>();
        ((GameObject)Instantiate(Resources.Load("Prefabs/UI/Time Control Panel Root"));
        var trafficLight = gameObject.AddComponent<TrafficLightController>();
        //trafficLight.trafficLightPanel = trafficLightPanelGO;*/
        GameObject gameKernel = Instantiate((GameObject)Resources.Load("Prefabs/Game Kernel"));
        LevelManager lm = gameKernel.GetComponentInChildren<LevelManager>();
        lm.timeToSolve = 100;
        lm.timeToLoop = 100;
        print(gameKernel.GetComponentInChildren<GameEngine>());
        //trafficLight.trafficLightUIPanel = trafficLightUI;
        trafficLight = gameKernel.GetComponentInChildren<TrafficLightController>();
        //TrafficLightUIController TLUIController = GameKernel.GetComponentInChildren<TrafficLightUIController>();
        trafficLight.SetValues(2, 8, 3);

        var expectedNameRed = "TrafficLight - Red";
        var expectedNameYellow = "TrafficLight - Yellow";
        var expectedNameGreen = "TrafficLight - Green";

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
    public IEnumerator GameSpeedChangingCarTesting()
    {
        var speed = 20;
        yield return PrepareScene(trafficAreasInteraction: false);
        gameEngine.verbose = (GameEngine.VerboseEnum)17;
        
        yield return CalculateTimesSpeed(GameEngine.GameSpeed.Normal, speed);
        yield return CalculateTimesSpeed(GameEngine.GameSpeed.Fast, speed);
        yield return CalculateTimesSpeed(GameEngine.GameSpeed.SuperFast, speed);
        PrintTimesSpeed();
        
        Assert.IsTrue(normalDuration / 2 - fastDuration < Epsilon);
        Assert.IsTrue(normalDuration / 3 - fastestDuration < Epsilon);
    }

    public IEnumerator CalculateTimesSpeed(GameEngine.GameSpeed gameSpeed, float speed)
    {
        yield return GameSpeedChangingCarTestingSetVehicle(gameSpeed);
        t = Time.realtimeSinceStartup;
        vehicle.ChangeSpeed(speed); // Acceleration is included in this test
        yield return new WaitWhile(() => 1 - bezier.NormalizedT >= double.Epsilon);
        switch (gameSpeed)
        {
            case GameEngine.GameSpeed.Normal:
                normalDuration = Time.realtimeSinceStartup - t; break;
            case GameEngine.GameSpeed.Fast:
                fastDuration = Time.realtimeSinceStartup - t; break;
            case GameEngine.GameSpeed.SuperFast:
                fastestDuration = Time.realtimeSinceStartup - t; break;
        }
    }

    public void PrintTimesSpeed()
    {
        print($"normal duration {normalDuration}, fast {fastDuration} expected fast: {normalDuration/2} actual diff: { (normalDuration/2)- fastDuration} -> {((normalDuration / 2) - fastDuration < Epsilon ? "PASS": "NO-PASS")}");
        print($"normal duration {normalDuration}, fastest {fastestDuration} expected fastest: {normalDuration/3} actual diff: {(normalDuration/3) - fastestDuration} -> {((normalDuration / 3) - fastestDuration < Epsilon ? "PASS": "NO-PASS")}");
    }

    private IEnumerator GameSpeedChangingCarTestingSetVehicle(GameEngine.GameSpeed gameSpeed)
    {
        gameEngine.Speed = gameSpeed;
        if (!vehicle.Looping) vehicle.LoopStarted();
        yield return new WaitWhile(() => vehicle.Looping); // Finished looping, start accelerating to 20 (default normal speed)
        yield return new WaitWhile(() => !vehicle.HasWaitedEnough); // Finished looping, start accelerating to 20 (default normal speed)
        
        // Accelerating is part of the test!
       // yield return new WaitWhile(() => vehicle.Accelerating);    // Finished accelerating
        bezier.NormalizedT = 0;
    }

    [UnityTest]
    public IEnumerator GameSpeedChangingCarTesting2()
    {
        float[] speedsToTest = { 10, 3, 30, 50, 80 };
        //float normalizedTCompletedLoop = 0.95f;
        yield return PrepareScene(trafficAreasInteraction: false);
       // gameEngine.verbose = (GameEngine.VerboseEnum)17;
        foreach (float s in speedsToTest)
        {

            yield return CalculateTimesSpeed(GameEngine.GameSpeed.Normal, s);
            yield return CalculateTimesSpeed(GameEngine.GameSpeed.Fast, s);
            yield return CalculateTimesSpeed(GameEngine.GameSpeed.SuperFast, s);
            print($"With speed: {s}");
            PrintTimesSpeed();

            Assert.IsTrue(normalDuration / 2 - fastDuration < Epsilon);
            Assert.IsTrue(normalDuration / 3 - fastestDuration < Epsilon);
        }
    }
    private IEnumerator SetGameToSpeedAndVehicleToSpeedAndPosition(float speed, GameEngine.GameSpeed gameSpeed)
    {
        gameEngine.Speed = gameSpeed;
        if (!vehicle.Looping) vehicle.LoopStarted();
        yield return new WaitWhile(() => !vehicle.HasWaitedEnough);
        yield return new WaitWhile(() => vehicle.Accelerating);
        bezier.NormalizedT = 0;
        vehicle.ChangeSpeed(speed);
        yield return new WaitWhile(() => vehicle.Accelerating);
    }
    
    

    GameObject gameKernel;
    GameEngine gameEngine;
    GameObject roadUsersGO;
    GameObject blackCar;
    VehicleController vehicle;
    LevelManager levelManager;
    BezierWalkerWithSpeedVariant bezier;

    [UnityTest]
    public IEnumerator SpeedChangingCarTesting()
    {
        float normalizedTCompletedLoop = 0.95f;
        float epsilon = 0.1f;
        float[] speedsToTest = new float[] { 1, 10, 5, 30, 0, 50 };
        yield return PrepareScene(false);
      //  gameEngine.verbose = (GameEngine.VerboseEnum)17;
      
        gameEngine.Speed = GameEngine.GameSpeed.Normal;
        //yield return new WaitForEndOfFrame(); // Wait for all the corresponding Awake and Start

        yield return new WaitWhile(() => vehicle.HasWaitedEnough);
        yield return new WaitWhile(() => vehicle.Accelerating);
        bezier.NormalizedT = 0;
        foreach (float s in speedsToTest)
        {
            vehicle.ChangeSpeed(s, 0.1f);
            yield return new WaitWhile(() => vehicle.Accelerating);
            print($"{s} {Mathf.Approximately(s, vehicle.ActualSpeed)}");
            Assert.True(Mathf.Approximately(s, vehicle.ActualSpeed));
        }
    }

    private IEnumerator PrepareScene(bool trafficAreasInteraction)
    {
        float tooLongTime = 200;
        gameKernel = Instantiate((GameObject)Resources.Load("Prefabs/Game Kernel"));
        if (!trafficAreasInteraction)
        {
            gameKernel.GetComponentsInChildren<TrafficLightController>().ToList()
                .ForEach(e => e.gameObject.SetActive(false));
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
        LogAssert.Expect(LogType.Exception,
            @"Exception: Root of Black Car(Clone)'s Bezier needs a reference to a BezierSpline component");
        blackCar = Instantiate((GameObject)Resources.Load("Prefabs/RoadUsers/Black Car"), roadUsersGO.transform);

        vehicle = blackCar.GetComponent<VehicleController>();
        vehicle.Spline =
            gameKernel.GetComponentsInChildren<BezierSpline>()[2]; // Asign the Spline 2 because we want it to go left
        vehicle.enabled = true; // Set enable because it disables automatically due to the aforementioned Exception
        vehicle.TimeToLoop = tooLongTime;
        levelManager = gameKernel.GetComponentInChildren<LevelManager>();
        levelManager.timeToSolve = tooLongTime; // Make level not solvable
        levelManager.timeToLoop = tooLongTime; // Make level not loopable

        bezier = blackCar.GetComponent<BezierWalkerWithSpeedVariant>();
        
        yield return new WaitForEndOfFrame();
    }

    // Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLight.GetType());

    //((SpriteRenderer)privateTypeMyClass.("spriteRenderer")).sprite
}