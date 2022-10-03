using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerTM : MonoBehaviour
{
    public static void LoadOptions()
    {
        SceneManager.LoadScene("OptionsScene");
    }
}
