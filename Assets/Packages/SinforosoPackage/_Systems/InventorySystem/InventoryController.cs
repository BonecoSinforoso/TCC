using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    #region membros

    [SerializeField] private int _capacity;
    [SerializeField] private List<InventorySlot> _inventorySlots;

    [SerializeField] private bool _keep; //mantem itens setados no inspector
    [SerializeField] private bool _autoPopulate; //adiciona itens aleatorios
    [SerializeField] private bool _autoShow;
    [SerializeField] private bool _beingDisplayed;

    private List<Item> _items;

    private List<InventorySlot> InventorySlots
    {
        set
        {
            SetItems();

            if (_beingDisplayed) InventoryManager.Instance.UpdateInventorySlots();
        }
    }

    #endregion

    #region metodos

    private void Start()
    {
        if (!_keep) InventorySlots = new(_capacity);
        else
        {
            for (int i = 0; i < _capacity; i++)
            {
                if (i >= _inventorySlots.Count)
                {
                    _inventorySlots.Add(null);
                }
            }
        }

        if (_autoPopulate) Populate();
        if (_autoShow) Show();

        SetItems();
    }

    private void SetItems()
    {
        _items = new();

        foreach (InventorySlot inventorySlot in _inventorySlots)
        {
            if (inventorySlot != null)
                _items.Add(inventorySlot.Item);
            else _items.Add(null);
        }
    }

    [ContextMenu(nameof(Populate))]
    private void Populate()
    {
        InventorySlots = InventoryManager.Instance.GetRandomInventorySlots();
    }

    [ContextMenu(nameof(Show))]
    private void Show()
    {
        _beingDisplayed = true;
        InventoryManager.Instance.Show(_capacity, _inventorySlots, this);
    }

    public void AddItem(Item item, int amount) //falta verificar se a quant for maior q o stack,,, dividir a quant,,, ter item em suspensao
    {
        //se item ja existe em algum slot e tem espaco
        if (_items.Contains(item))
        {
            int i = 0;

            foreach (InventorySlot inventorySlot in _inventorySlots) //precisa disso pq pode nn estar livre para quant desejada
            {
                if (inventorySlot.Item == item)
                {
                    if (inventorySlot.AddAmount(amount))
                    {
                        Debug.Log("adicionado ao slot: " + i);
                        InventoryManager.Instance.UpdateInventorySlots();
                        return;
                    }
                }

                i++;
            }
        }

        //se tem algum slot livre
        int _freeSlotIndex = FreeSlotIndex();

        if (_freeSlotIndex != -1)
        {
            _inventorySlots[_freeSlotIndex] = new(item, amount);
            InventoryManager.Instance.UpdateInventorySlots();
            return;
        }

        //nao tem slot livre
        Debug.Log("nao tem slot disponivel");
    }

    public int FreeSlotIndex()
    {
        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            if (_inventorySlots[i].IsFree) return i;
        }

        return -1;
    }

    public void Hide() => _beingDisplayed = false;

    #endregion

    [ContextMenu(nameof(Test))]
    private void Test()
    {
        Item item = InventoryManager.Instance.Items[0];

        AddItem(item, Random.Range(0, item.MaxAmount + 1));
    }
}
