using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Script_GeralManager : MonoBehaviour
{
    [SerializeField] CanvasGroup cg_pausa;

    [SerializeField] GameObject obj_fb;
    [SerializeField] TextMeshProUGUI txt_fb;
    [SerializeField] Color[] color_fb;

    bool pausado = false;
    public static Script_GeralManager instance;

    private void Start()
    {
        instance = this;
        Application.targetFrameRate = 60;
        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) //redundancia de aviao para evitar bugs
        {
            if (pausado)
            {
                if (Time.timeScale == 0)
                {
                    pausado = false;
                    Time.timeScale = 1;

                    cg_pausa.alpha = 0;
                    cg_pausa.blocksRaycasts = false;
                    cg_pausa.interactable = false;
                }
            }
            else
            {
                if (Time.timeScale == 1)
                {
                    pausado = true;
                    Time.timeScale = 0;

                    cg_pausa.alpha = 1;
                    cg_pausa.blocksRaycasts = true;
                    cg_pausa.interactable = true;
                }
            }
        }
    }

    public void SceneMenuLoad()
    {
        SceneManager.LoadScene("Scene_menu");
    }

    public void SceneInicioLoad()
    {
        SceneManager.LoadScene("Scene_inicio");
    }

    public void FbSet(int _valor)
    {
        obj_fb.SetActive(true);
        obj_fb.GetComponent<Image>().color = color_fb[_valor];

        Time.timeScale = 0;

        txt_fb.text = _valor == 0 ? "VOCÊ PERDEU!" : "VOCÊ GANHOU!";
    }
}