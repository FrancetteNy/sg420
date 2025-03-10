using SG420UILibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UIElements;

public class InventarController : MonoBehaviour
{
    public static InventarController Instance { get; private set; }
    private VisualElement _root;
    private Button _tab1, _tab2, _tab3;
    private VisualElement _content1, _content2, _content3;
    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(VisualElement root)
    {
        _root = root;
        _root.style.display = DisplayStyle.None;

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideInventar.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        _tab1 = _root.Q<Button>("tab1");
        _tab2 = _root.Q<Button>("tab2");
        _tab3 = _root.Q<Button>("tab3");

        _content1 = _root.Q<VisualElement>("tab-content1");
        _content2 = _root.Q<VisualElement>("tab-content2");
        _content3 = _root.Q<VisualElement>("tab-content3");

        _tab1.clicked += () => ShowTab(_tab1, _content1);
        _tab1.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _tab2.clicked += () => ShowTab(_tab2, _content2);
        _tab2.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _tab3.clicked += () => ShowTab(_tab3, _content3);
        _tab3.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        ShowTab(_tab1, _content1);

    }

    void ShowTab(Button selectedTab, VisualElement activeTab)
    {

        _content1.style.display = DisplayStyle.None;
        _content2.style.display = DisplayStyle.None;
        _content3.style.display = DisplayStyle.None;

        _tab1.RemoveFromClassList("active");
        _tab2.RemoveFromClassList("active");
        _tab3.RemoveFromClassList("active");

        activeTab.Clear();

        if (activeTab == _content1)

        {
            var gameState = GameStateManagerSingleton.Instance.GameState;

            if (gameState == null || gameState.SamenInventar == null || gameState.SamenInventar.List == null)
            {
                return;
            }

            foreach (var item in gameState.SamenInventar.List)
            {
                var itemElement = CreateInventoryItemElement(item);
                activeTab.Add(itemElement);
            }
        }


        selectedTab.AddToClassList("active");
        activeTab.style.display = DisplayStyle.Flex;
    }

    private VisualElement CreateInventoryItemElement(Samen item)
    {

        var itemElement = new VisualElement();
        itemElement.AddToClassList("inventory-item");


        var nameLabel = new Label(item.Name);
        nameLabel.AddToClassList("item-name");
        itemElement.Add(nameLabel);

        var detailsLabel = new Label($"Sorte: {item.Sorte}");
        detailsLabel.AddToClassList("item-details");
        itemElement.Add(detailsLabel);

        var quantityLabel = new Label($"Menge: {item.Quantity}");
        quantityLabel.name = "item-quantity";
        quantityLabel.AddToClassList("item-quantity");
        itemElement.Add(quantityLabel);


        return itemElement;
    }


    public bool UpdateSeedQuantity(string seedName)
    {
        var gameState = GameStateManagerSingleton.Instance.GameState;
        if (gameState == null || gameState.SamenInventar == null || gameState.SamenInventar.List == null)
        {
            return false;
        }

        var seedItem = gameState.SamenInventar.List.Find(item => item.Sorte == seedName);
        if (seedItem == null)
        {
            return false;
        }

        if (seedItem.Quantity > 0)
        {
            seedItem.Quantity--;

            UpdateSeedQuantityInUI(seedItem);

            GameStateManagerSingleton.Instance.Save();

            RefreshInventory();
            return true;
        }
        else
        {
            return false;
        }
        
    }

    private void UpdateSeedQuantityInUI(Samen seedItem)
    {
        var itemElement = _content1.Q<VisualElement>(seedItem.Sorte);
        if (itemElement != null)
        {
            var quantityLabel = itemElement.Q<Label>("item-quantity");
            quantityLabel.text = $"Menge: {seedItem.Quantity}";
        }
    }


    public void RefreshInventory()
    {
        _content1.Clear();
        var gameState = GameStateManagerSingleton.Instance.GameState;
        if (gameState == null || gameState.SamenInventar == null || gameState.SamenInventar.List == null)
        {
            return;
        }

        foreach (var item in gameState.SamenInventar.List)
        {
            var itemElement = CreateInventoryItemElement(item);
            _content1.Add(itemElement);
        }
    }
}