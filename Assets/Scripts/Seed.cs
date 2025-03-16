using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SeedItem", menuName = "Inventory/Seed")]
public class Seed : InventoryItem
{
    public Strain Type;
    public bool IsFeminized;
}
