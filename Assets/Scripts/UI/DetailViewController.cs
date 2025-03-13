using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static Age;

public class DetailViewController : MonoBehaviour
{
    // Events
    public static Action PlantsChanged;

    // Camera and Managers
    private List<PlantController> _plantControllers;
    private Camera _detailViewCamera;
    private DetailViewCameraController _cameraController;
    private DetailViewUIManager _uiManager;
    private DetailViewPlantManager _detailViewplantManager;
    private Action _closeAction;
    private Vector3 _cameraMoveVector;
    private Vector3 _rotateVector;
    private bool _isInitialized = false;
    private PlantManager _plantManager;





    private void Start()
    {
        PlantsChanged += UpdatePlantControllers;
    }


    private void UpdatePlantControllers()
    {
        _plantControllers = _plantManager.Plants.Select((plant) => plant.GetComponent<PlantController>()).ToList();
    }

    public void Initialize(Camera detailViewCamera, PlantManager plantManager)
    {
        _detailViewCamera = detailViewCamera;
        _plantManager = plantManager;
        UpdatePlantControllers();
        _cameraController = new DetailViewCameraController(detailViewCamera, GetComponent<UIDocument>().rootVisualElement.Q<Image>("plant-view"));
        _detailViewplantManager = new DetailViewPlantManager(_plantControllers, OnPlantChanged);
        _uiManager = new DetailViewUIManager(GetComponent<UIDocument>(), OnButtonDown, OnButtonUp, OnDetailHovered);
        _isInitialized = true;
        this.enabled = false;
    }
    private void Update()
    {
        if (!_isInitialized)
        {
            return;
        }
        if (_detailViewplantManager.CurrentPlantIndex < 0)
        {
            return;
        }

        // Update Camera
        _cameraController.UpdatePosition(_cameraMoveVector,
            _detailViewplantManager.GetCurrentPlantClampMin(),
            _detailViewplantManager.GetCurrentPlantClampMax(),
            DetailViewConstants.CameraLerpSpeed);

        // Update Plant Rotation
        _detailViewplantManager.UpdateRotations(_rotateVector);
    }

    private void OnDisable()
    {
        _detailViewplantManager.ResetCurrentPlantTransform();
        _detailViewplantManager.ResetAllPlants();
    }
    private void OnButtonDown(UIButton buttonId)
    {
        switch (buttonId)
        {
            case UIButton.PREVIOUSPLANT:
                _detailViewplantManager.SwitchToPreviousPlant();
                _cameraController.SetInitialPosition(_detailViewplantManager.GetCurrentPlantPosition());
                _uiManager.UpdatePlantData(_detailViewplantManager.GetCurrentPlantDataAsDictionary());
                break;
            case UIButton.NEXTPLANT:
                _detailViewplantManager.SwitchToNextPlant();
                _cameraController.SetInitialPosition(_detailViewplantManager.GetCurrentPlantPosition());
                _uiManager.UpdatePlantData(_detailViewplantManager.GetCurrentPlantDataAsDictionary());
                break;
            case UIButton.STARTSEXING:
                Debug.Log("Starting the sexing minigame...");
                break;
            case UIButton.CLOSEVIEW:
                CloseView();
                break;
            case UIButton.ZOOMIN:
                _cameraMoveVector = Vector3.forward;
                break;
            case UIButton.ZOOMOUT:
                _cameraMoveVector = Vector3.back;
                break;
            case UIButton.MOVERIGHT:
                _cameraMoveVector = Vector3.left;
                break;
            case UIButton.MOVELEFT:
                _cameraMoveVector = Vector3.right;
                break;
            case UIButton.MOVEUP:
                _cameraMoveVector = Vector3.down;
                break;
            case UIButton.MOVEDOWN:
                _cameraMoveVector = Vector3.up;
                break;
            case UIButton.ROTATERIGHT:
                _rotateVector = Vector3.down;
                break;
            case UIButton.ROTATELEFT:
                _rotateVector = Vector3.up;
                break;
            case UIButton.ROTATEUP:
                _rotateVector = Vector3.left;
                break;
            case UIButton.ROTATEDOWN:
                _rotateVector = Vector3.right;
                break;
            case UIButton.OPENWATERINGSUBMENU:
                _uiManager.ShowSubmenu(Submenu.WATERINGSUBMENU);
                break;
            case UIButton.CLOSEWATERINGSUBMENU:
                _uiManager.CloseCurrentSubmenu();
                break;
            case UIButton.WATERPLANTS:
                _detailViewplantManager.AddWaterAndFertilizer(_uiManager.GetWaterValue(), _uiManager.GetFertilizerValue());
                _uiManager.UpdatePlantData(_detailViewplantManager.GetCurrentPlantDataAsDictionary());
                _uiManager.CloseCurrentSubmenu();
                break;
            case UIButton.OPENCHANGEPOTSUBMENU:
                _uiManager.ShowSubmenu(Submenu.CHANGEPOTSUBMENU);
                break;
            case UIButton.CLOSECHANGEPOTSUBMENU:
                _uiManager.CloseCurrentSubmenu();
                break;
            case UIButton.CHANGETOCULTIVATION:
            case UIButton.CHANGETOSMALL:
            case UIButton.CHANGETOMEDIUM:
            case UIButton.CHANGETOLARGE:
                _detailViewplantManager.ChangePotSize(buttonId);
                _uiManager.UpdatePlantData(_detailViewplantManager.GetCurrentPlantDataAsDictionary());
                _uiManager.CloseCurrentSubmenu();
                break;
            case UIButton.CONFIRMSEED:
                bool isPlantedSuccessfully = _detailViewplantManager.PlantSeedInCurrentPot(_uiManager.GetSeedValue());
                if (isPlantedSuccessfully)
                {
                    _uiManager.UpdatePlantData(_detailViewplantManager.GetCurrentPlantDataAsDictionary());
                    UIEvents.AddNotification.Invoke(new NotificationData("Erfolgreiche Pflanzung", $"Der Samen {_uiManager.GetSeedValue()} wurde erfolgreich gepflanzt.", 5));
                } 
                else
                {
                    UIEvents.AddNotification.Invoke(new NotificationData("Fehler bei der Pflanzung", $"Verifizieren Sie die Mengen des Samens {_uiManager.GetSeedValue()}.", 5));
                }
                
                _detailViewplantManager.PlantSeedInCurrentPot(_uiManager.GetSeedValue());
                _uiManager.UpdatePlantData(_detailViewplantManager.GetCurrentPlantDataAsDictionary());
                break;
            case UIButton.HARVEST:
                UIEvents.ShowModalView?.Invoke(
                    "Warnung",
                    "Du bist dabei diese Pflanze zu ernten. Die Ernte kann nicht rückgängig gemacht werden! Bist du dir sicher?",
                    HarvestPlant);
                break;
            default:
                Debug.Log("Button without associated action pressed");
                break;
        }
    }

    private void HarvestPlant()
    {
        var currentPlantData = _detailViewplantManager.GetCurrentPlantData();

        int scoreToAdd = DetailViewConstants.ScorePerGrowthStage[currentPlantData.Age.Stage];
        GameStateManagerSingleton.Instance.GameState.CurrentScore += scoreToAdd;

        string notificationTitle = $"{currentPlantData.Strain} geerntet";
        string notificationBody = $"Das hat dir {scoreToAdd} Punkte gegeben. Du hast jetzt {GameStateManagerSingleton.Instance.GameState.CurrentScore} Punkte.";
        UIEvents.AddNotification(new NotificationData(notificationTitle, notificationBody, 3));

        _detailViewplantManager.HarvestCurrentPlant();
        _uiManager.UpdatePlantData(_detailViewplantManager.GetCurrentPlantDataAsDictionary());
        _plantManager.ManagePlantStageModel(_detailViewplantManager.GetCurrentPlantController().gameObject);
        GameState.UpdateHUD?.Invoke();

        PlantGenerator plantGenerator = _detailViewplantManager.GetCurrentPlantController().gameObject.GetComponentInChildren<PlantGenerator>();
        plantGenerator.GenerateCannabisPlant();
    }

    private void OnButtonUp(UIButton buttonId)
    {
        switch (buttonId)
        {
            case UIButton.ZOOMIN:
            case UIButton.ZOOMOUT:
            case UIButton.MOVERIGHT:
            case UIButton.MOVELEFT:
            case UIButton.MOVEUP:
            case UIButton.MOVEDOWN:
                _cameraMoveVector = Vector3.zero;
                break;
            case UIButton.ROTATERIGHT:
            case UIButton.ROTATELEFT:
            case UIButton.ROTATEUP:
            case UIButton.ROTATEDOWN:
                _rotateVector = Vector3.zero;
                break;
            default:
                break;
        }
    }
    private void OnPlantChanged(int currentIndex)
    {
        _uiManager.UpdatePlantNavigationButtons(
           currentIndex > 0,
           currentIndex < _detailViewplantManager.Plantcount - 1
       );

    }

    public void ActivateView(int plantControllerIndex, Action closeAction)
    {
        _detailViewplantManager.SetCurrentPlant(plantControllerIndex);
        
        foreach (var plant in _plantControllers)
        {
            plant.GetComponentInChildren<PlantGenerator>().SetLayerMask();
        }
        
        _cameraController.SetInitialPosition(_detailViewplantManager.GetCurrentPlantPosition());
        _uiManager.UpdatePlantData(_detailViewplantManager.GetCurrentPlantDataAsDictionary());

        _uiManager.ShowView();
        _detailViewCamera.enabled = true;

        _closeAction = closeAction;

        if (!GameStateManagerSingleton.Instance.GameState.OnboardingDoneData.DetailviewOnboardingIsDone)
        {
            GameStateManagerSingleton.Instance.GameState.OnboardingDoneData.DetailviewOnboardingIsDone = true;
            _uiManager.StartOnboarding();
        }
    }




    private void OnDetailHovered(string detailName)
    {
        string wikiEntry = DummyWiki.GetWikiEntry(detailName);
        _uiManager.UpdateWikiText(wikiEntry);
    }

    public void CloseView()
    {
        _uiManager.HideView();
        _detailViewCamera.enabled = false;
        _detailViewplantManager.ResetCurrentPlantTransform();

        _closeAction?.Invoke();
    }

}

public enum UIButton
{
    PREVIOUSPLANT,
    NEXTPLANT,
    STARTSEXING,
    CLOSEVIEW,
    ZOOMIN,
    ZOOMOUT,
    MOVERIGHT,
    MOVELEFT,
    MOVEUP,
    MOVEDOWN,
    ROTATERIGHT,
    ROTATELEFT,
    ROTATEUP,
    ROTATEDOWN,
    OPENWATERINGSUBMENU,
    CLOSEWATERINGSUBMENU,
    WATERPLANTS,
    OPENCHANGEPOTSUBMENU,
    CLOSECHANGEPOTSUBMENU,
    CHANGETOCULTIVATION,
    CHANGETOSMALL,
    CHANGETOMEDIUM,
    CHANGETOLARGE,
    CONFIRMSEED,
    HARVEST,

}
// Constants for readability and configurability
public static class DetailViewConstants
{
    public const float CameraClampOffset = 0.3f;
    public const float CameraLerpSpeed = 5f;
    public const float CameraZoomFactor = 2f;
    public const float RotationSpeedFactor = 0.5f;
    public const float DefaultWaterValue = 10f;
    public const float DefaultFertilizerValue = 0f;
    public const float MaximumSlowdown = 0.3f;

    public const float XInitialCameraToPlantOffset = 0.0f;
    public const float YInitialCameraToPlantOffset = 0.75f;
    public const float ZInitialCameraToPlantOffset = -5.0f;
    public static Dictionary<UIButton, string> NameOfButtons = new Dictionary<UIButton, string>() {
        {UIButton.PREVIOUSPLANT, "previous-plant-button" },
        {UIButton.NEXTPLANT, "next-plant-button"},
        {UIButton.STARTSEXING, "start-sexing-button"},
        {UIButton.CLOSEVIEW, "close-button"},
        {UIButton.ZOOMIN, "zoom-in-button"},
        {UIButton.ZOOMOUT, "zoom-out-button"},
        {UIButton.MOVERIGHT, "move-right-button"},
        {UIButton.MOVELEFT,"move-left-button" },
        {UIButton.MOVEUP, "move-up-button"},
        {UIButton.MOVEDOWN, "move-down-button"},
        {UIButton.ROTATERIGHT, "rotate-right-button"},
        {UIButton.ROTATELEFT, "rotate-left-button"},
        {UIButton.ROTATEUP, "rotate-up-button"},
        {UIButton.ROTATEDOWN, "rotate-down-button"},
        { UIButton.OPENWATERINGSUBMENU,"open-watering-submenu-button" },
        { UIButton.CLOSEWATERINGSUBMENU,"close-watering-submenu-button"  },
        { UIButton.WATERPLANTS,"water-plant-button"  },
        { UIButton.OPENCHANGEPOTSUBMENU,"open-change-pot-submenu-button" },
        { UIButton.CLOSECHANGEPOTSUBMENU,"close-change-pot-submenu-button" },
        { UIButton.CHANGETOCULTIVATION,"change-to-cultivation-pot-button" },
        { UIButton.CHANGETOSMALL,"change-to-small-pot-button" },
        { UIButton.CHANGETOMEDIUM,"change-to-medium-pot-button" },
        { UIButton.CHANGETOLARGE,"change-to-large-pot-button" },
        { UIButton.CONFIRMSEED, "confirm-seed-button" },
        { UIButton.HARVEST, "harvest-button" },

    };
    public static Dictionary<Submenu, string> NameOfSubmenues = new Dictionary<Submenu, string>() {
        {Submenu.WATERINGSUBMENU, "watering-submenu" },
        {Submenu.CHANGEPOTSUBMENU, "change-pot-submenu"},
    };
    public static Dictionary<GrowthStage, int> ScorePerGrowthStage = new Dictionary<GrowthStage, int> {
            {GrowthStage.EMPTY, 0 },
            {GrowthStage.GERMINATION, 2 },
            {GrowthStage.SEEDLING, 4 },
            {GrowthStage.VEGETATIVEGROWTH, 6 },
            {GrowthStage.FLOWERING, 10 },
            {GrowthStage.FADED, 3 },
        };
}

public class DetailViewCameraController
{
    private Camera _camera;
    private Vector3 _futurePosition;
    private Vector3 _initialPosition;

    public DetailViewCameraController(Camera camera, Image imageToRenderCameraTo)
    {
        _camera = camera;
        _futurePosition = _camera.transform.position;
        var cameraView = new RenderTexture(Screen.height, Screen.height, 24);
        imageToRenderCameraTo.image = cameraView;
        _camera.targetTexture = cameraView;
    }

    public void UpdatePosition(Vector3 moveVector, Vector3 clampMin, Vector3 clampMax, float lerpSpeed)
    {
        var slowDown = Mathf.Abs(_initialPosition.z / _camera.transform.position.z);

        var x = moveVector.x * Mathf.Clamp(slowDown, DetailViewConstants.MaximumSlowdown, 1.0f);
        var y = moveVector.y * Mathf.Clamp(slowDown, DetailViewConstants.MaximumSlowdown, 1.0f);
        var z = moveVector.z;

        _futurePosition += new Vector3(x, y, z) * Time.deltaTime;
        _futurePosition = new Vector3(
            Mathf.Clamp(_futurePosition.x, clampMin.x, clampMax.x),
            Mathf.Clamp(_futurePosition.y, clampMin.y, clampMax.y),
            Mathf.Clamp(_futurePosition.z, clampMin.z, clampMax.z)
        );

        _camera.transform.position = Vector3.Lerp(_camera.transform.position, _futurePosition, Time.deltaTime * lerpSpeed);
    }

    public void SetInitialPosition(Vector3 plantPosition)
    {
        _futurePosition = plantPosition + new Vector3(DetailViewConstants.XInitialCameraToPlantOffset, DetailViewConstants.YInitialCameraToPlantOffset, DetailViewConstants.ZInitialCameraToPlantOffset);
        _initialPosition = _futurePosition;
    }

    public void Disable()
    {
        _camera.enabled = false;
    }
}

public enum Submenu
{
    WATERINGSUBMENU,
    CHANGEPOTSUBMENU,
}

public class DetailViewUIManager
{
    private VisualElement _background;
    private Label _wikiTextLabel;
    private Dictionary<string, Label> _detailLabels = new();
    private Button _previousPlantButton;
    private Button _nextPlantButton;
    private VisualElement _seedSelectionContainer;
    private VisualElement _plantInfo;
    private DropdownField _seedDropdown;
    public InventarController InventoryController; 


    public DetailViewUIManager(UIDocument document, Action<UIButton> onButtonDown, Action<UIButton> onButtonUp, Action<string> onDetailHovered)
    {
        _background = document.rootVisualElement.Q<VisualElement>("background");

        // Configure Buttons
        SetupButtons(onButtonDown, onButtonUp);
        SetupDropDowns();

        // Collect and Configure Detail Labels
        _wikiTextLabel = _background.Q<Label>("wiki-text");
        SetupDetailLabels(onDetailHovered);

        _plantInfo = _background.Q<VisualElement>("plantinfo");

        SetupSeedContainer(onDetailHovered);

        SetupSliders();
    }


    private void SetupSeedContainer(Action<string> onDetailHovered)
    {
        _seedSelectionContainer = _background.Q<VisualElement>("seed-selection-container");
        _seedSelectionContainer.RegisterCallback<MouseEnterEvent>((_) => onDetailHovered("Die Aussaat"));
        _seedSelectionContainer.style.display = DisplayStyle.None;
    }
    private void SetupSliders()
    {
        var waterValueLabel = _background.Q<Label>("water-value-label");
        var fertilizerValueLabel = _background.Q<Label>("fertilizer-value-label");
        _background.Q<Slider>("water-slider").RegisterValueChangedCallback(v => waterValueLabel.text = v.newValue.ToString());
        _background.Q<Slider>("fertilizer-slider").RegisterValueChangedCallback(v => fertilizerValueLabel.text = v.newValue.ToString());
    }

    private void SetupButtons(Action<UIButton> onButtonDown, Action<UIButton> onButtonUp)
    {
        _previousPlantButton = _background.Q<Button>("previous-plant-button");
        _nextPlantButton = _background.Q<Button>("next-plant-button");
        foreach (var (enumValue, buttonName) in DetailViewConstants.NameOfButtons)
        {
            var button = _background.Q<Button>(buttonName);
            if (button != null)
            {
                button.RegisterCallback<PointerDownEvent>((_) => onButtonDown(enumValue), TrickleDown.TrickleDown);
                button.RegisterCallback<PointerUpEvent>((_) => onButtonUp(enumValue));
                button.RegisterCallback<PointerDownEvent>((_) => SoundManagerSingleton.Instance.PlaySound("Click"), TrickleDown.TrickleDown);

            }
        }
    }
    private void SetupDropDowns()
    {
        _seedDropdown = _background.Q<DropdownField>("seed-type-dropdown");
        _seedDropdown.RegisterCallback<PointerDownEvent>(_ =>
        {
            SoundManagerSingleton.Instance.PlaySound("Click");
        });
        _seedDropdown.RegisterValueChangedCallback(_ =>
        {
            SoundManagerSingleton.Instance.PlaySound("Click");
        });
    }


    private void SetupDetailLabels(Action<string> onDetailHovered)
    {
        foreach (var labelName in new[] { "water", "nutrients", "potsize", "strain", "sex", "age", "growthStage" })
        {
            var row = _background.Q<VisualElement>(labelName);
            var valueLabel = row.Q<Label>("value");
            _detailLabels[labelName] = valueLabel;

            row.RegisterCallback<MouseEnterEvent>((_) => onDetailHovered(labelName));
        }
    }

    public void UpdatePlantData(Dictionary<string, object> plantData)
    {
        foreach (var (name, label) in _detailLabels)
        {
            label.text = plantData[name]?.ToString() ?? string.Empty;

        }
        if (plantData.TryGetValue("strain", out var strain))
        {
            string strainString = strain?.ToString() ?? string.Empty;

            if (strainString.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                _seedSelectionContainer.style.display = DisplayStyle.Flex;
                _plantInfo.style.display = DisplayStyle.None;
                
            }
            else
            {
                _seedSelectionContainer.style.display = DisplayStyle.None;
                _plantInfo.style.display = DisplayStyle.Flex;

            }
        }
        else
        {
            Debug.LogWarning("Der Schlüssel 'Strain' fehlt oder ist in plantData nicht korrekt.");
            _seedSelectionContainer.style.display = DisplayStyle.None;
        }

    }
    public void UpdatePlantNavigationButtons(bool canGoPrevious, bool canGoNext)
    {
        _previousPlantButton.SetEnabled(canGoPrevious);
        _nextPlantButton.SetEnabled(canGoNext);
    }

    public void UpdateWikiText(string text)
    {
        _wikiTextLabel.text = text;
    }

    public void ShowView()
    {
        _background.style.display = DisplayStyle.Flex;
    }

    public void HideView()
    {
        UIEvents.HideDetailView.Invoke();
    }
    private Submenu _currentSubmenu;
    public void ShowSubmenu(Submenu submenu)
    {
        switch (submenu)
        {
            case Submenu.WATERINGSUBMENU:
                _background.Q<Slider>("water-slider").value = DetailViewConstants.DefaultWaterValue;
                _background.Q<Slider>("fertilizer-slider").value = DetailViewConstants.DefaultFertilizerValue;
                break;
            case Submenu.CHANGEPOTSUBMENU:
                break;
            default:
                break;
        }
        _background.Q<VisualElement>("submenues").style.display = DisplayStyle.Flex;
        _background.Q<VisualElement>(DetailViewConstants.NameOfSubmenues[submenu]).style.display = DisplayStyle.Flex;
        _currentSubmenu = submenu;
    }
    public void CloseCurrentSubmenu()
    {
        _background.Q<VisualElement>("submenues").style.display = DisplayStyle.None;
        _background.Q<VisualElement>(DetailViewConstants.NameOfSubmenues[_currentSubmenu]).style.display = DisplayStyle.None;
    }

    internal float GetWaterValue()
    {
        return _background.Q<Slider>("water-slider").value;
    }

    public Strain GetSeedValue()
    {
        string selectedValue = _seedDropdown.value;
        if (Enum.TryParse<Strain>(selectedValue, out var strainValue))
            return strainValue;
        return Strain.None;
    }
    internal float GetFertilizerValue()
    {
        return _background.Q<Slider>("fertilizer-slider").value;
    }

    internal void StartOnboarding()
    {
        var plantView = _background.Q<Image>("plant-view");
        var rotateControlButtons = _background.Q<VisualElement>("rotate-control-buttons");
        var cameraControlButtons = _background.Q<VisualElement>("camera-control-buttons");
        var zoomControlButtons = _background.Q<VisualElement>("zoom-control-buttons");
        var wiki = _background.Q<VisualElement>("wiki");
        UIEvents.ShowOnboardingView(new() { 
            new(plantView, "Pflanzenübersicht", "Hier kannst du dir die Pflanzen genauer anschauen"),
            new(_previousPlantButton, "Vorherige Pflanze", "Mit diesem Button kannst du dir die vorherige Pflanze anschauen"),
            new(_nextPlantButton, "Nächste Pflanze", "Hiermit kannst du zur nächsten Pflanze springen"),
            new(cameraControlButtons, "Kamera bewegen", "Mit diesen Buttons kannst du die Kamera bewegen, um die Pflanze besser ins Bild zu rücken"),
            new(rotateControlButtons, "Pflanze bewegen", "Mit diesen Buttons kannst du die Pflanze nach unten und oben bewegen und drehen"),
            new(zoomControlButtons, "Heranzoomen", "Mit diesen Buttons kannst du die Zoomstufe einstellen"),
            new(_seedSelectionContainer, "Anpflanzen einer Pflanze", "Hier kannst du eine neue Pflanze in den Topf anpflanzen. Danach erfährst du hier mehr Informationen über die Pflanze"),
            new(wiki, "Enzyklopädie", "Wenn du über verschiedene Elemente mit der Maus fährst, kannst du hier weitere Informationen erhalten"),
        });
    }
}


public class DetailViewPlantManager
{
    public int Plantcount => _plants.Count;
    public bool HasPlantsToReset => _plantsToReset.Count > 0;
    private List<PlantController> _plants;
    private Quaternion _savedPlantRotation;
    private Dictionary<int, Quaternion> _plantsToReset = new();
    private Action<int> _onPlantChanged;

    public int CurrentPlantIndex { get; private set; } = -1;

    public DetailViewPlantManager(List<PlantController> plants, Action<int> onPlantChanged)
    {
        _plants = plants;
        _onPlantChanged = onPlantChanged;

    }

    public void SetCurrentPlant(int plantIndex)
    {
        if (plantIndex == -1 && CurrentPlantIndex != -1)
            return;
        CurrentPlantIndex = plantIndex;
        _onPlantChanged.Invoke(CurrentPlantIndex);
        SavePlantTransform();
    }

    public void ResetCurrentPlantTransform()
    {
        if (CurrentPlantIndex < 0)
            return;
        _plantsToReset[CurrentPlantIndex] = _savedPlantRotation;
    }

    public void SwitchToPreviousPlant()
    {
        if (CurrentPlantIndex <= 0)
            return;
        ResetCurrentPlantTransform();
        CurrentPlantIndex--;
        _onPlantChanged.Invoke(CurrentPlantIndex);
        SavePlantTransform();
    }

    public void SwitchToNextPlant()
    {
        if (CurrentPlantIndex >= _plants.Count - 1)
            return;
        ResetCurrentPlantTransform();
        CurrentPlantIndex++;
        _onPlantChanged.Invoke(CurrentPlantIndex);
        SavePlantTransform();
    }

    public void ResetAllPlants()
    {
        foreach (var (plantIndex, savedRotation) in _plantsToReset)
        {
            var plant = _plants[plantIndex];

            plant.transform.rotation = savedRotation;
        }
        _plantsToReset.Clear();
    }

    public void UpdateRotations(Vector3 rotationVector)
    {
        if (CurrentPlantIndex < 0)
            return;

        var currentPlant = _plants[CurrentPlantIndex];
        currentPlant.transform.Rotate(rotationVector, Space.World);


        List<int> entriesToRemoveFromPlantsToReset = new();
        foreach (var (plantIndex, savedRotation) in _plantsToReset)
        {
            var plant = _plants[plantIndex];
            if (plant.transform.rotation == savedRotation)
            {
                entriesToRemoveFromPlantsToReset.Add(plantIndex);
                continue;
            }
            plant.transform.rotation = Quaternion.Lerp(plant.transform.rotation, savedRotation, Time.deltaTime * DetailViewConstants.CameraLerpSpeed);
        }
        foreach (var entry in entriesToRemoveFromPlantsToReset)
        {
            _plantsToReset.Remove(entry);
        }
    }

    public Vector3 GetCurrentPlantClampMin()
    {
        var currentPlant = _plants[CurrentPlantIndex].transform.position;
        return new Vector3(currentPlant.x + DetailViewConstants.XInitialCameraToPlantOffset - DetailViewConstants.CameraClampOffset,
                           currentPlant.y + DetailViewConstants.YInitialCameraToPlantOffset - DetailViewConstants.CameraClampOffset,
                           currentPlant.z + DetailViewConstants.ZInitialCameraToPlantOffset);
    }

    public Vector3 GetCurrentPlantClampMax()
    {
        var currentPlant = _plants[CurrentPlantIndex].transform.position;
        return new Vector3(currentPlant.x + DetailViewConstants.XInitialCameraToPlantOffset + DetailViewConstants.CameraClampOffset,
                           currentPlant.y + DetailViewConstants.YInitialCameraToPlantOffset + DetailViewConstants.CameraClampOffset,
                           currentPlant.z);
    }

    public PlantController GetCurrentPlantController()
    {
        return _plants[CurrentPlantIndex];
    }
    public Vector3 GetCurrentPlantPosition()
    {
        return _plants[CurrentPlantIndex].transform.position;
    }

    public Dictionary<string, object> GetCurrentPlantDataAsDictionary()
    {
        return _plants[CurrentPlantIndex].PlantData.DataDictionary();
    }
    public PlantData GetCurrentPlantData()
    {
        return _plants[CurrentPlantIndex].PlantData;
    }
    public void AddWaterAndFertilizer(float waterAmount, float fertilizerAmount)
    {
        if (CurrentPlantIndex < 0)
            return;
        var currentPlant = _plants[CurrentPlantIndex];
        currentPlant.PlantData.Soil.StoredWater += waterAmount;
        currentPlant.PlantData.Soil.StoredNutrients += fertilizerAmount;
    }

    public void ChangePotSize(UIButton buttonType)
    {
        Potsize potsize;
        switch (buttonType)
        {
            case UIButton.CHANGETOCULTIVATION:
                potsize = Potsize.Cultivation;
                break;
            case UIButton.CHANGETOSMALL:
                potsize = Potsize.Small;
                break;
            case UIButton.CHANGETOMEDIUM:
                potsize = Potsize.Medium;
                break;
            case UIButton.CHANGETOLARGE:
                potsize = Potsize.Large;
                break;
            default:
                potsize = Potsize.Cultivation;
                break;

        }
        var currentPlant = _plants[CurrentPlantIndex];
        currentPlant.PlantData.Potsize = potsize;
    }

    private void SavePlantTransform()
    {
        var currentPlant = _plants[CurrentPlantIndex];
        _savedPlantRotation = currentPlant.transform.rotation;
    }
    public bool PlantSeedInCurrentPot(Strain seedType)
    {
        if (CurrentPlantIndex < 0 || CurrentPlantIndex >= _plants.Count)
        {
            return false;
        }
        var currentPlant = _plants[CurrentPlantIndex];
        if (!currentPlant.IsPlantable())
        {
            return false;
        }
        bool plantable = InventarController.Instance.UpdateSeedQuantity(seedType.ToString());
        if (plantable)
        {
            currentPlant.PlantSeed(seedType);
            return true;
        }
        else 
        {
            return false;
        }
        
    }

    internal void HarvestCurrentPlant()
    {
        var currentPlant = _plants[CurrentPlantIndex];
        var gamestate = GameStateManagerSingleton.Instance.GameState;
        gamestate.HarvestedPlantDataList.List.Add(currentPlant.PlantData);
        PlantData emptyPlantData = new();
        gamestate.PlantDataList.List[CurrentPlantIndex] = emptyPlantData;
        _plants[CurrentPlantIndex].PlantData = emptyPlantData;
    }
}

public class DummyWiki
{
    public static string GetWikiEntry(string entryTitle)
    {
        return $"Das ist ein Enzyklop�dieeintrag f�r {entryTitle}.";
    }
}

