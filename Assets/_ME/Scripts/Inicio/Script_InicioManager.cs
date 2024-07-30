using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Script_InicioManager : MonoBehaviour
{
    public static Script_InicioManager script_InicioManager;

    [SerializeField] CanvasGroup cg_creditos;
    [SerializeField] CanvasGroup cg_tutorial;
    [SerializeField] TextMeshProUGUI[] txt_fundo;
    [SerializeField] float txtFundoChangeCd;
    [SerializeField] TextMeshProUGUI txt_versao;

    [SerializeField] Image img_tutorial;
    [SerializeField] TextMeshProUGUI txt_tutorial;

    [SerializeField] int jogoId;
    [SerializeField] int infoId;
    [SerializeField] Sprite[] spr_infinity;
    [SerializeField] string[] txt_infinity;
    [SerializeField] Sprite[] spr_flappy;
    [SerializeField] string[] txt_flappy;
    [SerializeField] Sprite[] spr_pong;
    [SerializeField] string[] txt_pong;

    void Start()
    {
        Application.targetFrameRate = 60;

        Time.timeScale = 1;

        txt_versao.text = Application.version;

        Invoke(nameof(TextsFundo_Set), txtFundoChangeCd);
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

    public void CgTutorialToggle()
    {
        cg_tutorial.alpha = cg_tutorial.alpha == 0 ? 1 : 0;
        cg_tutorial.blocksRaycasts = cg_tutorial.alpha == 1;
        cg_tutorial.interactable = cg_tutorial.alpha == 1;
    }

    public void GameSair()
    {
        Application.Quit();
    }

    void TextsFundo_Set()
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

        Invoke(nameof(TextsFundo_Set), txtFundoChangeCd);
    }

    public void JogoId_Set(int _valor)
    {
        jogoId = _valor;
        infoId = 0;

        ImgTutorial_Set();
        TxtTutorial_Set();
    }

    public void InfoId_Set(int _valor)
    {
        infoId += _valor;

        if (infoId < 0) infoId = 2;
        if (infoId > 2) infoId = 0;

        ImgTutorial_Set();
        TxtTutorial_Set();
    }

    void ImgTutorial_Set()
    {
        switch (jogoId)
        {
            case 0:
                img_tutorial.sprite = spr_infinity[infoId];
                break;
            case 1:
                img_tutorial.sprite = spr_flappy[infoId];
                break;
            case 2:
                img_tutorial.sprite = spr_pong[infoId];
                break;
        }
    }

    void TxtTutorial_Set()
    {
        switch (jogoId)
        {
            case 0:
                txt_tutorial.text = txt_infinity[infoId];
                break;
            case 1:
                txt_tutorial.text = txt_flappy[infoId];
                break;
            case 2:
                txt_tutorial.text = txt_pong[infoId];
                break;
        }
    }
}