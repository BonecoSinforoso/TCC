using UnityEngine;

public class Script_playerTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float moveSpeedX;
    [SerializeField] float puloForca;
    [SerializeField] float posLimiteX;

    GameObject obj_adm;
    Rigidbody rb;
    Animator animator;

    int pontos = 0;
    bool perdeu = false;
    bool puloPode = false;

    void Start()
    {
        obj_adm = GameObject.FindGameObjectWithTag("ADM");
        rb = GetComponent<Rigidbody>();

        animator = transform.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (perdeu) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Pulo();
        }

        animator.SetBool("_jump", !puloPode);

        rb.velocity = Vector3.forward * moveSpeed + new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeedX, rb.velocity.y, 0);

        if (transform.position.x < -posLimiteX) transform.position = new Vector3(-posLimiteX, transform.position.y, transform.position.z);
        else if (transform.position.x > posLimiteX) transform.position = new Vector3(posLimiteX, transform.position.y, transform.position.z);
    }

    void Pulo()
    {
        if (!puloPode) return;
        puloPode = false;
        rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);
    }

    void Perdeu()
    {
        perdeu = true;
        rb.velocity = Vector3.zero;
        obj_adm.GetComponent<Script_admGameTudo>().PerdeuSet(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
            obj_adm.GetComponent<Script_admGameTudo>().TextoPontosChange(0, pontos);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Carro"))
        {
            Perdeu();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Onibus"))
        {
            Perdeu();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Chao"))
        {
            puloPode = true;
        }
    }
}