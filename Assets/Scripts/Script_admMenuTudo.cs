using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_admMenuTudo : MonoBehaviour
{
    [SerializeField] string[] cenaNome;

    public void SceneChange(int _valor)
    {

        SceneManager.LoadScene(cenaNome[_valor]);
    }
}