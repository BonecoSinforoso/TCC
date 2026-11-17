using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    // Bola da mesa de treino. Mesma rebatida do BolaPong.
    public class PongTrainingBall : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("t_arena")]
        [SerializeField] private Transform _arenaTransform;

        private Rigidbody _rigidbody;

        #endregion

        #region metodos

        public void Configure(Transform _t_arena)
        {
            _arenaTransform = _t_arena;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            float x = _arenaTransform.InverseTransformPoint(transform.position).x;

            if (other.gameObject.CompareTag("Player"))
            {
                HitPaddle(other, x < -8.7f);
            }

            if (other.gameObject.CompareTag("IA"))
            {
                HitPaddle(other, x > 8.7f);
            }

            if (other.gameObject.CompareTag("Parede"))
            {
                ToggleVerticalSpeed();
            }
        }

        private void HitPaddle(Collider paddle, bool passedPaddle)
        {
            ToggleHorizontalSpeed();
            if (passedPaddle) ToggleVerticalSpeed();
            if (transform.position.z + 0.5f < paddle.transform.position.z && _rigidbody.linearVelocity.z > 0) ToggleVerticalSpeed();
            if (transform.position.z - 0.5f > paddle.transform.position.z && _rigidbody.linearVelocity.z < 0) ToggleVerticalSpeed();

            if (paddle.TryGetComponent(out AgentPong agent)) agent.HitBall();
        }

        private void ToggleHorizontalSpeed()
        {
            _rigidbody.linearVelocity = new Vector3(-_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        }

        private void ToggleVerticalSpeed()
        {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, -_rigidbody.linearVelocity.z);
        }

        #endregion
    }
}
