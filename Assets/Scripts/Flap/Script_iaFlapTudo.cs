using UnityEngine;

public class Script_iaFlapTudo : MonoBehaviour
{
    [SerializeField] float puloForca;
    [SerializeField] float alturaMin;
    [SerializeField] float puloCd;

    bool puloPode = true;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.position.y < alturaMin)
        {
            if (puloPode)
            {
                puloPode = false;
                rb.velocity = Vector3.zero;
                rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);

                Invoke(nameof(PuloPodeReset), puloCd);
            }            
        }
    }

    void PuloPodeReset()
    {
        puloPode = true;
    }
}
