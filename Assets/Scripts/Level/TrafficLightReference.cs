using UnityEngine;

namespace Level
{
    public class TrafficLightReference : MonoBehaviour
    {
        /**
     * This piece of code is exclusively to have the reference of the traffic light in its corresponding traffic area
     */
        [SerializeField] private TrafficLightController trafficLight;

        public TrafficLightController TrafficLight => trafficLight;

        private void Start()
        {
            if (!trafficLight)
                throw new System.Exception($"Traffic Light Reference {name} must have a reference of a Traffic Light Controller");
        }
    }
}
