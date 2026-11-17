using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SinforosoPackage
{
    public class ObjectPositioner : EditorWindow
    {
        private Vector3 _startPosition = Vector3.zero;
        private Vector3 _offset = Vector3.zero;
        private List<Transform> _objectsToPosition = new();
        private Vector2 _scroll;

        [MenuItem("Tools/Position Assets")]
        public static void ShowWindow() => GetWindow<ObjectPositioner>("Position Assets");

        private void OnGUI()
        {
            GUILayout.Label("Configurações de Posicionamento", EditorStyles.boldLabel);
            _startPosition = EditorGUILayout.Vector3Field("Posição Inicial", _startPosition);
            _offset = EditorGUILayout.Vector3Field("Offset", _offset);

            GUILayout.Space(10);

            if (GUILayout.Button("Adicionar selecionados"))
            {
                foreach (GameObject go in Selection.gameObjects)
                {
                    if (go != null && !_objectsToPosition.Contains(go.transform)) _objectsToPosition.Add(go.transform);
                }
            }

            if (GUILayout.Button("Limpar lista")) _objectsToPosition.Clear();

            GUILayout.Space(10);

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(150));

            for (int i = 0; i < _objectsToPosition.Count; i++) _objectsToPosition[i] = (Transform)EditorGUILayout.ObjectField(_objectsToPosition[i], typeof(Transform), true);

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            if (GUILayout.Button("Posicionar Objetos"))
            {
                Undo.RecordObjects(_objectsToPosition.ToArray(), "Posicionar Objetos");

                for (int i = 0; i < _objectsToPosition.Count; i++)
                {
                    if (_objectsToPosition[i] != null) _objectsToPosition[i].position = _startPosition + _offset * i;
                }
            }
        }
    }
}
