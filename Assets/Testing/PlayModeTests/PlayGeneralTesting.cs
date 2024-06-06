using System;
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
    #region SceneElements

    GameObject gameKernel;
    GameEngine gameEngine;
    GameObject roadUsersGO;
    GameObject blackCar;
    GameObject pedestrian1;
    VehicleController vehicle;
    PedestrianController pedestrian;
    LevelManager levelManager;
    BezierWalkerWithSpeedVariant bezier;
    TrafficLightController trafficLight;

    [System.Flags]
    private enum RoadUserType
    {
        None = ~1,
        Vehicle = 1 << 1,
        Pedestrian = 1 << 2,
        Everything = ~0
    }

    #endregion

    private const float tooLongTime = 200;
    float Epsilon => 0.11f;
    private bool FinishLineReached => 1 - bezier.NormalizedT <= Epsilon;
    private bool FinishLineNotReached => !FinishLineReached;

    public bool CantStartMoving => vehicle ? !vehicle.CanStartMoving : !pedestrian.CanStartMoving;
    public bool Accelerating => vehicle ? vehicle.Accelerating : pedestrian.Accelerating;
    public bool Looping => vehicle ? vehicle.Looping : pedestrian.Looping;
    public bool CantMove => vehicle ? !vehicle.CanMove : !pedestrian.CanMove;

    #region ReturnVariablesForCoroutines

    private float t = 0;
    private float normalDuration = 0;
    private float fastDuration = 0;
    private float fastestDuration = 0;

    #endregion

    [UnityTest]
    public IEnumerator ScenesHistoryTesting()
    {
        // expected values
        string mainMenuSceneName = "Main Menu";
        string levelsSceneName = "Levels Scene";
        string level1SceneName = "Level001";
        string tutorialSceneName = "Tutorial 1";

        MenuController menu = Instantiate(Resources.Load("Prefabs/UI/Menu controller") as MenuController);
        yield return WaitForStart();

        // Initial values
        Assert.AreEqual(-1, HistoryTracker.instance.GetHistoryIndex());
        Assert.AreEqual(0, HistoryTracker.instance.GetSavedScenes());

        // Loading Main Menu (default scene)
        // Current state index = 0 | [0] Main menu (meaning after scene loaded)
        menu.LoadMainMenuScene();
        yield return new WaitForSeconds(1);
        CheckExpectedScenes(0, 1, mainMenuSceneName);

        // Loading Levels scene
        // Saving Main Menu at index 0
        // Current state index = 1 | [0] Main menu, [1] Levels
        menu.LoadLevelsScene();
        yield return new WaitForSeconds(1);
        CheckExpectedScenes(1, 2, levelsSceneName);

        // Loading Tutorial
        // Current state index = 2 | [0] Main menu, [1] Levels, [2] Tutorial
        menu.LoadTutorial();
        yield return new WaitForSeconds(1);
        CheckExpectedScenes(2, 3, tutorialSceneName);

        // Loading Level001
        // Current state index = 3 | [0] Main menu, [1] Levels, [2] Tutorial, [3] Level001
        menu.LoadLevel(1);
        yield return new WaitForSeconds(1);
        CheckExpectedScenes(3, 4, level1SceneName);

        // Go to previous scene (Tutorial)
        // Current state index = 2 | [0] Main menu, [1] Levels, [2] Tutorial, [3] Level001
        HistoryTracker.instance.LoadPreviousScene();
        yield return new WaitForSeconds(1);
        CheckExpectedScenes(2, 4, tutorialSceneName);

        // Go to previous scene (Levels)
        // Current state index = 1 | [0] Main menu, [1] Levels, [2] Tutorial, [3] Level001
        HistoryTracker.instance.LoadPreviousScene();
        yield return new WaitForSeconds(1);
        CheckExpectedScenes(1, 4, levelsSceneName);


        // Go to previous scene (Levels)
        // Current state index = 0 | [0] Main menu, [1] Levels, [2] Tutorial, [3] Level001
        HistoryTracker.instance.LoadPreviousScene();
        yield return new WaitForSeconds(1);
        CheckExpectedScenes(0, 4, mainMenuSceneName);

        // Go to next scene (Levels)
        // Current state index = 1 | [0] Main menu, [1] Levels, [2] Tutorial, [3] Level001
        HistoryTracker.instance.LoadNextScene();
        yield return new WaitForSeconds(1);
        CheckExpectedScenes(1, 4, levelsSceneName);

        // Load Main menu Scene (Main menu)
        // Current state index = 2 | [0] Main menu, [1] Levels, [2] Main menu (should replace history with the Main menu in position 2)
        menu.LoadMainMenuScene();
        yield return new WaitForSeconds(1);
        CheckExpectedScenes(2, 3, levelsSceneName);
    }

    [UnityTest]
    public IEnumerator MINIScenesHistoryTesting()
    {
        // expected values
        string mainMenuSceneName = "Main Menu";
        /* string levelsSceneName = "Levels Scene";
         string level1SceneName = "Level001";
         string tutorialSceneName = "Tutorial Scene";*/
        MenuController menu = Instantiate(Resources.Load("Prefabs/UI/Menu controller")) as MenuController;
        yield return new WaitForSeconds(1);
        // Initial values
        Assert.AreEqual(-1, HistoryTracker.instance.GetHistoryIndex());
        Assert.AreEqual(0, HistoryTracker.instance.GetSavedScenes());


        menu.LoadMainMenuScene();
        yield return new WaitForSeconds(1);
        CheckExpectedScenes(0, 1, mainMenuSceneName);

        menu.LoadLevel(1);
        yield return new WaitForSeconds(1);


        menu.LoadLevel(1);
        yield return new WaitForSeconds(1);
        throw new NotImplementedException();
    }

    [UnityTest]
    public IEnumerator TrafficLightsTiming()
    {
        Debug.LogWarning("Testing Traffic Timing, this test takes up to 13 seconds, please be patient :)");
        yield return PrepareScene(true);
        /*gameKernel = Instantiate((GameObject)Resources.Load("Prefabs/Game Kernel"));
        LevelManager lm = gameKernel.GetComponentInChildren<LevelManager>();
        lm.timeToSolve = 100;
        lm.timeToLoop = 100;
        print(gameKernel.GetComponentInChildren<GameEngine>());
        //trafficLight.trafficLightUIPanel = trafficLightUI;
        trafficLight = gameKernel.GetComponentInChildren<TrafficLightController>();*/
        //TrafficLightUIController TLUIController = GameKernel.GetComponentInChildren<TrafficLightUIController>();
        trafficLight.SetValues(2, 8, 3);

        var expectedNameRed = "TrafficLight - Red";
        var expectedNameYellow = "TrafficLight - Yellow";
        var expectedNameGreen = "TrafficLight - Green";

        yield return WaitAndCheckTrafficLight(1, expectedNameRed);
        yield return WaitAndCheckTrafficLight(1, expectedNameGreen);
        yield return WaitAndCheckTrafficLight(3, expectedNameYellow);
        yield return WaitAndCheckTrafficLight(8, expectedNameRed);
    }

    [UnityTest]
    public IEnumerator GameSpeedChangingVehicleTesting()
    {
        var speed = 20;
        yield return PrepareScene(false, RoadUserType.Vehicle);
        //gameEngine.verbose = GameEngine.VerboseEnum.Speed | GameEngine.VerboseEnum.GameTrace;

        yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.Normal, speed);
        yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.Fast, speed);
        yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.SuperFast, speed);
        PrintTimesSpeed();

        Assert.IsTrue(Approx(normalDuration / 2, fastDuration));
        Assert.IsTrue(Approx(normalDuration / 3, fastestDuration));
    }

    [UnityTest]
    public IEnumerator GameSpeedChangingVehicleTesting2()
    {
        float[] speedsToTest = { 10, 3, 30, 50, 80 };
        //float normalizedTCompletedLoop = 0.95f;
        yield return PrepareScene(trafficAreasInteraction: false, RoadUserType.Vehicle);
        // gameEngine.verbose = (GameEngine.VerboseEnum)17;
        foreach (float s in speedsToTest)
        {
            yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.Normal, s);
            yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.Fast, s);
            yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.SuperFast, s);
            print($"With speed: {s}");
            PrintTimesSpeed();

            Assert.IsTrue(Approx(normalDuration / 2, fastDuration));
            Assert.IsTrue(Approx(normalDuration / 3, fastestDuration));
        }
    }

    [UnityTest]
    public IEnumerator GameSpeedChangingPedestrianTesting()
    {
        var speed = 20;
        yield return PrepareScene(false, RoadUserType.Pedestrian);
        gameEngine.Verbose = GameEngine.VerboseEnum.Speed | GameEngine.VerboseEnum.GameTrace;

        yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.Normal, speed);
        yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.Fast, speed);
        yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.SuperFast, speed);
        PrintTimesSpeed();

        Assert.IsTrue(Approx(normalDuration / 2, fastDuration));
        Assert.IsTrue(Approx(normalDuration / 3, fastestDuration));
    }

    [UnityTest]
    public IEnumerator GameSpeedChangingPedestrianTesting2()
    {
        float[] speedsToTest = { 10, 3, 30, 50, 80 };
        //float normalizedTCompletedLoop = 0.95f;
        yield return PrepareScene(trafficAreasInteraction: false, RoadUserType.Pedestrian);
        gameEngine.Verbose = GameEngine.VerboseEnum.Speed | GameEngine.VerboseEnum.GameTrace;
        foreach (float s in speedsToTest)
        {
            yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.Normal, s);
            yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.Fast, s);
            yield return CalculateTimesSpeedRoadUser(GameEngine.GameSpeed.SuperFast, s);
            print($"With speed: {s}");
            PrintTimesSpeed();

            Assert.IsTrue(Approx(normalDuration / 2, fastDuration));
            Assert.IsTrue(Approx(normalDuration / 3, fastestDuration));
        }
    }

    [UnityTest]
    public IEnumerator SpeedChangingVehicleTesting()
    {
        float[] speedsToTest = new float[] { 1, 10, 5, 30, 0, 50 };
        yield return PrepareScene(false, RoadUserType.Vehicle);

        gameEngine.Verbose = GameEngine.VerboseEnum.GameTrace | GameEngine.VerboseEnum.Speed;

        gameEngine.Speed = GameEngine.GameSpeed.Normal;
        //yield return new WaitForEndOfFrame(); // Wait for all the corresponding Awake and Start

        yield return WaitWhileCantMove();
        yield return WaitWhileAccelerating();
        bezier.NormalizedT = 0;
        foreach (float s in speedsToTest)
        {
            vehicle.ChangeSpeed(s, 0.2f);
            yield return WaitWhileAccelerating();
            print($"{s} ahora {vehicle.CurrentSpeed} {Mathf.Approximately(s, vehicle.CurrentSpeed)}");
            Assert.True(Approx(s, vehicle.CurrentSpeed));
        }
    }

    [UnityTest]
    public IEnumerator SpeedChangingPedestrianTesting()
    {
        float[] speedsToTest = new float[] { 1, 10, 5, 30, 0, 50 };
        yield return PrepareScene(false, RoadUserType.Pedestrian);

        gameEngine.Verbose = GameEngine.VerboseEnum.GameTrace | GameEngine.VerboseEnum.Speed;

        gameEngine.Speed = GameEngine.GameSpeed.Normal;
        //yield return new WaitForEndOfFrame(); // Wait for all the corresponding Awake and Start

        yield return WaitWhileCantMove();
        yield return WaitWhileAccelerating();
        bezier.NormalizedT = 0;
        foreach (float s in speedsToTest)
        {
            pedestrian.ChangeSpeed(s, 0.2f);
            yield return WaitWhileAccelerating();
            print($"{s} ahora {pedestrian.CurrentSpeed} {Mathf.Approximately(s, pedestrian.CurrentSpeed)}");
            Assert.True(Approx(s, pedestrian.CurrentSpeed));
        }
    }

    [UnityTest]
    public IEnumerator LoopVehicleTest()
    {
        yield return PrepareScene(false, RoadUserType.Vehicle);
        //gameEngine.verbose = GameEngine.VerboseEnum.GameTrace | GameEngine.VerboseEnum.Speed;
        Vector3 initPos = new Vector3(49.38f, 4.31f, 0.00f);
        yield return WaitWhileFinishLineNotReached();
        yield return WaitWhileOnFinishLine();
        // yield return new WaitForSeconds(1);
        print(
            $"Expected pos: {initPos}, Actual pos: {vehicle.transform.position}, diff: {(initPos - vehicle.transform.position).magnitude}");
        Assert.IsTrue((initPos - vehicle.transform.position).magnitude < Epsilon);
    }

    [UnityTest]
    public IEnumerator LoopPedestrianTest()
    {
        yield return PrepareScene(false, RoadUserType.Pedestrian);
        //gameEngine.verbose = GameEngine.VerboseEnum.GameTrace | GameEngine.VerboseEnum.Speed;
        Vector3 initPos = new Vector3(-4.62f, 33.00f, 0.00f);
        yield return WaitWhileFinishLineNotReached();
        yield return WaitWhileOnFinishLine();
        // yield return new WaitForSeconds(1);
        print(
            $"Expected pos: {initPos}, Actual pos: {pedestrian.transform.position}, diff: {(initPos - pedestrian.transform.position).magnitude}");
        Assert.IsTrue((initPos - pedestrian.transform.position).magnitude < Epsilon);
    }

    #region Helper methods

    private IEnumerator PrepareScene(bool trafficAreasInteraction, RoadUserType roadUserType = RoadUserType.None)
    {
        gameKernel = Instantiate((GameObject)Resources.Load("Prefabs/Game Kernel"));
        if (trafficAreasInteraction)
            SetUpTrafficLight();
        else
            DeactivateTrafficLightsAndAreas();

        gameEngine = gameKernel.GetComponentInChildren<GameEngine>();
        switch (roadUserType)
        {
            case RoadUserType.Vehicle:
                SetUpVehicle();
                break;
            case RoadUserType.Pedestrian:
                SetUpPedestrian();
                break;
        }

        SetUpLevelManager();

        yield return WaitForStart();
    }


    private void DeactivateTrafficLightsAndAreas()
    {
        trafficLight = gameKernel.GetComponentInChildren<TrafficLightController>();
        trafficLight.SetValues(0, 0, (int)tooLongTime); // Traffic light always green
        gameKernel.GetComponentsInChildren<TrafficArea>().ToList().ForEach(e => e.gameObject.SetActive(false));
    }

    private void SetUpTrafficLight()
    {
        trafficLight = gameKernel.GetComponentInChildren<TrafficLightController>();
        trafficLight.SetValues(0, 0, (int)tooLongTime); // Traffic light always green
    }

    private void SetUpPedestrian()
    {
        roadUsersGO = GameObject.Find("RoadUsers");
        // Avoid an exception in Vehicle caused because it expects to have BezierSpline on Awake (selected in Inspector)
        LogAssert.Expect(LogType.Exception,
            @"Exception: Root of Pedestrian1(Clone)'s Bezier needs a reference to a BezierSpline component");
        pedestrian1 = Instantiate((GameObject)Resources.Load("Prefabs/RoadUsers/Pedestrian1"), roadUsersGO.transform);

        pedestrian = pedestrian1.GetComponent<PedestrianController>();
        pedestrian.Spline =
            gameKernel.GetComponentsInChildren<BezierSpline>()[1]; // Asign the Spline 2 because we want it to go left
        pedestrian.enabled = true; // Set enable because it disables automatically due to the aforementioned Exception
        pedestrian.TimeToLoop = tooLongTime;
        bezier = pedestrian.GetComponent<BezierWalkerWithSpeedVariant>();
    }

    private void SetUpVehicle()
    {
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
        bezier = blackCar.GetComponent<BezierWalkerWithSpeedVariant>();
    }

    public void SetUpLevelManager()
    {
        levelManager = gameKernel.GetComponentInChildren<LevelManager>();
        levelManager.TimeToSolve = tooLongTime; // Make level not solvable
        levelManager.TimeToLoop = tooLongTime; // Make level not loopable
    }

    public void CheckExpectedScenes(int indexHistory, int scenesSaved, string sceneName)
    {
        Assert.AreEqual(indexHistory, HistoryTracker.instance.GetHistoryIndex());
        Assert.AreEqual(scenesSaved, HistoryTracker.instance.GetSavedScenes());
        Assert.AreEqual(sceneName, SceneManager.GetActiveScene().name);
    }

    private IEnumerator WaitAndCheckTrafficLight(float timeToWait, string expectedName)
    {
        yield return new WaitForSeconds(timeToWait);
        Assert.AreEqual(expectedName, trafficLight.image.sprite.name);
    }

    private IEnumerator CalculateTimesSpeedRoadUser(GameEngine.GameSpeed gameSpeed, float speed)
    {
        yield return ChangeGameSpeedAndWaitRoadUser(gameSpeed);
        t = Time.time;
        if (vehicle) vehicle.ChangeSpeed(speed); // Acceleration is included in this test
        else pedestrian.ChangeSpeed(speed);
        yield return WaitWhileFinishLineNotReached();
        yield return WaitWhileOnFinishLine();
        switch (gameSpeed)
        {
            case GameEngine.GameSpeed.Normal:
                normalDuration = Time.time - t;
                break;
            case GameEngine.GameSpeed.Fast:
                fastDuration = Time.time - t;
                break;
            case GameEngine.GameSpeed.SuperFast:
                fastestDuration = Time.time - t;
                break;
        }
    }

    private IEnumerator ChangeGameSpeedAndWaitRoadUser(GameEngine.GameSpeed gameSpeed)
    {
        gameEngine.Speed = gameSpeed;
        if (vehicle) vehicle.LoopStarted();
        else pedestrian.LoopStarted();
        // Finished looping, start accelerating to 20 (default normal speed)
        yield return WaitWhileLooping();
        yield return WaitWhileCantMove();
        // Finished looping, start accelerating to 20 (default normal speed)
        //  yield return WaitWhileAccelerating();    // Finished accelerating normal Speed (we will set the right one afterwards)
        bezier.NormalizedT = 0;
        //yield return WaitWhileOnFinishLine();
    }


    private void PrintTimesSpeed()
    {
        print(
            $"normal duration {normalDuration}, fast {fastDuration} expected fast: {normalDuration / 2} actual diff: {(normalDuration / 2) - fastDuration} -> {(Approx((normalDuration / 2), fastDuration) ? "PASS" : "NO-PASS")}");
        print(
            $"normal duration {normalDuration}, fastest {fastestDuration} expected fastest: {normalDuration / 3} actual diff: {(normalDuration / 3) - fastestDuration} -> {(Approx((normalDuration / 3), fastestDuration) ? "PASS" : "NO-PASS")}");
    }

    private IEnumerator SetGameToSpeedAndVehicleToSpeedAndPosition(float speed, GameEngine.GameSpeed gameSpeed)
    {
        gameEngine.Speed = gameSpeed;
        if (!vehicle.Looping) vehicle.LoopStarted();
        yield return WaitWhileCantStartMoving();
        yield return WaitWhileAccelerating();
        bezier.NormalizedT = 0;
        vehicle.ChangeSpeed(speed);
        yield return WaitWhileAccelerating();
    }

    #endregion

    #region Utilities

    public bool Approx(float val1, float val2)
    {
        return Math.Abs(val1 - val2) < Epsilon;
    }

    public bool Approx(float val1, float val2, float epsilon)
    {
        return Math.Abs(val1 - val2) < epsilon;
    }

    #endregion

    #region WaitingMethods

    public IEnumerator WaitWhileFinishLineNotReached()
    {
        while (FinishLineNotReached)
        {
            yield return null;
        }
    }

    public IEnumerator WaitWhileOnFinishLine()
    {
        while (FinishLineReached)
        {
            print("Still in finish line");
            yield return null;
        }

        // yield return WaitForStart();
    }

    private IEnumerator WaitWhileCantStartMoving()
    {
        while (CantStartMoving)
        {
            print("Can't start moving yet");
            yield return null;
        }
    }

    private IEnumerator WaitWhileAccelerating()
    {
        while (Accelerating)
        {
            print("Not yet accelerated");
            yield return null;
        }
    }

    private IEnumerator WaitWhileLooping()
    {
        while (Looping)
        {
            print("Still looping");

            yield return null;
        }
    }

    private IEnumerator WaitWhileCantMove()
    {
        while (CantMove)
        {
            print("Cant move yet");
            yield return null;
        }
    }


    private IEnumerator WaitForStart()
    {
        yield return new WaitForFixedUpdate();
    }

    #endregion

    // Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType privateTypeMyClass = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(trafficLight.GetType());

    //((SpriteRenderer)privateTypeMyClass.("spriteRenderer")).sprite
}