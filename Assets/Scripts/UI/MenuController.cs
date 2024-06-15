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

    [SerializeField] private GameObject levelButtonPrefab;

    [SerializeField] private GameObject levelsGrid;
    [SerializeField] private GameObject pauseMenu;

    /// <summary>
    /// [0] unsolved level, [1] solved level
    /// </summary>

    [SerializeField] private Sprite[] solvedSprites = new Sprite[2];

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
                Level.GameEngine.Instance.ChangeSpeed(GameSpeed.Paused);
            }
        }
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.name == "Levels Scene")
            LoadLevelsList();
    }

    public void NewGame()
    {
        LoadScene("Level001");
    }

    public void LoadGame(int level)
    {
        LoadScene("Level" + level.ToString("D3"));
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
        LoadScene("Tutorial 1");
    }

    public void LoadMainMenuScene()
    {
        LoadScene("Main Menu");
    }

    public void LoadLevelsScene()
    {
        LoadScene("Levels Scene");
    }

    public void LoadLevel(int level)
    {
        LoadLevel("Level" + level.ToString("D3"));
    }

    public void LoadLevel(string level)
    {
        LoadScene(level);
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    private void LoadLevelsList()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        Transform levelsGameObject = GameObject.Find("Levels").transform;

        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            if (sceneName.Contains("Level") && sceneName != "Levels Scene")
            {
                GameObject levelButton = Instantiate(levelButtonPrefab, levelsGameObject);
                levelButton.name = sceneName;
                levelButton.GetComponentInChildren<TMP_Text>().text = int.Parse(sceneName.Substring(5)).ToString();

                levelButton.transform.GetComponent<Image>().sprite = solvedSprites[PlayerPrefs.GetInt(sceneName, 0)];

                levelButton.GetComponent<Button>().onClick.AddListener(
                    delegate { SceneManager.LoadScene(levelButton.name); });
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}