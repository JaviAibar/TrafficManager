using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficArea : MonoBehaviour
{
    /**
     *  This script will control all colliders of the crossroads
     */

    public bool stopArea;
    public GameEngine.Direction direction;
    private TrafficLightController trafficLight;
    // Start is called before the first frame update
    void Start()
    {
        trafficLight = GetComponentInParent<TrafficLightReference>().trafficLight;
    }

    public TrafficLightController GetTrafficLight()
    {
        return trafficLight;
    }
}
