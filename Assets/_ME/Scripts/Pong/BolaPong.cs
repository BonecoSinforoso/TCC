using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class BolaPong : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("t_move")]
        [SerializeField] private Transform _moveTransform;
        public AudioClip[] AudioClips;

        private Rigidbody _rigidbody;
        private AudioSource _audioSource;
        public static bool Danger;
        public static float DangerPos;

        [FormerlySerializedAs("lr")]
        [SerializeField] private LineRenderer _trajectoryRenderer;

        #endregion

        #region metodos

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            Danger = false;

            if (_rigidbody.linearVelocity != Vector3.zero) _moveTransform.forward = transform.position + (new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z) * 10);

            Vector3 direction = _moveTransform.forward;

            Debug.DrawRay(transform.position, direction); //mostra o raio no editor da unity

            Physics.Raycast(transform.position, _moveTransform.forward, out RaycastHit hit, 50f);

            if (hit.collider) //se o raycast colidiu com algo
            {
                _trajectoryRenderer.SetPosition(0, transform.position);
                _trajectoryRenderer.SetPosition(1, hit.point);


                if (hit.collider.CompareTag("Parede")) //se colidiu com a parede
                {
                    direction = Vector3.Reflect(direction, hit.normal);
                    Debug.DrawRay(hit.point, direction);

                    Physics.Raycast(hit.point, direction, out RaycastHit hit2, 50f);


                    if (hit2.collider) //se segundo raycast colidiu com algo
                    {
                        _trajectoryRenderer.SetPosition(2, hit2.point);


                        if (hit2.collider.CompareTag("Perigo")) //se houve colisao com a baliza do AR
                        {
                            Danger = true;
                            DangerPos = hit.point.z;

                        }
                    }
                    else
                    {
                        _trajectoryRenderer.SetPosition(2, hit.point);
                    }
                }
                else if (hit.collider.CompareTag("Perigo")) //se a bola colidiu diretamente com o perigo (difere do rebatimento)
                {

                    _trajectoryRenderer.SetPosition(2, hit.point);

                    Danger = true;
                    DangerPos = hit.point.z;
                }
            }
            else //se nn houve colisao
            {

                _trajectoryRenderer.SetPosition(0, transform.position);
                _trajectoryRenderer.SetPosition(1, transform.position);
                _trajectoryRenderer.SetPosition(2, transform.position);
            }

            if (transform.position.x <= -11f || transform.position.x >= 11f)
            {
                PongManager.Instance.ResetBall(transform.position.x < 0 ? 1 : 2);

                _audioSource.PlayOneShot(AudioClips[1]);
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ToggleHorizontalSpeed();
                if (transform.position.x < -8.7f) ToggleVerticalSpeed();
                if (transform.position.z + 0.5f < other.gameObject.transform.position.z && _rigidbody.linearVelocity.z > 0) ToggleVerticalSpeed();
                if (transform.position.z - 0.5f > other.gameObject.transform.position.z && _rigidbody.linearVelocity.z < 0) ToggleVerticalSpeed();

                _audioSource.Play();
            }

            if (other.gameObject.CompareTag("IA"))
            {
                ToggleHorizontalSpeed();
                if (transform.position.x > 8.7f) ToggleVerticalSpeed();
                if (transform.position.z + 0.5f < other.gameObject.transform.position.z && _rigidbody.linearVelocity.z > 0) ToggleVerticalSpeed();
                if (transform.position.z - 0.5f > other.gameObject.transform.position.z && _rigidbody.linearVelocity.z < 0) ToggleVerticalSpeed();

                _audioSource.Play();
            }

            if (other.gameObject.CompareTag("Parede"))
            {
                ToggleVerticalSpeed();

                _audioSource.Play();
            }
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
