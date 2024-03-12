using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_MenuManager : MonoBehaviour
{
    [SerializeField] string[] cenaNome;
    [SerializeField] TextMeshProUGUI txt_flapPontos;
    int pontos = 0;

    private void Start()
    {
        Application.targetFrameRate = 60;

        Time.timeScale = 1;

        Invoke(nameof(PontosSet), 0.1f);
    }

    public void SceneChange(int _valor)
    {
        SceneManager.LoadScene(cenaNome[_valor]);
    }

    void PontosSet()
    {
        pontos++;

        TextFlapPontosSet();
        Invoke(nameof(PontosSet), 0.1f);
    }

    void TextFlapPontosSet()
    {
        txt_flapPontos.text = pontos.ToString();
    }

    public void SceneInicioLoad()
    {
        SceneManager.LoadScene("Scene_inicio");
    }
}