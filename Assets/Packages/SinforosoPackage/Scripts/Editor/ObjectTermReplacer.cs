using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SinforosoPackage
{
    public class ObjectTermReplacer : EditorWindow
    {
        private List<Object> _objectsToRename = new();
        private string _search;
        private string _replace;
        private Vector2 _scroll;

        [MenuItem("Tools/Replace In Asset Names")]
        public static void ShowWindow() => GetWindow<ObjectTermReplacer>("Replace In Names");

        private void OnGUI()
        {
            GUILayout.Label("Substituir termo no nome", EditorStyles.boldLabel);

            _search = EditorGUILayout.TextField("Procurar por", _search);
            _replace = EditorGUILayout.TextField("Substituir por", _replace);

            GUILayout.Space(10);

            if (GUILayout.Button("Adicionar selecionados"))
            {
                foreach (Object obj in Selection.objects)
                {
                    if (!_objectsToRename.Contains(obj))
                        _objectsToRename.Add(obj);
                }
            }

            if (GUILayout.Button("Limpar lista"))
                _objectsToRename.Clear();

            GUILayout.Space(10);

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(150));

            for (int i = 0; i < _objectsToRename.Count; i++)
                _objectsToRename[i] = EditorGUILayout.ObjectField(_objectsToRename[i], typeof(Object), true);

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            if (GUILayout.Button("Aplicar substituição"))
            {
                for (int i = 0; i < _objectsToRename.Count; i++)
                {
                    Object obj = _objectsToRename[i];
                    if (obj == null) continue;

                    string oldName = obj.name;

                    if (string.IsNullOrEmpty(_search) || !oldName.Contains(_search))
                        continue;

                    string newName = oldName.Replace(_search, _replace);

                    string path = AssetDatabase.GetAssetPath(obj);
                    if (!string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.RenameAsset(path, newName); // renomeia mantendo resto igual [web:8][web:10]
                    }
                    else if (obj is GameObject go)
                    {
                        go.name = newName;
                    }
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
