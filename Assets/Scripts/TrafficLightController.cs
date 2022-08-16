using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TrafficLightController : MonoBehaviour
{
    public GameObject trafficLightPanel;
    public TrafficLightUIController trafficLightUIPanel;
    public float red, yellow, green;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f) && hit.transform.name == gameObject.name)
            {
                print(hit.transform.name);
                trafficLightPanel.SetActive(true);
                trafficLightUIPanel.SetValues(red, yellow, green);
                trafficLightUIPanel.SetSender(gameObject.GetComponent<TrafficLightController>());
            }
        }
    }

    internal void SetValues(float? redParam, float? yellowParam, float? greenParam)
    {
        if (redParam != null) red = redParam.Value; 
        if (yellowParam != null) yellow = yellowParam.Value; 
        if (greenParam != null) green = greenParam.Value; 
    }
}
