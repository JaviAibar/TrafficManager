using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightReference : MonoBehaviour
{
    /**
     * This piece of code is exclusively to have the reference of the traffic light in its corresponding traffic area
     */
    public TrafficLightController trafficLight;
    private void Start()
    {
        if (!trafficLight)
            throw new System.Exception($"Traffic Light Reference {name} must have a reference of a Traffic Light Controller");
    }
}
