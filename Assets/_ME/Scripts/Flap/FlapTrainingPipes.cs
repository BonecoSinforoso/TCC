using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    // Par de canos da arena de treino. Mesma logica do FlapPipeController, mas em posicao local da arena
    // e sem depender do FlapManager (singleton), para varias arenas rodarem na mesma cena.
    public class FlapTrainingPipes : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed = 2;
        [FormerlySerializedAs("subidaSpeed")]
        [SerializeField] private float _riseSpeed = 1;
        [FormerlySerializedAs("posMinX")]
        [SerializeField] private float _minX = -6;
        [FormerlySerializedAs("canosDist")]
        [SerializeField] private float _pipeDistance = 6;
        [FormerlySerializedAs("canosOutro")]
        [SerializeField] private FlapTrainingPipes _otherPipes;
        [FormerlySerializedAs("alturaMax")]
        [SerializeField] private float _maxHeight = 2.5f;

        private bool _movable;
        private bool _rising;

        #endregion

        #region metodos

        public void Configure(FlapTrainingPipes newOther)
        {
            _otherPipes = newOther;
        }

        public void Restart(float x, bool movable)
        {
            _movable = movable;
            _rising = Random.Range(0, 2) == 0;

            transform.localPosition = new Vector3(x, Random.Range(-_maxHeight, _maxHeight), 0);
        }

        private void FixedUpdate()
        {
            Vector3 pos = transform.localPosition;

            pos.x -= _moveSpeed * Time.fixedDeltaTime;

            if (pos.x < _minX)
            {
                pos.x = _otherPipes.transform.localPosition.x + _pipeDistance;
                pos.y = Random.Range(-_maxHeight, _maxHeight);
            }

            if (_movable)
            {
                if (_rising)
                {
                    pos.y += _riseSpeed * Time.fixedDeltaTime;
                    if (pos.y >= _maxHeight) _rising = false;
                }
                else
                {
                    pos.y -= _riseSpeed * Time.fixedDeltaTime;
                    if (pos.y <= -_maxHeight) _rising = true;
                }
            }

            transform.localPosition = pos;
        }

        #endregion
    }
}
