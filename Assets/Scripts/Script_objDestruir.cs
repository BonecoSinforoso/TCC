using UnityEngine;

public class Script_objDestruir : MonoBehaviour
{
    public void Cu(GameObject _obj)
    {
        if (transform.position.z + 10 < _obj.transform.position.z) Destroy(gameObject);
    }
}