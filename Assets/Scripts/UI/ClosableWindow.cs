using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosableWindow : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            print("What");
            gameObject.SetActive(false);
        }
    }
}
