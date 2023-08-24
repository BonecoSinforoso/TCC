using UnityEngine;

public class Script_playerTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float moveSpeedX;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = Vector3.forward * moveSpeed + new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeedX, rb.velocity.y, 0);
    }
}