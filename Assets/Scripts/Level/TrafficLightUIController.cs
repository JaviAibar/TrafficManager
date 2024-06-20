using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Level
{
    public class TrafficLightUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI redText;
        [SerializeField] private TextMeshProUGUI greenText;
        [SerializeField] private TextMeshProUGUI yellowText;

        [SerializeField] private Slider redSlider;
        [SerializeField] private Slider greenSlider;
        [SerializeField] private Slider yellowSlider;

        public int Red { get => (int)redSlider.value; set { redSlider.value = value; redText.text = value.ToString(); } }
        public int Yellow { get => (int)yellowSlider.value; set { yellowSlider.value = value; yellowText.text = value.ToString(); } }
        public int Green { get => (int)greenSlider.value; set { greenSlider.value = value; greenText.text = value.ToString(); } }

        public TrafficLightController Sender { get => sender; set => sender = value; }

        private TrafficLightController sender;

        private void OnEnable()
        {
            redSlider.onValueChanged.AddListener(RedSliderChange);
            greenSlider.onValueChanged.AddListener(GreenSliderChange);
            yellowSlider.onValueChanged.AddListener(YellowSliderChange);
        }

        private void OnDisable()
        {
            redSlider.onValueChanged.RemoveListener(RedSliderChange);
            greenSlider.onValueChanged.RemoveListener(GreenSliderChange);
            yellowSlider.onValueChanged.RemoveListener(YellowSliderChange);
        }

        public void SetValues(int red, int yellow, int green)
        {
            Red = red;
            Yellow = yellow;
            Green = green;
        }
        void RedSliderChange(float value)
        {
            redText.text = value.ToString();
        }

        void GreenSliderChange(float value)
        {
           greenText.text = value.ToString();
        }
        void YellowSliderChange(float value)
        {
           yellowText.text = value.ToString();
        }

        public void Cancel() => gameObject.SetActive(false);

        public void Accept()
        {
            sender.SetValues(Red, Yellow, Green) ;
            gameObject.SetActive(false);
        }
    }
}