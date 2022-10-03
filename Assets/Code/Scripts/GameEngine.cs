using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{
    static GameEngine gameEngine;
    public float x, y;

    private void Awake()
    {
        if (gameEngine == null)
        {
            gameEngine = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum GameState : ushort
    {
        Paused = 0,
        Normal = 1,
        Fast = 2,
        SuperFast = 10
    }

    public enum Direction
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
        Center
    }

    public Sprite[] trafficLightSprites = new Sprite[4];
    public Image pauseImage;
    public Image playImage;
    public Image fastImage;
    public Image fastestImage;
    private GameState speed = GameState.Normal;

    public void SetIconColor(GameState state)
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
        SetSpeed(GameState.Paused);

    }

    public void ResumeGame()
    {
        SetSpeed(GameState.Normal);
    }

    public void FastGame()
    {
        SetSpeed(GameState.Fast);
    }

    public void FastestGame()
    {
        SetSpeed(GameState.SuperFast);
    }

    public void SetSpeed(GameState state)
    {
        /* TrafficLightController[] controllers = FindObjectsOfType<TrafficLightController>();
         foreach (TrafficLightController t in controllers)
         {
             t.SetSpeed(state);
         }*/
        speed = state;
    }

    public int GetSpeed()
    {
        return (int)speed;
    }

    public static Direction Vector3ToDirection(Vector3 dir)
    {
        float angle = Vector3.Angle(dir, Vector3.up);
        float angle2 = Vector3.Angle(dir, Vector3.right);

        if (angle2 > 90)
        {
            angle = 360 - angle;
        }
        int dirSegment = (int)((angle + 22) / 45);
        return (Direction)(dirSegment >= 8 ? dirSegment - 8 : dirSegment);
        
    }

    private void OnDrawGizmos()

    {

        Gizmos.color = Color.red;

        //Gizmos.DrawLine(transform.position, marker.position);

        Gizmos.DrawRay(Vector3.zero, new Vector3(x, y, 0));



        Gizmos.color = Color.green;

        Gizmos.DrawRay(Vector3.zero, Vector3.up);

        Gizmos.DrawRay(Vector3.zero, Vector3.right);

        Gizmos.DrawRay(Vector3.zero, Vector3.down);

        Gizmos.DrawRay(Vector3.zero, Vector3.left);



        Gizmos.DrawRay(Vector3.zero, Vector3.up + Vector3.right);

        Gizmos.DrawRay(Vector3.zero, Vector3.down + Vector3.right);

        Gizmos.DrawRay(Vector3.zero, Vector3.down + Vector3.left);

        Gizmos.DrawRay(Vector3.zero, Vector3.up + Vector3.left);

    }
}
