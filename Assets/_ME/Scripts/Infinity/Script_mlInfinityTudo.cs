using UnityEngine;

public class Script_mlInfinityTudo : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float puloForca;

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

        animator.SetBool("_jump", !puloPode);

        rb.linearVelocity = Vector3.forward * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
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
        Script_InfinityManager.instance.Perdeu_Set(2);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
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