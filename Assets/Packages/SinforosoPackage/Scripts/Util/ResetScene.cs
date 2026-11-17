using UnityEngine;
using UnityEngine.SceneManagement;

namespace SinforosoPackage
{
    public class ResetScene : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
