using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TrafficLightController : MonoBehaviour
{
    [HideInInspector] public GameEngine gameEngine;
    
    public TrafficLightUIController trafficLightUIPanel;
    /// <summary>
    ///[0] red, [1] green, [2] yellow
    /// </summary>
    public int[] trafficLightTimeAmounts;
    private TMPro.TMP_Text timerText;
    [HideInInspector] public Image image; // Kept public for the sake of Testing
    [SerializeField] private float timer;
    private TrafficLightColour trafficLightColour;
    public int timeOffset = 0;
    [System.Serializable]
    public enum TrafficLightColour : ushort
    {
        Off = 0,
        Red = 1,
        Green = 2,
        Yellow = 3
    }

    private void OnEnable()
    {
        EventManager.OnLoopEnded += OnLoopStart;
    }

    private void OnDisable()
    {
        EventManager.OnLoopEnded -= OnLoopStart;
    }

    private void OnLoopStart()
    {
        timer = timeOffset;
    }

    private void Awake()
    {
        image = GetComponent<Image>();
        gameEngine = FindObjectOfType<GameEngine>();
        timerText = GetComponentInChildren<TMPro.TMP_Text>();
    }
    private void Start()
    {
        OnLoopStart();
    }
    void Update()
    {
        int speed = (int)gameEngine.GetGameSpeed();
        timer += Time.deltaTime * speed;

        if (speed > 0 && (timer - (int)timer) <= 0.1f)
        {
            DecideTrafficLightColor();
        }
    }

    public void OpenMenu()
    {
        trafficLightUIPanel.gameObject.SetActive(true);
        trafficLightUIPanel.SetValues(trafficLightTimeAmounts[0], trafficLightTimeAmounts[1], trafficLightTimeAmounts[2]);
        trafficLightUIPanel.SetSender(gameObject.GetComponent<TrafficLightController>());
    }

    public void SetValues(int? redParam, int? greenParam, int? yellowParam)
    {
        if (trafficLightTimeAmounts == null) trafficLightTimeAmounts = new int[] { 1, 1, 1};
        if (redParam != null) trafficLightTimeAmounts[0] = redParam.Value;
        if (greenParam != null) trafficLightTimeAmounts[1] = greenParam.Value;
        if (yellowParam != null) trafficLightTimeAmounts[2] = yellowParam.Value;
    }

    public void DecideTrafficLightColor()
    {
        // Precalculation and courtesy variables
        int red = trafficLightTimeAmounts[0];
        int green = trafficLightTimeAmounts[1];
        int yellow = trafficLightTimeAmounts[2];
        int redGreen = red + green;
        float sumTimeColors = green + red + yellow;
        float timerSumTimeColorsRest = timer % sumTimeColors;

        if (timerSumTimeColorsRest <= red) // If time cycle is inside red time, then red
        {
            if (trafficLightColour != TrafficLightColour.Red)
            {
                trafficLightColour = TrafficLightColour.Red;
                EventManager.RaiseOnTrafficLightChanged(TrafficLightColour.Red);
                image.sprite = gameEngine.trafficLightSprites[(int)TrafficLightColour.Red];
            }
            timerText.text = "" + (red == 1 ? "" : (int)(red - timerSumTimeColorsRest + 1));
        }
        else if (timerSumTimeColorsRest <= redGreen) // If time cycle is inside redGreen time, then green
        {
            if (trafficLightColour != TrafficLightColour.Green)
            {
                trafficLightColour = TrafficLightColour.Green;
                EventManager.RaiseOnTrafficLightChanged(TrafficLightColour.Green);
                image.sprite = gameEngine.trafficLightSprites[(int)TrafficLightColour.Green];
            }
            timerText.text = "" + (green == 1 ? "" : (int)(redGreen - timerSumTimeColorsRest + 1));
        }
        else  // If time cycle is inside redGreenYellow time, then yellow
        {
            if (trafficLightColour != TrafficLightColour.Yellow)
            {
                trafficLightColour = TrafficLightColour.Yellow;
                EventManager.RaiseOnTrafficLightChanged(TrafficLightColour.Yellow);
                image.sprite = gameEngine.trafficLightSprites[(int)TrafficLightColour.Yellow];
            }
            timerText.text = "" + (yellow == 1 ? "" : (int)(sumTimeColors - timerSumTimeColorsRest + 1));
        }
        
    }

    public TrafficLightColour GetState()
    {
        return trafficLightColour;
    }

    public void SetState(TrafficLightColour colour)
    {
        trafficLightColour = colour;
        image.sprite = gameEngine.trafficLightSprites[(int)colour];
    }

    public string GetTimerText()
    {
        return timerText.text;
    }

    public void SetTimerText(string text)
    {
        timerText.text = text;
    }

    public float GetTimeInPlay() {
        return timer;
    }
}
