using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{
    public static GameEngine instance;
    public float x, y; // Debug
    public bool verbose = true;

    public Sprite[] trafficLightSprites = new Sprite[4];
    public Sprite[] lightIndicator = new Sprite[3];
    public Image pauseImage;
    public Image playImage;
    public Image fastImage;
    public Image fastestImage;
    private GameSpeed speed = GameSpeed.None;
    public Image[] timeControlImages = new Image[4];

    private void Awake()
    {
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

    private void Start()
    {
        ChangeSpeed((int)GameSpeed.Normal);
    }

    public enum GameSpeed : ushort
    {
        None = 0,
        Paused = 0,
        Normal = 1,
        Fast = 2,
        SuperFast = 3
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


    public void ChangeSpeed(int newSpeed)
    {
        GameSpeed gameSpeed = ((GameSpeed)newSpeed);
        if (gameSpeed != speed) // If speed changed
        {
            for (int i = 0; i < timeControlImages.Length; i++) 
                if (i != newSpeed)
                    timeControlImages[i].color = Color.black;
                else
                    timeControlImages[i].color = new Color(0.165f, 0.6706f, 0.149f);
                    //timeControlImages[i].color = new Color(42f, 171f, 38f, 255f)/255f;
                    
            speed  = gameSpeed;
            EventManager.RaiseOnGameSpeedChanged(gameSpeed);
        }
    }
  

    public GameSpeed GetGameSpeed()
    {
        return speed;
    }
    
    public static Direction Vector2ToDirection(Vector2 dir)
    {
        float angle = Vector2.Angle(dir, Vector3.up);
        float angle2 = Vector2.Angle(dir, Vector3.right);

        if (angle2 > 90)
        {
            angle = 360 - angle;
        }
        int dirSegment = (int)((angle + 22) / 45);
        return (Direction)(dirSegment >= 8 ? dirSegment - 8 : dirSegment);
        
    }

    public static Direction Vector3ToDirection(Vector3 dir)
    {
        return Vector2ToDirection(dir);
    }

    public void Print(string msg)
    {
        if (verbose)
            print(msg);
    }
}
