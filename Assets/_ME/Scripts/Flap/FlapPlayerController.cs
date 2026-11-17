using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class FlapPlayerController : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("puloForca")]
        [SerializeField] private float _jumpForce;
        [FormerlySerializedAs("subidaRot")]
        [SerializeField] private float _riseRotation;
        [FormerlySerializedAs("quedaRot")]
        [SerializeField] private float _fallRotation;

        [FormerlySerializedAs("txt_pontos")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        private int _score = 0;

        private Rigidbody _rigidbody;

        #endregion

        #region metodos

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (transform.eulerAngles.z < 5) transform.eulerAngles += Vector3.forward * 10;
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

                FlapManager.Instance.PlayAudio(0);
            }

            if (_rigidbody.linearVelocity.y < 0) transform.eulerAngles += _fallRotation * Time.deltaTime * Vector3.forward;
            if (_rigidbody.linearVelocity.y > 0) transform.eulerAngles += _riseRotation * Time.deltaTime * Vector3.forward;

            if (transform.eulerAngles.z > 300)
            {
                float newAngle = transform.eulerAngles.z;
                newAngle -= 360;

                if (newAngle < -10) newAngle = -10;
                transform.eulerAngles = new Vector3(0, 0, newAngle);
            }
            else if (transform.eulerAngles.z > 30)
            {
                float newAngle = 30;
                transform.eulerAngles = new Vector3(0, 0, newAngle);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Chao"))
            {
                //Debug.LogError("dsajdhas");

                FlapManager.Instance.PlayAudio(2);

                FlapManager.Instance.CallShowResult(0);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cano"))
            {
                //Debug.LogError("dsajdhas");

                FlapManager.Instance.PlayAudio(2);

                FlapManager.Instance.CallShowResult(0);
            }

            if (other.CompareTag("Ponto"))
            {
                _score++;
                SetScoreText();

                FlapManager.Instance.PlayAudio(1);

                if (_score >= FlapManager.Instance.ScoreToWin) FlapManager.Instance.CallShowResult(1);
            }
        }

        private void SetScoreText()
        {
            _scoreText.text = _score.ToString();
        }

        #endregion
    }
}
