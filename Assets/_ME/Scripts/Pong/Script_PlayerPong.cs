using UnityEngine;

public class Script_PlayerPong : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private float energiaMax;
    [SerializeField] private float energiaAtual;
    [SerializeField] private float energiaConsumo;
    [SerializeField] private float energiaRecarga;
    [SerializeField] private Transform t_energiaBarra;
    [SerializeField] private MeshRenderer mesh_energiaBarra;
    [SerializeField] private Color[] color;

    private bool travado = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
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
                rb.linearVelocity = _v * moveSpeed * Vector3.forward;
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
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

    private void EnergiaBarraTamanho_Set()
    {
        t_energiaBarra.localScale = new Vector3(1, 1, energiaAtual / energiaMax);
    }

    private void Travado_Set(bool _valor)
    {
        travado = _valor;

        EnergiaBarraCor_Set(_valor);
    }

    private void EnergiaBarraCor_Set(bool _valor)
    {
        Material material = mesh_energiaBarra.material;
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", _valor ? color[0] : color[1]);
    }
}