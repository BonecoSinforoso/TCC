using UnityEngine;

public class Script_canosTudo : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float subidaSpeed;
    [SerializeField] private float posMinX;
    [SerializeField] private GameObject obj_canosOutro;
    [SerializeField] private float alturaMax;
    [SerializeField] private Transform t_ponto;

    private Rigidbody rb;
    private bool movel;
    public bool subindo;

    private void Start()
    {
        movel = Script_FlapManager.instance.movel;

        rb = GetComponent<Rigidbody>();
        AlturaSet();
    }

    private void Update()
    {
        rb.linearVelocity = Vector3.left * moveSpeed;

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

    private void AlturaSet()
    {
        transform.position = new Vector2(transform.position.x, Random.Range(-alturaMax, alturaMax));
    }
}