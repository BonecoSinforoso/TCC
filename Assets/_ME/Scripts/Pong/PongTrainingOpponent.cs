using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    // Adversario da mesa de treino: copia do IaPong, com a trajetoria da bola calculada por conta.
    public class PongTrainingOpponent : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed = 7;
        [SerializeField] private Transform _arenaTransform;
        [FormerlySerializedAs("rb_bola")]
        [SerializeField] private Rigidbody _ballRigidbody;

        [FormerlySerializedAs("energiaMax")]
        [SerializeField] private float _maxEnergy = 100;
        [FormerlySerializedAs("energiaAtual")]
        [SerializeField] private float _currentEnergy = 100;
        [FormerlySerializedAs("energiaConsumo")]
        [SerializeField] private float _energyConsumption = 100;
        [FormerlySerializedAs("energiaRecarga")]
        [SerializeField] private float _energyRecharge = 25;

        //medidas da cena do jogo: face do col_perigo (x = 9.5) e face interna das paredes (z = 5.5)
        private const float _dangerX = 9.5f;
        private const float _wallZ = 5.5f;

        private Rigidbody _rigidbody;
        private float _side; //-1 = esquerda, 1 = direita

        #endregion

        #region metodos

        public void Configure(Transform _t_arena, Rigidbody newBallBody)
        {
            _arenaTransform = _t_arena;
            _ballRigidbody = newBallBody;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _side = _arenaTransform.InverseTransformPoint(transform.position).x < 0 ? -1 : 1;
        }

        public void ResetEnergy()
        {
            _currentEnergy = _maxEnergy;
        }

        private void FixedUpdate()
        {
            float z = _arenaTransform.InverseTransformPoint(transform.position).z;
            bool moved = false;

            if (_currentEnergy > 5) //verifica a energia
            {
                if (CalculateDanger(out float pos)) //verifica se a bola esta indo em direcao ao gol
                {
                    if (z + 1 < pos) //bola acima da raquete
                    {
                        if (z < 3.5f)
                        {
                            _rigidbody.linearVelocity = Vector3.forward * _moveSpeed;
                            moved = true;
                        }
                    }

                    if (z - 1 > pos) //bola abaixo da raquete
                    {
                        if (z > -3.5f)
                        {
                            _rigidbody.linearVelocity = Vector3.back * _moveSpeed;
                            moved = true;
                        }
                    }
                }
            }

            if (moved)
            {
                _currentEnergy -= _energyConsumption * Time.fixedDeltaTime;
                if (_currentEnergy < 0) _currentEnergy = 0;
            }
            else
            {
                _currentEnergy += _energyRecharge * Time.fixedDeltaTime;
                if (_currentEnergy > _maxEnergy) _currentEnergy = _maxEnergy;

                _rigidbody.linearVelocity = Vector3.zero;
            }
        }

        // Igual aos dois raycasts do BolaPong: reto ate o gol, ou ate a parede se rebater antes.
        private bool CalculateDanger(out float pos)
        {
            pos = 0;

            Vector3 ball = _arenaTransform.InverseTransformPoint(_ballRigidbody.position);
            Vector3 vel = _ballRigidbody.linearVelocity;

            if (vel.x * _side <= 0) return false; //bola parada ou indo para o outro lado

            float timeToGoal = (_dangerX * _side - ball.x) / vel.x;
            if (timeToGoal < 0) return false; //bola ja passou

            float zAtGoal = ball.z + vel.z * timeToGoal;

            if (Mathf.Abs(zAtGoal) <= _wallZ) //chega direto no gol
            {
                pos = zAtGoal;
                return true;
            }

            pos = vel.z > 0 ? _wallZ : -_wallZ; //bate na parede antes

            float timeToWall = (pos - ball.z) / vel.z;
            float zAfter = pos - vel.z * (timeToGoal - timeToWall);

            return Mathf.Abs(zAfter) <= _wallZ; //com duas rebatidas o original nn enxerga o perigo
        }

        #endregion
    }
}
