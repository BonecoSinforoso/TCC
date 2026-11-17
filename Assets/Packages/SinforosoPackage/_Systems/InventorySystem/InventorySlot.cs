using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    #region membros

    #region serializadas

    [SerializeField] private Item _item;
    [SerializeField] private int _amount;

    #endregion

    #region propriedades

    public Item Item => _item;
    public int Amount => _amount;
    public bool IsFree => _item == null;

    //item
    public string Name => _item.Name;
    public Sprite Sprite => _item.Sprite;
    public int MaxAmount => _item.MaxAmount;

    #endregion

    #endregion

    //construtor
    public InventorySlot(Item item, int amount)
    {
        _item = item;
        _amount = amount;
    }

    public bool AddAmount(int amount)
    {
        if (_amount + amount <= MaxAmount)
        {
            _amount += amount;
            return true;
        }

        return false;
    }
}
