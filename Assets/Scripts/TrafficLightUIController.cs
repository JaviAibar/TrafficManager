using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightUIController : MonoBehaviour
{
    public InputField redInputField;
    public InputField yellowInputField;
    public InputField greenInputField;

    public Slider redSlider;
    public Slider yellowSlider;
    public Slider greenSlider;

    private TrafficLightController sender;
    public void SetSender(TrafficLightController senderParam)
    {
        sender = senderParam;
    }
    public void SetRed(float red)
    {
        redInputField.text = "" + red;
    }

    public void SetYellow(float yellow)
    {
        yellowInputField.text = "" + yellow;
    }
    public void SetGreen(float green)
    {
        greenInputField.text = "" + green;
    }
    public void SetValues(float red, float yellow, float green)
    {
        SetRed(red);
        SetYellow(yellow);
        SetGreen(green);
        print(red + " " + yellow + " " + green);
        SetSliders(red, yellow, green);
    }

    private void SetSliders(float red, float yellow, float green)
    {
        redSlider.value = red;
        yellowSlider.value = yellow;
        greenSlider.value = green;
    }
   
    public float? GetRed()
    {
        float value;
        if (float.TryParse(redInputField.text, out value)) return value;
        return null;
    }

    public float? GetYellow()
    {
        float value;
        if (float.TryParse(yellowInputField.text, out value)) return value;
        return null;
    }
    public float? GetGreen()
    {
        float value;
        if (float.TryParse(greenInputField.text, out value)) return value;
        return null;
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Accept()
    {
        sender.SetValues(GetRed(), GetYellow(), GetGreen()) ;
        gameObject.SetActive(false);
    }
}
