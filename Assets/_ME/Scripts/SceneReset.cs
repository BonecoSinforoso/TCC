using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReset : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ResetScene();
    }

    public void ResetScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
