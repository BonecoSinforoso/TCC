using UnityEngine;

public class Script_playerTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float moveSpeedX;
    [SerializeField] float puloForca;

    GameObject obj_adm;
    Rigidbody rb;
    int pontos = 0;
    bool perdeu = false;

    void Start()
    {
        obj_adm = GameObject.FindGameObjectWithTag("ADM");
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (perdeu) return;
        if (Input.GetKeyDown(KeyCode.Space)) rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);
        rb.velocity = Vector3.forward * moveSpeed + new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeedX, rb.velocity.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
            obj_adm.GetComponent<Script_admGameTudo>().TextoPontosChange(0, pontos);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Carro"))
        {
            perdeu = true;
            rb.velocity = Vector3.zero;
        }
    }
}