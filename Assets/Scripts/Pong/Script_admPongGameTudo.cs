using UnityEngine;

public class Script_admPongGameTudo : MonoBehaviour
{
    [SerializeField] GameObject obj_bola;
    public int lado = 0;
    [SerializeField] float bolaMoveSpeed;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BolaMove(lado);
        }
    }

    void BolaMove(int _lado)
    {
        Rigidbody _rb = obj_bola.GetComponent<Rigidbody>();

        _rb.isKinematic = false;

        _lado = _lado == 0 ? Random.Range(1, 3) : _lado;

        int _cu = Random.Range(0, 2) == 0 ? -1 : 1;

        if (_lado == 1) _rb.velocity = Vector3.left * bolaMoveSpeed + ((Random.Range(0, 2) == 0 ? -1 : 1) * bolaMoveSpeed) * Vector3.forward;
        else _rb.velocity = Vector3.right * bolaMoveSpeed + ((Random.Range(0, 2) == 0 ? -1 : 1) * bolaMoveSpeed) * Vector3.forward;
    }
}