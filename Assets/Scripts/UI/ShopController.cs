using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ShopController : MonoBehaviour
{
    private VisualElement _root;
    private VisualElement _tabContent, _shopContainer;

    public void Initialize(VisualElement root)
    {

        _root = root;

        _root.style.display = DisplayStyle.None;

        _shopContainer = _root.Q<VisualElement>("Shop");
        _tabContent = _root.Q<VisualElement>("content");

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideShop.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        LoadShopItems();

    }
    private void LoadShopItems()
    {
        var shopItems = Resources.LoadAll<ShopItem>("");
        
        foreach (var item in shopItems)
        {
            var itemElement = CreateShopItemElement(item);
            _tabContent.Add(itemElement);
        }
    }

   
    private VisualElement CreateShopItemElement(ShopItem item)
    {
        var itemElement = new VisualElement();
        itemElement.AddToClassList("shop-item");

        var nameLabel = new Label(item.InventoryItem.Name);
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

        if (gameState.Money >= item.Price)
        {
            gameState.Money -= item.Price;

            gameState.Inventory.List.Add(item.InventoryItem);

            UIEvents.AddNotification.Invoke(new NotificationData("Erfolgreicher Einkauf", $"{item.InventoryItem.Name} zum Inventar hinzugefügt.", 5));
        }
        else
        {
            UIEvents.AddNotification.Invoke(new NotificationData("Ungenügendes Geld.", $"Sie haben nur {gameState.Money}€, aber {item.InventoryItem.Name} kostet {item.Price}€.", 5));
        }
        GameState.UpdateHUD?.Invoke();
        MessageSystem.FireEvent(MessageSystemEvent.InventoryUpdated);
    }

}

