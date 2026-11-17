using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_MenuManager : MonoBehaviour
{
    [SerializeField] private string[] cenaNome;
    [SerializeField] private TextMeshProUGUI txt_flapPontos;
    private int pontos = 0;

    private void Start()
    {
        Invoke(nameof(PontosSet), 0.1f);
    }

    public void SceneChange(int _valor)
    {
        SceneManager.LoadScene(cenaNome[_valor]);
    }

    private void PontosSet()
    {
        pontos++;

        TextFlapPontosSet();
        Invoke(nameof(PontosSet), 0.1f);
    }

    private void TextFlapPontosSet()
    {
        txt_flapPontos.text = pontos.ToString();
    }

    public void SceneInicioLoad()
    {
        SceneManager.LoadScene("Scene_inicio");
    }
}