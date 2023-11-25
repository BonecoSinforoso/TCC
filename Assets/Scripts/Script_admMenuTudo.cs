using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_admMenuTudo : MonoBehaviour
{
    void Start()
    {
        //Invoke(nameof(Cu), 2f);
    }

    void Update()
    {

    }

    void Cu()
    {
        SceneManager.LoadScene("Scene_gameInfinity");
    }
}
