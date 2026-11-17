#if UNITY_EDITOR && UNITY_6000_3_OR_NEWER
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

[InitializeOnLoad]
public static class SceneSwitcherToolbarToUnity6
{
    private static string[] _sceneNames = new string[0];
    private static string _lastActiveScene = "";

    private static int CurrentMode
    {
        get => EditorPrefs.GetInt("SceneSwitcher_CurrentMode", 0);
        set => EditorPrefs.SetInt("SceneSwitcher_CurrentMode", value);
    }

    static SceneSwitcherToolbarToUnity6()
    {
        RefreshSceneList();
        SelectCurrentScene();
        EditorSceneManager.activeSceneChangedInEditMode += (prev, current) => UpdateSceneSelection();
    }

    [MainToolbarElement("SceneSwitcher/Mode", defaultDockPosition = MainToolbarDockPosition.Left)]
    public static MainToolbarElement ModeDropdown()
    {
        string[] modes = { "Enabled Build Scenes", "All Build Scenes", "All Scenes" };
        MainToolbarContent content = new MainToolbarContent(modes[CurrentMode]);

        return new MainToolbarDropdown(content, (rect) =>
        {
            if (EditorApplication.isPlaying) return;

            GenericMenu menu = new();

            for (int i = 0; i < modes.Length; i++)
            {
                int index = i;

                menu.AddItem(new GUIContent(modes[i]), CurrentMode == index, () =>
                {
                    CurrentMode = index;
                    RefreshSceneList();
                    SelectCurrentScene();

                    MainToolbar.Refresh("SceneSwitcher/Mode");
                    MainToolbar.Refresh("SceneSwitcher/Scenes");
                });
            }
            menu.DropDown(rect);
        });
    }

    [MainToolbarElement("SceneSwitcher/Scenes", defaultDockPosition = MainToolbarDockPosition.Left)]
    public static MainToolbarElement SceneDropdown()
    {
        CheckAndRefreshScenes();
        string currentDisplay = string.IsNullOrEmpty(_lastActiveScene) ? "Select Scene" : _lastActiveScene;
        MainToolbarContent content = new MainToolbarContent(currentDisplay);

        return new MainToolbarDropdown(content, (rect) =>
        {
            if (EditorApplication.isPlaying) return;

            CheckAndRefreshScenes();
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < _sceneNames.Length; i++)
            {
                string sceneName = _sceneNames[i];

                menu.AddItem(new GUIContent(sceneName), sceneName == _lastActiveScene, () =>
                {
                    LoadScene(sceneName);
                    MainToolbar.Refresh("SceneSwitcher/Scenes");
                });
            }
            menu.DropDown(rect);
        });
    }

    private static void RefreshSceneList()
    {
        switch (CurrentMode)
        {
            case 0: _sceneNames = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => Path.GetFileNameWithoutExtension(s.path)).ToArray(); break;
            case 1: _sceneNames = EditorBuildSettings.scenes.Select(s => Path.GetFileNameWithoutExtension(s.path)).Distinct().ToArray(); break;
            case 2: _sceneNames = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).Select(p => Path.GetFileNameWithoutExtension(p)).ToArray(); break;
        }
    }

    private static void CheckAndRefreshScenes()
    {
        string[] currentScenes = CurrentMode switch
        {
            0 => EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => Path.GetFileNameWithoutExtension(s.path)).ToArray(),
            1 => EditorBuildSettings.scenes.Select(s => Path.GetFileNameWithoutExtension(s.path)).Distinct().ToArray(),
            _ => Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).Select(p => Path.GetFileNameWithoutExtension(p)).ToArray(),
        };

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
            _lastActiveScene = currentScene;
        }
        else
        {
            _lastActiveScene = currentScene + " (not in build index)";
            _sceneNames = new[] { _lastActiveScene }.Concat(_sceneNames).ToArray();
        }
    }

    private static void UpdateSceneSelection()
    {
        string currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);

        if (currentScene != _lastActiveScene)
        {
            _lastActiveScene = currentScene;
            _sceneNames = _sceneNames.Where(name => !name.EndsWith(" (not in build index)")).ToArray();
            SelectCurrentScene();

            MainToolbar.Refresh("SceneSwitcher/Scenes");
        }
    }

    private static void LoadScene(string sceneName)
    {
        sceneName = sceneName.Replace(" (not in build index)", "");
        string scenePath = "";

        scenePath = CurrentMode switch
        {
            0 => EditorBuildSettings.scenes.FirstOrDefault(s => s.enabled && s.path.Contains(sceneName))?.path,
            1 => EditorBuildSettings.scenes.FirstOrDefault(s => s.path.Contains(sceneName))?.path,
            _ => Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == sceneName),
        };

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
}
#endif
