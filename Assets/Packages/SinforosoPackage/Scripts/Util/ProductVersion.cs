using TMPro;
using UnityEngine;

namespace SinforosoPackage
{
    public class ProductVersion : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = Application.version;
            Destroy(this);
        }
    }
}
