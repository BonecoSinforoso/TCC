using UnityEngine;

namespace SinforosoPackage
{
    public class TrackingManager : MonoBehaviour
    {
        public static TrackingManager Instance { get; private set; }

        [SerializeField] private GameObject _followerPrefab;

        private GameObject _lastFollower = null;

        private void Start()
        {
            Instance = this;
        }

        public void SpawnFollower(Transform transform)
        {
            if (_lastFollower != null) Destroy(_lastFollower);

            GameObject follower = Instantiate(_followerPrefab, Vector3.zero, Quaternion.identity);
            follower.transform.parent = transform;

            _lastFollower = follower;
        }
    }
}
