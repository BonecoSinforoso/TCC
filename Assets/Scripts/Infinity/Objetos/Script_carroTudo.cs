using UnityEngine;

public class Script_carroTudo : Script_objDestruir
{
    [SerializeField] float moveSpeed;
    Rigidbody rb;
    GameObject obj;
    GameObject obj_player;

    void Start()
    {
        obj_player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        transform.eulerAngles = new Vector3(0, 180, 0);
    }

    void Update()
    {
        if (obj != null) DistanciaVerificar(obj);
        rb.velocity = Vector3.back + new Vector3(0, rb.velocity.y, 0);

        if (transform.position.y < -2f)
        {
            rb.velocity = new Vector3(0, 0, rb.velocity.z);
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }

        if (transform.position.z + 10 < obj_player.transform.position.z) Destroy(gameObject);
    }

    public void ObjSet(GameObject _obj)
    {
        obj = _obj;
    }
}