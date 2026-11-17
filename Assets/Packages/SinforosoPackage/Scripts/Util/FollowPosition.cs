using UnityEngine;

namespace SinforosoPackage
{
    public class FollowPosition : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;

        private void Update()
        {
            if (_targetTransform == null) return;

            transform.position = _targetTransform.position;
        }

        public void SetTargetPositionTransform(Transform targetTransform) => _targetTransform = targetTransform;
    }
}
