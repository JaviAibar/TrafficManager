using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public float timeToLoop;        // The time in which the level is looped
    public float timeToSolve = 6;   // The amount of time that must be taken to consider the level solved
    public float timeLeftToSolve;   // The amount of time still needed to consider the level solved

    public float timer;
    public GameObject solvedPanel;

    public Image iconImage;
    public TMPro.TMP_Text timeIndicator;
    // Color ambar = new Color(1, 0.7461f, 0);
    private List<RoadUser> usersStopped;
    public Sprite minimumSpeedSprite;
    public Sprite forbiddenSprite;

    public string nextLevel;

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

    private void Start()
    {
        usersStopped = new List<RoadUser>();
        SetTrafficLightIndicator(Color.green);
        LevelInit();
    }

    private void OnRoadUserStopped(RoadUser responsible)
    {
        if (usersStopped.Find(x => x.GetInstanceID() == responsible.GetInstanceID()) == null)
        {
            GameEngine.instance.Print(responsible.name + " stopped!");
            usersStopped.Add(responsible);
           // print("users stopped " + usersStopped.Count);
            ResetTimeLeftToSolve();
            SetTrafficLightIndicator(Color.red);
        }
    }
    private void OnRoadUserMoving(RoadUser responsible)
    {
        GameEngine.instance.Print(responsible.name + " is moving again (was not necessary stopped). Lets list who's still stopped");
        usersStopped.Remove(responsible);
        // print("users still stopped " + usersStopped.Count);
        GameEngine.instance.Print("There are " + usersStopped.Count + (usersStopped.Count == 1 ? " ("+usersStopped[0].name+")":" users stopped"));

        if (usersStopped.Count == 0)
        {
            SetTrafficLightIndicator(Color.green);
        }
    }

    private void OnRoadUserCollision(RoadUser affected1, RoadUser affected2)
    {
        SetTrafficLightIndicator(Color.red);
        GameEngine.instance.Print("Accident between " + affected1.name + " and " + affected2.name);
        // TODO: Restart in seconds
    }

    private void Update()
    {
        GameEngine.GameSpeed gameSpeed = GameEngine.instance.GetGameSpeed();
        if (gameSpeed != GameEngine.GameSpeed.Paused)
        {
            float timeIncrement = Time.deltaTime * (int)gameSpeed;
            timer += timeIncrement;
            if (timer >= timeToLoop)
            {
                LevelInit();
            }

            if (usersStopped.Count == 0)
            {
                timeLeftToSolve -= timeIncrement;
                if (timeLeftToSolve >= 0)
                {
                    timeIndicator.text = ((int)timeLeftToSolve).ToString();
                }
                if (timeLeftToSolve <= 0)
                {
                    LevelSolved();
                }
            }
        }
    }

    private void LevelSolved()
    {
        solvedPanel.SetActive(true);
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
    }

    public void LevelInit()
    {
        ResetTimeLeftToSolve();
        timer = 0;
        EventManager.RaiseOnLoopEnded();
    }

    public void ResetTimeLeftToSolve()
    {
        timeLeftToSolve = timeToSolve;
    }

    public void SetTrafficLightIndicator(Color colour)
    {
        if (colour == Color.red)
        {
            timeIndicator.text = "";
            iconImage.sprite = forbiddenSprite;
            ResetTimeLeftToSolve();
        } else if (colour == Color.green && iconImage.sprite == forbiddenSprite) // If we are changing from red to green
        {
            iconImage.sprite = minimumSpeedSprite;
            ResetTimeLeftToSolve();
        }
        //iconImage.color = colour;
        //shineImage.color = colour;
    }

    public void NextLevel()
    {
        throw new NotImplementedException();
    }
    public void ShowAllLevels()
    {
        SceneManager.LoadScene("Levels scene");

    }

}

