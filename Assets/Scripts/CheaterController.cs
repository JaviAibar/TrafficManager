using System.Collections.Generic;
using Level;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class CheaterController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField console;
    [SerializeField] private GameObject consoleGameObject;
    private SoundFxManager soundManager;
    private LevelManager level;
    private MenuController menu;
    private Dictionary<string, Action> commandDictionary;


    private void Awake()
    {
        level = FindObjectOfType<LevelManager>();
        menu = FindObjectOfType<MenuController>();
        if (menu == null)
        {
            menu = new GameObject().AddComponent<MenuController>();
        }
        soundManager = FindObjectOfType<SoundFxManager>();
    }

    private void Start()
    {
        inputField.Select();
        inputField.ActivateInputField();

        commandDictionary = new Dictionary<string, Action>
        {
            { "solve", () => { SolveLevel(); GoNextLevel(); } },
            { "next", GoNextLevel },
            { "unsolve", () => { UnsolveLevel(); ReloadLevel(); } },
            { "reload", ReloadLevel },
            { "main_menu", LoadMainMenu },
            { "mute", MuteSound }
        };
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            string trimmedInput = inputField.text.Trim();

            if (commandDictionary.TryGetValue(trimmedInput, out Action action))
            {
                action.Invoke();
            }
            else
            {
                ShowHelpMessage();
            }
        }
    }

    private void SolveLevel()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
    }

    private void GoNextLevel()
    {
        level.GoNextLevel();
    }

    private void UnsolveLevel()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 0);
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadMainMenu()
    {
        menu.LoadMainMenuScene();
    }

    private void MuteSound()
    {
        soundManager.SwitchMuteAllSounds();
        Destroy(gameObject);
    }

    private void ShowHelpMessage()
    {
        consoleGameObject.SetActive(true);
        console.text =
            $"next - Loads next level\nreload - Reloads current level\nmain_menu - Loads the main menu\nsolve - Sets levels as solved and loads next one\nmute - mutes or unmutes general volume\nhelp - Shows this message";
        inputField.Select();
        inputField.ActivateInputField();
    }
}