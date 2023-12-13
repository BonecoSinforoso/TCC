using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Script_admPongGameTudo : MonoBehaviour
{
    [SerializeField] GameObject obj_bola;
    int lado = 0;
    [SerializeField] float bolaMoveSpeed;
    [SerializeField] TextMeshPro txt_placar;
    [SerializeField] TrailRenderer tr_bola;
    [Space]

    [Header("FB")]
    [SerializeField] GameObject obj_fb;
    [SerializeField] float fbGanharTempo;
    [SerializeField] TextMeshProUGUI txt_fb;
    [SerializeField] Color[] color_fb;

    readonly int[] pontos = { 0, 0 };

    Rigidbody rb_bola;
    bool bolaMovendo = false;

    void Start()
    {
        Application.targetFrameRate = 60;

        Time.timeScale = 1;

        rb_bola = obj_bola.GetComponent<Rigidbody>();

        Invoke(nameof(BolaMoveSpeedUp), 10f);
        StartCoroutine(Call_FbSet());
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

        rb_bola.isKinematic = false;
        tr_bola.emitting = true;
        float _bolaMoveSpeedX = Random.Range(5, 8);
        float _bolaMoveSpeedY = 5 + (5 - _bolaMoveSpeedX);

        bolaMovendo = true;
        _lado = _lado == 0 ? Random.Range(1, 3) : _lado;

        if (_lado == 1) rb_bola.velocity = Vector3.left * _bolaMoveSpeedX + ((Random.Range(0, 2) == 0 ? -1 : 1) * _bolaMoveSpeedY) * Vector3.forward;
        else rb_bola.velocity = Vector3.right * _bolaMoveSpeedX + ((Random.Range(0, 2) == 0 ? -1 : 1) * _bolaMoveSpeedY) * Vector3.forward;
    }

    public void BolaReset(int _lado)
    {
        FbSet(obj_bola.transform.position.x < 0 ? 0 : 1);

        bolaMovendo = false;
        lado = _lado;
        obj_bola.transform.position = Vector3.zero;
        rb_bola.velocity = Vector3.zero;
        rb_bola.isKinematic = true;
        tr_bola.emitting = false;

        if (_lado == 1) pontos[1]++;
        else pontos[0]++;

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

    IEnumerator Call_FbSet()
    {
        yield return new WaitForSeconds(fbGanharTempo);

        FbSet(1);
    }

    public void FbSet(int _valor)
    {
        obj_fb.SetActive(true);
        obj_fb.GetComponent<Image>().color = color_fb[_valor];

        Time.timeScale = 0;

        txt_fb.text = _valor == 0 ? "PERDEU!" : "GANHOU!";
    }
}