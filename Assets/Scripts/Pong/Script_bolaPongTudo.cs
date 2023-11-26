using UnityEngine;

public class Script_bolaPongTudo : MonoBehaviour
{
    [SerializeField] Transform t_move;

    GameObject obj_adm;
    Rigidbody rb;
    LayerMask layerMask;

    public static bool perigo;

    void Start()
    {
        obj_adm = GameObject.FindGameObjectWithTag("ADM");
        rb = GetComponent<Rigidbody>();

        layerMask = LayerMask.GetMask("Perigo");
    }

    private void Update()
    {
        if (rb.velocity != Vector3.zero) t_move.forward = transform.position + new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //if (Physics.Raycast(transform.position, t_move.forward, 20f, layerMask)) Debug.Log("xrk");
        perigo = Physics.Raycast(transform.position, t_move.forward, 20f, layerMask);

        if (transform.position.x <= -11f || transform.position.x >= 11f)
        {
            obj_adm.GetComponent<Script_admPongGameTudo>().BolaReset(transform.position.x < 0 ? 1 : 2);
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
        }

        if (other.gameObject.CompareTag("IA"))
        {
            MoveSpeedHorizontalToggle();
            if (transform.position.x > 8.7f) MoveSpeedVerticalToggle();
        }

        if (other.gameObject.CompareTag("Parede"))
        {
            MoveSpeedVerticalToggle();
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