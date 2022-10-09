using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    
    public float timeToLoop;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToLoop)
        {
            EventManager.OnLoopEnded();
            timer = 0;
        }
    }
}
