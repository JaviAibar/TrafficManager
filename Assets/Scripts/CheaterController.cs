using System.Collections;
using System.Collections.Generic;
using Level;
using UnityEngine;
using TMPro;

public class CheaterController : MonoBehaviour
{
    private TMP_InputField inputField;
    private LevelManager level;
    private void Awake()
    {
        level = FindObjectOfType<LevelManager>();
        inputField = GetComponentInChildren<TMP_InputField>();
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
        } else if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputField.text.Trim() == "next")
            {
                level.GoNextLevel();
            }
        }
    }
}
