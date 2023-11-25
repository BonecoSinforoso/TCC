using UnityEngine;

public class Script_bolaPongTudo : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MoveSpeedHorizontalToggle();
        }

        if (collision.gameObject.CompareTag("IA"))
        {
            MoveSpeedHorizontalToggle();
        }

        if (collision.gameObject.CompareTag("Parede"))
        {
            MoveSpeedVerticalToggle();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MoveSpeedHorizontalToggle();
        }

        if (other.gameObject.CompareTag("IA"))
        {
            MoveSpeedHorizontalToggle();
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