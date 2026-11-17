using UnityEngine;

namespace SinforosoPackage
{
    public class InitialVariator : MonoBehaviour
    {
        [SerializeField] private float _position;
        [SerializeField] private float _size;
        [SerializeField] private float _rotX;
        [SerializeField] private float _rotY;
        [SerializeField] private float _rotZ;

        [SerializeField] private Transform _moveTransform;

        private void Start()
        {
            _moveTransform.eulerAngles = Vector3.up * Random.Range(0, 360);

            transform.position += _moveTransform.forward * Random.Range(-_position, _position);
            transform.localScale += Vector3.one * Random.Range(-_size, _size);
            transform.eulerAngles = new Vector3(Random.Range(-_rotX, _rotX), Random.Range(-_rotY, _rotY), Random.Range(-_rotZ, _rotZ));

            Destroy(this);
        }
    }
}
