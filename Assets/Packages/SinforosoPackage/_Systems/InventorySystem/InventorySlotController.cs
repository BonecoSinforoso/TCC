using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotController : MonoBehaviour
{
    [SerializeField] private Image _img;
    [SerializeField] private TextMeshProUGUI _amountTxt;

    public void Setup(InventorySlot inventorySlot)
    {
        if (inventorySlot != null)
        {
            _img.sprite = inventorySlot.Sprite;
            _amountTxt.text = inventorySlot.Amount.ToString();
            return;
        }

        _img.sprite = null;
        _amountTxt.text = "0";
    }
}
