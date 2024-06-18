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
        [SerializeField] private float timeToLoop;        // The time in which the level is looped (in principle, contant)
        [SerializeField] private float timeToSolve = 6;   // The amount of time that must be taken to consider the level solved (in principle, contant)
        [SerializeField] private float timeToResetLevelAfterCollision = 2f;
        [SerializeField] private float timer;

        [SerializeField] private GameObject solvedPanel;
        [SerializeField] private TMP_Text timeIndicator;
        // Color ambar = new Color(1, 0.7461f, 0);
        [SerializeField] private Sprite minimumSpeedSprite;
        [SerializeField] private Sprite forbiddenSprite;
        [SerializeField] private List<ParticleSystem> particleSystems;

        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject clock;
        [SerializeField] private TMP_Text levelTitle;
        
        [SerializeField] private string nextLevel;
        [SerializeField] private bool unsolvable;

        private float timeLeftToLoop;
        private float timeLeftToSolve;   // The amount of time still needed to consider the level solved
        private SoundFxManager soundFx;
        private List<RoadUser> stoppedUsers;
        private bool blockedResolvabilityUntilRestart = false;

        public string NextLevel => nextLevel;
        public bool ConditionsToSolve => stoppedUsers.Count == 0 && !blockedResolvabilityUntilRestart && !unsolvable;
        public bool GameRunning => timeToSolve > 0 && GameEngine.Instance.IsRunning;
        // The time in which the level is looped (in principle, contant)
        public float TimeToLoop
        {
            get => timeToLoop;
            set => timeToLoop = value;
        }
        // The amount of time that must be taken to consider the level solved (in principle, contant)
        public float TimeToSolve
        {
            get => timeToSolve;
            set => timeToSolve = value;
        }
        public float TimeToResetLevelAfterCollision => timeToResetLevelAfterCollision;
        public float Timer => timer;

        public TMP_Text TimeIndicator
        {
            get => timeIndicator;
            set => timeIndicator = value;
        }

        public Image IconImage {
            get => iconImage; 
            set => iconImage = value;
        }

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
            string stoppedUsersString = ": " + string.Join(',', stoppedUsers.Select(e => e.name));
            Print($"{responsible.name} is moving again (was not necessary stopped).Lets list who's still stopped\nThere are {stoppedUsers.Count}{(stoppedUsers.Count > 0 ? stoppedUsersString : " users stopped")}", VerboseEnum.SolutionConditions);

            if (stoppedUsers.Count == 0)
                SetSolvedIndicator(true);
        }

        // TODO: Consider moving to RoadUser
        private void OnRoadUserCollision(RoadUser affected1, RoadUser affected2)
        {
            SetSolvedIndicator(false);
            blockedResolvabilityUntilRestart = true;
            Print("Accident between " + affected1.name + " and " + affected2.name, VerboseEnum.Physics);
            timer = timeLeftToLoop - timeToResetLevelAfterCollision * (int)Instance.Speed; // Set some seconds, to understand the situation
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
            // TODO: Test
            if (!GameRunning) return;

            float timeIncrement = Time.fixedDeltaTime * (int)GameEngine.Instance.Speed;
            timer += timeIncrement;
            timeLeftToLoop = timeToLoop;

            // This was a try to autoset timeloop to the maximum trafficLight cycle, but wasn't really a good idea
            // I keep it because could be reviewed to look for a solution, maybe configuring how many cycles should a level be
            /* List<TrafficLightController> trafficLightControllers = FindObjectsOfType<TrafficLightController>().ToList();
         List<int> trafficLightTimes = trafficLightControllers.Select(e => e.GetTotalAmountTime()).ToList();
         realTimeToLoop = trafficLightTimes.Max();*/
            //   throw new Exception("Time to loop in Level Manager must never be 0 or below!");

            if (timer >= timeLeftToLoop)
            {
                Print($"Level reset", VerboseEnum.GameTrace);
                LevelInit();
            }

            if (!ConditionsToSolve) return;

            StepToSolve(timeIncrement);
        }

        private void StepToSolve(float timeIncrement)
        {
            timeLeftToSolve -= timeIncrement;
            int timeInt = (int)timeLeftToSolve;

            soundFx.PlaySolvedSound(timeInt);
            // TODO: Test (I've taken "=" in this comparison
            if (timeLeftToSolve > 0)
                timeIndicator.text = timeInt.ToString();
            else
                LevelSolved();
        }

        private void LevelSolved()
        {
            Print("Level solved: activating panel", VerboseEnum.GameTrace);
            solvedPanel.SetActive(true);
            ActivateParticleSystems();
            SaveLevelAsCompleted();
            GameEngine.Instance.ChangeSpeed(GameSpeed.Paused);
        }

        private void SaveLevelAsCompleted()
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
        }

        private void ActivateParticleSystems() => particleSystems.ForEach(e => e.Play());

        public void LevelInit()
        {
            blockedResolvabilityUntilRestart = false;

            ResetTimeLeftToSolve();
            SetSolvedIndicator(true);

            EventManager.RaiseOnLoopStarted();
        }

        public void ResetTimeLeftToSolve()
        {
            timeLeftToSolve = timeToSolve;
            //timeIndicator.text = ((int)timeLeftToSolve).ToString();
            timer = 0;
        }

        // TODO: Refactor
        public void SetSolvedIndicator(bool conditionsToSolveSatisfied)
        {
            if (!conditionsToSolveSatisfied)
            {
                ResetSolvedIndicator();
            }
            else if (iconImage.sprite == forbiddenSprite && !blockedResolvabilityUntilRestart) // If we are changing from red to green
            {
                iconImage.sprite = minimumSpeedSprite;
                ResetTimeLeftToSolve();
                timeIndicator.text = ((int)timeLeftToSolve).ToString();
            }
            //iconImage.color = colour;
            //shineImage.color = colour;
        }

        private void ResetSolvedIndicator()
        {
            timeIndicator.text = "";
            iconImage.sprite = forbiddenSprite;
            if (blockedResolvabilityUntilRestart) return;
            if ((int)timeLeftToSolve < 3 && !blockedResolvabilityUntilRestart) // not sound if restarting
                soundFx.PlayFailSound();
            ResetTimeLeftToSolve();
        }

        public void GoNextLevel()
        {
            if (!string.IsNullOrEmpty(nextLevel))
                SceneManager.LoadScene(nextLevel);
            else
                throw new Exception($"No level set as next in {levelTitle}!");
        }
        public void ShowAllLevels()
        {
            SceneManager.LoadScene("Levels scene");
        }

        public IEnumerator ActivateClock()
        {
            clock.SetActive(true);
            yield return new WaitForSeconds(timeToResetLevelAfterCollision * (int) Instance.Speed);
            clock.SetActive(false);
        }

        public bool WasAlreadyStopped(RoadUser responsible)
        {
            return stoppedUsers.Find(x => x.GetInstanceID() == responsible.GetInstanceID());
        }
    }
}

