using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Script_admInicioTudo : MonoBehaviour
{
    [SerializeField] CanvasGroup cg_creditos;
    [SerializeField] TextMeshProUGUI[] txt_fundo;
    [SerializeField] float txtFundoChangeCd;

    void Start()
    {
        Application.targetFrameRate = 60;

        Time.timeScale = 1;

        Invoke(nameof(TextsFundoSet), txtFundoChangeCd);
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

    void TextsFundoSet()
    {
        foreach (TextMeshProUGUI _txt_fundo in txt_fundo)
        {
            string _txt = "";

            for (int i = 0; i < 20; i++)
            {
                _txt += Random.Range(0, 2).ToString();
            }

            _txt_fundo.text = _txt;
        }

        Invoke(nameof(TextsFundoSet), txtFundoChangeCd);
    }
}