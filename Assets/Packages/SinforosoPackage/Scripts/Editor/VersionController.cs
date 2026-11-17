using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class VersionController : EditorWindow
{
    private int _major = 0;
    private int _minor = 0;
    private int _patch = 0;
    private int _release = 0;

    // prefixo/sufixo
    private string _prefix = "";
    private string _suffix = "";

    // versoes
    private string _currentVersion = "";
    private string _previewVersion = "";

    private Vector2 _scrollPosition;
    private bool _autoIncrement = true;
    private GUIStyle _headerStyle;
    private GUIStyle _previewStyle;

    [MenuItem("Version/Version Manager")]
    public static void ShowWindow()
    {
        VersionController window = GetWindow<VersionController>("Version Manager");
        window.minSize = new Vector2(400, 500);
        window.Show();
    }

    private void OnEnable()
    {
        ParseCurrentVersion();
        UpdatePreview();
    }

    private void OnGUI()
    {
        InitializeStyles();

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        DrawHeader();
        DrawCurrentVersionInfo();
        DrawVersionInputs();
        DrawPrefixSuffixInputs();
        DrawOptions();
        DrawPreview();
        DrawButtons();
        DrawQuickActions();

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            UpdatePreview();
        }
    }

    private void InitializeStyles()
    {
        if (_headerStyle == null)
        {
            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };
        }

        if (_previewStyle == null)
        {
            _previewStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
        }
    }

    private void DrawHeader()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Unity Version Manager", _headerStyle);

        EditorGUILayout.Space(10);
    }

    private void DrawCurrentVersionInfo()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Current Version Info", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField($"Bundle Version: {PlayerSettings.bundleVersion}");
        EditorGUILayout.LabelField($"Application Version: {Application.version}");

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refresh", GUILayout.Width(80)))
        {
            ParseCurrentVersion();
            UpdatePreview();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
    }

    private void DrawVersionInputs()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Version Numbers", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Major", GUILayout.Width(60));
        _major = Mathf.Max(0, EditorGUILayout.IntField(_major, GUILayout.Width(60)));

        EditorGUILayout.LabelField("Minor", GUILayout.Width(60));
        _minor = Mathf.Max(0, EditorGUILayout.IntField(_minor, GUILayout.Width(60)));

        EditorGUILayout.LabelField("Patch", GUILayout.Width(60));
        _patch = Mathf.Max(0, EditorGUILayout.IntField(_patch, GUILayout.Width(60)));

        EditorGUILayout.LabelField("Build", GUILayout.Width(60));
        _release = Mathf.Max(0, EditorGUILayout.IntField(_release, GUILayout.Width(60)));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }

    private void DrawPrefixSuffixInputs()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Version Labels", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Prefix:", GUILayout.Width(50));

        _prefix = EditorGUILayout.TextField(_prefix, GUILayout.ExpandWidth(true));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Suffix:", GUILayout.Width(50));

        _suffix = EditorGUILayout.TextField(_suffix, GUILayout.ExpandWidth(true));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Alpha", GUILayout.Height(25)))
        {
            _suffix = "alpha";
            UpdatePreview();
        }

        if (GUILayout.Button("Beta", GUILayout.Height(25)))
        {
            _suffix = "beta";
            UpdatePreview();
        }

        if (GUILayout.Button("RC", GUILayout.Height(25)))
        {
            _suffix = "rc";
            UpdatePreview();
        }

        if (GUILayout.Button("Clear", GUILayout.Height(25)))
        {
            _suffix = "";
            UpdatePreview();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(5);
    }

    private void DrawOptions()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        _autoIncrement = EditorGUILayout.Toggle("Auto-increment on build", _autoIncrement);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(5);
    }

    private void DrawPreview()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField(_previewVersion, _previewStyle, GUILayout.Height(40));

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
    }

    private void DrawButtons()
    {
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("Apply Version", GUILayout.Height(35)))
        {
            ApplyVersion();
        }

        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Reset to Current", GUILayout.Height(35)))
        {
            ParseCurrentVersion();
            UpdatePreview();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
    }

    private void DrawQuickActions()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Major +1"))
        {
            _major++;
            UpdatePreview();
        }

        if (GUILayout.Button("Minor +1"))
        {
            _minor++;
            UpdatePreview();
        }

        if (GUILayout.Button("Patch +1"))
        {
            _patch++;
            UpdatePreview();
        }

        if (GUILayout.Button("Build +1"))
        {
            _release++;
            UpdatePreview();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Next Major\n(Reset Minor/Patch)"))
        {
            _major++;
            _minor = 0;
            _patch = 0;
            UpdatePreview();
        }

        if (GUILayout.Button("Next Minor\n(Reset Patch)"))
        {
            _minor++;
            _patch = 0;
            UpdatePreview();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void ParseCurrentVersion()
    {
        _currentVersion = PlayerSettings.bundleVersion;

        // Regex para capturar prefix, números e suffix
        string pattern = @"^([a-zA-Z]*)\s*(\d+)\.(\d+)\.(\d+)\.(\d+)\s*([a-zA-Z]*)$";
        Match match = Regex.Match(_currentVersion, pattern);

        if (match.Success)
        {
            _prefix = match.Groups[1].Value;
            _major = int.Parse(match.Groups[2].Value);
            _minor = int.Parse(match.Groups[3].Value);
            _patch = int.Parse(match.Groups[4].Value);
            _release = int.Parse(match.Groups[5].Value);
            _suffix = match.Groups[6].Value;
        }
        else
        {
            // Fallback para formato simples
            string[] parts = _currentVersion.Split('.');

            if (parts.Length >= 4)
            {
                _major = int.Parse(parts[0]);
                _minor = int.Parse(parts[1]);
                _patch = int.Parse(parts[2]);
                _release = int.Parse(parts[3]);
            }
        }
    }

    private void UpdatePreview()
    {
        string versionNumbers = $"{_major:D1}.{_minor:D1}.{_patch:D1}.{_release:D1}";

        _previewVersion = "";

        if (!string.IsNullOrEmpty(_prefix))
            _previewVersion += _prefix + " ";

        _previewVersion += versionNumbers;

        if (!string.IsNullOrEmpty(_suffix))
            _previewVersion += " " + _suffix;
    }

    private void ApplyVersion()
    {
        if (EditorUtility.DisplayDialog("Apply Version",
            $"Are you sure you want to change the version to:\n{_previewVersion}?",
            "Yes", "Cancel"))
        {
            PlayerSettings.bundleVersion = _previewVersion;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Version updated to: {_previewVersion}");
            ShowNotification(new GUIContent($"Version set to: {_previewVersion}"));

            // Refresh current version display
            ParseCurrentVersion();
        }
    }
}
