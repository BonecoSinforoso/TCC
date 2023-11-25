using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_admInicioTudo : MonoBehaviour
{
    [SerializeField] CanvasGroup cg_creditos;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("Scene_menu");
    }

    public void CgCreditosToggle()
    {
        cg_creditos.alpha = cg_creditos.alpha == 0 ? 1 : 0;
        cg_creditos.blocksRaycasts = cg_creditos.alpha == 1;
        cg_creditos.interactable = cg_creditos.alpha == 1;
    }

    public void GameSair()
    {
        Application.Quit();
    }
}