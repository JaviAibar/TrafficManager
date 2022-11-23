using UnityEngine;

namespace Level
{
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

#pragma warning disable CS0108, CS0114
        private BoxCollider2D collider;
#pragma warning restore CS0108, CS0114

        void Start() => trafficLight = GetComponentInParent<TrafficLightReference>().trafficLight;

        public bool SameDirection(Vector3 dir) => GameEngine.Vector3ToDirection(dir) == direction;

    
    }
}
