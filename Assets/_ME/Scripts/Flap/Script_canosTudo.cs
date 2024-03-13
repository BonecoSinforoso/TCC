using UnityEngine;

public class Script_canosTudo : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float subidaSpeed;
    [SerializeField] float posMinX;
    [SerializeField] GameObject obj_canosOutro;
    [SerializeField] float alturaMax;
    [SerializeField] Transform t_ponto;

    Rigidbody rb;
    bool movel;
    public bool subindo;

    void Start()
    {
        movel = Script_FlapManager.instance.movel;

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

        if (movel)
        {
            if (subindo)
            {
                transform.position += subidaSpeed * Time.deltaTime * Vector3.up;

                if (transform.position.y >= alturaMax) subindo = false;
            }
            else
            {
                transform.position += subidaSpeed * Time.deltaTime * Vector3.down;

                if (transform.position.y <= -alturaMax) subindo = true;
            }
        }

        t_ponto.position = transform.position;
    }

    void AlturaSet()
    {
        transform.position = new Vector2(transform.position.x, Random.Range(-alturaMax, alturaMax));
    }
}