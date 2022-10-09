using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void TrafficLightControlEvent(TrafficLightController.TrafficLightColour colour);
    public static event TrafficLightControlEvent OnTrafficLightChanged;

    public static void TrafficLightChanged(TrafficLightController.TrafficLightColour colour)
    {
        if (OnTrafficLightChanged != null)
            OnTrafficLightChanged(colour);
    }
}