using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class InfinityCharacterController : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed;
        [FormerlySerializedAs("puloForca")]
        [SerializeField] private float _jumpForce;

        private Rigidbody _rigidbody;
        private Animator _animator;

        private int _score = 0;
        private bool _lost = false;
        private bool _canJump = false;

        #endregion

        #region metodos

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = transform.GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (_lost) return;

            _animator.SetBool("_jump", !_canJump);

            _rigidbody.linearVelocity = Vector3.forward * _moveSpeed + new Vector3(0, _rigidbody.linearVelocity.y, 0);
        }

        private void Jump()
        {
            if (!_canJump) return;
            _canJump = false;
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
        private void Lose()
        {
            _lost = true;
            _rigidbody.linearVelocity = Vector3.zero;
            InfinityManager.Instance.SetLost(2);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Energetico"))
            {
                _score += 10;
                Destroy(other.gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Carro"))
            {
                Lose();
                Destroy(collision.gameObject);
            }
            if (collision.gameObject.CompareTag("Onibus"))
            {
                Lose();
                Destroy(collision.gameObject);
            }
            if (collision.gameObject.CompareTag("Chao"))
            {
                _canJump = true;
            }
        }

        #endregion
    }
}
