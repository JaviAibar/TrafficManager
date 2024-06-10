using UnityEngine;

namespace Level
{
    /**
     *  This script will control all colliders of the crossroads
     */
    public class TrafficArea : MonoBehaviour
    {
        [SerializeField] private bool stopArea;
        [SerializeField] private GameEngine.Direction direction;

        private TrafficLightController trafficLight;
        private BoxCollider2D boxCollider;

        public TrafficLightController TrafficLight => trafficLight;
        public bool StopArea => stopArea;
        public bool IsCenter => direction == GameEngine.Direction.Center;

        void Start() => trafficLight = GetComponentInParent<TrafficLightReference>().TrafficLight;

        public bool SameDirection(Vector3 dir) => GameEngine.Vector3ToDirection(dir) == direction;
    }
}
