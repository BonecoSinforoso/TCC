using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class Carro : ObjDestruir
    {
        #region membros

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed;
        private Rigidbody _rigidbody;
        private GameObject _target;
        private GameObject _playerObject;

        #endregion

        #region metodos

        private void Start()
        {
            _playerObject = GameObject.FindGameObjectWithTag("Player");
            _rigidbody = GetComponent<Rigidbody>();
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        private void Update()
        {
            if (_target != null) CheckDistance(_target);
            _rigidbody.linearVelocity = Vector3.back + new Vector3(0, _rigidbody.linearVelocity.y, 0);

            if (transform.position.y < -2f)
            {
                _rigidbody.linearVelocity = new Vector3(0, 0, _rigidbody.linearVelocity.z);
                transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            }

            if (transform.position.z + 10 < _playerObject.transform.position.z) Destroy(gameObject);
        }

        public void SetObject(GameObject _obj)
        {
            _target = _obj;
        }

        #endregion
    }
}
