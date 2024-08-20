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

        if (energiaAtual > 5)
        {
            if (Script_BolaPong.perigo)
            {
                if (transform.position.z + 1 < Script_BolaPong.pos)
                {
                    if (transform.position.z < 3.5f)
                    {
                        rb.velocity = Vector3.forward * moveSpeed;
                        _andou = true;
                    }
                }

                if (transform.position.z - 1 > Script_BolaPong.pos)
                {
                    if (transform.position.z > -3.5f)
                    {
                        rb.velocity = Vector3.back * moveSpeed;
                        _andou = true;
                    }
                }
            }
        }

        if (_andou)
        {
            energiaAtual -= energiaConsumo * Time.deltaTime;
            if (energiaAtual < 0) energiaAtual = 0;
        }
        else
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
