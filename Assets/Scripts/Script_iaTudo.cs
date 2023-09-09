using UnityEngine;

public class Script_iaTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float puloForca;
    [SerializeField] float puloDist;
    [SerializeField] float t;

    GameObject obj_adm;
    Rigidbody rb;
    int pontos = 0;
    bool perdeu = false;
    bool puloPode = false;

    bool esq = false;
    bool cen = false;
    bool dir = false;

    bool bloq_esq;
    bool bloq_cen;
    bool bloq_dir;

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
            if (!esq && !cen && !dir)
            {
                if (_hit1.collider.CompareTag("Onibus"))
                {
                    cen = true;
                }
                else if (_hit1.collider.CompareTag("Carro"))
                {
                    if (Mathf.Abs(transform.position.z - _hit1.point.z) <= puloDist)
                    {
                        Pulo();
                    }
                }
            }
        }
        if (Physics.Raycast(new Vector3(20f, 0.5f, transform.position.z) + Vector3.forward, Vector3.forward, out RaycastHit _hit2, 10f))
        {
            if (!esq && !cen && !dir)
            {
                if (_hit2.collider.CompareTag("Onibus"))
                {
                    if (transform.position.x == 20)
                    {
                        esq = Random.Range(0, 2) == 0;
                        dir = !esq;
                    }
                    else if (transform.position.x < 20)
                    {
                        esq = true;
                    }
                    else
                    {
                        dir = true;
                    }
                }
                else if (_hit2.collider.CompareTag("Carro"))
                {
                    if (Mathf.Abs(transform.position.z - _hit2.point.z) <= puloDist)
                    {
                        Pulo();
                    }
                }
            }
        }
        if (Physics.Raycast(new Vector3(22.5f, 0.5f, transform.position.z) + Vector3.forward, Vector3.forward, out RaycastHit _hit3, 10f))
        {
            if (!esq && !cen && !dir)
            {
                if (_hit3.collider.CompareTag("Onibus"))
                {
                    cen = true;
                }
                else if (_hit3.collider.CompareTag("Carro"))
                {
                    if (Mathf.Abs(transform.position.z - _hit3.point.z) <= puloDist)
                    {
                        Pulo();
                    }
                }
            }
        }

        //
        if (esq)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(17.5f, transform.position.y, transform.position.z), t);
            if (Mathf.Abs(transform.position.x - 17.5f) < 0.05f) esq = false;
        }
        if (cen)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(20f, transform.position.y, transform.position.z), t);
            if (Mathf.Abs(transform.position.x - 20f) < 0.05f) cen = false;
        }
        if (dir)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(22.5f, transform.position.y, transform.position.z), t);
            if (Mathf.Abs(transform.position.x - 22.5f) < 0.05f) dir = false;
        }
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
        obj_adm.GetComponent<Script_admGameTudo>().PerdeuSet(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
            obj_adm.GetComponent<Script_admGameTudo>().TextoPontosChange(1, pontos);
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