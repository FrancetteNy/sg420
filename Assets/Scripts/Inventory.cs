using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    // Utilisation d'un dictionnaire pour stocker les objets par leur type et quantité
    public Dictionary<string, int> Items;

    public Inventory()
    {
        Items = new Dictionary<string, int>();
    }

    // Ajouter un item (comme une graine ou une récolte) à l'inventaire
    public void AddItem(string itemName, int quantity)
    {
        if (Items.ContainsKey(itemName))
        {
            Items[itemName] += quantity;
        }
        else
        {
            Items[itemName] = quantity;
        }
    }

    // Retirer un item de l'inventaire
    public bool RemoveItem(string itemName, int quantity)
    {
        if (Items.ContainsKey(itemName) && Items[itemName] >= quantity)
        {
            Items[itemName] -= quantity;
            if (Items[itemName] == 0)
            {
                Items.Remove(itemName);
            }
            return true;
        }
        return false;
    }

    // Vérifier si un item existe dans l'inventaire
    public bool HasItem(string itemName)
    {
        return Items.ContainsKey(itemName);
    }

    // Afficher l'inventaire (par exemple dans le Debug ou l'UI)
    public void ShowInventory()
    {
        foreach (var item in Items)
        {
            Debug.Log($"{item.Key}: {item.Value}");
        }
    }
}
