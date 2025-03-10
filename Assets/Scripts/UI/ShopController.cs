using SG420UILibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UIElements;


public class ShopController : MonoBehaviour
{
    private DetailViewUIManager _uiManager;
    private VisualElement _root;
    private VisualElement _tabContent1, _shopContainer;
    private GameState gameState;

    public void Initialize(VisualElement root)
    {

        _root = root;

        _root.style.display = DisplayStyle.None;

        _shopContainer = _root.Q<VisualElement>("Shop");
        _tabContent1 = _root.Q<VisualElement>("tab-content1");

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideShop.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        LoadShopItems();

    }
    private void LoadShopItems()
    {
       
        var shopItems = new List<ShopItem>
        {
            new ShopItem("Indica", 10),
            new ShopItem("Sativa", 10),
            new ShopItem("Ruderalis", 10)
        };

        
        foreach (var item in shopItems)
        {
            var itemElement = CreateShopItemElement(item);
            _tabContent1.Add(itemElement);
        }
    }

   
    private VisualElement CreateShopItemElement(ShopItem item)
    {
        var itemElement = new VisualElement();
        itemElement.AddToClassList("shop-item");

        var nameLabel = new Label(item.Name);
        nameLabel.AddToClassList("shop-item-name");
        itemElement.Add(nameLabel);

        var priceLabel = new Label($"Preis : {item.Price} €");
        priceLabel.AddToClassList("shop-item-price");
        itemElement.Add(priceLabel);

        var buyButton = new Button();
        buyButton.text = "Kaufen";
        buyButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        buyButton.clicked += () => OnBuyButtonClicked(item);
        buyButton.AddToClassList("shop-item-button");
        itemElement.Add(buyButton);

        return itemElement;
    }

    
    private void OnBuyButtonClicked(ShopItem item)
    {
        var gameState = GameStateManagerSingleton.Instance.GameState;
        if (gameState == null)
        {
            return;
        }

        if (gameState.Geld >= item.Price)
        {
            gameState.Geld -= item.Price;

            AddSeedToInventory(item);

            GameStateManagerSingleton.Instance.Save();

            InventarController.Instance.RefreshInventory();

            UIEvents.AddNotification.Invoke(new NotificationData("Erfolgreicher Einkauf", $"{item.Name}_Samen zum Inventar hinzugefügt.", 5));
        }
        else
        {
            UIEvents.AddNotification.Invoke(new NotificationData("Ungenügendes Geld.", $"Sie haben {gameState.Geld} nur verfügbar.", 5));
        }
    }


    private void AddSeedToInventory(ShopItem item)
    {
        var gameState = GameStateManagerSingleton.Instance.GameState;
        if (gameState == null || gameState.SamenInventar == null || gameState.SamenInventar.List == null)
        {
            return;
        }

        var existingSeed = gameState.SamenInventar.List.Find(seed => seed.Sorte == item.Name);
        if (existingSeed != null)
        {
            existingSeed.Quantity++; 
        }
        
    }
}
public class ShopItem
{
    public string Name { get; }
    public int Price { get; }
   

    public ShopItem(string name, int price)
    {
        Name = name;
        Price = price;
       
    }
}

