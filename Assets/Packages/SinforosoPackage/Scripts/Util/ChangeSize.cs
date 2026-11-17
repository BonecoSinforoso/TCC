using UnityEngine;

namespace SinforosoPackage
{
    public class ChangeSize : MonoBehaviour
    {
        [SerializeField] private float _sizeOffset;

        private void Update()
        {
            transform.localScale += _sizeOffset * Time.deltaTime * Vector3.one;
        }
    }
}
