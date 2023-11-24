using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_sceneReset : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}