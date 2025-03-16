using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ShopController : MonoBehaviour
{
    private VisualElement _root;
    private VisualElement _tabContent, _shopContainer;
    private GameState _gameState;
    private Dictionary<Button, Action> _onclickedActions;

    public void Initialize(VisualElement root)
    {
        _gameState = GameStateManagerSingleton.Instance.GameState;
        _root = root;

        _root.style.display = DisplayStyle.None;

        _shopContainer = _root.Q<VisualElement>("Shop");
        _tabContent = _root.Q<VisualElement>("content");

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideShop.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _onclickedActions = new Dictionary<Button, Action>();
        LoadShopItems();
        MessageSystem.StartListening(MessageSystemEvent.InventoryUpdated, LoadShopItems);
    }
    private void OnDestroy()
    {
        MessageSystem.StopListening(MessageSystemEvent.InventoryUpdated, LoadShopItems);
        foreach (var (button, action) in _onclickedActions)
        {
            button.clicked -= action;
        }
        _onclickedActions.Clear();
    }
    private void LoadShopItems()
    {
        var shopItems = Resources.LoadAll<ShopItem>("");
        _tabContent.Clear();
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

        var nameText = item.InventoryItem.Name;
        if (item.InventoryItem is Seed seed)
        {
            if (_gameState.AvailableSeedsPerType.TryGetValue(seed, out var amount))
            {
                nameText += $" ({amount})";
            }
            else
            {
                nameText += $" (0)";
            }
        }

        var nameLabel = new Label(nameText);
        nameLabel.AddToClassList("shop-item-name");
        itemElement.Add(nameLabel);

        var priceLabel = new Label($"Preis : {item.Price} €");
        priceLabel.AddToClassList("shop-item-price");
        itemElement.Add(priceLabel);

        var buyButton = new Button();
        buyButton.text = "Kaufen";
        Action onclickAction = () =>
        {
            SoundManagerSingleton.Instance.PlaySound("Click");
            OnBuyButtonClicked(item);
        };
        buyButton.clicked += onclickAction;
        _onclickedActions[buyButton] = onclickAction;
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

            if (item.InventoryItem is Seed seed)
            {
                if (seed.IsFeminized)
                {
                    MessageSystem.FireEvent(MessageSystemEvent.BuyFeminizedSeed);
                }
            }
        }
        else
        {
            UIEvents.AddNotification.Invoke(new NotificationData("Ungenügendes Geld.", $"Sie haben nur {gameState.Money}€, aber {item.InventoryItem.Name} kostet {item.Price}€.", 5));
        }
        GameState.UpdateHUD?.Invoke();
        MessageSystem.FireEvent(MessageSystemEvent.InventoryUpdated);
    }

}

