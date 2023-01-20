using System.Collections;
using System.Collections.Generic;
using Level;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CheaterController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField console;
    [SerializeField] private GameObject consoleGameObject;
    private LevelManager level;
    private MenuController menu;

    private void Awake()
    {
        level = FindObjectOfType<LevelManager>();
        menu = FindObjectOfType<MenuController>() ?? new GameObject().AddComponent<MenuController>();
    }

    private void Start()
    {
        inputField.Select();
        inputField.ActivateInputField();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            /* if (inputField.text.Trim() == "next")
             {
                 level.GoNextLevel();
             }*/

            switch (inputField.text.Trim())
            {
                case "next":
                    level.GoNextLevel();
                    break;
                case "reload":
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    break;
                case "main_menu":
                    menu.LoadMainMenuScene();
                    break;
                default:
                    /*ScrollView scroll = console.GetComponentInParent<ScrollView>();
                    scroll.gameObject.SetActive(true);*/
                    consoleGameObject.gameObject.SetActive(true);
                    console.text =
                        $"next - Loads next level\nreload - Reloads current level\nmain_menu - Loads the main menu\nhelp - Shows this message";
                    break;
            }
        }
    }
}