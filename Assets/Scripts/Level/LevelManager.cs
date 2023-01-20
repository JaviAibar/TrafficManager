using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Level.GameEngine;
namespace Level
{
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
        public TMP_Text timeIndicator;
        // Color ambar = new Color(1, 0.7461f, 0);
        private List<RoadUser> stoppedUsers;
        public Sprite minimumSpeedSprite;
        public Sprite forbiddenSprite;

        [SerializeField] private string nextLevel;
        public string NextLevel => nextLevel;
        public List<ParticleSystem> particleSystems;
        private bool blockedResolvabilityUntilRestart = false;
        public GameObject clock;
        public bool unsolvable;
        public TMP_Text levelTitle;

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
            stoppedUsers = new List<RoadUser>();
        }

        private void Start()
        {
            levelTitle.text = SceneManager.GetActiveScene().name;
            LevelInit();
            if (unsolvable) Debug.LogWarning("Level set to unsolvable!");
        }
        private void OnRoadUserStopped(RoadUser responsible)
        {
            if (WasAlreadyStopped(responsible)) return;
            Print(responsible.name + " stopped!", VerboseEnum.SolutionConditions);
            stoppedUsers.Add(responsible);
            SetSolvedIndicator(false);
        }
        private void OnRoadUserMoving(RoadUser responsible)
        {
            stoppedUsers.Remove(responsible);
            string stoppedUsersString = ": "+string.Join(',',stoppedUsers.Select(e => e.name));
            Print($"{responsible.name} is moving again (was not necessary stopped).Lets list who's still stopped\nThere are {stoppedUsers.Count}{(stoppedUsers.Count > 0 ? stoppedUsersString : " users stopped")}", VerboseEnum.SolutionConditions);

            if (stoppedUsers.Count == 0)
                SetSolvedIndicator(true);
        }

        private void OnRoadUserCollision(RoadUser affected1, RoadUser affected2)
        {
            SetSolvedIndicator(false);
            blockedResolvabilityUntilRestart = true;
            Print("Accident between " + affected1.name + " and " + affected2.name, VerboseEnum.Physics);
            timer = realTimeToLoop - timeToResetLevelAfterCollision * (int)instance.Speed; // Set some seconds, to understand the situation
            GameObject.Find("Jukebox")?.GetComponent<AudioSource>().Stop(); // TODO: When Music Controll is complete, change this
            if (affected1 is PedestrianController pedestrian)
                pedestrian.BeRunOver();
            StartCoroutine(nameof(ActivateClock));
            /* affected1.bezier.speed = 0;
         affected2.bezier.speed = 0;
         affected2.GetHitMovement(affected1.transform.up);
         affected1.GetHitMovement(affected1.transform.up);*/
        }

        private void FixedUpdate()
        {
            if (timeToSolve <= 0) return;
            GameSpeed gameSpeed = GameEngine.instance.Speed;
            if (gameSpeed == GameSpeed.Paused) return;

            float timeIncrement = Time.fixedDeltaTime * (int)gameSpeed;
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
                Print($"Level reset", VerboseEnum.GameTrace);
                LevelInit();
            }

            if (stoppedUsers.Count != 0 || blockedResolvabilityUntilRestart || unsolvable) return;

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
            Print("Level solved: activating panel", VerboseEnum.GameTrace);
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
            blockedResolvabilityUntilRestart = false;
          //  ResetTimeLeftToSolve();
          ResetTimeLeftToSolve();
            SetSolvedIndicator(true);

           // SetSolvedIndicator(true);
            EventManager.RaiseOnLoopStarted();
        }

        public void ResetTimeLeftToSolve()
        {
            timeLeftToSolve = timeToSolve;
            timeIndicator.text = ((int)timeLeftToSolve).ToString();
            timer = 0;
        }

        public void SetSolvedIndicator(bool conditionsToSolveSatisfied)
        {
            if (!conditionsToSolveSatisfied)
            {
                timeIndicator.text = "";
                iconImage.sprite = forbiddenSprite;
                if (blockedResolvabilityUntilRestart) return;
                if ((int)timeLeftToSolve < 3)
                {
                    soundFx.PlayFailSound();
                }
                ResetTimeLeftToSolve();
            }
            else if (iconImage.sprite == forbiddenSprite && !blockedResolvabilityUntilRestart) // If we are changing from red to green
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
            yield return new WaitForSeconds(timeToResetLevelAfterCollision * (int)instance.Speed);
            clock.SetActive(false);
        }

        public bool WasAlreadyStopped(RoadUser responsible)
        {
            return stoppedUsers.Find(x => x.GetInstanceID() == responsible.GetInstanceID());
        }
    }
}

