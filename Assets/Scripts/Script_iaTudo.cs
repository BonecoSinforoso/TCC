using UnityEngine;

public class Script_iaTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    GameObject obj_adm;
    Rigidbody rb;
    int pontos = 0;

    void Start()
    {
        obj_adm = GameObject.FindGameObjectWithTag("ADM");
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = Vector3.forward * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energetico"))
        {
            pontos += 10;
            obj_adm.GetComponent<Script_admGameTudo>().TextoPontosChange(0, pontos);
        }
    }
}