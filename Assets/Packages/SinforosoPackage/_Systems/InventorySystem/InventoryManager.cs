using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private Item[] _items;
    [SerializeField] private GameObject[] _inventorySlots;

    [Header("atuais")]
    [SerializeField] private InventoryController _currentInventoryController;
    [SerializeField] private List<InventorySlot> _currentInventorySlots;

    private int _currentCapacity = 0;
    public Item[] Items => _items;

    //metodos
    private void Start()
    {
        Instance = this;
    }

    private void Hide()
    {
        _currentInventoryController.Hide();
        _currentInventoryController = null;
    }

    public void Show(int _capacity, List<InventorySlot> inventorySlots, InventoryController inventoryController)
    {
        _currentCapacity = _capacity;
        _currentInventorySlots = inventorySlots;
        _currentInventoryController = inventoryController;

        UpdateInventorySlots();
    }

    public void UpdateInventorySlots()
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            _inventorySlots[i].SetActive(i < _currentCapacity);

            InventorySlotController inventorySlotController = _inventorySlots[i].GetComponent<InventorySlotController>();

            if (i < _currentInventorySlots.Count && _currentInventorySlots[i].Item != null)
            {
                inventorySlotController.Setup(_currentInventorySlots[i]);
            }
            else
            {
                inventorySlotController.Setup(null);
            }
        }
    }

    public List<InventorySlot> GetRandomInventorySlots()
    {
        List<InventorySlot> inventorySlots = new();

        foreach (Item item in _items)
        {
            InventorySlot inventorySlot = new(item, Random.Range(1, item.MaxAmount + 1));

            inventorySlots.Add(inventorySlot);
        }

        return inventorySlots;
    }
}
