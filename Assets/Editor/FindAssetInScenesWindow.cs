using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class FindAssetInScenesWindow : EditorWindow
{
    private string assetGUID;
    private string assetPath;
    private List<string> scenesUsingAsset = new List<string>();
    private Vector2 scrollPosition;

    public static void ShowWindow(string assetGUID, string assetPath)
    {
        FindAssetInScenesWindow window = GetWindow<FindAssetInScenesWindow>("Find Asset in Scenes");
        window.assetGUID = assetGUID;
        window.assetPath = assetPath;
        window.FindScenes();
        window.Show();
    }

    private void FindScenes()
    {
        scenesUsingAsset.Clear();

        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });

        foreach (string sceneGUID in sceneGUIDs)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            
            string sceneText = File.ReadAllText(scenePath);

            if (sceneText.Contains(assetGUID))
            {
                scenesUsingAsset.Add(scenePath);
            }
        }

        string result = $"escenas para {assetGUID}:\n";
        foreach (string sceneGUID in sceneGUIDs)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            result += "- " + scenePath + " "+ sceneGUID + "\n";
        }
        EditorUtility.DisplayDialog("Resultado de la Búsqueda", result, "OK");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scenes that contain the asset:", EditorStyles.boldLabel);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

        foreach (string scenePath in scenesUsingAsset)
        {
            if (GUILayout.Button(scenePath, EditorStyles.linkLabel))
            {
                OpenSceneAndSearchAsset(scenePath, assetPath);
            }
        }

        GUILayout.EndScrollView();
    }

    private void OpenSceneAndSearchAsset(string scenePath, string assetPath)
    {
        EditorSceneManager.OpenScene(scenePath);

        string searchFilter = $"ref:{assetPath}";

        EditorApplication.delayCall += () => SearchHierarchy(searchFilter);
    }

    private void SearchHierarchy(string searchFilter)
    {
        var hierarchyWindow = GetHierarchyWindow();
        if (hierarchyWindow != null)
        {
            SetSearchFilter(hierarchyWindow, searchFilter);
        }
    }

    private static EditorWindow GetHierarchyWindow()
    {
        var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
        foreach (var window in windows)
        {
            if (window.GetType().Name == "SceneHierarchyWindow")
            {
                return window;
            }
        }
        return null;
    }

    private static void SetSearchFilter(EditorWindow hierarchyWindow, string searchFilter)
    {
        var searchField = hierarchyWindow.GetType().GetField("m_SearchFilter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (searchField != null)
        {
            searchField.SetValue(hierarchyWindow, searchFilter);
            var method = hierarchyWindow.GetType().GetMethod("SetSearchFilter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(hierarchyWindow, new object[] { searchFilter, 0, false, false });
            }
        }
    }
}
