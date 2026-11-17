using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class LayerToggle : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("layerIniIndex")]
        [SerializeField] private int _initialLayerIndex;
        [FormerlySerializedAs("layer")]
        [SerializeField] private string[] _layerNames;
        [SerializeField] private GameObject[] _objects;
        [FormerlySerializedAs("layerAtualIndex")]
        [SerializeField] private int _currentLayerIndex;

        #endregion

        #region metodos

        private void Start()
        {
            _currentLayerIndex = _initialLayerIndex;

            foreach (GameObject obj in _objects)
            {
                obj.layer = LayerMask.NameToLayer(_layerNames[_currentLayerIndex]);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleLayer();
            }
        }

        private void ToggleLayer()
        {
            _currentLayerIndex = _currentLayerIndex == 0 ? 1 : 0;

            foreach (GameObject obj in _objects)
            {
                obj.layer = LayerMask.NameToLayer(_layerNames[_currentLayerIndex]);
            }
        }

        #endregion
    }
}
