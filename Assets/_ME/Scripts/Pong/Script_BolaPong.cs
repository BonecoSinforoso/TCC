using UnityEngine;

public class Script_BolaPong : MonoBehaviour
{
    [SerializeField] Transform t_move;
    public AudioClip[] audioClip;
    [SerializeField] string debug_01;
    [SerializeField] string debug_02;

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

        Debug.DrawRay(transform.position, _direcao); //mostra o raio no editor da unity

        Physics.Raycast(transform.position, t_move.forward, out RaycastHit _hit, 50f);

        if (_hit.collider) //se o raycast colidiu com algo
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, _hit.point);

            debug_01 = "so colidiu";

            if (_hit.collider.CompareTag("Parede")) //se colidiu com a parede
            {
                _direcao = Vector3.Reflect(_direcao, _hit.normal);
                Debug.DrawRay(_hit.point, _direcao);

                Physics.Raycast(_hit.point, _direcao, out RaycastHit _hit2, 50f);

                debug_01 = "so parede";

                if (_hit2.collider) //se segundo raycast colidiu com algo
                {
                    lr.SetPosition(2, _hit2.point);

                    debug_01 = "parede 2";

                    if (_hit2.collider.CompareTag("Perigo")) //se houve colisao com a baliza do AR
                    {
                        perigo = true;
                        pos = _hit.point.z;

                        debug_01 = "perigo";
                    }
                }
                else
                {
                    lr.SetPosition(2, _hit.point);
                }
            }
            else if (_hit.collider.CompareTag("Perigo")) //se a bola colidiu diretamente com o perigo (difere do rebatimento)
            {
                debug_01 = "so perigo";

                lr.SetPosition(2, _hit.point);

                perigo = true;
                pos = _hit.point.z;
            }
        }
        else //se nn houve colisao
        {
            debug_01 = "nada";

            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, transform.position);
            lr.SetPosition(2, transform.position);
        }

        if (transform.position.x <= -11f || transform.position.x >= 11f)
        {
            Script_PongManager.instance.BolaReset(transform.position.x < 0 ? 1 : 2);

            audioSource.PlayOneShot(audioClip[1]);
        }

        if (debug_01 == "so colidiu")
        {
            debug_02 = _hit.collider.name;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MoveSpeedHorizontal_Toggle();
            if (transform.position.x < -8.7f) MoveSpeedVertical_Toggle();
            if (transform.position.z + 0.5f < other.gameObject.transform.position.z && rb.velocity.z > 0) MoveSpeedVertical_Toggle();
            if (transform.position.z - 0.5f > other.gameObject.transform.position.z && rb.velocity.z < 0) MoveSpeedVertical_Toggle();

            audioSource.Play();
        }

        if (other.gameObject.CompareTag("IA"))
        {
            MoveSpeedHorizontal_Toggle();
            if (transform.position.x > 8.7f) MoveSpeedVertical_Toggle();
            if (transform.position.z + 0.5f < other.gameObject.transform.position.z && rb.velocity.z > 0) MoveSpeedVertical_Toggle();
            if (transform.position.z - 0.5f > other.gameObject.transform.position.z && rb.velocity.z < 0) MoveSpeedVertical_Toggle();

            audioSource.Play();
        }

        if (other.gameObject.CompareTag("Parede"))
        {
            MoveSpeedVertical_Toggle();

            audioSource.Play();
        }
    }

    void MoveSpeedHorizontal_Toggle()
    {
        rb.velocity = new Vector3(-rb.velocity.x, 0, rb.velocity.z);
    }

    void MoveSpeedVertical_Toggle()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, -rb.velocity.z);
    }
}