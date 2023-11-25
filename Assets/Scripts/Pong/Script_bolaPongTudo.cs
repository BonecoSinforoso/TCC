using UnityEngine;

public class Script_bolaPongTudo : MonoBehaviour
{
    GameObject obj_adm;
    Rigidbody rb;

    void Start()
    {
        obj_adm = GameObject.FindGameObjectWithTag("ADM");
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
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