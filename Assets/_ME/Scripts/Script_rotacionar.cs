using UnityEngine;

public class Script_Rotacionar : MonoBehaviour
{
    [SerializeField] Vector3 valor;

    void Update()
    {
        transform.Rotate(valor * Time.deltaTime);
    }
}