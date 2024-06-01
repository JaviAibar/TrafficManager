using System;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using Level;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class QuickSettingsEditor : EditorWindow
{
   // public GameEngine gameEngine;
   public GameEngine.VerboseEnum verbose;
   // In the future this should be a list, and therefore a selector of all traffic lights
   [SerializeField]
   //public TrafficLightController trafficLight;
   public static int red;
   public static int yellow;
   public static int green;
   public static float trafficLightOffset;

   [SerializeField] public bool applyChanges = false;
   
    [MenuItem("TrafficManager/Quick Settings")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(QuickSettingsEditor));
        window.titleContent = new GUIContent("Quick Settings");
        window.Show();
    }
   /* private void Awake()
    {
        Debug.Log("Awake!");
        Debug.Log(trafficLight);
        
        /*red = trafficLight.TimeRed;
        yellow = trafficLight.TimeYellow;
        green = trafficLight.TimeGreen;*/
        /*trafficLight.TimeRed = red;
        Debug.Log($"Resultado {trafficLight.TimeRed}");
        trafficLight.TimeYellow = yellow;
        trafficLight.TimeGreen = green;
    }*/
    void OnGUI()
    {
        GameEngine g = FindObjectOfType<GameEngine>();
        TrafficLightController trafficLight = FindObjectOfType<TrafficLightController>();
//        g.Awake();
//        trafficLight.hideFlags = HideFlags.DontSave;
        verbose = (GameEngine.VerboseEnum)EditorGUILayout.EnumFlagsField("Verbose", verbose);
        g.Verbose = verbose;
        red = EditorGUILayout.IntSlider("Red", red, 1, 10);
        yellow = EditorGUILayout.IntSlider("Yellow", yellow, 1, 10);
        green = EditorGUILayout.IntSlider("Green", green, 1, 10);
        trafficLightOffset = EditorGUILayout.Slider("trafficLightOffset", trafficLightOffset, 0, 10);
        
        if (GUILayout.Button("Start recording Traffic Light changes"))
        {
            applyChanges = true;
        }
        if (GUILayout.Button("Stop recording Traffic Light changes"))
        {
            applyChanges = false;
        }
        if (applyChanges)
        {
            trafficLight.TimeRed = red;
            trafficLight.TimeYellow = yellow;
            trafficLight.TimeGreen = green;
            trafficLight.timeOffset = trafficLightOffset;
        }
        
        
    }


   /* void Update()
    {
        if (EditorApplication.isPlaying)
        {
            
            if (!trafficLight) trafficLight = FindObjectOfType<TrafficLightController>();
            if (applyChanges)
            {
                Debug.Log($"Nuevo valor {red}");
                trafficLight.TimeRed = red;
                Debug.Log($"Resultado {trafficLight.TimeRed}");
                trafficLight.TimeYellow = yellow;
                trafficLight.TimeGreen = green;
                applyChanges = false;
            }
        }*/
/*
        if (!level) return;
            if (Time.frameCount <= 5)
            {
                frames = new List<LevelEditorHelper.FrameInfo>();
                timeElapsed = 0;
            }

            if (Time.frameCount == 5)
            {
                Debug.Log(
                    "WARNING: Level Editor Helper is open, that means that frames will recorded and PlayMode will be stopped when time to loop is reached. If you want to check how it loops, please close Level Editor Helper window. Thank you");
            }

            LevelEditorHelper.FrameInfo frameInfo = new LevelEditorHelper.FrameInfo();
            frameInfo.users = new List<LevelEditorHelper.RoadUserInfo>();
            frameInfo.lights = new List<LevelEditorHelper.TrafficLightInfo>();
            foreach (RoadUser r in roadUsers)
            {
                LevelEditorHelper.RoadUserInfo rui = new LevelEditorHelper.RoadUserInfo();
                rui.instanceID = r.GetInstanceID();
                rui.position = new LevelEditorHelper.Vector3Serializable(r.transform.position.x, r.transform.position.y);
                rui.zRotation = r.transform.rotation.eulerAngles.z;
                frameInfo.users.Add(rui);
                //Debug.Log("Saved " + frames.Count + " frames");
            }

            foreach (TrafficLightController t in trafficLights)
            {
                LevelEditorHelper.TrafficLightInfo tli = new LevelEditorHelper.TrafficLightInfo();
                tli.instanceID = t.GetInstanceID();
                tli.colour = t.State;
                tli.text = t.TimerText;
                frameInfo.lights.Add(tli);
            }

            timeElapsed += Time.deltaTime;
            frameInfo.time = timeElapsed;
            frameInfo.solveIndicator.solvedSprite = level.iconImage.sprite;
            frameInfo.solveIndicator.timeLeft = level.timeIndicator.text;

            frames.Add(frameInfo);

            if (stopAtLoop && Time.time >= level.timeToLoop) EditorApplication.ExitPlaymode();
        }*/
    //}
}