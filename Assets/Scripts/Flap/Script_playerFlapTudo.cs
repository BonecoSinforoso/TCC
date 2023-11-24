using TMPro;
using UnityEngine;

public class Script_playerFlapTudo : MonoBehaviour
{
    [SerializeField] float puloForca;
    [SerializeField] float subidaRot;
    [SerializeField] float quedaRot;

    [SerializeField] TextMeshProUGUI txt_pontos;
    int pontos = 0;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (transform.eulerAngles.z < 5) transform.eulerAngles += Vector3.forward * 10;
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);
        }

        if (rb.velocity.y < 0) transform.eulerAngles += quedaRot * Time.deltaTime * Vector3.forward;
        if (rb.velocity.y > 0) transform.eulerAngles += subidaRot * Time.deltaTime * Vector3.forward;

        if (transform.eulerAngles.z > 300)
        {
            float _novoAngulo = transform.eulerAngles.z;
            _novoAngulo -= 360;

            if (_novoAngulo < -10) _novoAngulo = -10;
            transform.eulerAngles = new Vector3(0, 0, _novoAngulo);
        }
        else if (transform.eulerAngles.z > 30)
        {
            float _novoAngulo = 30;
            transform.eulerAngles = new Vector3(0, 0, _novoAngulo);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Chao"))
        {
            //Debug.LogError("dsajdhas");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cano"))
        {
            //Debug.LogError("dsajdhas");
        }

        if (other.CompareTag("Ponto"))
        {
            pontos++;
            TextPontosSet();
        }
    }

    void TextPontosSet()
    {
        txt_pontos.text = pontos.ToString();
    }
}