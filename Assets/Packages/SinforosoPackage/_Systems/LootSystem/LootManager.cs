using UnityEngine;

namespace SinforosoPackage
{
    public class LootManager : MonoBehaviour
    {
        public static LootManager Instance { get; private set; }

        [SerializeField] private LootLocation[] _lootLocations;

        private void Start()
        {
            Instance = this;
        }
    }
}
