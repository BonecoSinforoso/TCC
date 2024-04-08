using UnityEngine;

public class Script_iaInfinityTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float puloForca;
    [SerializeField] float puloDist;
    [SerializeField] float t;

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

    [SerializeField] LineRenderer[] lr;

    bool sentidoMudarPode = true;

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

        lr[0].startColor = bloq_esq ? Color.red : Color.green;
        lr[0].endColor = bloq_esq ? Color.red : Color.green;
        lr[1].startColor = bloq_esq ? Color.red : Color.green;
        lr[1].endColor = bloq_esq ? Color.red : Color.green;

        lr[2].startColor = bloq_cen ? Color.red : Color.green;
        lr[2].endColor = bloq_cen ? Color.red : Color.green;
        lr[3].startColor = bloq_cen ? Color.red : Color.green;
        lr[3].endColor = bloq_cen ? Color.red : Color.green;

        lr[4].startColor = bloq_dir ? Color.red : Color.green;
        lr[4].endColor = bloq_dir ? Color.red : Color.green;
        lr[5].startColor = bloq_dir ? Color.red : Color.green;
        lr[5].endColor = bloq_dir ? Color.red : Color.green;

        lr[0].SetPosition(0, new Vector3(17.5f, 0.5f, transform.position.z));
        lr[0].SetPosition(1, new Vector3(17.5f, 0.5f, transform.position.z + 6f));

        lr[1].SetPosition(0, new Vector3(17.5f, 2f, transform.position.z));
        lr[1].SetPosition(1, new Vector3(17.5f, 0f, transform.position.z));

        lr[2].SetPosition(0, new Vector3(20f, 0.5f, transform.position.z));
        lr[2].SetPosition(1, new Vector3(20f, 0.5f, transform.position.z + 6f));

        lr[3].SetPosition(0, new Vector3(20f, 2f, transform.position.z));
        lr[3].SetPosition(1, new Vector3(20f, 0f, transform.position.z));

        lr[4].SetPosition(0, new Vector3(22.5f, 0.5f, transform.position.z));
        lr[4].SetPosition(1, new Vector3(22.5f, 0.5f, transform.position.z + 6f));

        lr[5].SetPosition(0, new Vector3(22.5f, 2f, transform.position.z));
        lr[5].SetPosition(1, new Vector3(22.5f, 0f, transform.position.z));

        bool _carro = false;

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

            _carro = true;
        }

        lr[6].startColor = _carro ? Color.red : Color.green;
        lr[6].endColor = _carro ? Color.red : Color.green;

        lr[6].SetPosition(0, transform.position + Vector3.forward);
        lr[6].SetPosition(1, transform.position + Vector3.forward * 5);

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
        Script_InfinityManager.instance.PerdeuSet(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
            Script_InfinityManager.instance.TextoPontosChange(1, pontos);
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