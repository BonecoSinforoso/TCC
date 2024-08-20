using UnityEngine;

public class Script_ObjDestruir : MonoBehaviour
{
    public void Distancia_Verificar(GameObject _obj)
    {
        if (transform.position.z + 10 < _obj.transform.position.z) Destroy(gameObject);
    }
}