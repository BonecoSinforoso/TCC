using UnityEngine;

public class Script_iaInfinityTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float puloForca;
    [SerializeField] float puloDist;
    [SerializeField] float t;

    GameObject obj_adm;
    Rigidbody rb;
    Animator animator;

    int pontos = 0;
    bool perdeu = false;
    bool puloPode = false;

    [SerializeField] bool esq = false;
    [SerializeField] bool cen = false;
    [SerializeField] bool dir = false;

    [SerializeField] bool bloq_esq;
    [SerializeField] bool bloq_cen;
    [SerializeField] bool bloq_dir;

    bool sentidoMudarPode = true;

    void Start()
    {
        obj_adm = GameObject.FindGameObjectWithTag("ADM");
        rb = GetComponent<Rigidbody>();
        animator = transform.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (perdeu) return;

        animator.SetBool("_jump", !puloPode);

        rb.velocity = Vector3.forward * moveSpeed + new Vector3(0, rb.velocity.y, 0);

        Debug.DrawRay(new Vector3(17.5f, 0.5f, transform.position.z), Vector3.forward * 5f, Color.red);
        Debug.DrawRay(new Vector3(17.5f, 2f, transform.position.z), Vector3.down * 2f, Color.red);

        Debug.DrawRay(new Vector3(20f, 0.5f, transform.position.z), Vector3.forward * 5f, Color.red);
        Debug.DrawRay(new Vector3(20f, 2f, transform.position.z), Vector3.down * 2f, Color.red);

        Debug.DrawRay(new Vector3(22.5f, 0.5f, transform.position.z), Vector3.forward * 5f, Color.red);
        Debug.DrawRay(new Vector3(22.5f, 2f, transform.position.z), Vector3.down * 2f, Color.red);

        bloq_esq = false;
        bloq_cen = false;
        bloq_dir = false;

        if (Physics.Raycast(new Vector3(17.5f, 0.5f, transform.position.z), Vector3.forward, 6f, 1 << 3)) bloq_esq = true;
        if (Physics.Raycast(new Vector3(17.5f, 2f, transform.position.z), Vector3.down, 2f, 1 << 3)) bloq_esq = true;

        if (Physics.Raycast(new Vector3(20f, 0.5f, transform.position.z), Vector3.forward, 6f, 1 << 3)) bloq_cen = true;
        if (Physics.Raycast(new Vector3(20f, 2f, transform.position.z), Vector3.down, 2f, 1 << 3)) bloq_cen = true;

        if (Physics.Raycast(new Vector3(22.5f, 0.5f, transform.position.z), Vector3.forward, 6f, 1 << 3)) bloq_dir = true;
        if (Physics.Raycast(new Vector3(22.5f, 2f, transform.position.z), Vector3.down, 2f, 1 << 3)) bloq_dir = true;

        //proprio
        if (Physics.Raycast(transform.position + Vector3.forward, Vector3.forward, out RaycastHit _hit, 5f))
        {
            if (_hit.collider.CompareTag("Carro"))
            {
                if (Mathf.Abs(transform.position.z - _hit.point.z) < puloDist)
                {
                    Pulo();
                }
            }
        }

        if (bloq_esq)
        {
            if (transform.position.x < 19)
            {
                if (bloq_cen)
                {
                    dir = true;
                }
                else
                {
                    cen = true;
                }
            }
        }
        if (bloq_cen)
        {
            if (transform.position.x >= 19.2f && transform.position.x <= 20.8f)
            {
                if (!bloq_esq && !bloq_dir)
                {
                    if (sentidoMudarPode)
                    {
                        sentidoMudarPode = false;
                        esq = Random.Range(0, 2) == 0;
                        dir = !esq;
                    }
                }
                else //nn eh else if pq tenho q tratar quando tudo tiver block
                {
                    if (!bloq_esq) esq = true;
                    if (!bloq_dir) dir = true;
                }
            }
        }
        if (bloq_dir)
        {
            if (transform.position.x > 21)
            {
                if (bloq_cen)
                {
                    esq = true;
                }
                else
                {
                    cen = true;
                }
            }
        }

        //
        if (esq)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(17.5f, transform.position.y, transform.position.z), t);
            if (Mathf.Abs(transform.position.x - 17.5f) < 0.05f)
            {
                esq = false;
                sentidoMudarPode = true;
            }
        }
        else if (cen)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(20f, transform.position.y, transform.position.z), t);
            if (Mathf.Abs(transform.position.x - 20f) < 0.05f) cen = false;
        }
        else if (dir)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(22.5f, transform.position.y, transform.position.z), t);
            if (Mathf.Abs(transform.position.x - 22.5f) < 0.05f)
            {
                dir = false;
                sentidoMudarPode = true;
            }
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
        obj_adm.GetComponent<Script_admInfinityGameTudo>().PerdeuSet(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
            obj_adm.GetComponent<Script_admInfinityGameTudo>().TextoPontosChange(1, pontos);
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