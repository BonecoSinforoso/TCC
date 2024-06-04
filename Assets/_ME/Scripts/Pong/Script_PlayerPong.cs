using UnityEngine;

public class Script_PlayerPong : MonoBehaviour
{
    [SerializeField] float moveSpeed;

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
        float _v = Input.GetAxisRaw("Vertical");
        bool _andou = false;

        if (energiaAtual > 5)
        {
            _andou = _v != 0;
            rb.velocity = _v * moveSpeed * Vector3.forward;
        }
        else rb.velocity = Vector3.zero;

        if (_andou)
        {
            energiaAtual -= energiaConsumo * Time.deltaTime;
            if (energiaAtual < 0) energiaAtual = 0;
        }
        else
        {
            energiaAtual += energiaRecarga * Time.deltaTime;
            if (energiaAtual > energiaMax) energiaAtual = energiaMax;
        }

        EnergiaBarraTamanhoSet();
    }

    void EnergiaBarraTamanhoSet()
    {
        t_energiaBarra.localScale = new Vector3(1, 1, energiaAtual / energiaMax);
    }
}