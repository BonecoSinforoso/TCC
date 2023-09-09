using UnityEngine;

public class Script_objDestruirTudo : MonoBehaviour
{
    GameObject obj;

    void Update()
    {
        if (obj == null) return;
        if (transform.position.z + 10 < obj.transform.position.z) Destroy(gameObject);
    }

    public void ObjSet(GameObject _obj)
    {
        obj = _obj;
    }
}