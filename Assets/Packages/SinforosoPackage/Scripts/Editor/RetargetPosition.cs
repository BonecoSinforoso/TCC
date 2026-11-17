using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SinforosoPackage
{
    public class RetargetPosition : EditorWindow
    {
        private Transform _targetTransform;

        [MenuItem("Tools/Retarget Position")]
        public static void ShowWindow() => GetWindow<RetargetPosition>("Retarget Position");

        private void OnGUI()
        {
            GUILayout.Label("Retarget Parent Position", EditorStyles.boldLabel);

            _targetTransform = (Transform)EditorGUILayout.ObjectField("Target Transform", _targetTransform, typeof(Transform), true);

            GUILayout.Space(10);

            GUI.enabled = _targetTransform != null;

            if (GUILayout.Button("Retarget")) ApplyRetargetPosition();

            GUI.enabled = true;
        }

        private void ApplyRetargetPosition()
        {
            if (_targetTransform == null)
            {
                Debug.LogWarning("No transform selected.");
                return;
            }

            Undo.RegisterCompleteObjectUndo(_targetTransform, "Retarget Parent Position");

            List<Transform> childs = new();
            List<Vector3> positions = new();

            foreach (Transform child in _targetTransform)
            {
                childs.Add(child);
                positions.Add(child.position);
            }

            _targetTransform.position = Vector3.zero;

            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].position = positions[i];
            }

            Debug.Log($"Position reset for '{_targetTransform.name}'");
        }
    }
}
