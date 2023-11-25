using UnityEngine;

public class Script_playerPongTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float energiaMax;
    [SerializeField] float energiaAtual;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = Input.GetAxisRaw("Vertical") * moveSpeed * Vector3.forward;
    }
}