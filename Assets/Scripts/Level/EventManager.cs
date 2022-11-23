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
            if (OnTrafficLightChanged != null) OnTrafficLightChanged(colour);
        }

        public static void RaiseOnTimeIsOver()
        {
            if (OnLoopStarted != null) OnLoopStarted();
        }

        public static void RaiseOnRoadUserStopped(RoadUser responsible)
        {
            if (OnRoadUserStopped != null) OnRoadUserStopped(responsible);
        }
        public static void RaiseOnGameSpeedChanged(GameSpeed speed)
        {
            if (OnGameSpeedChanged != null) OnGameSpeedChanged(speed);
        }

        public static void RaiseOnRoadUserCollision(RoadUser affected1, RoadUser affected2)
        {
            if (OnRoadUserCollision != null) OnRoadUserCollision(affected1, affected2);
        }

        public static void RaiseOnLoopStarted()
        {
            if (OnLoopStarted != null) OnLoopStarted();
        }

        public static void RaiseOnRoadUserMoving(RoadUser responsible)
        {
            if (OnRoadUserMoving != null) OnRoadUserMoving(responsible);
        }
    }
}
