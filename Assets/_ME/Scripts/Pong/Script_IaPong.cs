using UnityEngine;

public class Script_IaPong : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform t_bola;
    [SerializeField] private Rigidbody rb_bola;

    [SerializeField] private float energiaMax;
    [SerializeField] private float energiaAtual;
    [SerializeField] private float energiaConsumo;
    [SerializeField] private float energiaRecarga;
    [SerializeField] private Transform t_energiaBarra;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bool _andou = false;

        if (energiaAtual > 5) //verifica a energia
        {
            if (Script_BolaPong.perigo) //verifica se a bola oferece perigo (no caso se est� indo em direcao ao gol)
            {
                if (transform.position.z + 1 < Script_BolaPong.pos) //verifica se a bola est� acima da raquete
                {
                    if (transform.position.z < 3.5f)
                    {
                        rb.linearVelocity = Vector3.forward * moveSpeed;
                        _andou = true;
                    }
                }

                if (transform.position.z - 1 > Script_BolaPong.pos) //verifica se a bola est� abaixo da raquete
                {
                    if (transform.position.z > -3.5f)
                    {
                        rb.linearVelocity = Vector3.back * moveSpeed;
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

            rb.linearVelocity = Vector3.zero;
        }

        EnergiaBarraTamanho_Set();
    }

    private void EnergiaBarraTamanho_Set()
    {
        t_energiaBarra.localScale = new Vector3(1, 1, energiaAtual / energiaMax);
    }
}
