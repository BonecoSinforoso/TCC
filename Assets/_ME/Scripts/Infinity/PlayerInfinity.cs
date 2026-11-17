using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class PlayerInfinity : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed;
        [FormerlySerializedAs("puloForca")]
        [SerializeField] private float _jumpForce;
        [FormerlySerializedAs("posLimiteX")]
        [SerializeField] private float _limitX;
        [FormerlySerializedAs("txt_moveSpeed")]
        [SerializeField] private TextMeshProUGUI _speedText;

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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            _animator.SetBool("_jump", !_canJump);

            _moveSpeed = 5 + ((int)transform.position.z / 100);
            _moveSpeed = _moveSpeed > 8 ? 8 : _moveSpeed;
            _speedText.text = _moveSpeed + "m/s";

            _rigidbody.linearVelocity = Vector3.forward * _moveSpeed + Vector3.up * _rigidbody.linearVelocity.y;

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.position += Vector3.left * 2.5f;
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.position += Vector3.right * 2.5f;
            }

            if (transform.position.x < -_limitX) transform.position = new Vector3(-_limitX, transform.position.y, transform.position.z);
            else if (transform.position.x > _limitX) transform.position = new Vector3(_limitX, transform.position.y, transform.position.z);
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
            InfinityManager.Instance.SetLost(0);
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Energetico"))
            {
                _score += 10;
                Destroy(other.gameObject);
            }

            if (other.CompareTag("Oculos"))
            {
                InfinityManager.Instance.BlindIa();
            }
        }

        #endregion
    }
}
