using UnityEngine;

public class Script_canosTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float posMinX;
    [SerializeField] GameObject obj_canosOutro;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = Vector3.left * moveSpeed;

        if (transform.position.x < posMinX)
        {
            transform.position = obj_canosOutro.transform.position + Vector3.right * 6;
        }
    }
}