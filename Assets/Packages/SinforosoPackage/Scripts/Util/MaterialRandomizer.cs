using UnityEngine;

namespace SinforosoPackage
{
    public class MaterialRandomizer : MonoBehaviour
    {
        [SerializeField] private Renderer _rend;
        [SerializeField] private Material[] _materials;

        private void Start()
        {
            _rend.material = _materials[Random.Range(0, _materials.Length)];

            Destroy(this);
        }
    }
}
