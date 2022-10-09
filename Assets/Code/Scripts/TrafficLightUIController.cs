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
    public void SetRed(int red)
    {
        redInputField.text = "" + red;
    }

    public void SetYellow(int yellow)
    {
        yellowInputField.text = "" + yellow;
    }
    public void SetGreen(int green)
    {
        greenInputField.text = "" + green;
    }
    public void SetValues(int red, int green, int yellow)
    {
        SetRed(red);
        SetYellow(yellow);
        SetGreen(green);
        SetSliders(red, yellow, green);
    }

    private void SetSliders(int red, int yellow, int green)
    {
        redSlider.value = red;
        yellowSlider.value = yellow;
        greenSlider.value = green;
    }
   
    public int? GetRed()
    {
        int value;
        if (int.TryParse(redInputField.text, out value)) return value;
        return null;
    }

    public int? GetYellow()
    {
        int value;
        if (int.TryParse(yellowInputField.text, out value)) return value;
        return null;
    }
    public int? GetGreen()
    {
        int value;
        if (int.TryParse(greenInputField.text, out value)) return value;
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
