using UnityEngine;

public class Script_bolaPongTudo : MonoBehaviour
{
    [SerializeField] Transform t_move;
    public AudioClip[] audioClip;
    [SerializeField] string cu;
    [SerializeField] string ue;

    Rigidbody rb;
    AudioSource audioSource;
    [SerializeField] LayerMask layerMask;
    public static bool perigo;
    public static float pos;

    [SerializeField] LineRenderer lr;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        perigo = false;

        if (rb.velocity != Vector3.zero) t_move.forward = transform.position + (new Vector3(rb.velocity.x, 0, rb.velocity.z) * 10);

        Vector3 _direcao = t_move.forward;

        Debug.DrawRay(transform.position, _direcao);

        //Physics.Raycast(transform.position, t_move.forward, out RaycastHit _hit, layerMask);
        Physics.Raycast(transform.position, t_move.forward, out RaycastHit _hit, 50f);

        if (_hit.collider)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, _hit.point);

            cu = "so colidiu";

            if (_hit.collider.CompareTag("Parede"))
            {
                _direcao = Vector3.Reflect(_direcao, _hit.normal);
                Debug.DrawRay(_hit.point, _direcao);

                //Physics.Raycast(_hit.point, _direcao, out RaycastHit _hit2, layerMask);
                Physics.Raycast(_hit.point, _direcao, out RaycastHit _hit2, 50f);

                cu = "so parede";

                if (_hit2.collider)
                {
                    lr.SetPosition(2, _hit2.point);

                    cu = "parede 2";

                    if (_hit2.collider.CompareTag("Perigo"))
                    {
                        perigo = true;
                        pos = _hit.point.z;

                        cu = "perigo";
                    }
                }
                else
                {
                    lr.SetPosition(2, _hit.point);
                }
            }
            else if (_hit.collider.CompareTag("Perigo"))
            {
                cu = "so perigo";

                lr.SetPosition(2, _hit.point);

                perigo = true;
                pos = _hit.point.z;
            }
        }
        else
        {
            cu = "poha ninhua";

            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, transform.position);
            lr.SetPosition(2, transform.position);
        }

        if (transform.position.x <= -11f || transform.position.x >= 11f)
        {
            Script_PongManager.instance.BolaReset(transform.position.x < 0 ? 1 : 2);

            audioSource.PlayOneShot(audioClip[1]);
        }

        if (cu == "so colidiu")
        {
            ue = _hit.collider.name;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MoveSpeedHorizontalToggle();
            if (transform.position.x < -8.7f) MoveSpeedVerticalToggle();
            if (transform.position.z + 0.5f < other.gameObject.transform.position.z && rb.velocity.z > 0) MoveSpeedVerticalToggle();
            if (transform.position.z - 0.5f > other.gameObject.transform.position.z && rb.velocity.z < 0) MoveSpeedVerticalToggle();

            audioSource.Play();
        }

        if (other.gameObject.CompareTag("IA"))
        {
            MoveSpeedHorizontalToggle();
            if (transform.position.x > 8.7f) MoveSpeedVerticalToggle();
            if (transform.position.z + 0.5f < other.gameObject.transform.position.z && rb.velocity.z > 0) MoveSpeedVerticalToggle();
            if (transform.position.z - 0.5f > other.gameObject.transform.position.z && rb.velocity.z < 0) MoveSpeedVerticalToggle();

            audioSource.Play();
        }

        if (other.gameObject.CompareTag("Parede"))
        {
            MoveSpeedVerticalToggle();

            audioSource.Play();
        }
    }

    void MoveSpeedHorizontalToggle()
    {
        rb.velocity = new Vector3(-rb.velocity.x, 0, rb.velocity.z);
    }

    void MoveSpeedVerticalToggle()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, -rb.velocity.z);
    }
}