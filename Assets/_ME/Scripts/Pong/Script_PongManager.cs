using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Script_PongManager : MonoBehaviour
{
    public static Script_PongManager instance;

    [SerializeField] GameObject obj_bola;
    int lado = 0;
    [SerializeField] float bolaMoveSpeed;
    [SerializeField] TextMeshPro txt_tempo;
    [SerializeField] TextMeshPro txt_placar;
    [SerializeField] TextMeshPro txt_aperte;
    [SerializeField] TrailRenderer tr_bola;
    [Space]

    [Header("FB")]
    [SerializeField] GameObject obj_fb;
    [SerializeField] int fbGanharTempo;
    [SerializeField] TextMeshProUGUI txt_fb;
    [SerializeField] Color[] color_fb;

    readonly int[] pontos = { 0, 0 };

    Rigidbody rb_bola;
    bool bolaMovendo = false;
    int tempoAtual = 0;

    void Start()
    {
        instance = this;

        Application.targetFrameRate = 60;

        Time.timeScale = 1;

        rb_bola = obj_bola.GetComponent<Rigidbody>();

        txt_tempo.text = fbGanharTempo.ToString() + "s";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BolaMover(lado);
        }
    }

    void BolaMover(int _lado)
    {
        if (bolaMovendo) return;

        txt_aperte.gameObject.SetActive(false);

        Invoke(nameof(BolaMoveSpeedUp), 10f);
        InvokeRepeating(nameof(TempoSet), 1f, 1f);
        tempoAtual = 0;

        rb_bola.isKinematic = false;
        tr_bola.emitting = true;
        float _bolaMoveSpeedX = Random.Range(5, 8);
        float _bolaMoveSpeedY = 5 + (5 - _bolaMoveSpeedX);

        bolaMovendo = true;
        _lado = _lado == 0 ? Random.Range(1, 3) : _lado;

        if (_lado == 1) rb_bola.velocity = Vector3.left * _bolaMoveSpeedX + ((Random.Range(0, 2) == 0 ? -1 : 1) * _bolaMoveSpeedY) * Vector3.forward;
        else rb_bola.velocity = Vector3.right * _bolaMoveSpeedX + ((Random.Range(0, 2) == 0 ? -1 : 1) * _bolaMoveSpeedY) * Vector3.forward;
    }

    public void BolaReset(int _lado) //recebe 1 ou 2
    {
        CancelInvoke();
        txt_aperte.gameObject.SetActive(true);
        TextTempoSet(fbGanharTempo);

        //bola
        bolaMovendo = false;
        lado = _lado;
        obj_bola.transform.position = Vector3.zero;
        rb_bola.velocity = Vector3.zero;
        rb_bola.isKinematic = true;
        tr_bola.emitting = false;

        //
        if (_lado == 1) pontos[1]++;
        else pontos[0]++;

        if (pontos[1] == 3) FbSet(0);
        else if (pontos[0] == 3) FbSet(1);

        TextPlacarSet();
    }

    void TextPlacarSet()
    {
        txt_placar.text = pontos[0].ToString() + " x " + pontos[1].ToString();
    }

    void BolaMoveSpeedUp()
    {
        if (Random.Range(0, 2) == 0)
        {
            if (rb_bola.velocity.x > 0) rb_bola.velocity += Vector3.right;
            else rb_bola.velocity += Vector3.left;
        }
        else
        {
            if (rb_bola.velocity.z > 0) rb_bola.velocity += Vector3.forward;
            else rb_bola.velocity += Vector3.back;
        }

        Invoke(nameof(BolaMoveSpeedUp), 10f);
    }

    void TempoSet()
    {
        tempoAtual++;
        TextTempoSet();

        if (fbGanharTempo - tempoAtual == 0)
        {
            obj_bola.GetComponent<AudioSource>().PlayOneShot(obj_bola.GetComponent<Script_bolaPongTudo>().audioClip[1]);
            BolaReset(0);
        }
    }

    void TextTempoSet(int _valor = -1)
    {
        if(_valor == -1) txt_tempo.text = (fbGanharTempo - tempoAtual).ToString() + "s";
        else
        {
            txt_tempo.text = _valor.ToString() + "s";
        }
    }

    public void FbSet(int _valor)
    {
        obj_fb.SetActive(true);
        obj_fb.GetComponent<Image>().color = color_fb[_valor];

        Time.timeScale = 0;

        txt_fb.text = _valor == 0 ? "PERDEU!" : "GANHOU!";
    }

    public void SceneMenu()
    {
        SceneManager.LoadScene("Scene_menu");
    }
}