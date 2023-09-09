using UnityEngine;

public class Script_mlTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    GameObject obj_adm;
    Rigidbody rb;
    int pontos = 0;
    bool perdeu = false;

    void Start()
    {
        obj_adm = GameObject.FindGameObjectWithTag("ADM");
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (perdeu) return;
        rb.velocity = Vector3.forward * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
            obj_adm.GetComponent<Script_admGameTudo>().TextoPontosChange(2, pontos);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Carro"))
        {
            perdeu = true;
            rb.velocity = Vector3.zero;
            Destroy(collision.gameObject);
        }
    }
}