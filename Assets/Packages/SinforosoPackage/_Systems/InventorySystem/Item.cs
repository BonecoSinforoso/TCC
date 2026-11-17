using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private int _maxAmount;

    public string Name => _name;
    public Sprite Sprite => _sprite;
    public int MaxAmount => _maxAmount;
}
