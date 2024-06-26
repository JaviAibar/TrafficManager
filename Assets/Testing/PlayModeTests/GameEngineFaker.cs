using BezierSolution;
using Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameEngineFaker
{
    private static GameEngineFaker instance;
    public GameObject GameKernel { get; internal set; }
    public GameEngine GameEngine { get; internal set; }

    public BezierWalkerWithSpeedVariant Bezier { get; internal set; }
    public BezierSpline BezierSpline { get; internal set; }
    private GameEngineFaker() { }
    public static GameEngineFaker CreateDefaultPlayground()
    {
        if (GameEngineFaker.instance == null) GameEngineFaker.instance = new GameEngineFaker();
        GameEngineFaker.instance.Init();
        return instance;
    }

    private void Init()
    {
        if (GameKernel != null)
        {
           // MonoBehaviour.Destroy(BezierSpline.gameObject);
           // onoBehaviour.Destroy(Bezier.gameObject);
           // MonoBehaviour.Destroy(GameEngine.gameObject);
            MonoBehaviour.Destroy(GameKernel);
        }
        Debug.Break();

        GameKernel = MonoBehaviour.Instantiate((GameObject)Resources.Load("Prefabs/Game Kernel"));
        SetLevelManagerUnsolvable();
        GameEngine = GameKernel.GetComponentInChildren<GameEngine>();
        Debug.Log(GameEngine);
        BezierSpline = GameKernel.GetComponentInChildren<BezierSolution.BezierSpline>();
    }

    public BezierWalkerWithSpeedVariant GetBezier()
    {
        return Bezier;
    }

    public void SetBezier(RoadUser roadUser)
    {
        Bezier = roadUser.GetComponent<BezierWalkerWithSpeedVariant>();
    }

    public void SetLevelManagerUnsolvable()
    {
        float TOO_LONG_TIME = 200;
        var levelManager = GameKernel.GetComponentInChildren<LevelManager>();
        levelManager.TimeToSolve = TOO_LONG_TIME; // Make level not solvable
        levelManager.TimeToLoop = TOO_LONG_TIME; // Make level not loopable
    }
}

