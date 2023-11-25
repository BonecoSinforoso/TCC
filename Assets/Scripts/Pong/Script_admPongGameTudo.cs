using UnityEngine;
using TMPro;

public class Script_admPongGameTudo : MonoBehaviour
{
    [SerializeField] GameObject obj_bola;
    int lado = 0;
    [SerializeField] float bolaMoveSpeed;
    [SerializeField] TextMeshPro txt_placar;
    readonly int[] pontos = { 0, 0 };

    Rigidbody rb_bola;
    bool bolaMovendo = false;

    void Start()
    {
        rb_bola = obj_bola.GetComponent<Rigidbody>();
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

        float _bolaMoveSpeedX = Random.Range(5, 8);
        float _bolaMoveSpeedY = 5 + (5 - _bolaMoveSpeedX);

        bolaMovendo = true;
        _lado = _lado == 0 ? Random.Range(1, 3) : _lado;

        if (_lado == 1) rb_bola.velocity = Vector3.left * _bolaMoveSpeedX + ((Random.Range(0, 2) == 0 ? -1 : 1) * _bolaMoveSpeedY) * Vector3.forward;
        else rb_bola.velocity = Vector3.right * _bolaMoveSpeedX + ((Random.Range(0, 2) == 0 ? -1 : 1) * _bolaMoveSpeedY) * Vector3.forward;
    }

    public void BolaReset(int _lado)
    {
        bolaMovendo = false;
        lado = _lado;
        obj_bola.transform.position = Vector3.zero;
        rb_bola.velocity = Vector3.zero;

        if (_lado == 1) pontos[1]++;
        else pontos[0]++;

        TextPlacarSet();
    }

    void TextPlacarSet()
    {
        txt_placar.text = pontos[0].ToString() + " x " + pontos[1].ToString();
    }
}