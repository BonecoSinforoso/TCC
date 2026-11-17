using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class IaInfinity : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed;
        [FormerlySerializedAs("puloForca")]
        [SerializeField] private float _jumpForce;
        [FormerlySerializedAs("puloDist")]
        [SerializeField] private float _jumpDistance;
        [FormerlySerializedAs("t")]
        [SerializeField] private float _lerpFactor;
        [SerializeField] private TextMeshProUGUI _speedText;

        private Rigidbody _rigidbody;
        private Animator _animator;

        private int _score = 0;
        private bool _lost = false;
        private bool _canJump = false;

        [FormerlySerializedAs("esq")]
        [SerializeField] private bool _goLeft = false;
        [FormerlySerializedAs("cen")]
        [SerializeField] private bool _goCenter = false;
        [FormerlySerializedAs("dir")]
        [SerializeField] private bool _goRight = false;

        [FormerlySerializedAs("bloq_esq")]
        [SerializeField] private bool _blockedLeft;
        [FormerlySerializedAs("bloq_cen")]
        [SerializeField] private bool _blockedCenter;
        [FormerlySerializedAs("bloq_dir")]
        [SerializeField] private bool _blockedRight;

        [FormerlySerializedAs("lr")]
        [SerializeField] private LineRenderer[] _rayRenderers;

        private bool _canChangeSide = true;

        [FormerlySerializedAs("raioFrenteTamanho")]
        [SerializeField] private float _forwardRayLength;

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

            _moveSpeed = 5 + ((int)transform.position.z / 100);
            _moveSpeed = _moveSpeed > 8 ? 8 : _moveSpeed;
            _speedText.text = _moveSpeed + "m/s";

            _rigidbody.linearVelocity = Vector3.forward * _moveSpeed + new Vector3(0, _rigidbody.linearVelocity.y, 0);

            Debug.DrawRay(new Vector3(17.5f, 0.5f, transform.position.z), Vector3.forward * 5f, Color.red);
            Debug.DrawRay(new Vector3(17.5f, 2f, transform.position.z), Vector3.down * 2f, Color.red);

            Debug.DrawRay(new Vector3(20f, 0.5f, transform.position.z), Vector3.forward * 5f, Color.red);
            Debug.DrawRay(new Vector3(20f, 2f, transform.position.z), Vector3.down * 2f, Color.red);

            Debug.DrawRay(new Vector3(22.5f, 0.5f, transform.position.z), Vector3.forward * 5f, Color.red);
            Debug.DrawRay(new Vector3(22.5f, 2f, transform.position.z), Vector3.down * 2f, Color.red);

            _blockedLeft = false;
            _blockedCenter = false;
            _blockedRight = false;

            //logica para definir qual espaco esta bloqueado/livre
            if (Physics.Raycast(new Vector3(17.5f, 0.5f, transform.position.z), Vector3.forward, _forwardRayLength, 1 << 3)) _blockedLeft = true;
            if (Physics.Raycast(new Vector3(17.5f, 2f, transform.position.z), Vector3.down, 2f, 1 << 3)) _blockedLeft = true;

            if (Physics.Raycast(new Vector3(20f, 0.5f, transform.position.z), Vector3.forward, _forwardRayLength, 1 << 3)) _blockedCenter = true;
            if (Physics.Raycast(new Vector3(20f, 2f, transform.position.z), Vector3.down, 2f, 1 << 3)) _blockedCenter = true;

            if (Physics.Raycast(new Vector3(22.5f, 0.5f, transform.position.z), Vector3.forward, _forwardRayLength, 1 << 3)) _blockedRight = true;
            if (Physics.Raycast(new Vector3(22.5f, 2f, transform.position.z), Vector3.down, 2f, 1 << 3)) _blockedRight = true;

            DrawRay();

            bool carDetected = false;

            // Raycast emitido a partir da base do personagem, posicionado pr�ximo ao ch�o
            // Sua fun��o � detectar obst�culos quando o personagem est� caindo ap�s um pulo        
            if (Physics.Raycast(new Vector3(transform.position.x, 0, transform.position.z) + Vector3.forward, Vector3.forward, out RaycastHit hit, 5f))
            {
                if (hit.collider.CompareTag("Carro"))
                {
                    if (_rigidbody.linearVelocity.z > 8.5f && !_canJump)
                    {
                        if (transform.position.x < 19) _blockedLeft = true;
                        else if (transform.position.x < 21) _blockedCenter = true;
                        else _blockedRight = true;
                    }

                    if (Mathf.Abs(transform.position.z - hit.point.z) < _jumpDistance)
                    {
                        Jump();
                    }
                }

                carDetected = true;
            }

            _rayRenderers[6].startColor = carDetected ? Color.red : Color.green;
            _rayRenderers[6].endColor = carDetected ? Color.red : Color.green;

            _rayRenderers[6].SetPosition(0, new Vector3(transform.position.x, 0, transform.position.z) + Vector3.forward);
            _rayRenderers[6].SetPosition(1, new Vector3(transform.position.x, 0, transform.position.z) + Vector3.forward * _forwardRayLength);

            //logica q define a acao se baseando nos locais livres/bloqueados
            if (_blockedLeft)
            {
                if (transform.position.x < 19)
                {
                    if (_blockedCenter)
                    {
                        _goRight = true;
                    }
                    else
                    {
                        _goCenter = true;
                    }
                }
            }
            if (_blockedCenter)
            {
                if (transform.position.x >= 19.2f && transform.position.x <= 20.8f)
                {
                    if (!_blockedLeft && !_blockedRight)
                    {
                        if (_canChangeSide)
                        {
                            _canChangeSide = false;
                            _goLeft = Random.Range(0, 2) == 0;
                            _goRight = !_goLeft;
                        }
                    }
                    else //nn eh else if pq tenho q tratar quando tudo tiver block
                    {
                        if (!_blockedLeft) _goLeft = true;
                        if (!_blockedRight) _goRight = true;
                    }
                }
            }
            if (_blockedRight)
            {
                if (transform.position.x > 21)
                {
                    if (_blockedCenter)
                    {
                        _goLeft = true;
                    }
                    else
                    {
                        _goCenter = true;
                    }
                }
            }

            //define para qual lugar ir
            if (_goLeft) //vai para esquerda
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(17.5f, transform.position.y, transform.position.z), _lerpFactor);
                if (Mathf.Abs(transform.position.x - 17.5f) < 0.05f)
                {
                    _goLeft = false;
                    _canChangeSide = true;
                }
            }
            else if (_goCenter) //vai para centro
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(20f, transform.position.y, transform.position.z), _lerpFactor);
                if (Mathf.Abs(transform.position.x - 20f) < 0.05f) _goCenter = false;
            }
            else if (_goRight) //vai para direita
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(22.5f, transform.position.y, transform.position.z), _lerpFactor);
                if (Mathf.Abs(transform.position.x - 22.5f) < 0.05f)
                {
                    _goRight = false;
                    _canChangeSide = true;
                }
            }
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
            InfinityManager.Instance.SetLost(1);
        }

        private void DrawRay() //aqui eh onde os raios sao desenhados para o jogador
        {
            _rayRenderers[0].startColor = _blockedLeft ? Color.red : Color.green;
            _rayRenderers[0].endColor = _blockedLeft ? Color.red : Color.green;
            _rayRenderers[1].startColor = _blockedLeft ? Color.red : Color.green;
            _rayRenderers[1].endColor = _blockedLeft ? Color.red : Color.green;

            _rayRenderers[2].startColor = _blockedCenter ? Color.red : Color.green;
            _rayRenderers[2].endColor = _blockedCenter ? Color.red : Color.green;
            _rayRenderers[3].startColor = _blockedCenter ? Color.red : Color.green;
            _rayRenderers[3].endColor = _blockedCenter ? Color.red : Color.green;

            _rayRenderers[4].startColor = _blockedRight ? Color.red : Color.green;
            _rayRenderers[4].endColor = _blockedRight ? Color.red : Color.green;
            _rayRenderers[5].startColor = _blockedRight ? Color.red : Color.green;
            _rayRenderers[5].endColor = _blockedRight ? Color.red : Color.green;

            _rayRenderers[0].SetPosition(0, new Vector3(17.5f, 0.5f, transform.position.z));
            _rayRenderers[0].SetPosition(1, new Vector3(17.5f, 0.5f, transform.position.z + _forwardRayLength));

            _rayRenderers[1].SetPosition(0, new Vector3(17.5f, 2f, transform.position.z));
            _rayRenderers[1].SetPosition(1, new Vector3(17.5f, 0f, transform.position.z));

            _rayRenderers[2].SetPosition(0, new Vector3(20f, 0.5f, transform.position.z));
            _rayRenderers[2].SetPosition(1, new Vector3(20f, 0.5f, transform.position.z + _forwardRayLength));

            _rayRenderers[3].SetPosition(0, new Vector3(20f, 2f, transform.position.z));
            _rayRenderers[3].SetPosition(1, new Vector3(20f, 0f, transform.position.z));

            _rayRenderers[4].SetPosition(0, new Vector3(22.5f, 0.5f, transform.position.z));
            _rayRenderers[4].SetPosition(1, new Vector3(22.5f, 0.5f, transform.position.z + _forwardRayLength));

            _rayRenderers[5].SetPosition(0, new Vector3(22.5f, 2f, transform.position.z));
            _rayRenderers[5].SetPosition(1, new Vector3(22.5f, 0f, transform.position.z));
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
                InfinityManager.Instance.BlindPlayer();
            }
        }

        public void SetForwardRayLength(float value)
        {
            _forwardRayLength = value;
        }

        #endregion
    }
}
