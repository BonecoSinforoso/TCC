using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class Borrao : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("rect_borrao")]
        [SerializeField] private RectTransform _blurRectTransform;
        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed;
        private bool _descending = false;

        #endregion

        #region metodos

        private void Update()
        {
            if (_descending)
            {
                if (_blurRectTransform.offsetMax.y > -1080) _blurRectTransform.offsetMax += _moveSpeed * Time.deltaTime * Vector2.down;
            }
            else
            {
                if (_blurRectTransform.offsetMax.y < 0) _blurRectTransform.offsetMax += _moveSpeed * Time.deltaTime * Vector2.up;
            }
        }

        private void OnMouseEnter()
        {
            _descending = true;
        }

        private void OnMouseExit()
        {
            _descending = false;
        }

        #endregion
    }
}
