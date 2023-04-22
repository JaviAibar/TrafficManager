using UnityEngine;
using static Level.GameEngine;

namespace Level
{
    public class EventManager : MonoBehaviour
    {
        public delegate void TrafficLightControlEvent(TrafficLightController.TrafficLightColour colour);
        public delegate void LevelConditionAlteredEvent(RoadUser responsible);
        public delegate void AccidentEvent(RoadUser affected1, RoadUser affected2);
        public delegate void GameSpeedControlEvent(GameSpeed speed);
        public delegate void TimeControlEvent();

        public static event TrafficLightControlEvent OnTrafficLightChanged;

        public static event TimeControlEvent OnLoopStarted;

        public static event LevelConditionAlteredEvent OnRoadUserStopped;
        public static event LevelConditionAlteredEvent OnRoadUserMoving;
        public static event AccidentEvent OnRoadUserCollision; 

        public static event GameSpeedControlEvent OnGameSpeedChanged;
    
        public static void RaiseOnTrafficLightChanged(TrafficLightController.TrafficLightColour colour)
        {
            OnTrafficLightChanged?.Invoke(colour);
        }

        public static void RaiseOnTimeIsOver()
        {
            OnLoopStarted?.Invoke();
        }

        public static void RaiseOnRoadUserStopped(RoadUser responsible)
        {
            OnRoadUserStopped?.Invoke(responsible);
        }
        public static void RaiseOnGameSpeedChanged(GameSpeed speed)
        {
            OnGameSpeedChanged?.Invoke(speed);
        }

        public static void RaiseOnRoadUserCollision(RoadUser affected1, RoadUser affected2)
        {
            OnRoadUserCollision?.Invoke(affected1, affected2);
        }

        public static void RaiseOnLoopStarted()
        {
            OnLoopStarted?.Invoke();
        }

        public static void RaiseOnRoadUserMoving(RoadUser responsible)
        {
            OnRoadUserMoving?.Invoke(responsible);
        }
    }
}
