using UnityEngine;

public class Script_bolaPongTudo : MonoBehaviour
{
    [SerializeField] Transform t_move;
    public AudioClip[] audioClip;

    Rigidbody rb;
    AudioSource audioSource;
    LayerMask layerMask;
    public static bool perigo;
    public static float pos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        layerMask = LayerMask.GetMask("Perigo");
    }

    private void Update()
    {
        perigo = false;

        if (rb.velocity != Vector3.zero) t_move.forward = transform.position + new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 _direcao = t_move.forward;

        Debug.DrawRay(transform.position, _direcao);

        Physics.Raycast(transform.position, t_move.forward, out RaycastHit _hit, layerMask);

        if (_hit.collider)
        {
            if (_hit.collider.CompareTag("Parede"))
            {
                _direcao = Vector3.Reflect(_direcao, _hit.normal);
                Debug.DrawRay(_hit.point, _direcao);

                Physics.Raycast(_hit.point, _direcao, out RaycastHit _hit2, layerMask);

                if (_hit2.collider)
                {
                    if (_hit2.collider.CompareTag("Perigo"))
                    {
                        //Debug.Log("kkk");

                        perigo = true;
                        pos = _hit.point.z;
                    }
                }
            }
            else if (_hit.collider.CompareTag("Perigo"))
            {
                //Debug.Log("uekkk");

                perigo = true;
                pos = _hit.point.z;
            }
        }

        if (transform.position.x <= -11f || transform.position.x >= 11f)
        {
            Script_PongManager.instance.BolaReset(transform.position.x < 0 ? 1 : 2);

            audioSource.PlayOneShot(audioClip[1]);
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