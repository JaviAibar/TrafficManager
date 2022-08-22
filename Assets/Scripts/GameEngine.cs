using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{
    public enum trafficLightState : ushort
    {
        Off = 0,
        Red = 1,
        Green = 2,
        Yellow = 3
    }
    public enum GameStates : ushort
    {
        Paused = 0,
        Normal = 1,
        Fast = 2,
        SuperFast = 10
    }

    public Sprite[] trafficLightSprites = new Sprite[4];
    public Image pauseImage;
    public Image playImage;
    public Image fastImage;
    public Image fastestImage;

    public void SetIconColor(GameStates state)
    {

        /*  switch (state)
          {
              case GameStates.Paused:
                  pauseImage.color = 
          }
          pauseImage;
        playImage;
       fastImage;
       fastestImage;*/
    }

    public void PauseGame()
    {
        SetSpeed(GameStates.Paused);

    }

    public void ResumeGame()
    {
        SetSpeed(GameStates.Normal);
    }

    public void FastGame()
    {
        SetSpeed(GameStates.Fast);
    }

    public void FastestGame()
    {
        SetSpeed(GameStates.SuperFast);
    }

    public void SetSpeed(GameStates state)
    {
        TrafficLightController[] controllers = FindObjectsOfType<TrafficLightController>();
        foreach (TrafficLightController t in controllers)
        {
            t.SetSpeed(state);
        }
    }
}
