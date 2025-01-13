using System.Collections.Generic;
using UnityEngine;

public class Samen : Inventory
{
    // Propriété ou méthode spécifique à Samen

    public void AddSeed(string seedName, int quantity)
    {
        AddItem(seedName, quantity);
        Debug.Log($"{quantity} graines de {seedName} ajoutées à l'inventaire.");
    }
}
