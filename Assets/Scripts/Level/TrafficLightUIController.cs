using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightUIController : MonoBehaviour
{
    public InputField redInputField;
    public InputField greenInputField;
    public InputField yellowInputField;

    public Slider redSlider;
    public Slider greenSlider;
    public Slider yellowSlider;

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

    public void SetValues(int red, int green, int yellow)
    {
        // SetSliders(red, green, yellow);
        // SetInputFields(red, green, yellow);
        SetRed(red);
        SetGreen(green);
        SetYellow(yellow);
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
    public void SetRed(float value)
    {
        redInputField.text = value.ToString();
    }
    public void SetGreen(float value)
    {
        greenInputField.text = value.ToString();
    }
    public void SetYellow(float value)
    {
        yellowInputField.text = value.ToString();
    }
   
    public int? GetRed()
    {
        int value;
        if (int.TryParse(redInputField.text, out value)) return value;
        return null;
    }

    public int? GetGreen()
    {
        int value;
        if (int.TryParse(greenInputField.text, out value)) return value;
        return null;
    }

    public int? GetYellow()
    {
        int value;
        if (int.TryParse(yellowInputField.text, out value)) return value;
        return null;
    }

    public void SetSender(TrafficLightController senderParam)
    {
        sender = senderParam;
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Accept()
    {
        sender.SetValues(GetRed(), GetGreen(), GetYellow()) ;
        gameObject.SetActive(false);
    }
}
