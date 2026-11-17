using UnityEngine;

namespace SinforosoPackage
{
    public class CompassManager : MonoBehaviour
    {
        public static CompassManager Instance { get; private set; }

        private void Start()
        {
            Instance = this;
        }
    }
}
