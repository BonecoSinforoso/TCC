using UnityEngine;

public class Script_PlayerPong : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    [SerializeField] float energiaMax;
    [SerializeField] float energiaAtual;
    [SerializeField] float energiaConsumo;
    [SerializeField] float energiaRecarga;
    [SerializeField] Transform t_energiaBarra;
    [SerializeField] MeshRenderer mesh_energiaBarra;
    [SerializeField] Color[] color;

    bool travado = false;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float _v = Input.GetAxisRaw("Vertical");
        bool _andou = false;

        if (travado)
        {
            energiaAtual += energiaRecarga * Time.deltaTime;

            if (energiaAtual > energiaMax)
            {
                energiaAtual = energiaMax;
                Travado_Set(false);
            }
        }
        else
        {
            if (energiaAtual > energiaConsumo * Time.deltaTime)
            {
                _andou = _v != 0;
                rb.velocity = _v * moveSpeed * Vector3.forward;
            }
            else
            {
                rb.velocity = Vector3.zero;
                Travado_Set(true);
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
            }
        }

        EnergiaBarraTamanho_Set();
    }

    void EnergiaBarraTamanho_Set()
    {
        t_energiaBarra.localScale = new Vector3(1, 1, energiaAtual / energiaMax);
    }

    void Travado_Set(bool _valor)
    {
        travado = _valor;

        EnergiaBarraCor_Set(_valor);
    }

    void EnergiaBarraCor_Set(bool _valor)
    {
        Material material = mesh_energiaBarra.material;
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", _valor ? color[0] : color[1]);
    }
}