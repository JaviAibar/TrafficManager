using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BezierSolution;
using System.Linq;
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

    private bool FinishLineReached => 1 - bezier.NormalizedT <= HelperUtilities.Epsilon;
    private bool FinishLineNotReached => !FinishLineReached;

    public bool CantStartMoving => vehicle ? !vehicle.CanStartMoving : !pedestrian.CanStartMoving;
    public bool Accelerating => vehicle ? vehicle.Accelerating : pedestrian.Accelerating;
    public bool Looping => vehicle ? vehicle.Looping : pedestrian.Looping;
    public bool CantMove => vehicle ? !vehicle.CanMove : !pedestrian.CanMove;



    #region Helper methods

   /* private IEnumerator PrepareScene(bool trafficAreasInteraction, RoadUserType roadUserType = RoadUserType.None)
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
    }*/
    

    
    

    private IEnumerator WaitAndCheckTrafficLight(float timeToWait, string expectedName)
    {
        yield return new WaitForSeconds(timeToWait);
        Assert.AreEqual(expectedName, trafficLight.image.sprite.name);
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