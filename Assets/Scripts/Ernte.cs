using UnityEngine;

public class Ernte : Inventory
{
    // Propriété ou méthode spécifique à Ernte
    public void Harvest(string cropName, int quantity)
    {
        AddItem(cropName, quantity);
        Debug.Log($"{quantity} unités de récolte de {cropName} ajoutées à l'inventaire.");
    }

    public bool UseHarvest(string cropName, int quantity)
    {
        if (RemoveItem(cropName, quantity))
        {
            Debug.Log($"{quantity} unités de récolte de {cropName} utilisées.");
            return true;
        }
        else
        {
            Debug.Log($"Pas assez de récolte de {cropName} pour utiliser {quantity} unités.");
            return false;
        }
    }
}