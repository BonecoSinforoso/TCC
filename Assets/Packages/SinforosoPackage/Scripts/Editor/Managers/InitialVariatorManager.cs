using UnityEditor;
using UnityEngine;

namespace SinforosoPackage
{
    public class InitialVariatorManager : EditorWindow
    {
        private float _pos;
        private float _size;
        private float _rotX;
        private float _rotY;
        private float _rotZ;

        [MenuItem("Tools/Apply to All InitialVariators")]
        public static void ShowWindow() => GetWindow<InitialVariatorManager>("InitialVariator Manager");

        private void OnGUI()
        {
            GUILayout.Label("Configurações Padrão para Todos", EditorStyles.boldLabel);

            _pos = EditorGUILayout.FloatField("Position Range", _pos);
            _size = EditorGUILayout.FloatField("Size Variation", _size);
            _rotX = EditorGUILayout.FloatField("Rotação X", _rotX);
            _rotY = EditorGUILayout.FloatField("Rotação Y", _rotY);
            _rotZ = EditorGUILayout.FloatField("Rotação Z", _rotZ);

            GUILayout.Space(10);

            if (GUILayout.Button("Aplicar a Todos na Cena"))
            {
                ApplyToAll();
            }
        }

        private void ApplyToAll()
        {
            InitialVariator[] initialVariators = FindObjectsByType<InitialVariator>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (InitialVariator initialVariator in initialVariators)
            {
                Undo.RecordObject(initialVariator, "Update InitialVariator");

                initialVariator.GetType().GetField("position", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(initialVariator, _pos);
                initialVariator.GetType().GetField("size", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(initialVariator, _size);
                initialVariator.GetType().GetField("rotX", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(initialVariator, _rotX);
                initialVariator.GetType().GetField("rotY", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(initialVariator, _rotY);
                initialVariator.GetType().GetField("rotZ", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(initialVariator, _rotZ);

                EditorUtility.SetDirty(initialVariator);
            }

            Debug.Log($"Atualizados {initialVariators.Length} InitialVariator(s)");
        }
    }
}
