using UnityEngine;

namespace TCC
{
    public class ObjDestruir : MonoBehaviour
    {
        #region membros

        #endregion

        #region metodos

        public void CheckDistance(GameObject obj)
        {
            if (transform.position.z + 10 < obj.transform.position.z) Destroy(gameObject);
        }

        #endregion
    }
}
