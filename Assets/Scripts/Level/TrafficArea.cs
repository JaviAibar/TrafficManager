using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficArea : MonoBehaviour
{
    /**
     *  This script will control all colliders of the crossroads
     */

    [SerializeField] private bool stopArea;
    public bool StopArea => stopArea;

    public GameEngine.Direction direction;

    private TrafficLightController trafficLight;
    public TrafficLightController TrafficLight => trafficLight;
    public bool IsCenter => direction == GameEngine.Direction.Center;

    void Start() => trafficLight = GetComponentInParent<TrafficLightReference>().trafficLight;

    public bool SameDirection(Vector3 dir) => GameEngine.Vector3ToDirection(dir) == direction;
}
