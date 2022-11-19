using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using static GameEngine;
public class LevelManager : MonoBehaviour
{
    public float timeToLoop;        // The time in which the level is looped
    private float realTimeToLoop;
    public float timeToSolve = 6;   // The amount of time that must be taken to consider the level solved
    public float timeLeftToSolve;   // The amount of time still needed to consider the level solved
    public float timeToResetLevelAfterCollision = 2f;

    public float timer;
    public GameObject solvedPanel;
    private SoundFxManager soundFx;

    public Image iconImage;
    public TMPro.TMP_Text timeIndicator;
    // Color ambar = new Color(1, 0.7461f, 0);
    private List<RoadUser> usersStopped;
    public Sprite minimumSpeedSprite;
    public Sprite forbiddenSprite;

    [SerializeField] private string nextLevel;
    public string NextLevel => nextLevel;
    public List<ParticleSystem> particleSystems;
    private bool blockedResolvabilityUntilRestart = false;
    public GameObject clock;

    private void OnEnable()
    {
        EventManager.OnRoadUserStopped += OnRoadUserStopped;
        EventManager.OnRoadUserMoving += OnRoadUserMoving;
        EventManager.OnRoadUserCollision += OnRoadUserCollision;
    }

    private void OnDisable()
    {
        EventManager.OnRoadUserStopped -= OnRoadUserStopped;
        EventManager.OnRoadUserMoving -= OnRoadUserMoving;
        EventManager.OnRoadUserCollision -= OnRoadUserCollision;
    }

    private void Awake()
    {
        soundFx = FindObjectOfType<SoundFxManager>();
    }

    private void Start()
    {
        usersStopped = new List<RoadUser>();
        SetSolvedIndicator(true);
        LevelInit();
    }
    private void OnRoadUserStopped(RoadUser responsible)
    {
        if (usersStopped.Find(x => x.GetInstanceID() == responsible.GetInstanceID()) == null)
        {
            Print(responsible.name + " stopped!");
            usersStopped.Add(responsible);
            // print("users stopped " + usersStopped.Count);
            SetSolvedIndicator(false);
        }
    }
    private void OnRoadUserMoving(RoadUser responsible)
    {
        Print(responsible.name + " is moving again (was not necessary stopped). Lets list who's still stopped");
        usersStopped.Remove(responsible);
        // print("users still stopped " + usersStopped.Count);
        Print("There are " + usersStopped.Count + (usersStopped.Count == 1 ? " (" + usersStopped[0].name + ")" : " users stopped"));

        if (usersStopped.Count == 0)
        {
            SetSolvedIndicator(true);
        }
    }

    private void OnRoadUserCollision(RoadUser affected1, RoadUser affected2)
    {
        SetSolvedIndicator(false);
        blockedResolvabilityUntilRestart = true;
        Print("Accident between " + affected1.name + " and " + affected2.name);
        timer = realTimeToLoop - timeToResetLevelAfterCollision; // Set some seconds, to understand the situation
        GameObject.Find("Jukebox")?.GetComponent<AudioSource>().Stop(); // TODO: When Music Controll is complete, change this
        if (affected1 is PedestrianController)
        {
            ((PedestrianController)affected1).BeRanOver();
        }
        StartCoroutine("ActivateClock");
        /* affected1.bezier.speed = 0;
         affected2.bezier.speed = 0;
         affected2.GetHitMovement(affected1.transform.up);
         affected1.GetHitMovement(affected1.transform.up);*/
    }

    private void Update()
    {
        if (timeToSolve <= 0) return;
        GameSpeed gameSpeed = GameEngine.instance.Speed;
        if (gameSpeed == GameSpeed.Paused) return;

        float timeIncrement = Time.deltaTime * (int)gameSpeed;
        timer += timeIncrement;
        realTimeToLoop = timeToLoop;
        if (timeToLoop <= 0)
        {
            /* List<TrafficLightController> trafficLightControllers = FindObjectsOfType<TrafficLightController>().ToList();
             List<int> trafficLightTimes = trafficLightControllers.Select(e => e.GetTotalAmountTime()).ToList();
             realTimeToLoop = trafficLightTimes.Max();*/
            throw new Exception("Time to loop in Level Manager must never be 0 or below!");
        }
        if (timer >= realTimeToLoop)
        {
            LevelInit();
        }

        if (usersStopped.Count != 0 || blockedResolvabilityUntilRestart) return;

        timeLeftToSolve -= timeIncrement;
        int timeInt = (int)timeLeftToSolve;

        soundFx.PlaySolvedSound(timeInt);
        if (timeLeftToSolve >= 0)
        {
            timeIndicator.text = ((int)timeLeftToSolve).ToString();
        }
        if (timeLeftToSolve <= 0)
        {
            LevelSolved();
        }
    }

    private void LevelSolved()
    {
        solvedPanel.SetActive(true);
        ActivateParticleSystems();
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
        GameEngine.instance.ChangeSpeed(GameSpeed.Paused);
    }

    private void ActivateParticleSystems()
    {
        particleSystems.ForEach(e => e.Play());
    }

    public void LevelInit()
    {
        EventManager.RaiseOnLoopStarted();
        ResetTimeLeftToSolve();
        timer = 0;
    }

    public void ResetTimeLeftToSolve()
    {
        timeLeftToSolve = timeToSolve;
        blockedResolvabilityUntilRestart = false;
    }

    public void SetSolvedIndicator(bool conditionsToSolveFullfilled)
    {
        if (blockedResolvabilityUntilRestart)
        {
            timeIndicator.text = "";
            iconImage.sprite = forbiddenSprite;
            return;
        }
        if (!conditionsToSolveFullfilled)
        {
            timeIndicator.text = "";
            iconImage.sprite = forbiddenSprite;
            if ((int)timeLeftToSolve < 3)
            {
                soundFx.PlayFailSound();
            }
            ResetTimeLeftToSolve();
        }
        else if (conditionsToSolveFullfilled && iconImage.sprite == forbiddenSprite) // If we are changing from red to green
        {
            iconImage.sprite = minimumSpeedSprite;
            ResetTimeLeftToSolve();
        }
        //iconImage.color = colour;
        //shineImage.color = colour;
    }

    public void GoNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevel))
        {
            SceneManager.LoadScene(nextLevel);
        }
    }
    public void ShowAllLevels()
    {
        SceneManager.LoadScene("Levels scene");
    }

    public IEnumerator ActivateClock()
    {
        clock.SetActive(true);
        yield return new WaitForSeconds(timeToResetLevelAfterCollision);
        clock.SetActive(false);
    }
}

