using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class TrafficLightController : MonoBehaviour
{
    public GameEngine gameEngine;
    public GameObject trafficLightPanel;
    public TrafficLightUIController trafficLightUIPanel;
    public float red, yellow, green;
    public float accumulativeYellow, accumulativeGreen;
    public Image image;
    [SerializeField] private float timer;
    public float speed = 0;

    private void Awake()
    {
        image = GetComponent<Image>();
        gameEngine = FindObjectOfType<GameEngine>();
    }
    private void Start()
    {
        timer = 0;
    }
    void Update()
    {
        
        timer += Time.deltaTime * speed;
        if (speed > 0)
        {
            if (timer % accumulativeYellow <= red)
            {
                image.sprite = gameEngine.trafficLightSprites[(int)GameEngine.trafficLightState.Red];
            }
            else if (timer % accumulativeYellow <= accumulativeGreen)
            {
                image.sprite = gameEngine.trafficLightSprites[(int)GameEngine.trafficLightState.Green];
            }
            else
            {
                image.sprite = gameEngine.trafficLightSprites[(int)GameEngine.trafficLightState.Yellow];
            }
        }
      /*  if (Input.GetMouseButtonDown(0))
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
        }*/
    }

    public void OpenMenu()
    {
        trafficLightPanel.SetActive(true);
        trafficLightUIPanel.SetValues(red, yellow, green);
        trafficLightUIPanel.SetSender(gameObject.GetComponent<TrafficLightController>());
    }
        public void SetValues(float? redParam, float? yellowParam, float? greenParam)
    {
        if (redParam != null) red = redParam.Value; 
        if (yellowParam != null) yellow = yellowParam.Value; 
        if (greenParam != null) green = greenParam.Value;

        accumulativeGreen = red + green;
        accumulativeYellow = accumulativeGreen + yellow;
    }

    public void SetSpeed(GameEngine.GameStates state)
    {
        speed = (float)state;
    }
}
