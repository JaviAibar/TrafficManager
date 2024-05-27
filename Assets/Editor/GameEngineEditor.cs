//C# Example (LookAtPointEditor.cs)
/*using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEngine))]
//[CanEditMultipleObjects]
public class LookAtPointEditor : Editor
{
    SerializedProperty verbose;
    static string[] options = new string[] {"Speed", "Physics", "TrafficLightChanges", "Everything" };

    void OnEnable()
    {
        verbose = serializedObject.FindProperty("verbose");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        verbose.intValue = EditorGUILayout.MaskField("Verbose", verbose.intValue, options);
      //  verbose.intValue = EditorGUILayout.MaskField("Verbose", verbose.intValue, options);
     //   Debug.Log($"Selected {verbose.intValue}");
        /*if (lookAtPoint.vector3Value.y > (target as GameEngine).transform.position.y)
        {
            EditorGUILayout.LabelField("(Above this object)");
        }
        if (lookAtPoint.vector3Value.y < (target as GameEngine).transform.position.y)
        {
            EditorGUILayout.LabelField("(Below this object)");
        }*/


     /*   serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        /*var t = (target as GameEngine);

        EditorGUI.BeginChangeCheck();
        Vector3 pos = Handles.PositionHandle(t.lookAtPoint, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Move point");
            t.lookAtPoint = pos;
            t.Update();
        }*/
   // }
//}
