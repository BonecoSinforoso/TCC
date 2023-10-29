using UnityEngine;

public class Script_playerFlapTudo : MonoBehaviour
{
    [SerializeField] float puloForca;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);
        }
    }
}