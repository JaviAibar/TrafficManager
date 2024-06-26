using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Level
{
    public class GameEngine : MonoBehaviour
    {

        public bool menuOpen = false;
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

        [SerializeField] private VerboseEnum verbose;
        [SerializeField] private Sprite[] trafficLightSprites = new Sprite[4];
        [SerializeField] Sprite[] lightIndicator = new Sprite[3];
        [SerializeField] private GameObject mouse;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image[] timeControlImages = new Image[4];
        [SerializeField] private GameObject timeControlImagesContainer;
        [SerializeField] private GameObject cheaterPanel;
        private GameSpeed speed = GameSpeed.None;
        private static GameEngine instance;

        public Sprite[] TrafficLightSprites => trafficLightSprites;
        public Sprite[] LightIndicator => lightIndicator;

        /// <summary>
        /// Setting raises a call that can affect road users
        /// </summary>
        public GameSpeed Speed
        {
            get => speed;
            set => ChangeSpeed(value);
        }
        public static GameEngine Instance => instance;
        public bool IsPaused => Speed == GameSpeed.Paused;
        public bool IsRunning => !IsPaused;
        public bool IsNormalSpeed => Speed == GameSpeed.Normal;
        public bool IsFastSpeed => Speed == GameSpeed.Fast;
        public bool IsFastestSpeed => Speed == GameSpeed.SuperFast;
        public Color GreenColor => new Color(0.165f, 0.6706f, 0.149f);
        public VerboseEnum Verbose {
            get => verbose;
            set => verbose = value;
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

        private void OnEnable() => timeControlImages ??= timeControlImagesContainer.GetComponentsInChildren<Image>();

        public void Awake()
        {
            if (instance == null)
                instance = this;

            timeControlImages = GetTimeControlImagesExceptContainer();
            ChangeSpeed(GameSpeed.Normal);
            canvas = FindObjectOfType<Canvas>();
            Application.targetFrameRate = 120;
        }

        private Image[] GetTimeControlImagesExceptContainer()
        {
            return timeControlImagesContainer.GetComponentsInChildren<Image>().Skip(1).Take(4).ToArray();
        }

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
                    GameEngine.Instance.Speed = GameSpeed.Paused;
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


        public static bool CompareDirections(Vector3 dir1, Vector3 dir2)
        {
            return Vector3ToDirection(dir1) == Vector3ToDirection(dir2);
        }
    }
}