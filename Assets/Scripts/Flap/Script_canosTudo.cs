using UnityEngine;

public class Script_canosTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float posMinX;
    [SerializeField] GameObject obj_canosOutro;
    [SerializeField] float alturaMax;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        AlturaSet();
    }

    void Update()
    {
        rb.velocity = Vector3.left * moveSpeed;

        if (transform.position.x < posMinX)
        {
            transform.position = obj_canosOutro.transform.position + Vector3.right * 6;
            AlturaSet();
        }
    }

    void AlturaSet()
    {
        transform.position = new Vector2(transform.position.x, Random.Range(-alturaMax, alturaMax));
    }
}