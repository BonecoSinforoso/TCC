using UnityEngine;

public class RotateController : MonoBehaviour
{
    [SerializeField] private Vector3 valor;

    private void Update()
    {
        transform.Rotate(valor * Time.deltaTime);
    }
}