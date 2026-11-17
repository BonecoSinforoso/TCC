using UnityEngine;

public class Script_Carro : Script_ObjDestruir
{
    [SerializeField] private float moveSpeed;
    private Rigidbody rb;
    private GameObject obj;
    private GameObject obj_player;

    private void Start()
    {
        obj_player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        transform.eulerAngles = new Vector3(0, 180, 0);
    }

    private void Update()
    {
        if (obj != null) Distancia_Verificar(obj);
        rb.linearVelocity = Vector3.back + new Vector3(0, rb.linearVelocity.y, 0);

        if (transform.position.y < -2f)
        {
            rb.linearVelocity = new Vector3(0, 0, rb.linearVelocity.z);
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }

        if (transform.position.z + 10 < obj_player.transform.position.z) Destroy(gameObject);
    }

    public void Obj_Set(GameObject _obj)
    {
        obj = _obj;
    }
}