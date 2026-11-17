using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class IaFlap : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("puloForca")]
        [SerializeField] private float _jumpForce;
        [FormerlySerializedAs("puloCd")]
        [SerializeField] private float _jumpCooldown;

        [FormerlySerializedAs("alturaMin")]
        [SerializeField] private float _minHeight;
        [FormerlySerializedAs("coefY")]
        [SerializeField] private float _heightMargin;

        [FormerlySerializedAs("t_cano")]
        [SerializeField] private Transform[] _pipeTransforms;
        [FormerlySerializedAs("distanciaCanoFrente")]
        [SerializeField] private float _pipeAheadDistance;

        [FormerlySerializedAs("txt_pontos")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Header("Visualizacao")]
        [FormerlySerializedAs("lr_canoAlturaDif")]
        [SerializeField] private LineRenderer _pipeHeightDiffRenderer;
        [FormerlySerializedAs("lr_canoDistHor")]
        [SerializeField] private LineRenderer _pipeHorizontalRenderer;
        [FormerlySerializedAs("lr_chao")]
        [SerializeField] private LineRenderer _groundRenderer;

        private int _score = 0;
        private bool _canJump = true;
        private bool _movable; //se canos sao moveis ou nn

        private Rigidbody _rigidbody;

        #endregion

        #region metodos

        private void Start()
        {
            _movable = FlapManager.Instance.Movable;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            int nearestPipeIndex = -1;

            //codigo para definir se o pulo eh necessario ou nn
            for (int i = 0; i < _pipeTransforms.Length; i++) //loop q verifica todos os canos
            {
                if (transform.position.x < _pipeTransforms[i].position.x) //condicao caso o passaro esteja a esquerda do cano
                {
                    if (_pipeTransforms[i].transform.position.x - transform.position.x <= _pipeAheadDistance)
                    {
                        if (transform.position.y + _heightMargin < _pipeTransforms[i].position.y)
                        {
                            Jump();
                        }
                    }
                }
                else ////condicao caso o passaro esteja a direita do cano
                {
                    if (Mathf.Abs(transform.position.x - _pipeTransforms[i].position.x) < 1) //condicao para evitar a colisao com o cano quando estiverem muito proximos (logo quando ele passa pela estrela)
                    {
                        if (transform.position.y + _heightMargin < _pipeTransforms[i].position.y) //verifica se a altura do passaro esta num intervalo perigoso
                        {
                            Jump();
                        }
                    }
                }

                //logica para definir qual cano estah mais proximo
                if (i != 0)
                {
                    if (Vector2.Distance(transform.position, _pipeTransforms[i].position) < Vector2.Distance(transform.position, _pipeTransforms[nearestPipeIndex].position))
                        nearestPipeIndex = i;
                }
                else nearestPipeIndex = 0;
            }

            if (transform.position.y < _minHeight) //verifica se o passaro esta numa posicao muito baixa
            {
                Jump();
            }

            //QUANDO O JOGADOR APERTA A TECLA Q
            //exibicao grafica dos parametros utilizados pelo agente reativo para realizar as acoes
            _pipeHeightDiffRenderer.SetPosition(0, new Vector3(transform.position.x - 0.5f, transform.position.y, 0));
            _pipeHeightDiffRenderer.SetPosition(1, new Vector3(transform.position.x - 0.5f, _pipeTransforms[nearestPipeIndex].position.y, 0));

            _pipeHorizontalRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 0.5f, 0));
            _pipeHorizontalRenderer.SetPosition(1, new Vector3(_pipeTransforms[nearestPipeIndex].position.x, transform.position.y + 0.5f, 0));

            _groundRenderer.SetPosition(0, transform.position);
            _groundRenderer.SetPosition(1, new Vector3(transform.position.x, -5, 0));
        }

        private void Jump()
        {
            if (!_canJump) return;
            _canJump = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetCanJump), _jumpCooldown);
        }

        private void ResetCanJump()
        {
            _canJump = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Chao"))
            {
                Time.timeScale = 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cano"))
            {
                Time.timeScale = 0;
            }

            if (other.CompareTag("Ponto"))
            {
                _score++;
                SetScoreText();
            }
        }

        private void SetScoreText()
        {
            _scoreText.text = _score.ToString();
        }

        #endregion
    }
}
