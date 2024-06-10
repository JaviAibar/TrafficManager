using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Level
{
    [System.Serializable]
    public class TrafficLightController : MonoBehaviour
    {
        [HideInInspector] public Image image; // Kept public for the sake of Testing

        [SerializeField] private TrafficLightUIController trafficLightUIPanel;
        [SerializeField] private float timeOffset = 0;
        [SerializeField] private float timer;

        private TMPro.TMP_Text timerText;
        private GameEngine gameEngine;
        /// <summary>
        ///[0] red, [1] yellow, [2] green
        /// </summary>
        private TrafficLightColour state;

        public int[] timeAmounts = new int[3];
        public int TimeRed { get => timeAmounts[0]; set => timeAmounts[0] = value;}
        public int TimeYellow { get => timeAmounts[1]; set => timeAmounts[1] = value;}
        public int TimeGreen { get => timeAmounts[2]; set => timeAmounts[2] = value; }
        public bool IsTimerWhole => (timer - (int)timer) <= 0.1f;
        public float TimeInPlay => timer;
        public string TimerText { get => timerText.text; set => timerText.text = value; }
        public TrafficLightUIController TrafficLightUIPanel
        {
            get => trafficLightUIPanel;
            set => trafficLightUIPanel = value;
        }
        public TrafficLightColour State
        {
            get => state;
            set
            {
                state = value;
                image.sprite = gameEngine.TrafficLightSprites[(int)value];
                EventManager.RaiseOnTrafficLightChanged(value);
            }
        }
        public bool IsRed => State == TrafficLightColour.Red;
        public bool IsYellow => State == TrafficLightColour.Yellow;
        public bool IsGreen => State == TrafficLightColour.Green;
        public int TotalAmountTime => timeAmounts.Sum();
        public float TimeOffset
        {
            get => timeOffset;
            set => timeOffset = value;
        }
        [System.Serializable]
        public  enum TrafficLightColour : ushort
        {
            Off = 0,
            Red = 1,
            Green = 2,
            Yellow = 3
        }

        private void OnEnable() => EventManager.OnLoopStarted += OnLoopStart;

        private void OnDisable() => EventManager.OnLoopStarted -= OnLoopStart;

        private void OnLoopStart() => timer = timeOffset;

        private void Awake()
        {
            image = GetComponent<Image>();
            gameEngine = FindObjectOfType<GameEngine>();
            timerText = GetComponentInChildren<TMPro.TMP_Text>();
        }
        private void Start() => OnLoopStart();

        void FixedUpdate()
        {
            int speed = (int)gameEngine.Speed;
            timer += Time.fixedDeltaTime * speed;

            if (speed > 0 && IsTimerWhole) // If timer is whole number
            {
                DecideTrafficLightColor();
            }
        }

        public void OpenMenu()
        {
            trafficLightUIPanel.gameObject.SetActive(true);
            trafficLightUIPanel.SetValues(TimeRed, TimeYellow, TimeGreen);
            trafficLightUIPanel.Sender = gameObject.GetComponent<TrafficLightController>();
        }

        public void SetValues(int redParam, int yellowParam, int greenParam)
        {
            timeAmounts ??= new int[] { 1, 1, 1 };
            TimeRed = redParam;
            TimeYellow = yellowParam;
            TimeGreen = greenParam;
        }

        public void DecideTrafficLightColor()
        {
            // Precalculation and courtesy variables
            int redGreen = TimeRed + TimeGreen;
            float sumTimeColors = timeAmounts.Sum();
            float timerSumTimeColorsRest = timer % sumTimeColors;

            if (timerSumTimeColorsRest <= TimeRed) // If time cycle is inside red time, then red
            {
                if (state != TrafficLightColour.Red)
                {
                    State = TrafficLightColour.Red;
                    // image.sprite = gameEngine.TrafficLightSprites[(int)TrafficLightColour.Red];
                }
                timerText.text = "" + (TimeRed == 1 ? "" : (int)(TimeRed - timerSumTimeColorsRest + 1));
            }
            else if (timerSumTimeColorsRest <= redGreen) // If time cycle is inside redGreen time, then green
            {
                if (state != TrafficLightColour.Green)
                {
                    State = TrafficLightColour.Green;
                    //    image.sprite = gameEngine.TrafficLightSprites[(int)TrafficLightColour.Green];
                }
                timerText.text = "" + (TimeGreen == 1 ? "" : (int)(redGreen - timerSumTimeColorsRest + 1));
            }
            else  // If time cycle is inside redGreenYellow time, then yellow
            {
                if (state != TrafficLightColour.Yellow)
                {
                    State = TrafficLightColour.Yellow;
                    //image.sprite = gameEngine.TrafficLightSprites[(int)TrafficLightColour.Yellow];
                }
                timerText.text = "" + (TimeYellow == 1 ? "" : (int)(sumTimeColors - timerSumTimeColorsRest + 1));
            }

        }


    }
}
