using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_GeralManager : MonoBehaviour
{
    [SerializeField] CanvasGroup cg_pausa;

    bool pausado = false;

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
                }
            }
            else
            {
                if (Time.timeScale == 1)
                {
                    pausado = true;
                    Time.timeScale = 0;

                    cg_pausa.alpha = 1;
                }
            }
        }
    }

    public void SceneMenuLoad()
    {
        SceneManager.LoadScene("Scene_menu");
    }
}