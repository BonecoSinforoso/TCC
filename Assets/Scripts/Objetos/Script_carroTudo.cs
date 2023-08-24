using UnityEngine;

public class Script_carroTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.eulerAngles = new Vector3(0, 180, 0);
    }

    void Update()
    {
        rb.velocity = Vector3.back + new Vector3(0, rb.velocity.y, 0);
    }
}