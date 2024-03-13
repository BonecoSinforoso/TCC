using UnityEngine;

public class Script_mlInfinityTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float puloForca;

    Rigidbody rb;
    Animator animator;

    int pontos = 0;
    bool perdeu = false;
    bool puloPode = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = transform.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (perdeu) return;

        animator.SetBool("_jump", !puloPode);

        rb.velocity = Vector3.forward * moveSpeed + new Vector3(0, rb.velocity.y, 0);
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
        Script_InfinityManager.instance.PerdeuSet(2);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
            Script_InfinityManager.instance.TextoPontosChange(2, pontos);
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