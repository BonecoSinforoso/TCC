using UnityEngine;

namespace SinforosoPackage
{
    public class CannonController : MonoBehaviour
    {
        [SerializeField] private Transform _pipeTransform;
        [SerializeField] private Transform _spawnTransform;
        [SerializeField] private float _shootForce;
        [SerializeField][Range(0, -90)] private float _targetAngle;

        [Header("prefabs")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private GameObject _followerPrefab;

        private void Update()
        {
            _pipeTransform.eulerAngles = new Vector3(_targetAngle, 0, 0);
        }

        [SinforosoButton]
        private void Shoot()
        {
            GameObject shoot = Instantiate(_projectilePrefab, _spawnTransform.position, Quaternion.identity);
            shoot.transform.eulerAngles = _spawnTransform.eulerAngles;
            shoot.GetComponent<Rigidbody>().AddForce(shoot.transform.forward * _shootForce, ForceMode.Impulse);

            TrackingManager.Instance.SpawnFollower(shoot.transform);
        }
    }
}
