using UnityEngine;

public class Script_playerTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float moveSpeedX;
    [SerializeField] float puloForca;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);
        rb.velocity = Vector3.forward * moveSpeed + new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeedX, rb.velocity.y, 0);
    }
}