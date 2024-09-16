using TMPro;
using UnityEngine;

public class Script_IaFlap : MonoBehaviour
{
    [SerializeField] float puloForca;
    [SerializeField] float puloCd;

    [SerializeField] float alturaMin;
    [SerializeField] float coefY;

    [SerializeField] Transform[] t_cano;
    [SerializeField] float distanciaCanoFrente;

    [SerializeField] TextMeshProUGUI txt_pontos;

    [Header("Visualizacao")]
    [SerializeField] LineRenderer lr_canoAlturaDif;
    [SerializeField] LineRenderer lr_canoDistHor;
    [SerializeField] LineRenderer lr_chao;

    int pontos = 0;
    bool puloPode = true;
    bool movel; //se canos sao moveis ou nn

    Rigidbody rb;

    void Start()
    {
        movel = Script_FlapManager.instance.movel;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        int _canoMaisProximoIndex = -1;

        //codigo para definir se o pulo eh necessario ou nn
        for (int i = 0; i < t_cano.Length; i++) //loop q verifica todos os canos
        {
            if (transform.position.x < t_cano[i].position.x) //condicao caso o passaro esteja a esquerda do cano
            {
                if (t_cano[i].transform.position.x - transform.position.x <= distanciaCanoFrente)
                {
                    if (transform.position.y + coefY < t_cano[i].position.y)
                    {
                        Pulo();
                    }
                }
            }
            else ////condicao caso o passaro esteja a direita do cano
            {
                if (Mathf.Abs(transform.position.x - t_cano[i].position.x) < 1) //condicao para evitar a colisao com o cano quando estiverem muito proximos (logo quando ele passa pela estrela)
                {
                    if (transform.position.y + coefY < t_cano[i].position.y) //verifica se a altura do passaro esta num intervalo perigoso
                    {
                        Pulo();
                    }
                }
            }

            //logica para definir qual cano estah mais proximo
            if (i != 0)
            {
                if (Vector2.Distance(transform.position, t_cano[i].position) < Vector2.Distance(transform.position, t_cano[_canoMaisProximoIndex].position))
                    _canoMaisProximoIndex = i;
            }
            else _canoMaisProximoIndex = 0;
        }

        if (transform.position.y < alturaMin) //verifica se o passaro esta numa posicao muito baixa
        {
            Pulo();
        }

        //QUANDO O JOGADOR APERTA A TECLA Q
        //exibicao grafica dos parametros utilizados pelo agente reativo para realizar as acoes
        lr_canoAlturaDif.SetPosition(0, new Vector3(transform.position.x - 0.5f, transform.position.y, 0));
        lr_canoAlturaDif.SetPosition(1, new Vector3(transform.position.x - 0.5f, t_cano[_canoMaisProximoIndex].position.y, 0));

        lr_canoDistHor.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 0.5f, 0));
        lr_canoDistHor.SetPosition(1, new Vector3(t_cano[_canoMaisProximoIndex].position.x, transform.position.y + 0.5f, 0));

        lr_chao.SetPosition(0, transform.position);
        lr_chao.SetPosition(1, new Vector3(transform.position.x, -5, 0));
    }

    void Pulo()
    {
        if (!puloPode) return;
        puloPode = false;
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);

        Invoke(nameof(PuloPode_Reset), puloCd);
    }

    void PuloPode_Reset()
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
            Txt_Pontos_Set();
        }
    }

    void Txt_Pontos_Set()
    {
        txt_pontos.text = pontos.ToString();
    }
}