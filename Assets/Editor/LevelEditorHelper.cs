using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Level;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Level.TrafficLightController;

[ExecuteInEditMode]
public class LevelEditorHelper : EditorWindow
{
    static int frame = 0;
    static float timeElapsed = 0;
    [SerializeField]
    public static bool stopAtLoop;

    public static LevelManager level;

    [SerializeField]
    public static RoadUser[] roadUsers;
    [SerializeField]
    public static TrafficLightController[] trafficLights;
    [SerializeField]
    public static List<FrameInfo> frames;
    [System.Serializable]
    public struct FrameInfo
    {
        public List<RoadUserInfo> users;
        public List<TrafficLightInfo> lights;
        public SolveIndicatorInfo solveIndicator;
        public float time;
    }

    [System.Serializable]
    public struct RoadUserInfo
    {

        public int instanceID;
        public Vector3Serializable position;
        public float zRotation;
    }

    [System.Serializable]
    public struct SolveIndicatorInfo
    {
        public Sprite solvedSprite;
        public string timeLeft;
    }

    [System.Serializable]
    public struct TrafficLightInfo
    {

        public int instanceID;
        [SerializeField]
        public TrafficLightColour colour;
        public string text;
    }


    [System.Serializable]
    public struct Vector3Serializable
    {
        public float x;
        public float y;
        public Vector3Serializable(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public override string ToString() => "[" + x + ", " + y + "]";
    }

    [MenuItem("Window/Level Creator Helper")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(LevelEditorHelper));
        window.Show();
    }
    private void OnEnable()
    {
        //EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
    }

    private void OnDisable()
    {

    }
    /*
    private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
    {
        Debug.Log("EditorApplication.isPlaying " + EditorApplication.isPlaying);
        if (!EditorApplication.isPlaying)
        {
            WriteFile();
        }
    }

    */


    void OnGUI()
    {
        level = GameObject.Find("Level Manager")?.GetComponent<LevelManager>();
        if (!level)
        {
            EditorGUILayout.LabelField("This window works only on a Level Scene:");
            EditorGUILayout.LabelField("Change to a Level Scene or add a Level Manager to this Scene");
            return;
        }

        roadUsers = FindObjectsOfType<RoadUser>();
        trafficLights = FindObjectsOfType<TrafficLightController>();
        EditorGUILayout.LabelField("Level time " + level.timeToLoop);
        stopAtLoop = EditorGUILayout.Toggle("Stop at the loop start", stopAtLoop); 


        if (frames != null && EditorApplication.isPlaying) EditorGUILayout.LabelField("Simulation still on play...");
        if (frames != null && frames.Count > 0 && !EditorApplication.isPlaying)
        {
            //Debug.Log("Pidiendo " + timer);

            frame = EditorGUILayout.IntSlider(frame, 0, frames.Count - 1);
            EditorGUILayout.LabelField("Frames guardados " + frames.Count);
            EditorGUILayout.LabelField("Selected frame: " + frame);
            roadUsers = FindObjectsOfType<RoadUser>();
            trafficLights = FindObjectsOfType<TrafficLightController>();
            FrameInfo selectedFrameInfo = frames[frame];
            for (int i = 0; i < roadUsers.Length; i++)
            {
                RoadUserInfo selectedRoadUserInfo = selectedFrameInfo.users.Find(x => x.instanceID == roadUsers[i].GetInstanceID());

                roadUsers[i].transform.transform.position = new Vector3(selectedRoadUserInfo.position.x, selectedRoadUserInfo.position.y);
                roadUsers[i].transform.eulerAngles = new Vector3(0, 0, selectedRoadUserInfo.zRotation);
            }

            for (int i = 0; i < trafficLights.Length; i++)
            {
                TrafficLightInfo selectedTrafficLightInfo = selectedFrameInfo.lights.Find(x => x.instanceID == trafficLights[i].GetInstanceID());
                trafficLights[i].GetComponent<Image>().sprite = GameEngine.instance.TrafficLightSprites[(int)selectedTrafficLightInfo.colour];
                trafficLights[i].GetComponentInChildren<TMPro.TMP_Text>().text = selectedTrafficLightInfo.text;
            }
            level.timeIndicator.text = selectedFrameInfo.solveIndicator.timeLeft;
            level.iconImage.sprite = selectedFrameInfo.solveIndicator.solvedSprite;
            EditorGUILayout.LabelField("Time elapsed in seconds: " + selectedFrameInfo.time);

        }
        if (GUILayout.Button("Play"))
            EditorApplication.EnterPlaymode();

        if (GUILayout.Button("Reset"))
            ResetInfo();
    }

    void ResetInfo() => frames = new List<FrameInfo>();

    void OnInspectorUpdate()
    {
        /*foreach (RoadUser r in roadUsers)
        {
            BezierSolution.BezierWalkerWithSpeed bezier = r.GetComponentInParent<BezierSolution.BezierWalkerWithSpeed>();
            //r.timer = timer;
            /*float constant = (r.speed * Time.deltaTime) / (bezier.spline.loop ? bezier.spline.Count : bezier.spline.Count * 3);
            for (int i = 0; i < 3; i++)
                bezier.NormalizedT += constant / bezier.spline.GetTangent(normalizedT).magnitude;
            bezier.NormalizedT = timer;*/
        /*if (timer > r.timeOffset)
        {
            float normalizedT = bezier.NormalizedT;
            Vector3 targetPos = bezier.spline.MoveAlongSpline(ref normalizedT, -bezier.speed);
            bezier.spline.
            r.transform.position = targetPos;
            bezier.NormalizedT = normalizedT;
            //bezier.Execute(0.5f);
        }
    }*/
        /*if (Selection.activeTransform)
            Selection.activeTransform.localScale = new Vector3(scale, scale, scale);*/
        /*
        if (frames != null && frames.Count > 0 && !EditorApplication.isPlaying)
        {
            Debug.Log("Pidiendo " + timer);

            roadUsers = FindObjectsOfType<RoadUser>();
            trafficAreas = FindObjectsOfType<TrafficLightController>();
            FrameInfo selectedFrameInfo = frames[timer];
            for (int i = 0; i < roadUsers.Length; i++)
            {
                RoadUser selectedRoadUserInfo = selectedFrameInfo.users.Find(x => x.GetInstanceID() == roadUsers[i].GetInstanceID());
                Debug.Log("RoadUsers " + roadUsers);
                Debug.Log("RoadUsers[0] " + roadUsers[0]);
                Debug.Log("selectedRoadUserInfo " + selectedRoadUserInfo);
                Debug.Log("frames[0].users " + frames[0].users);
                Debug.Log("frames[0].users[0] " + frames[0].users[0]);

                roadUsers[i].transform.position = selectedRoadUserInfo.transform.position;
            }
        }*/
    }



    void Update()
    {
        if (EditorApplication.isPlaying)
        {
            if (!level) return;
            if (Time.frameCount <= 5)
            {
                frames = new List<FrameInfo>();
                timeElapsed = 0;
            }
            if (Time.frameCount == 5)
            {
                Debug.Log("WARNING: Level Editor Helper is open, that means that frames will recorded and PlayMode will be stopped when time to loop is reached. If you want to check how it loops, please close Level Editor Helper window. Thank you");
            }

            FrameInfo frameInfo = new FrameInfo();
            frameInfo.users = new List<RoadUserInfo>();
            frameInfo.lights = new List<TrafficLightInfo>();
            foreach (RoadUser r in roadUsers)
            {
                RoadUserInfo rui = new RoadUserInfo();
                rui.instanceID = r.GetInstanceID();
                rui.position = new Vector3Serializable(r.transform.position.x, r.transform.position.y);
                rui.zRotation = r.transform.rotation.eulerAngles.z;
                frameInfo.users.Add(rui);
                //Debug.Log("Saved " + frames.Count + " frames");
            }
            foreach (TrafficLightController t in trafficLights)
            {
                TrafficLightInfo tli = new TrafficLightInfo();
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
        }
    }
    /*private void WriteFile()
    {
        string path = $"{Application.dataPath}/tmp.dat";
        Debug.Log(path);
        Debug.Log("Frames al guardar " + frames[0]);
        File.WriteAllBytes(path, ObjectToByteArray(frames));
    }

    private void ReadFile()
    {
        string path = $"{Application.dataPath}/tmp.dat";
        if (!File.Exists(path))
        {
            EditorUtility.DisplayDialog("No se encuentra el archivo",
                $"El archivo tmp.dat no se encuentra", "Aceptar");
        }
        else
        {

            ByteArrayToObject(File.ReadAllBytes(path));
        }
    }
    */
    // Convert an object to a byte array
    private byte[] ObjectToByteArray(List<FrameInfo> obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        Debug.Log("ms is nujll " + ms);
        return ms.ToArray();
    }

    // Convert a byte array to an Object
    private void ByteArrayToObject(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        frames = (List<FrameInfo>)binForm.Deserialize(memStream);
    }
}
