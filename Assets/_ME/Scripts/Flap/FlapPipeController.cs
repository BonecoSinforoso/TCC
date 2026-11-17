using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class FlapPipeController : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed;
        [FormerlySerializedAs("subidaSpeed")]
        [SerializeField] private float _riseSpeed;
        [FormerlySerializedAs("posMinX")]
        [SerializeField] private float _minX;
        [FormerlySerializedAs("obj_canosOutro")]
        [SerializeField] private GameObject _otherPipesObject;
        [FormerlySerializedAs("alturaMax")]
        [SerializeField] private float _maxHeight;
        [FormerlySerializedAs("t_ponto")]
        [SerializeField] private Transform _pointTransform;

        private Rigidbody _rigidbody;
        private bool _movable;
        [FormerlySerializedAs("subindo")]
        public bool Rising;

        #endregion

        #region metodos

        private void Start()
        {
            _movable = FlapManager.Instance.Movable;

            _rigidbody = GetComponent<Rigidbody>();
            SetHeight();
        }

        private void Update()
        {
            _rigidbody.linearVelocity = Vector3.left * _moveSpeed;

            if (transform.position.x < _minX)
            {
                transform.position = _otherPipesObject.transform.position + Vector3.right * 6;
                SetHeight();
            }

            if (_movable)
            {
                if (Rising)
                {
                    transform.position += _riseSpeed * Time.deltaTime * Vector3.up;

                    if (transform.position.y >= _maxHeight) Rising = false;
                }
                else
                {
                    transform.position += _riseSpeed * Time.deltaTime * Vector3.down;

                    if (transform.position.y <= -_maxHeight) Rising = true;
                }
            }

            _pointTransform.position = transform.position;
        }

        private void SetHeight()
        {
            transform.position = new Vector2(transform.position.x, Random.Range(-_maxHeight, _maxHeight));
        }

        #endregion
    }
}
