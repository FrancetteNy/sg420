using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/ShopItem")]
public class ShopItem : ScriptableObject
{
    public InventoryItem InventoryItem;
    public int Price;
}

