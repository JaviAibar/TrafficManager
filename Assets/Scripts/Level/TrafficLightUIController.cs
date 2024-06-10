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
            redInputField.onValueChanged.AddListener(redInputFieldChange);
            greenInputField.onValueChanged.AddListener(greenInputFieldChange);
            yellowInputField.onValueChanged.AddListener(yellowInputFieldChange);

            redSlider.onValueChanged.AddListener(redSliderChange);
            greenSlider.onValueChanged.AddListener(greenSliderChange);
            yellowSlider.onValueChanged.AddListener(yellowSliderChange);
        }

        private void OnDisable()
        {
            redInputField.onValueChanged.RemoveListener(redInputFieldChange);
            greenInputField.onValueChanged.RemoveListener(greenInputFieldChange);
            yellowInputField.onValueChanged.RemoveListener(yellowInputFieldChange);

            redSlider.onValueChanged.RemoveListener(redSliderChange);
            greenSlider.onValueChanged.RemoveListener(greenSliderChange);
            yellowSlider.onValueChanged.RemoveListener(yellowSliderChange);
        }

        public void SetValues(int red, int yellow, int green)
        {
            Red = red;
            Yellow = yellow;
            Green = green;
        }
        void redSliderChange(float value)
        {
            redInputField.text = value.ToString();
        }

        void greenSliderChange(float value)
        {
            greenInputField.text = value.ToString();
        }
        void yellowSliderChange(float value)
        {
            yellowInputField.text = value.ToString();
        }
        void redInputFieldChange(string value)
        {
            float valueFloat;
            float.TryParse(value, out valueFloat);
            redSlider.value = valueFloat;
        }

        void greenInputFieldChange(string value)
        {
            float valueFloat;
            float.TryParse(value, out valueFloat);
            greenSlider.value = valueFloat;

        }
        void yellowInputFieldChange(string value)
        {
            float valueFloat;
            float.TryParse(value, out valueFloat);
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


   /* private void SetInputFields(int red, int green, int yellow)
    {
        redInputField.text= red.ToString();
        greenInputField.text = green.ToString();
        yellowInputField.text = yellow.ToString();
    }

    private void SetSliders(int red, int green, int yellow)
    {
        redSlider.value = red;
        greenSlider.value = green;
        yellowSlider.value = yellow;
    }
   */