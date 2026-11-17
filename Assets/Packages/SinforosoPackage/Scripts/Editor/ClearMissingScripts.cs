using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SinforosoPackage
{
    public class ClearMissingScripts : EditorWindow
    {
        [MenuItem("Tools/Clear Missing Scripts")]
        public static void ClearScene()
        {
            if (!EditorUtility.DisplayDialog("Confirmar Limpeza", "Deseja remover todos os scripts missing da cena ativa? Isso é irreversível!", "Sim", "Cancelar"))
                return;

            GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            int totalRemoved = 0;

            foreach (GameObject gameObject in gameObjects)
            {
                totalRemoved += LimparGameObject(gameObject);
            }

            EditorUtility.DisplayDialog("Limpeza Concluída", $"Removidos {totalRemoved} scripts missing!", "OK");

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static int LimparGameObject(GameObject gameObject)
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);

            foreach (Transform childTransform in gameObject.transform)
            {
                removed += LimparGameObject(childTransform.gameObject);
            }

            return removed;
        }
    }
}
