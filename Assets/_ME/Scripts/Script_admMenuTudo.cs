using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Script_admMenuTudo : MonoBehaviour
{
    [SerializeField] string[] cenaNome;
    [SerializeField] TextMeshProUGUI txt_flapPontos;
    int pontos = 0;

    private void Start()
    {
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
}