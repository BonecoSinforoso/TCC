using TMPro;
using UnityEngine;

namespace SinforosoPackage
{
    public class ProductName : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = Application.productName;
            Destroy(this);
        }
    }
}
