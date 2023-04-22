using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using System.Runtime.ExceptionServices;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using static Level.GameEngine;
using UnityEngine.Localization.Settings;

public class MenuController : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public GameObject levelsGrid;
    public GameObject pauseMenu;

    /// <summary>
    /// [0] unsolved level, [1] solved level
    /// </summary>
    public Sprite[] solvedSprites = new Sprite[2];

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (pauseMenu)
            {
                pauseMenu.SetActive(true);
                Level.GameEngine.instance.ChangeSpeed(GameSpeed.Paused);
            }
        }
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        print($"Scene {scene.name} loaded");
        //SavePrevScene();
        if (scene.name == "Levels Scene")
        {
            LoadLevelsList();
        }

    }

    public void NewGame()
    {
        LoadSceneAndSave("Level001");
    }

    

    public void LoadGame(int level)
    {
        LoadSceneAndSave("Level" + level.ToString("D3"));
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings Menu", LoadSceneMode.Additive);
    }

    public void UnloadPauseMenu()
    {
        SceneManager.UnloadSceneAsync("Pause Menu");
    }

    public void LoadTutorial()
    {
        LoadSceneAndSave("Tutorial 1");
    }

    public void LoadMainMenuScene()
    {
        LoadSceneAndSave("Main Menu");
    }

    public void LoadLevelsScene()
    {
        LoadSceneAndSave("Levels Scene");
    }

    public void LoadLevel(int level)
    {
        LoadLevel("Level" + level.ToString("D3"));
    }

    public void LoadLevel(string level)
    {
        LoadSceneAndSave(level);
    }

    public void LoadPreviousScene()
    {
        HistoryTracker.instance.LoadPreviousScene();
    }

    public void LoadNextScene()
    {
        HistoryTracker.instance.LoadNextScene();
    }

    public void LoadSceneAndSave(string name)
    {
        SceneManager.LoadScene(name);
        HistoryTracker.instance.SavePrevScene();
    }

    private void LoadLevelsList()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        List<string> scenes = new List<string>();
        Transform levelsTransform = GameObject.Find("Levels").transform;
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            scenes.Add(sceneName);
        }
        print($"Scenes pre filter: {scenes.Count}");
        // Filtering
        scenes = scenes.FindAll(e => e.Contains("Level"));
        scenes.Remove("Levels Scene");
        print($"Scenes post filter: {scenes.Count}");

        foreach (string sceneName in scenes)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab, levelsTransform);
            levelButton.name = sceneName;
            levelButton.GetComponentInChildren<TMP_Text>().text = int.Parse(sceneName.Substring(5)).ToString();
            Image checkedImage = levelButton.transform.GetComponent<Image>();
            checkedImage.sprite = solvedSprites[PlayerPrefs.GetInt(sceneName, 0)];

            levelButton.GetComponent<Button>().onClick.AddListener(
                delegate { SceneManager.LoadScene(levelButton.name); });
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}