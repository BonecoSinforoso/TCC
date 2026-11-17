#if UNITY_EDITOR && !UNITY_2021_1_OR_NEWER
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneSwitcherWindow : EditorWindow
{
    private string[] _sceneNames = new string[0];
    private string[] _scenePaths = new string[0];
    private int _selectedIndex = 0;
    private int _currentMode = 0; // 0 = Enabled Build Scenes, 1 = All Build Scenes, 2 = All Scenes

    private readonly string[] _modeOptions = { "Enabled Build Scenes", "All Build Scenes", "All Scenes" };

    [MenuItem("Tools/Scene Switcher")]
    public static void ShowWindow() => GetWindow<SceneSwitcherWindow>("Scene Switcher");

    private void OnEnable()
    {
        _currentMode = EditorPrefs.GetInt("SceneSwitcher_CurrentMode_Old", 0);
        RefreshSceneList();
        SelectCurrentScene();
        EditorSceneManager.activeSceneChangedInEditMode += (prev, current) => SelectCurrentScene();
    }

    private void OnDisable() => EditorPrefs.SetInt("SceneSwitcher_CurrentMode_Old", _currentMode);

    private void OnGUI()
    {
        if (EditorApplication.isPlaying)
        {
            GUILayout.Label("Scene Switcher disabled during Play Mode.");
            return;
        }

        GUILayout.Label("Scene Switcher", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Dropdown de modos
        int newMode = EditorGUILayout.Popup("Mode:", _currentMode, _modeOptions);
        if (newMode != _currentMode)
        {
            _currentMode = newMode;
            RefreshSceneList();
            SelectCurrentScene();
        }

        EditorGUILayout.Space();

        if (_sceneNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes found.", MessageType.Warning);
            return;
        }

        // Dropdown de cenas
        int newIndex = EditorGUILayout.Popup("Select Scene:", _selectedIndex, _sceneNames);
        if (newIndex != _selectedIndex)
        {
            _selectedIndex = newIndex;
            LoadScene(_scenePaths[_selectedIndex]);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Refresh Scenes"))
        {
            RefreshSceneList();
            SelectCurrentScene();
        }
    }

    private void RefreshSceneList()
    {
        switch (_currentMode)
        {
            case 0: // Enabled Build Scenes
                _scenePaths = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
                break;

            case 1: // All Build Scenes
                _scenePaths = EditorBuildSettings.scenes.Select(scene => scene.path).Distinct().ToArray();
                break;

            case 2: // All Project Scenes
                _scenePaths = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);
                break;
        }

        _sceneNames = _scenePaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
    }

    private void SelectCurrentScene()
    {
        string currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
        int index = System.Array.IndexOf(_sceneNames, currentScene);

        if (index != -1) _selectedIndex = index;
        else if (_sceneNames.Length > 0) _selectedIndex = 0;
    }

    private void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scenePath);
    }
}
#endif
