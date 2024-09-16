using UnityEngine;

public class Script_IaPong : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Transform t_bola;
    [SerializeField] Rigidbody rb_bola;

    [SerializeField] float energiaMax;
    [SerializeField] float energiaAtual;
    [SerializeField] float energiaConsumo;
    [SerializeField] float energiaRecarga;
    [SerializeField] Transform t_energiaBarra;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        bool _andou = false;

        if (energiaAtual > 5) //verifica a energia
        {
            if (Script_BolaPong.perigo) //verifica se a bola oferece perigo (no caso se está indo em direcao ao gol)
            {
                if (transform.position.z + 1 < Script_BolaPong.pos) //verifica se a bola está acima da raquete
                {
                    if (transform.position.z < 3.5f)
                    {
                        rb.velocity = Vector3.forward * moveSpeed;
                        _andou = true;
                    }
                }

                if (transform.position.z - 1 > Script_BolaPong.pos) //verifica se a bola está abaixo da raquete
                {
                    if (transform.position.z > -3.5f)
                    {
                        rb.velocity = Vector3.back * moveSpeed;
                        _andou = true;
                    }
                }
            }
        }

        if (_andou) //se andou diminui a energia
        {
            energiaAtual -= energiaConsumo * Time.deltaTime;
            if (energiaAtual < 0) energiaAtual = 0;
        }
        else //se nn andou, aumenta a energia e seta a velocidade da raquete como 0
        {
            energiaAtual += energiaRecarga * Time.deltaTime;
            if (energiaAtual > energiaMax) energiaAtual = energiaMax;

            rb.velocity = Vector3.zero;
        }

        EnergiaBarraTamanho_Set();
    }

    void EnergiaBarraTamanho_Set()
    {
        t_energiaBarra.localScale = new Vector3(1, 1, energiaAtual / energiaMax);
    }
}
