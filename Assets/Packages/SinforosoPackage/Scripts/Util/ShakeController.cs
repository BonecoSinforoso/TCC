using System.Collections;
using UnityEngine;

namespace SinforosoPackage
{
    [DisallowMultipleComponent]
    public class ShakeController : MonoBehaviour
    {
        [SerializeField] private bool _innate;
        [SerializeField] private float _delay;
        [SerializeField] private bool _relative;

        [Header("posicao")]
        [SerializeField] private float _posX;
        [SerializeField] private float _posY;
        [SerializeField] private float _posZ;

        [Header("rotacao")]
        [SerializeField] private float _rotX;
        [SerializeField] private float _rotY;
        [SerializeField] private float _rotZ;

        private Vector3 _startPos;
        private Vector3 _startRot;

        private void Start()
        {
            _startPos = transform.localPosition;
            _startRot = transform.localEulerAngles;

            if (_innate) StartCoroutine(Shake_Coroutine());
        }

        public void StartShake() => StartCoroutine(Shake_Coroutine());
        public void StartShake(float duration) => StartCoroutine(Shake_Coroutine(duration));
        public void StopShake() => StopAllCoroutines();

        #region coroutines

        private IEnumerator Shake_Coroutine()
        {
            while (true)
            {
                if (_relative)
                {
                    transform.localPosition += new Vector3(Random.Range(-_posX, _posX), Random.Range(-_posY, _posY), Random.Range(-_posZ, _posZ));
                    transform.localEulerAngles += new Vector3(Random.Range(-_rotX, _rotX), Random.Range(-_rotY, _rotY), Random.Range(-_rotZ, _rotZ));
                }
                else
                {
                    transform.localPosition = _startPos + new Vector3(Random.Range(-_posX, _posX), Random.Range(-_posY, _posY), Random.Range(-_posZ, _posZ));
                    transform.localEulerAngles = _startRot + new Vector3(Random.Range(-_rotX, _rotX), Random.Range(-_rotY, _rotY), Random.Range(-_rotZ, _rotZ));
                }

                yield return new WaitForSeconds(_delay);
            }
        }

        private IEnumerator Shake_Coroutine(float duration)
        {
            float elpasedTime = 0;

            while (elpasedTime < duration)
            {
                if (_relative)
                {
                    transform.localPosition += new Vector3(Random.Range(-_posX, _posX), Random.Range(-_posY, _posY), Random.Range(-_posZ, _posZ));
                    transform.localEulerAngles += new Vector3(Random.Range(-_rotX, _rotX), Random.Range(-_rotY, _rotY), Random.Range(-_rotZ, _rotZ));
                }
                else
                {
                    transform.localPosition = _startPos + new Vector3(Random.Range(-_posX, _posX), Random.Range(-_posY, _posY), Random.Range(-_posZ, _posZ));
                    transform.localEulerAngles = _startRot + new Vector3(Random.Range(-_rotX, _rotX), Random.Range(-_rotY, _rotY), Random.Range(-_rotZ, _rotZ));
                }

                yield return new WaitForSeconds(_delay);

                elpasedTime += _delay;
            }
        }

        #endregion
    }
}
