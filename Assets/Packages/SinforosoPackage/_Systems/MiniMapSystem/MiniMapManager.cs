using UnityEngine;

namespace SinforosoPackage
{
    public class MiniMapManager : MonoBehaviour
    {
        public static MiniMapManager Instance { get; private set; }

        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _targetTransform;

        private void Start()
        {
            Instance = this;
        }
    }
}
