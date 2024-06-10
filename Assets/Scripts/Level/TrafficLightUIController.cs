using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Level
{
    public class TrafficLightUIController : MonoBehaviour
    {

        [SerializeField] private TMP_InputField redInputField;
        [SerializeField] private TMP_InputField greenInputField;
        [SerializeField] private TMP_InputField yellowInputField;

        [SerializeField] private Slider redSlider;
        [SerializeField] private Slider greenSlider;
        [SerializeField] private Slider yellowSlider;

        public int Red { get => (int)redSlider.value; set { redSlider.value = value; redInputField.text = value.ToString(); } }
        public int Yellow { get => (int)yellowSlider.value; set { yellowSlider.value = value; yellowInputField.text = value.ToString(); } }
        public int Green { get => (int)greenSlider.value; set { greenSlider.value = value; greenInputField.text = value.ToString(); } }

        public TrafficLightController Sender { get => sender; set => sender = value; }

        private TrafficLightController sender;

        private void OnEnable()
        {
            redInputField.onValueChanged.AddListener(RedInputFieldChange);
            greenInputField.onValueChanged.AddListener(GreenInputFieldChange);
            yellowInputField.onValueChanged.AddListener(YellowInputFieldChange);

            redSlider.onValueChanged.AddListener(RedSliderChange);
            greenSlider.onValueChanged.AddListener(GreenSliderChange);
            yellowSlider.onValueChanged.AddListener(YellowSliderChange);
        }

        private void OnDisable()
        {
            redInputField.onValueChanged.RemoveListener(RedInputFieldChange);
            greenInputField.onValueChanged.RemoveListener(GreenInputFieldChange);
            yellowInputField.onValueChanged.RemoveListener(YellowInputFieldChange);

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
            redInputField.text = value.ToString();
        }

        void GreenSliderChange(float value)
        {
            greenInputField.text = value.ToString();
        }
        void YellowSliderChange(float value)
        {
            yellowInputField.text = value.ToString();
        }
        void RedInputFieldChange(string value)
        {
            float.TryParse(value, out float valueFloat);
            redSlider.value = valueFloat;
        }

        void GreenInputFieldChange(string value)
        {
            float.TryParse(value, out float valueFloat);
            greenSlider.value = valueFloat;

        }
        void YellowInputFieldChange(string value)
        {
            float.TryParse(value, out float valueFloat);
            yellowSlider.value = valueFloat;
        }

        public void Cancel() => gameObject.SetActive(false);

        public void Accept()
        {
            sender.SetValues(Red, Yellow, Green) ;
            gameObject.SetActive(false);
        }
    }
}