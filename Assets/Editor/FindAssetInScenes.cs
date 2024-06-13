using UnityEditor;
using UnityEngine;

public class FindAssetInScenes : Editor
{
    [MenuItem("Assets/Find Asset in Scenes", false, 1000)]
    private static void FindAsset()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);

        FindAssetInScenesWindow.ShowWindow(assetGUID, assetPath);
    }
}
