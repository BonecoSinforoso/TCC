using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace SinforosoPackage
{
    public static class ListPopulator
    {
#if UNITY_EDITOR
        public static void PopulateList(Object target, string listFieldName, Type assetType)
        {
            FieldInfo field = target.GetType().GetField(listFieldName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (field == null || !IsList(field.FieldType))
            {
                Debug.LogError($"❌ Campo '{listFieldName}' não é List<> em {target.name}");
                return;
            }

            string[] guids = AssetDatabase.FindAssets($"t:{assetType.Name}");
            IList newList = CreateList(field.FieldType);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object asset = AssetDatabase.LoadAssetAtPath(path, assetType);
                if (asset != null) newList.Add(asset);
            }

            Undo.RecordObject(target, $"Populate {listFieldName}");
            field.SetValue(target, newList);
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();

            Debug.Log($"✅ {newList.Count} '{assetType.Name}' → '{listFieldName}' em {target.name}");
        }
#endif
        private static bool IsList(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

        private static IList CreateList(Type listType) => (IList)Activator.CreateInstance(listType, new object[] { });
    }
}
