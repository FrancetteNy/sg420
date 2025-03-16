using System;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryController : MonoBehaviour
{
    private VisualElement _root;
    private Button _seedInventoryButton, _harvestedPlantsInventoryButton, _driedPlantsInventoryButton;
    private VisualElement _inventoryContent;
    
    private GameState _gameState;

    private InventoryTab _currentTab = InventoryTab.Seeds;

    public void Initialize(VisualElement root)
    {
        _gameState = GameStateManagerSingleton.Instance.GameState;

        _root = root;
        _root.style.display = DisplayStyle.None;

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideInventar.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        _seedInventoryButton = _root.Q<Button>("seed-inventory-button");
        _harvestedPlantsInventoryButton = _root.Q<Button>("harvested-plants-inventory-button");
        _driedPlantsInventoryButton = _root.Q<Button>("dried-plants-inventory-button");

        _inventoryContent = _root.Q<VisualElement>("scroll-content");

        _seedInventoryButton.clicked += OnSeedInventoryButtonClicked;
        _harvestedPlantsInventoryButton.clicked += OnHarvestedPlantsInventoryButtonClicked;
        _driedPlantsInventoryButton.clicked += OnDriedPlantsInventoryButtonClicked;

        RefreshInventory();
        MessageSystem.StartListening(MessageSystemEvent.InventoryUpdated, RefreshInventory);
    }

    private void OnDestroy()
    {
        MessageSystem.StopListening(MessageSystemEvent.InventoryUpdated, RefreshInventory);
    }

    private void OnSeedInventoryButtonClicked()
    {
        _currentTab = InventoryTab.Seeds;
        InventoryButtonClicked();
    }

    private void OnHarvestedPlantsInventoryButtonClicked()
    {
        _currentTab = InventoryTab.HarvestedPlants;
        InventoryButtonClicked();
    }

    private void OnDriedPlantsInventoryButtonClicked()
    {
        _currentTab = InventoryTab.DriedPlants;
        InventoryButtonClicked();
    }

    private void InventoryButtonClicked()
    {
        RefreshInventory();
        SoundManagerSingleton.Instance.PlaySound("Click");

    }

    public void RefreshInventory()
    {
        _seedInventoryButton.EnableInClassList("active", _currentTab == InventoryTab.Seeds);
        _harvestedPlantsInventoryButton.EnableInClassList("active", _currentTab == InventoryTab.HarvestedPlants);
        _driedPlantsInventoryButton.EnableInClassList("active", _currentTab == InventoryTab.DriedPlants);

        _inventoryContent.Clear();
        switch (_currentTab)
        {
            case InventoryTab.Seeds:
                foreach (var (seed, amount) in _gameState.AvailableSeedsPerType)
                {
                    var seedElement = CreateSeedElement(seed, amount);
                    _inventoryContent.Add(seedElement);
                }
                break;
            case InventoryTab.HarvestedPlants:
                foreach (var plant in _gameState.HarvestedPlantDataList.List)
                {
                    var plantElement = CreatePlantElement(plant);
                    _inventoryContent.Add(plantElement);
                }
                break;
            case InventoryTab.DriedPlants:
                foreach (var plant in _gameState.CompletedDriedPlantDataList.List)
                {
                    var plantElement = CreatePlantElement(plant.OldPlantData);
                    _inventoryContent.Add(plantElement);
                }
                break;
            default:
                break;
        }
    }

    private VisualElement CreatePlantElement(PlantData plant)
    {
        var itemElement = new VisualElement();
        itemElement.AddToClassList("inventory-item");


        var nameLabel = new Label(plant.Strain.ToString());
        nameLabel.AddToClassList("item-name");
        itemElement.Add(nameLabel);

        var amountLabel = new Label($"Qualität: {plant.Quality}");
        itemElement.Add(amountLabel);

        return itemElement;
    }

    private VisualElement CreateSeedElement(Seed seed, int amount)
    {
        var itemElement = new VisualElement();
        itemElement.AddToClassList("inventory-item");


        var nameLabel = new Label(seed.Name);
        nameLabel.AddToClassList("item-name");
        itemElement.Add(nameLabel);

        var amountLabel = new Label($"Menge: {amount}");
        itemElement.Add(amountLabel);

        return itemElement;
    }
}

public enum InventoryTab
{
    Seeds,
    HarvestedPlants,
    DriedPlants
}
