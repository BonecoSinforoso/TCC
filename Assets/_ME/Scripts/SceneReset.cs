using UnityEngine;
using UnityEngine.SceneManagement;

namespace TCC
{
    public class SceneReset : MonoBehaviour
    {
        #region membros

        #endregion

        #region metodos

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) ResetScene();
        }

        public void ResetScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        #endregion
    }
}
