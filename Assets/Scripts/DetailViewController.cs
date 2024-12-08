using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DetailViewController : MonoBehaviour
{
    // Camera and Managers
    public Camera DetailViewCamera;
    public List<PlantController> PlantControllers;

    private DetailViewCameraController _cameraController;
    private DetailViewUIManager _uiManager;
    private DetailViewPlantManager _plantManager;

    private Action _closeAction;
    private Vector3 _cameraMoveVector;
    private Vector3 _rotateVector;

    private void Start()
    {
        // Initialize Controllers
        _cameraController = new DetailViewCameraController(DetailViewCamera, GetComponent<UIDocument>().rootVisualElement.Q<Image>("plant-view"));
        _plantManager = new DetailViewPlantManager(PlantControllers, OnPlantChanged);
        _uiManager = new DetailViewUIManager(GetComponent<UIDocument>(), OnButtonDown, OnButtonUp, OnDetailHovered);
    }

    private void Update()
    {
        if (_plantManager.CurrentPlantIndex < 0)
            return;

        // Update Camera
        _cameraController.UpdatePosition(_cameraMoveVector,
            _plantManager.GetCurrentPlantClampMin(),
            _plantManager.GetCurrentPlantClampMax(),
            DetailViewConstants.CameraLerpSpeed);

        // Update Plant Rotation
        _plantManager.UpdateRotations(_rotateVector);
    }
    
    private void OnButtonDown(UIButton buttonId)
    {
        switch (buttonId)
        {
            case UIButton.PREVIOUSPLANT:
                _plantManager.SwitchToPreviousPlant();
                _cameraController.SetInitialPosition(_plantManager.GetCurrentPlantPosition());
                _uiManager.UpdatePlantData(_plantManager.GetCurrentPlantData());
                break;
            case UIButton.NEXTPLANT:
                _plantManager.SwitchToNextPlant();
                _cameraController.SetInitialPosition(_plantManager.GetCurrentPlantPosition());
                _uiManager.UpdatePlantData(_plantManager.GetCurrentPlantData());
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
                _cameraMoveVector = Vector3.right;
                break;
            case UIButton.MOVELEFT:
                _cameraMoveVector = Vector3.left;
                break;
            case UIButton.MOVEUP:
                _cameraMoveVector = Vector3.up;
                break;
            case UIButton.MOVEDOWN:
                _cameraMoveVector = Vector3.down;
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
            default:
                break;
        }
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
            currentIndex < _plantManager.Plantcount - 1
        );
    }

    public void ActivateView(int plantControllerIndex, Action closeAction)
    {

        _plantManager.SetCurrentPlant(plantControllerIndex);
        _cameraController.SetInitialPosition(_plantManager.GetCurrentPlantPosition());
        _uiManager.UpdatePlantData(_plantManager.GetCurrentPlantData());

        _uiManager.ShowView();
        DetailViewCamera.enabled = true;

        _closeAction = closeAction;
    }


    private void OnDetailHovered(string detailName)
    {
        string wikiEntry = DummyWiki.GetWikiEntry(detailName);
        _uiManager.UpdateWikiText(wikiEntry);
    }

    private void CloseView()
    {
        _uiManager.HideView();
        DetailViewCamera.enabled = false;
        _plantManager.ResetCurrentPlantTransform();

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

public class DetailViewUIManager
{
    private VisualElement _background;
    private Label _wikiTextLabel;
    private Dictionary<string, Label> _detailLabels = new();
    private Button _previousPlantButton;
    private Button _nextPlantButton;
    public DetailViewUIManager(UIDocument document, Action<UIButton> onButtonDown, Action<UIButton> onButtonUp, Action<string> onDetailHovered)
    {
        _background = document.rootVisualElement.Q<VisualElement>("background");

        // Configure Buttons
        SetupButtons(onButtonDown, onButtonUp);

        // Collect and Configure Detail Labels
        _wikiTextLabel = _background.Q<Label>("wiki-text");
        SetupDetailLabels(onDetailHovered);
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
        _background.style.display = DisplayStyle.None;
    }
}


public class DetailViewPlantManager
{
    public int Plantcount => _plants.Count;
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

    public Vector3 GetCurrentPlantPosition()
    {
        return _plants[CurrentPlantIndex].transform.position;
    }

    public Dictionary<string, object> GetCurrentPlantData()
    {
        return _plants[CurrentPlantIndex].DataDictionary();
    }

    private void SavePlantTransform()
    {
        var currentPlant = _plants[CurrentPlantIndex];
        _savedPlantRotation = currentPlant.transform.rotation;
    }
}



public class DummyWiki
{
    public static string GetWikiEntry(string entryTitle)
    {
        return $"Das ist ein Enzyklopädieeintrag für {entryTitle}.";
    }
}

