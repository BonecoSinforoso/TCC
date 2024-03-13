using TMPro;
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

    [SerializeField] TextMeshProUGUI txt_pontos;
    int pontos = 0;

    bool puloPode = true;
    bool movel;

    Rigidbody rb;

    void Start()
    {
        movel = Script_FlapManager.instance.movel;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (movel)
        {
            //pra vc icaro do futuro
        }
        else
        {
            for (int i = 0; i < obj_cano.Length; i++)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Chao"))
        {
            Time.timeScale = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cano"))
        {
            Time.timeScale = 0;
        }

        if (other.CompareTag("Ponto"))
        {
            pontos++;
            TextPontosSet();
        }
    }

    void TextPontosSet()
    {
        txt_pontos.text = pontos.ToString();
    }
}