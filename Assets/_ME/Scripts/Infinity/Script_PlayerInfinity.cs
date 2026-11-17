using TMPro;
using UnityEngine;

public class Script_PlayerInfinity : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedX;
    [SerializeField] private float puloForca;
    [SerializeField] private float posLimiteX;
    [SerializeField] private TextMeshProUGUI txt_moveSpeed;

    private Rigidbody rb;
    private Animator animator;

    private int pontos = 0;
    private bool perdeu = false;
    private bool puloPode = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        animator = transform.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (perdeu) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Pulo();
        }

        animator.SetBool("_jump", !puloPode);

        moveSpeed = 5 + ((int)transform.position.z / 100);
        moveSpeed = moveSpeed > 8 ? 8 : moveSpeed;
        txt_moveSpeed.text = moveSpeed + "m/s";

        //rb.velocity = Vector3.forward * moveSpeed + new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeedX, rb.velocity.y, 0);
        rb.linearVelocity = Vector3.forward * moveSpeed + Vector3.up * rb.linearVelocity.y;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left * 2.5f;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += Vector3.right * 2.5f;
        }

        if (transform.position.x < -posLimiteX) transform.position = new Vector3(-posLimiteX, transform.position.y, transform.position.z);
        else if (transform.position.x > posLimiteX) transform.position = new Vector3(posLimiteX, transform.position.y, transform.position.z);
    }

    private void Pulo()
    {
        if (!puloPode) return;
        puloPode = false;
        rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);
    }

    private void Perdeu()
    {
        perdeu = true;
        rb.linearVelocity = Vector3.zero;
        Script_InfinityManager.instance.Perdeu_Set(0);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Oculos"))
        {
            Script_InfinityManager.instance.IaCegar();
        }
    }
}