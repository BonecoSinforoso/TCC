#if UNITY_EDITOR && UNITY_2021_1_OR_NEWER && !UNITY_6000_3_OR_NEWER
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
public static class SceneSwitcherToolbar
{
    private static string[] _sceneNames = new string[0];
    private static int _selectedIndex = 0;
    private static string _lastActiveScene = "";
    private static VisualElement _toolbarUI;

    private static readonly float _positionOffset = 180f; // Move closer to Play button
    private static readonly float _dropdownBoxHeight = 20f; // Dropdown button height

    private static int CurrentMode
    {
        get => EditorPrefs.GetInt("SceneSwitcher_CurrentMode", 0);
        set => EditorPrefs.SetInt("SceneSwitcher_CurrentMode", value);
    }

    static SceneSwitcherToolbar()
    {
        RefreshSceneList();
        SelectCurrentScene();

        // Hook into scene change events
        EditorSceneManager.activeSceneChangedInEditMode += (prev, current) => UpdateSceneSelection();
        EditorApplication.playModeStateChanged += OnPlayModeChanged;

        EditorApplication.delayCall += AddToolbarUI;
    }

    private static void AddToolbarUI()
    {
        System.Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        if (toolbarType == null) return;

        Object[] toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
        if (toolbars.Length == 0) return;

        Object toolbar = toolbars[0];
        FieldInfo rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
        if (rootField == null) return;

        if (rootField.GetValue(toolbar) is not VisualElement root) return;

        VisualElement leftContainer = root.Q("ToolbarZoneLeftAlign");
        if (leftContainer == null) return;

        // Remove old UI if it exists to prevent duplication
        if (_toolbarUI != null)
        {
            leftContainer.Remove(_toolbarUI);
        }

        _toolbarUI = new IMGUIContainer(OnGUI);
        _toolbarUI.style.marginLeft = _positionOffset;

        leftContainer.Add(_toolbarUI);
    }

    private static void OnGUI()
    {
        if (EditorApplication.isPlaying)
        {
            GUILayout.Label("Scene Switcher disabled during Play Mode.");
            return;
        }

        CheckAndRefreshScenes();

        if (_selectedIndex >= _sceneNames.Length) _selectedIndex = 0;

        GUILayout.BeginHorizontal();

        //Dropdown para selecionar o modo        
        string[] modes = { "Enabled Build Scenes", "All Build Scenes", "All Scenes" };
        int currentMode = CurrentMode;

        int newMode = EditorGUILayout.Popup(currentMode, modes, GUILayout.Width(150), GUILayout.Height(_dropdownBoxHeight));

        if (newMode != currentMode)
        {
            CurrentMode = newMode; // Save the current mode
            RefreshSceneList();
            SelectCurrentScene();
        }
        EditorGUI.EndDisabledGroup();

        //Dropdown para selecionar a cena
        GUIStyle _popupStyle = new(EditorStyles.popup)
        {
            fixedHeight = _dropdownBoxHeight
        };

        int _newIndex = EditorGUILayout.Popup(_selectedIndex, _sceneNames, _popupStyle, GUILayout.Width(150), GUILayout.Height(_dropdownBoxHeight));

        if (_newIndex != _selectedIndex)
        {
            _selectedIndex = _newIndex;
            LoadScene(_sceneNames[_selectedIndex]);
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();
    }

    private static void RefreshSceneList()
    {
        switch (CurrentMode)
        {
            case 0:
                _sceneNames = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => Path.GetFileNameWithoutExtension(scene.path)).ToArray();
                break;
            case 1:
                _sceneNames = EditorBuildSettings.scenes.Select(scene => Path.GetFileNameWithoutExtension(scene.path)).Distinct().ToArray();
                break;
            case 2:
                _sceneNames = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
                break;
        }
    }

    private static void CheckAndRefreshScenes()
    {
        string[] currentScenes;

        switch (CurrentMode)
        {
            case 0:
                currentScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => Path.GetFileNameWithoutExtension(scene.path)).ToArray();
                break;
            case 1:
                currentScenes = EditorBuildSettings.scenes.Select(scene => Path.GetFileNameWithoutExtension(scene.path)).Distinct().ToArray();
                break;
            case 2:
            default:
                currentScenes = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
                break;
        }

        if (!currentScenes.SequenceEqual(_sceneNames))
        {
            _sceneNames = currentScenes;
            SelectCurrentScene();
        }
    }

    private static void SelectCurrentScene()
    {
        string currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
        int index = System.Array.IndexOf(_sceneNames, currentScene);

        if (index != -1)
        {
            _selectedIndex = index;
            _lastActiveScene = currentScene;
        }
        else
        {
            // Append "(not in build index)" if the scene isn't listed
            string notInBuildName = currentScene + " (not in build index)";

            // Insert it at the beginning or replace first element
            _sceneNames = new[] { notInBuildName }.Concat(_sceneNames).ToArray();
            _selectedIndex = 0;
            _lastActiveScene = currentScene;
        }
    }

    private static void UpdateSceneSelection()
    {
        string currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);

        if (currentScene != _lastActiveScene)
        {
            _lastActiveScene = currentScene;

            // Remove any previous "(not in build index)" label to avoid duplicates
            _sceneNames = _sceneNames.Where(name => !name.EndsWith(" (not in build index)")).ToArray();

            SelectCurrentScene();
        }
    }

    private static void LoadScene(string sceneName)
    {
        string scenePath;

        switch (CurrentMode)
        {
            case 0:
                scenePath = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.enabled && scene.path.Contains(sceneName))?.path;
                break;
            case 1:
                scenePath = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path.Contains(sceneName))?.path; // Removido o filtro por scene.enabled
                break;
            case 2:
            default:
                scenePath = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).FirstOrDefault(path => Path.GetFileNameWithoutExtension(path) == sceneName);
                break;
        }

        if (!string.IsNullOrEmpty(scenePath))
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
        else
        {
            Debug.LogError("Scene not found: " + sceneName);
        }
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
        {
            EditorApplication.delayCall += () => AddToolbarUI();
        }
    }
}
#endif
