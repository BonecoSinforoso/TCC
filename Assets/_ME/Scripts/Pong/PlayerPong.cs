using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class PlayerPong : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed;

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
        [FormerlySerializedAs("mesh_energiaBarra")]
        [SerializeField] private MeshRenderer _energyBarRenderer;
        [FormerlySerializedAs("color")]
        [SerializeField] private Color[] _energyBarColors;

        private bool _locked = false;

        private Rigidbody _rigidbody;

        #endregion

        #region metodos

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            float v = Input.GetAxisRaw("Vertical");
            bool moved = false;

            if (_locked)
            {
                _currentEnergy += _energyRecharge * Time.deltaTime;

                if (_currentEnergy > _maxEnergy)
                {
                    _currentEnergy = _maxEnergy;
                    SetLock(false);
                }
            }
            else
            {
                if (_currentEnergy > _energyConsumption * Time.deltaTime)
                {
                    moved = v != 0;
                    _rigidbody.linearVelocity = v * _moveSpeed * Vector3.forward;
                }
                else
                {
                    _rigidbody.linearVelocity = Vector3.zero;
                    SetLock(true);
                }

                if (moved)
                {
                    _currentEnergy -= _energyConsumption * Time.deltaTime;
                    if (_currentEnergy < 0) _currentEnergy = 0;
                }
                else
                {
                    _currentEnergy += _energyRecharge * Time.deltaTime;
                    if (_currentEnergy > _maxEnergy) _currentEnergy = _maxEnergy;
                }
            }

            SetEnergyBarSize();
        }

        private void SetEnergyBarSize()
        {
            _energyBarTransform.localScale = new Vector3(1, 1, _currentEnergy / _maxEnergy);
        }

        private void SetLock(bool value)
        {
            _locked = value;

            SetEnergyBarColor(value);
        }

        private void SetEnergyBarColor(bool value)
        {
            Material material = _energyBarRenderer.material;
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", value ? _energyBarColors[0] : _energyBarColors[1]);
        }

        #endregion
    }
}
