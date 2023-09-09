using UnityEngine;

public class Script_iaTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float t;

    GameObject obj_adm;
    Rigidbody rb;
    int pontos = 0;
    bool perdeu = false;

    bool esq = false;
    bool cen = false;
    bool dir = false;

    void Start()
    {
        obj_adm = GameObject.FindGameObjectWithTag("ADM");
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (perdeu) return;
        rb.velocity = Vector3.forward * moveSpeed + new Vector3(0, rb.velocity.y, 0);

        if (Physics.Raycast(new Vector3(17.5f, 0.5f, transform.position.z) + Vector3.forward, Vector3.forward, out RaycastHit _hit1, 10f))
        {
            if (_hit1.collider.CompareTag("Carro"))
            {
                cen = true;
            }
        }
        if (Physics.Raycast(new Vector3(20f, 0.5f, transform.position.z) + Vector3.forward, Vector3.forward, out RaycastHit _hit2, 10f))
        {
            if (_hit2.collider.CompareTag("Carro"))
            {
                if(transform.position.x == 20)
                {
                    esq = Random.Range(0, 2) == 0;
                    dir = !esq;
                }
            }
        }
        if (Physics.Raycast(new Vector3(22.5f, 0.5f, transform.position.z) + Vector3.forward, Vector3.forward, out RaycastHit _hit3, 10f))
        {
            if (_hit3.collider.CompareTag("Carro"))
            {
                cen = true;
            }
        }

        //
        if (esq)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(17.5f, transform.position.y, transform.position.z), t);
        }
        if (cen)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(20f, transform.position.y, transform.position.z), t);
        }
        if (dir)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(22.5f, transform.position.y, transform.position.z), t);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
            obj_adm.GetComponent<Script_admGameTudo>().TextoPontosChange(1, pontos);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Carro"))
        {
            perdeu = true;
            rb.velocity = Vector3.zero;
        }
    }
}