using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_GeralManager : MonoBehaviour
{
    [SerializeField] CanvasGroup cg_pausa;

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
}