using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Level
{

    public class GameEngine : MonoBehaviour
    {
        public static GameEngine instance;

        public bool menuOpen = false;
        //[HideInInspector]
        // public uint verbose = 0;
        // http://answers.unity.com/answers/1767255/view.html
        [System.Flags]
        public enum VerboseEnum
        {
            Nothing = ~1,
            Speed = 1<<1,
            SpeedDetail = 1<<2,
            Physics = 1<<3,
            TrafficLightChanges = 1<<4,
            SolutionConditions = 1<<5,
            GameTrace = 1<<6,
            Everything = ~0
        }
        
        public VerboseEnum verbose;
        [SerializeField] private Sprite[] trafficLightSprites = new Sprite[4];
        public Sprite[] TrafficLightSprites => trafficLightSprites;
        [SerializeField] Sprite[] lightIndicator = new Sprite[3];
        public Sprite[] LightIndicator => lightIndicator;
        public GameObject mouse;
        [SerializeField] private Canvas canvas;
        private GameSpeed speed = GameSpeed.None;

        /// <summary>
        /// Setting raises a call that can affect road users
        /// </summary>
        public GameSpeed Speed
        {
            get => speed;
            set => ChangeSpeed(value);
        }

        public bool IsPaused => Speed == GameSpeed.Paused;
        public bool IsPlayed => !IsPaused;
        public bool IsNormalSpeed => Speed == GameSpeed.Normal;
        public bool IsFastSpeed => Speed == GameSpeed.Fast;
        public bool IsFastestSpeed => Speed == GameSpeed.SuperFast;
        public Color GreenColor => new Color(0.165f, 0.6706f, 0.149f);

        private Image[] timeControlImages = new Image[4];

        //[SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject timeControlImagesContainer;
        [SerializeField] private GameObject cheaterPanel;
        private float timeInPlay;

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

        private void OnEnable() => timeControlImages ??= timeControlImagesContainer.GetComponentsInChildren<Image>();

        public void Awake()
        {
            if (instance == null)
                instance = this;
            timeControlImages = GetTimeControlImagesExceptContainer();
            ChangeSpeed(GameSpeed.Normal);
            canvas = FindObjectOfType<Canvas>();
            Application.targetFrameRate = 120;
            //   CalculateBackgroundColor();
        }

        private Image[] GetTimeControlImagesExceptContainer()
        {
            return timeControlImagesContainer.GetComponentsInChildren<Image>().Skip(1).Take(4).ToArray();
        }

        /* 
     * Discarded this idea because it could get some uglier colors, so I decided to simply add an elegant dark grey colour.
     * [ContextMenu("Calculate Background Color")]
     public void CalculateBackgroundColor()
     {
         Texture2D tex = spriteRenderer.sprite.texture;
         Color[] colors = tex.GetPixels();
         colors = colors.Where((e, i) => i % (tex.width - 1) == 0 || i % tex.height == 0).ToArray();

         float red = colors.Select(e => e.r).Sum() / colors.Length;
         float green = colors.Select(e => e.g).Sum() / colors.Length;
         float blue = colors.Select(e => e.b).Sum() / colors.Length;
         Camera.main.backgroundColor = new Color(red, green, blue);
     }*/

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                Instantiate(cheaterPanel, canvas.transform);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!menuOpen)
                {
                    menuOpen = true;
                    SceneManager.LoadScene("Pause Menu", LoadSceneMode.Additive);
                   // SceneManager.LoadScene("Main Menu", LoadSceneMode.Additive);
                    GameEngine.instance.Speed = GameSpeed.Paused;
                }

                else
                {
                    SceneManager.UnloadSceneAsync("Pause Menu");
                    menuOpen = false;
                }
            }
        }

        public void ChangeSpeed(GameSpeed gameSpeed)
        {
            if (gameSpeed == Speed) return; // If speed changed
            for (int i = 0; i < timeControlImages.Length; i++)
                timeControlImages[i].color = i != (int)gameSpeed ? Color.black : GreenColor; // Selected color (Green)

            speed = gameSpeed;
            EventManager.RaiseOnGameSpeedChanged(gameSpeed);
        }


        public void ChangeSpeed(int newSpeed) => ChangeSpeed((GameSpeed)newSpeed); // Overload

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

        public static Direction Vector3ToDirection(Vector3 dir) => Vector2ToDirection(dir); // Overload


        public void PrintInstance(string msg, VerboseEnum type)
        {
            if (((byte)verbose & (byte)type) != 0)
                print(msg);
        }

        public static void Print(string msg, VerboseEnum type)
        {
            if (!instance) print("No GameEngine instance");
            instance.PrintInstance(msg, type);
        }

        private void SetCursorPosition()
        {
            Vector3 pos = Input.mousePosition;
            pos.z = canvas.transform.position.z;
            mouse.transform.position = Camera.main.ScreenToWorldPoint(pos);
        }
    }
}