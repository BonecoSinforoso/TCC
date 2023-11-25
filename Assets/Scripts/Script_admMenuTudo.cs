using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_admMenuTudo : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("Scene_gameInfinity");
    }
}