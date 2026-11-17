using UnityEngine;

namespace TCC
{
    public class DestroyBehindController : MonoBehaviour
    {
        #region membros

        private GameObject _target;

        #endregion

        #region metodos

        private void Update()
        {
            if (_target == null) return;
            if (transform.position.z + 10 < _target.transform.position.z) Destroy(gameObject);
        }

        public void SetObject(GameObject _obj)
        {
            _target = _obj;
        }

        #endregion
    }
}
