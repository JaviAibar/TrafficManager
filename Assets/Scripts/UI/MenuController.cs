using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using System.Runtime.ExceptionServices;
using UnityEngine.UI;
using TMPro;
using System;

public class MenuController : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public GameObject levelsGrid;
    public GameObject backButton;
    public GameObject nextButton;
    public static MenuController instance;
    private static List<int> prevScenes;
    private static int historyIndex = -1;
    /// <summary>
    /// [0] unsolved level, [1] solved level
    /// </summary>
    public Sprite[] solvedSprites = new Sprite[2];

    private void Awake()
    {
        if (prevScenes == null) 
            prevScenes = new List<int>();
            
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
       
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        print($"Scene {scene.name} loaded");
        //SavePrevScene();
        if (scene.name == "Levels Scene")
        {
            LoadLevelsList();
        }

        GenerateBackAndNextButtons();
    }


    public void GenerateBackAndNextButtons()
    {

       // print($"Enable, history: {historyIndex}, prev: {prevScenes.Count}");
        Canvas canvas = FindObjectOfType<Canvas>();
        if (historyIndex > 0 && canvas)
            Instantiate(backButton, canvas.transform).GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    LoadPreviousScene();
                });
        if (historyIndex < prevScenes.Count - 1 && canvas)
            Instantiate(nextButton, canvas.transform).GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    LoadNextScene();
                }); ;
    }

    public void NewGame()
    {
        LoadSceneAndSave("Tutorial Scene");
    }

    private void SavePrevScene()
    {
        historyIndex++;
        if (prevScenes.Count - historyIndex > 0)print($"Eliminando desde ind {historyIndex} un total de {prevScenes.Count - historyIndex} y a?adiendo la escena {SceneManager.GetActiveScene().name} a la lista");
        
        prevScenes.RemoveRange(historyIndex, prevScenes.Count - historyIndex);
        prevScenes.Add(SceneManager.GetActiveScene().buildIndex);
        print($"(SavePrevScene) Guardada escena {SceneManager.GetActiveScene().name}");
    }

    public void LoadGame(int level)
    {
        LoadSceneAndSave("Level" + level.ToString("D3"));
    }


    public void LoadTutorial()
    {
        LoadSceneAndSave("Tutorial Scene");
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
        // This method saves no previous scene because it would create a loop!
        int pediremos = historyIndex - 1;
        int buildIndex = prevScenes[pediremos];
        print($"(LoadPreviousScene: Se va a cargar la escena de indice {pediremos} cuya build index es {buildIndex} de nombre '{SceneManager.GetSceneByBuildIndex(buildIndex).name}' es valido? {SceneManager.GetSceneByBuildIndex(buildIndex).IsValid()}");
        SceneManager.LoadScene(prevScenes[historyIndex--]);
        print($"LoadPreviousScene historyIndex: {historyIndex}");
        //historyIndex--;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(prevScenes[historyIndex++]);
    }

    public void LoadSceneAndSave(string name)
    {
        SceneManager.LoadScene(name);
        SavePrevScene();
    }

    private void LoadLevelsList()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];
        Transform levelsTransform = GameObject.Find("Levels").transform;
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
        }

        foreach (string sceneName in scenes)
        {
            if (sceneName.Contains("Level") && sceneName != "Levels Scene")
            {
                GameObject levelButton = Instantiate(levelButtonPrefab, levelsTransform);
                levelButton.name = sceneName;
                levelButton.GetComponentInChildren<TMP_Text>().text = int.Parse(sceneName.Substring(5)).ToString();
                Image checkedImage = levelButton.transform.GetComponent<Image>();
                checkedImage.sprite = solvedSprites[PlayerPrefs.GetInt(sceneName, 0)];

                levelButton.GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    /*LevelLoader.currentLevel = (ScriptableObjectLevel)Resources.Load(LocalizationSettings.SelectedLocale.Formatter + "/Levels/Level " + levelButton.name.Split(" ")[1]);
                    SceneManager.LoadScene("Level");*/
                    SceneManager.LoadScene(levelButton.name);
                });
            }
        }
    }

    

    public int GetHistoryIndex()
    {
        return historyIndex;
    }

    public int GetSavedScenes()
    {
        return prevScenes.Count;
    }

    public void QuitGame()
    {
        Application.Quit();    
    }
}
