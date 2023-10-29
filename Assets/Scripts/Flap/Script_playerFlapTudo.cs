using UnityEngine;

public class Script_playerFlapTudo : MonoBehaviour
{
    [SerializeField] float puloForca;
    [SerializeField] float subidaRot;
    [SerializeField] float quedaRot;

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

        if (rb.velocity.y < 0) transform.eulerAngles += quedaRot * Time.deltaTime * Vector3.forward;
        if (rb.velocity.y > 0) transform.eulerAngles += subidaRot * Time.deltaTime * Vector3.forward;

        if (transform.eulerAngles.z > 300)
        {
            float bosta = transform.eulerAngles.z;
            bosta -= 360;
            if (bosta < -10) bosta = -10;
            transform.eulerAngles = new Vector3(0, 0, bosta);
        }
    }
}