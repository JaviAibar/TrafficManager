using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HistoryTracker : MonoBehaviour
{
    public GameObject backButton;
    public GameObject nextButton;
    public static HistoryTracker instance;

    private static List<int> prevScenes;
    private static int historyIndex = -1;
    public bool EnoughHistory => prevScenes.Count > 1;
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

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        GenerateBackAndNextButtons();
    }

    public void GenerateBackAndNextButtons()
    {
        // print($"Enable, history: {historyIndex}, prev: {prevScenes.Count}");
        Canvas canvas = FindObjectOfType<Canvas>();
        if (historyIndex > 0 && canvas)
            Instantiate(backButton, canvas.transform).GetComponent<Button>().onClick.AddListener(
                delegate { LoadPreviousScene(); });
        if (historyIndex < prevScenes.Count - 1 && canvas)
            Instantiate(nextButton, canvas.transform).GetComponent<Button>().onClick.AddListener(
                delegate { LoadNextScene(); });
        ;
    }

    public void SavePrevScene()
    {
        historyIndex++;
        if (prevScenes.Count - historyIndex > 0)
            print(
                $"Eliminando desde ind {historyIndex} un total de {prevScenes.Count - historyIndex} y anadiendo la escena {SceneManager.GetActiveScene().name} a la lista");

        prevScenes.RemoveRange(historyIndex, prevScenes.Count - historyIndex);
        prevScenes.Add(SceneManager.GetActiveScene().buildIndex);
        print($"(SavePrevScene) Guardada escena {SceneManager.GetActiveScene().name}");
    }
    // This method saves no previous scene because it would create a loop!


    public void LoadPreviousScene(string fallback)
    {
        if (!CanLoadPreviousScene())
            SceneManager.LoadScene(fallback);
    }

    public void LoadPreviousScene(int fallback=0)
    {
        if (!CanLoadPreviousScene())
            SceneManager.LoadScene(fallback);
    }

    private bool CanLoadPreviousScene()
    {
        if (!EnoughHistory) return false;
        int targetIndex = historyIndex - 1;
        int buildIndex = prevScenes[targetIndex];

        SceneManager.LoadScene(buildIndex);
        return true;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(prevScenes[historyIndex++]);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }
    public int GetHistoryIndex()
    {
        return historyIndex;
    }

    public int GetSavedScenes()
    {
        return prevScenes.Count;
    }
}
