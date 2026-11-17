using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class IaPong : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed;
        [FormerlySerializedAs("t_bola")]
        [SerializeField] private Transform _ballTransform;
        [FormerlySerializedAs("rb_bola")]
        [SerializeField] private Rigidbody _ballRigidbody;

        [FormerlySerializedAs("energiaMax")]
        [SerializeField] private float _maxEnergy;
        [FormerlySerializedAs("energiaAtual")]
        [SerializeField] private float _currentEnergy;
        [FormerlySerializedAs("energiaConsumo")]
        [SerializeField] private float _energyConsumption;
        [FormerlySerializedAs("energiaRecarga")]
        [SerializeField] private float _energyRecharge;
        [FormerlySerializedAs("t_energiaBarra")]
        [SerializeField] private Transform _energyBarTransform;

        private Rigidbody _rigidbody;

        #endregion

        #region metodos

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            bool moved = false;

            if (_currentEnergy > 5) //verifica a energia
            {
                if (BolaPong.Danger) //verifica se a bola oferece perigo (no caso se est� indo em direcao ao gol)
                {
                    if (transform.position.z + 1 < BolaPong.DangerPos) //verifica se a bola est� acima da raquete
                    {
                        if (transform.position.z < 3.5f)
                        {
                            _rigidbody.linearVelocity = Vector3.forward * _moveSpeed;
                            moved = true;
                        }
                    }

                    if (transform.position.z - 1 > BolaPong.DangerPos) //verifica se a bola est� abaixo da raquete
                    {
                        if (transform.position.z > -3.5f)
                        {
                            _rigidbody.linearVelocity = Vector3.back * _moveSpeed;
                            moved = true;
                        }
                    }
                }
            }

            if (moved) //se andou diminui a energia
            {
                _currentEnergy -= _energyConsumption * Time.deltaTime;
                if (_currentEnergy < 0) _currentEnergy = 0;
            }
            else //se nn andou, aumenta a energia e seta a velocidade da raquete como 0
            {
                _currentEnergy += _energyRecharge * Time.deltaTime;
                if (_currentEnergy > _maxEnergy) _currentEnergy = _maxEnergy;

                _rigidbody.linearVelocity = Vector3.zero;
            }

            SetEnergyBarSize();
        }

        private void SetEnergyBarSize()
        {
            _energyBarTransform.localScale = new Vector3(1, 1, _currentEnergy / _maxEnergy);
        }

        #endregion
    }
}
