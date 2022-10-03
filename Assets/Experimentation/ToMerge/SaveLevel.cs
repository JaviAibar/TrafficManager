using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveLevel : MonoBehaviour
{
    public int level;
    public string nombre;
    public Sprite background;
    void Start()
    {

        GameObject myGO = new GameObject();
        myGO.name = "Canvas To Save";
        Canvas canvas = myGO.AddComponent<Canvas>();
        myGO.AddComponent<GraphicRaycaster>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        GameObject newButton = DefaultControls.CreateButton(new DefaultControls.Resources());
        newButton.transform.SetParent(myGO.transform, false);
        newButton.GetComponentInChildren<Text>().text = "Save level!";
        newButton.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                Save();
            });
    }
    void Save()
    {
        if (level == 0)
        {
            throw new Exception("Level not set!");
        }
        else if (background == null)
        {
            throw new Exception("Background not set!");
        }
        else { 
           /* LevelScriptableObject level1 = ScriptableObject.CreateInstance<LevelScriptableObject>();
            level1.background = background;
            level1.id = level;
            string path = "Assets/ToMerge/Levels/Level "+level.ToString("D3")+".asset";
            AssetDatabase.CreateAsset(level1, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = level1;
            print("Hecho");*/
        }
    }
}



