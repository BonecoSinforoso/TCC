using UnityEngine;

namespace SinforosoPackage
{
    public class FollowerController : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _rotationSpeed = 2f;
        [SerializeField] private float _zoomSpeed = 5f;
        [SerializeField] private float _minDistance = 1f;
        [SerializeField] private float _maxDistance = 10f;

        private float _currentDistance = 5f;
        private bool _isRotating;

        private void Start()
        {
            _targetTransform = transform.parent;
            transform.parent = null;
        }

        private void Update()
        {
            if (_targetTransform == null)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = _targetTransform.position;

            Rotate();
            Zoom();
        }

        private void Rotate()
        {
            if (Input.GetMouseButtonDown(1)) _isRotating = true;
            if (Input.GetMouseButtonUp(1)) _isRotating = false;

            if (_isRotating)
            {
                float h = Input.GetAxis("Mouse X") * _rotationSpeed;
                float v = Input.GetAxis("Mouse Y") * _rotationSpeed;

                transform.RotateAround(_targetTransform.position, Vector3.up, h);
                transform.RotateAround(_targetTransform.position, transform.right, -v);
            }
        }

        private void Zoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            _currentDistance -= scroll * _zoomSpeed;
            _currentDistance = Mathf.Clamp(_currentDistance, _minDistance, _maxDistance);

            _cameraTransform.position = transform.position - transform.forward * _currentDistance;
        }
    }
}
