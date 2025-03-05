using SG420UILibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GroupWateringController : MonoBehaviour
{
    public static Action PlantsChanged;
    private VisualElement _root;

    private PlantManager _plantManager;
    public List<PlantController> PlantControllers;
    private GroupWateringViewPlantManager _groupWateringViewplantManager;
    private GroupWateringUIManager _uiManager;
    private Action _closeAction;

    private void Start()
    {
        PlantsChanged += UpdatePlantControllers;
    }

    private void UpdatePlantControllers()
    {
        PlantControllers = _plantManager.Plants.Select((plant) => plant.GetComponent<PlantController>()).ToList();
    }

    public void Initialize(VisualElement root, PlantManager plantManager)
    {
        _root = root;
        _plantManager = plantManager;
        UpdatePlantControllers();
        SetupButtons();
        _uiManager = new GroupWateringUIManager(GetComponent<UIDocument>(), OnButtonDown);
        _groupWateringViewplantManager = new GroupWateringViewPlantManager(PlantControllers);
    }

    private void SetupButtons()
    {
        var close_button = _root.Q<Button>("group-watering-close-window-button");
        close_button.clicked += () => UIEvents.HideGroupWateringView.Invoke();
        close_button.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
    }

    public enum GroupWateringButton
    {
        Close,
        WaterPlats,
    }

    private void OnButtonDown(GroupWateringButton buttonid)
    {
        switch (buttonid)
        {
            case GroupWateringButton.Close:
                CloseView();
                break;
            case GroupWateringButton.WaterPlats:
                _groupWateringViewplantManager.AddWater(_uiManager.GetWaterValue());
                _uiManager.UpdatePlantData(PlantControllers);
                CloseView();
                UIEvents.AddNotification.Invoke(new NotificationData("Pflanzen gieﬂen", $"Alle Pflanzen mit Wert {_uiManager.GetWaterValue()} gegossen.", 3));
                break;
            default:
                break;
        }
    }

    public void CloseView()
    {
        _uiManager.HideView();
        _closeAction?.Invoke();
    }

    public class GroupWateringUIManager
    {
        private VisualElement _background;
        private Dictionary<string, Label> _detailLabels = new();

        public GroupWateringUIManager(UIDocument document, Action<GroupWateringButton> onButtonDown)
        {
            _background = document.rootVisualElement.Q<VisualElement>("GroupWatering");
            SetupButtons(onButtonDown);
            SetupSliders();
        }

        private void SetupSliders()
        {
            var waterValueLabel = _background.Q<Label>("water-value-label");
            _background.Q<Slider>("water-slider").RegisterValueChangedCallback(v => waterValueLabel.text = v.newValue.ToString());
        }

        private void SetupButtons(Action<GroupWateringButton> onButtonDown)
        {
            foreach (var (enumValue, buttonName) in GroupWateringViewConstants.NameOfButtons)
            {
                var button = _background.Q<Button>(buttonName);
                if (button != null)
                {
                    button.RegisterCallback<PointerDownEvent>((_) => onButtonDown(enumValue), TrickleDown.TrickleDown);
                    button.RegisterCallback<PointerDownEvent>((_) => SoundManagerSingleton.Instance.PlaySound("Click"), TrickleDown.TrickleDown);
                }
            }
        }

        public void UpdatePlantData(List<PlantController> plants)
        {
            if (plants == null || plants.Count == 0)
                return;

            foreach (var plant in plants)
            {
                foreach (var (name, label) in _detailLabels)
                {
                    label.text = plant.PlantData.DataDictionary()[name]?.ToString() ?? string.Empty;
                }
            }
        }

        public void HideView()
        {
            UIEvents.HideGroupWateringView.Invoke();
        }

        internal float GetWaterValue()
        {
            return _background.Q<Slider>("water-slider").value;
        }
    }

    public static class GroupWateringViewConstants
    {
        public const float DefaultWaterValue = 10f;

        public static Dictionary<GroupWateringButton, string> NameOfButtons = new Dictionary<GroupWateringButton, string>() {
        {GroupWateringButton.Close,"close-watering-submenu-button"  },
        {GroupWateringButton.WaterPlats,"group-watering-button"},
        };
    }

    public class GroupWateringViewPlantManager
    {
        private List<PlantController> _plants;

        public GroupWateringViewPlantManager(List<PlantController> plants)
        {
            _plants = plants;
        }

        public void AddWater(float waterAmount)
        {
            if (_plants == null || _plants.Count == 0)
                return;

            foreach (var plant in _plants)
            {
                plant.PlantData.Soil.StoredWater += waterAmount;
            }
        }

    }
}
