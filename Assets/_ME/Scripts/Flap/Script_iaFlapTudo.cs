using TMPro;
using UnityEngine;

public class Script_iaFlapTudo : MonoBehaviour
{
    [SerializeField] float puloForca;
    [SerializeField] float puloCd;

    [SerializeField] float alturaMin;
    [SerializeField] float coefY;

    [SerializeField] Transform[] t_cano;
    [SerializeField] float distanciaCanoFrente;
    [SerializeField] float distanciaCanoAtras;

    [SerializeField] TextMeshProUGUI txt_pontos;

    [Header("Visualizacao")]
    [SerializeField] LineRenderer lr_canoAlturaDif;
    [SerializeField] LineRenderer lr_canoDistHor;
    [SerializeField] LineRenderer lr_chao;

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
        int canoMaisProximoIndex = -1;

        if (movel)
        {
            //pra vc icaro do futuro
        }
        else
        {
            for (int i = 0; i < t_cano.Length; i++)
            {
                if (transform.position.x < t_cano[i].position.x) //atras do cano
                {
                    if (t_cano[i].transform.position.x - transform.position.x <= distanciaCanoFrente)
                    {
                        if (transform.position.y + coefY < t_cano[i].position.y)
                        {
                            Pulo();
                        }
                    }
                }
                else //na frente do cano
                {
                    if (Mathf.Abs(transform.position.x - t_cano[i].position.x) < 1)
                    {
                        if (transform.position.y + coefY < t_cano[i].position.y)
                        {
                            Pulo();
                        }
                    }
                }

                //qual cano mais proximo
                if (i != 0)
                {
                    if (Vector2.Distance(transform.position, t_cano[i].position) < Vector2.Distance(transform.position, t_cano[canoMaisProximoIndex].position))
                        canoMaisProximoIndex = i;
                }
                else canoMaisProximoIndex = 0;
            }
        }

        if (transform.position.y < alturaMin)
        {
            Pulo();
        }

        lr_canoAlturaDif.SetPosition(0, new Vector3(transform.position.x - 0.5f, transform.position.y, 0));
        lr_canoAlturaDif.SetPosition(1, new Vector3(transform.position.x - 0.5f, t_cano[canoMaisProximoIndex].position.y, 0));

        lr_canoDistHor.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 0.5f, 0));
        lr_canoDistHor.SetPosition(1, new Vector3(t_cano[canoMaisProximoIndex].position.x, transform.position.y + 0.5f, 0));

        lr_chao.SetPosition(0, transform.position);
        lr_chao.SetPosition(1, new Vector3(transform.position.x, -5, 0));
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