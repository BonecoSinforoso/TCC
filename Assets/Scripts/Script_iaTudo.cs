using UnityEngine;

public class Script_iaTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = Vector3.forward * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }
}