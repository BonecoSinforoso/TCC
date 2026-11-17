using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class PongManager : MonoBehaviour
    {
        #region membros

        public static PongManager Instance { get; private set; }

        [FormerlySerializedAs("obj_bola")]
        [SerializeField] private GameObject _ballObject;
        private int _ballSide = 0;
        [FormerlySerializedAs("txt_tempo")]
        [SerializeField] private TextMeshPro _timeText;
        [FormerlySerializedAs("txt_placar")]
        [SerializeField] private TextMeshPro _scoreText;
        [FormerlySerializedAs("txt_aperte")]
        [SerializeField] private TextMeshPro _pressText;
        [FormerlySerializedAs("tr_bola")]
        [SerializeField] private TrailRenderer _ballTrail;

        [FormerlySerializedAs("fbGanharTempo")]
        [SerializeField] private int _matchDuration;

        private readonly int[] _scores = { 0, 0 };

        private Rigidbody _ballRigidbody;
        private bool _ballMoving = false;
        private int _currentTime = 0;

        #endregion

        #region metodos

        private void Start()
        {
            Instance = this;

            _ballRigidbody = _ballObject.GetComponent<Rigidbody>();

            _timeText.text = _matchDuration.ToString() + "s";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MoveBall(_ballSide);
            }
        }

        private void MoveBall(int side)
        {
            if (_ballMoving) return;

            _pressText.gameObject.SetActive(false);

            Invoke(nameof(SpeedUpBall), 10f);
            InvokeRepeating(nameof(SetTime), 1f, 1f);
            _currentTime = 0;

            _ballRigidbody.isKinematic = false;
            _ballTrail.emitting = true;
            float ballSpeedX = Random.Range(5, 8);
            float ballSpeedY = 5 + (5 - ballSpeedX);

            _ballMoving = true;
            side = side == 0 ? Random.Range(1, 3) : side;

            if (side == 1) _ballRigidbody.linearVelocity = Vector3.left * ballSpeedX + ((Random.Range(0, 2) == 0 ? -1 : 1) * ballSpeedY) * Vector3.forward;
            else _ballRigidbody.linearVelocity = Vector3.right * ballSpeedX + ((Random.Range(0, 2) == 0 ? -1 : 1) * ballSpeedY) * Vector3.forward;
        }

        public void ResetBall(int side) //recebe 1 ou 2
        {
            CancelInvoke();
            _pressText.gameObject.SetActive(true);
            SetTimeText(_matchDuration);

            //bola
            _ballMoving = false;
            _ballSide = side;
            _ballObject.transform.position = Vector3.zero;
            _ballRigidbody.linearVelocity = Vector3.zero;
            _ballRigidbody.isKinematic = true;
            _ballTrail.emitting = false;

            //
            if (side == 1) _scores[1]++;
            else _scores[0]++;

            if (_scores[1] == 3) CallShowResult(0);
            else if (_scores[0] == 3) CallShowResult(1);

            SetScoreText();
        }

        private void SetScoreText()
        {
            _scoreText.text = _scores[0].ToString() + " x " + _scores[1].ToString();
        }

        private void SpeedUpBall()
        {
            if (Random.Range(0, 2) == 0)
            {
                if (_ballRigidbody.linearVelocity.x > 0) _ballRigidbody.linearVelocity += Vector3.right;
                else _ballRigidbody.linearVelocity += Vector3.left;
            }
            else
            {
                if (_ballRigidbody.linearVelocity.z > 0) _ballRigidbody.linearVelocity += Vector3.forward;
                else _ballRigidbody.linearVelocity += Vector3.back;
            }

            Invoke(nameof(SpeedUpBall), 10f);
        }

        private void SetTime()
        {
            _currentTime++;
            SetTimeText();

            if (_matchDuration - _currentTime == 0)
            {
                _ballObject.GetComponent<AudioSource>().PlayOneShot(_ballObject.GetComponent<BolaPong>().AudioClips[1]);
                ResetBall(0);
            }
        }

        private void SetTimeText(int value = -1)
        {
            if (value == -1) _timeText.text = (_matchDuration - _currentTime).ToString() + "s";
            else
            {
                _timeText.text = value.ToString() + "s";
            }
        }

        public void CallShowResult(int value)
        {
            GeralManager.Instance.ShowResult(value);
        }

        #endregion
    }
}
