using UnityEngine;

public class Script_rotacionar : MonoBehaviour
{
    [SerializeField] Vector3 valor;

    void Update()
    {
        transform.Rotate(valor * Time.deltaTime);
    }
}