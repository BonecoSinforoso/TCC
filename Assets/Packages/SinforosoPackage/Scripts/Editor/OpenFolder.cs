using UnityEditor;
using UnityEngine;

public class OpenFolder : EditorWindow
{
    [MenuItem("Folder/Scripts")]
    private static void Scripts() => GoTo("Assets/_ME/Scripts");

    [MenuItem("Folder/Scenes")]
    private static void Scenes() => GoTo("Assets/_ME/Scenes");

    private static void GoTo(string folderPath)
    {
        string[] assets = AssetDatabase.FindAssets("", new[] { folderPath });

        string assetPath = folderPath;

        if (assets.Length > 0) assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

        Object folderObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
        Selection.activeObject = folderObject;
        EditorGUIUtility.PingObject(folderObject);
    }
}
