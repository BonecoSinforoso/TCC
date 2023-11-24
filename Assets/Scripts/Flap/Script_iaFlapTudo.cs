using UnityEngine;

public class Script_iaFlapTudo : MonoBehaviour
{
    [SerializeField] float puloForca;
    [SerializeField] float puloCd;

    [SerializeField] float alturaMin;
    [SerializeField] float coefY;

    [SerializeField] GameObject[] obj_cano;
    [SerializeField] float distanciaCanoFrente;
    [SerializeField] float distanciaCanoAtras;

    bool puloPode = true;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        for (int i = 0; i <= 1; i++)
        {
            if (transform.position.x < obj_cano[i].transform.position.x)
            {
                if (obj_cano[i].transform.position.x - transform.position.x <= distanciaCanoFrente)
                {
                    if (transform.position.y + coefY < obj_cano[i].transform.position.y)
                    {
                        Pulo();
                    }
                }
            }
            else
            {
                if (Mathf.Abs(transform.position.x - obj_cano[i].transform.position.x) < 1)
                {
                    if (transform.position.y + coefY < obj_cano[i].transform.position.y)
                    {
                        Pulo();
                    }
                }
            }
        }

        if (transform.position.y < alturaMin)
        {
            Pulo();
        }
    }

    void Pulo()
    {
        if (!puloPode) return;
        puloPode = false;
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);

        Invoke(nameof(PuloPodeReset), puloCd);
    }

    void PuloPodeReset()
    {
        puloPode = true;
    }
}