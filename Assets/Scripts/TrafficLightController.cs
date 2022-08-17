using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(SpriteRenderer))]
public class TrafficLightController : MonoBehaviour
{
    public GameEngine gameEngine;
    public GameObject trafficLightPanel;
    public TrafficLightUIController trafficLightUIPanel;
    public float red, yellow, green;
    public float accumulativeYellow, accumulativeGreen;
    public SpriteRenderer spriteRenderer;
    [SerializeField] private float timer;
    public float speed = 1;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameEngine = FindObjectOfType<GameEngine>();
    }
    private void Start()
    {
        timer = 0;
    }
    void Update()
    {
        timer += Time.deltaTime * speed;
        if (timer % accumulativeYellow <= red)
        {
            spriteRenderer.sprite = gameEngine.trafficLightSprites[(int)GameEngine.trafficLightState.Red];
        } else if (timer % accumulativeYellow <= accumulativeGreen) {
            spriteRenderer.sprite = gameEngine.trafficLightSprites[(int)GameEngine.trafficLightState.Green];
        }
        else
        {
            spriteRenderer.sprite = gameEngine.trafficLightSprites[(int)GameEngine.trafficLightState.Yellow];
        }
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
