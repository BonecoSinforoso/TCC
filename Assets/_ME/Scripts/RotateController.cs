using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    public class RotateController : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("valor")]
        [SerializeField] private Vector3 _rotationSpeed;

        #endregion

        #region metodos

        private void Update()
        {
            transform.Rotate(_rotationSpeed * Time.deltaTime);
        }

        #endregion
    }
}
