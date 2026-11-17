using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SinforosoPackage
{
    public class ObjectRenamer : EditorWindow
    {
        private List<Object> _renamedObjects = new();
        private string _newName;
        private string _sufix;
        private Vector2 _scroll;
        private bool _hasSufix;

        [MenuItem("Tools/Rename Assets")]
        public static void ShowWindow() => GetWindow<ObjectRenamer>("Rename Assets");

        private void OnGUI()
        {
            GUILayout.Label("Configurações de Renomeação", EditorStyles.boldLabel);

            _newName = EditorGUILayout.TextField("Nome", _newName);
            _hasSufix = EditorGUILayout.Toggle("Tem sufixo?", _hasSufix);
            _sufix = EditorGUILayout.TextField("Sufixo", _sufix);

            GUILayout.Space(10);

            if (GUILayout.Button("Adicionar selecionados"))
            {
                foreach (Object obj in Selection.objects)
                {
                    if (!_renamedObjects.Contains(obj)) _renamedObjects.Add(obj);
                }
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Limpar lista")) _renamedObjects.Clear();

            GUILayout.Space(10);

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(150));

            for (int i = 0; i < _renamedObjects.Count; i++) _renamedObjects[i] = EditorGUILayout.ObjectField(_renamedObjects[i], typeof(Object), true);

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            if (GUILayout.Button("Renomear Objetos"))
            {
                for (int i = 0; i < _renamedObjects.Count; i++)
                {
                    Object obj = _renamedObjects[i];

                    if (obj == null) continue;

                    string path = AssetDatabase.GetAssetPath(obj);

                    if (!string.IsNullOrEmpty(path))
                    {
                        string number = i < 10 ? $"0{i}" : i.ToString();

                        string newName = $"{_newName}_{number}";

                        if (_hasSufix)
                        {
                            newName = $"{newName}_{_sufix}";
                        }

                        AssetDatabase.RenameAsset(path, newName);
                    }
                    else if (obj is GameObject go)
                    {
                        string number = i < 10 ? $"0{i}" : i.ToString();
                        go.name = $"{_newName}_{number}_{_sufix}";
                    }
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
