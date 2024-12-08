using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DetailViewController : MonoBehaviour
{

    DetailViewCameraController _cameraController;


    // UI Elements
    public Camera DetailViewCamera;
    private RenderTexture _cameraView;
    private VisualElement _background;
    private Dictionary<string, Label> _labelsWithName = new();
    private Button _previousPlantButton;
    private Button _nextPlantButton;


    // Data and Control
    public List<PlantController> PlantControllers;
    private int _currentPlantControllerIndex = -1;
    private Action _closeAction;


    // Camera Move and Animation
    private Vector3 _rotateVector;

    private Quaternion _savedPlantRotation;
    private Dictionary<int, Quaternion> _plantsToReset = new();
    private void Start()
    {

        //Configure Camera
        var render_texture = _background.Q<Image>("plant-view");
        _cameraView = new RenderTexture(Screen.height, Screen.height, 24);
        render_texture.image = _cameraView;
        DetailViewCamera.targetTexture = _cameraView;
        _cameraController = new(DetailViewCamera, DetailViewCamera.transform.position);

        _background = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("background");


        Label wikiText = _background.Q<Label>("wiki-text");
        //Collect all Label that need to be filled
        foreach (var labelName in new string[] { "water", "nutrients", "potsize", "strain", "sex", "age", "growthStage" })
        {
            VisualElement rowOfDetailView = _background.Q<VisualElement>(labelName);
            _labelsWithName[labelName] = rowOfDetailView.Q<Label>("value");
            rowOfDetailView.RegisterCallback<MouseEnterEvent>((evt) => wikiText.text = DummyWiki.GetWikiEntry(labelName));
        }
        AddButtonCallbacks();
    }
    private void Update()
    {
        if (_currentPlantControllerIndex < 0)
            return;
        //NewMethod();
        _cameraController.Update();

        List<int> entriesToRemoveFromPlantsToReset = new();
        foreach (var (plantIndex, savedRotation) in _plantsToReset)
        {
            if (PlantControllers[plantIndex].transform.rotation == savedRotation)
            {
                entriesToRemoveFromPlantsToReset.Add(plantIndex);
                continue;
            }
            PlantControllers[plantIndex].transform.rotation = Quaternion.Lerp(PlantControllers[plantIndex].transform.rotation, savedRotation, Time.deltaTime * 5);
        }
        foreach (var entry in entriesToRemoveFromPlantsToReset)
        {
            _plantsToReset.Remove(entry);
        }
        PlantControllers[_currentPlantControllerIndex].transform.Rotate(_rotateVector, Space.World);
    }


    private void AddButtonCallbacks()
    {
        _background.Q<Button>("start-sexing").clicked += () => Debug.Log("totaly starting the sexing minigame");
        AddClickSound(_background.Q<Button>("start-sexing"));
        _background.Q<Button>("close-button").clicked += DisableView;
        AddClickSound(_background.Q<Button>("close-button"));

        _rotateVector = Vector3.zero;
        SetupControlButtons(
        _background.Q<VisualElement>("rotate-control-buttons"),
        new Dictionary<string, Vector3> {
                { "rotate-up-button", Vector3.left },
                { "rotate-down-button", Vector3.right },
                { "rotate-left-button", Vector3.up },
                { "rotate-right-button", Vector3.down },
            },
        (axis) => _rotateVector += (axis / 2) / (Mathf.Abs(_initialCameraPosition.z / DetailViewCamera.transform.position.z)),
        () => _rotateVector = Vector3.zero);

        SetupControlButtons(
            _background.Q<VisualElement>("camera-control-buttons"),
            new Dictionary<string, Vector3> {
                { "move-up-button", Vector3.up },
                { "move-down-button", Vector3.down},
                { "move-left-button", Vector3.left },
                { "move-right-button", Vector3.right },
            },
            (axis) => _cameraController.SetMovementVector(axis / 2),
            () => _cameraController.SetMovementVector(Vector3.zero));

        SetupControlButtons(
        _background.Q<VisualElement>("zoom-control-buttons"),
        new Dictionary<string, Vector3> {
                    { "zoom-in-button", Vector3.forward },
                    { "zoom-out-button", Vector3.back},
                },
            (axis) => _cameraController.SetMovementVector(axis),
            () => _cameraController.SetMovementVector(Vector3.zero));

        _previousPlantButton = _background.Q<Button>("previous-plant-button");
        _previousPlantButton.clicked += () => ShowOtherPlant(() => _currentPlantControllerIndex--);
        AddClickSound(_previousPlantButton);
        _nextPlantButton = _background.Q<Button>("next-plant-button");
        _nextPlantButton.clicked += () => ShowOtherPlant(() => _currentPlantControllerIndex++);
        AddClickSound(_nextPlantButton);

        var waterButton = _background.Q<Button>("open-watering-submenu-button");
        _submenues = _background.Q<VisualElement>("submenues");
        _wateringSubmenu = _submenues.Q<VisualElement>("watering-submenu");
        _waterSlider = _wateringSubmenu.Q<Slider>("water-slider");
        _fertilizerSlider = _wateringSubmenu.Q<Slider>("fertilizer-slider");
        waterButton.clicked += () => {
            _submenues.style.display = DisplayStyle.Flex;
            _wateringSubmenu.style.display = DisplayStyle.Flex;
            _waterSlider.value = 10;
            _fertilizerSlider.value = 0;
        };
        AddClickSound(waterButton);
        var closeWateringSubmenuButton = _wateringSubmenu.Q<Button>("close-watering-submenu-button");
        closeWateringSubmenuButton.clicked += CloseWaterSubmenu;
        AddClickSound(closeWateringSubmenuButton);
        var waterPlantsButton = _wateringSubmenu.Q<Button>("water-plant-button");
        waterPlantsButton.clicked += WaterCurrentPlant;
        waterPlantsButton.clicked += CloseWaterSubmenu;
        AddClickSound(waterPlantsButton);
        var potsizeButton = _background.Q<Button>("open-change-pot-submenu-button");
        potsizeButton.clicked += ChangePotOfCurrentPlant;
        AddClickSound(potsizeButton);
    }

    Slider _waterSlider;
    Slider _fertilizerSlider;
    VisualElement _submenues;
    VisualElement _wateringSubmenu;
    private void ChangePotOfCurrentPlant()
    {
        var currentPotsize = PlantControllers[_currentPlantControllerIndex].Potsize;
        switch (currentPotsize)
        {
            case Potsize.Cultivation:
                PlantControllers[_currentPlantControllerIndex].Potsize = Potsize.Small;
                break;
            case Potsize.Small:
                PlantControllers[_currentPlantControllerIndex].Potsize = Potsize.Medium;
                break;
            case Potsize.Medium:
                PlantControllers[_currentPlantControllerIndex].Potsize = Potsize.Large;
                break;
            case Potsize.Large:
                break;
        }
        FillDataLabel();
    }

    private void WaterCurrentPlant()
    {
        PlantControllers[_currentPlantControllerIndex].Soil.StoredWater += _waterSlider.value;
        PlantControllers[_currentPlantControllerIndex].Soil.StoredNutrients += _fertilizerSlider.value;
        FillDataLabel();
    }
    private void CloseWaterSubmenu()
    {
        _submenues.style.display = DisplayStyle.None;
        _wateringSubmenu.style.display = DisplayStyle.None;
    }
    void SetupControlButtons(VisualElement container, Dictionary<string, Vector3> buttonMappings, Action<Vector3> onPointerDown, Action onPointerUp)
    {
        if (container == null)
            return;

        foreach (var (buttonName, actionVector) in buttonMappings)
        {
            var button = container.Q<Button>(buttonName);
            if (button != null)
            {
                button.RegisterCallback<PointerDownEvent>((_) => onPointerDown(actionVector), TrickleDown.TrickleDown);
                button.RegisterCallback<PointerUpEvent>((_) => onPointerUp());
                AddClickSound(button);
            }
        }
    }
    private void AddClickSound(Button button)
    {
        button.RegisterCallback<PointerDownEvent>((_) => SoundManagerSingleton.Instance.PlaySound("Click"), TrickleDown.TrickleDown);
    }


    private void ShowOtherPlant(Action indexAction)
    {
        LoadTransform();
        indexAction.Invoke();
        SafeTransform();
        _previousPlantButton.SetEnabled(_currentPlantControllerIndex > 0);
        _nextPlantButton.SetEnabled(_currentPlantControllerIndex < PlantControllers.Count - 1);
        PopulateData();
    }

    public void ActivateView(int plantControllerIndex, Action closeAction)
    {
        _currentPlantControllerIndex = plantControllerIndex;
        _background.style.display = DisplayStyle.Flex;
        DetailViewCamera.enabled = true;
        _closeAction = closeAction;
        SafeTransform();
        ShowOtherPlant(() => _currentPlantControllerIndex = plantControllerIndex);
    }

    public void DisableView()
    {
        _background.style.display = DisplayStyle.None;
        DetailViewCamera.enabled = false;
        LoadTransform();
        _closeAction.Invoke();
    }

    private void SafeTransform()
    {
        _savedPlantRotation = PlantControllers[_currentPlantControllerIndex].transform.rotation;
    }

    private void LoadTransform()
    {
        _plantsToReset.Add(_currentPlantControllerIndex, _savedPlantRotation);
        _rotateVector = Vector3.zero;
    }

    private Vector3 _cameraOffsetVector = new(0, 0.75f, -5);
    private Vector3 _initialCameraPosition;
    private void PopulateData()
    {
        _cameraController.SetTarget(PlantControllers[_currentPlantControllerIndex].gameObject.transform.position);
        FillDataLabel();
    }

    private void FillDataLabel()
    {
        var data = PlantControllers[_currentPlantControllerIndex].DataDictionary();
        foreach (var (name, label) in _labelsWithName)
        {
            if (data[name] == null)
                continue;
            label.text = data[name].ToString();
        }
    }
}

public class  DetailViewCameraController
{
    private Camera _camera;
    private Vector3 _futurePosition;
    private Vector3 _plantPositionToTarget;
    private Vector3 _cameraOffsetVector = new(0, 0.75f, -5); //Vector3 cant be const
    private Vector3 _cameraMovementVector = Vector3.zero;
    public DetailViewCameraController(Camera camera, Vector3 futurePosition)
    {
        _camera = camera;
        _futurePosition = futurePosition;
        _plantPositionToTarget = futurePosition;
    }


    public void SetTarget(Vector3 plantPositionToTarget)
    {
        var target = plantPositionToTarget + _cameraOffsetVector;
        _futurePosition = target;
        _plantPositionToTarget = plantPositionToTarget;
    }


    public void SetMovementVector(Vector3 cameraMovementVector)
    {
        // slow down the camera movement when we are near the plant
        var slowDown = Mathf.Abs((_plantPositionToTarget + _cameraOffsetVector).z / _camera.transform.position.z);
        const float maximumSlowdown = 0.3f;
        var x = cameraMovementVector.x * Mathf.Clamp(slowDown, maximumSlowdown, 1.0f);
        var y = cameraMovementVector.y * Mathf.Clamp(slowDown, maximumSlowdown, 1.0f);
        var z = cameraMovementVector.z;
        _cameraMovementVector = new Vector3(x,y,z);
    }

    public void Update()
    {
        // make sure that the camera is near the actual plant
        var unclampedFutureCameraPosition = _futurePosition + _cameraMovementVector * Time.deltaTime;
        var initialCameraPosition = _plantPositionToTarget + _cameraOffsetVector;
        _futurePosition = new Vector3(
            Mathf.Clamp(unclampedFutureCameraPosition.x, initialCameraPosition.x - 0.3f, initialCameraPosition.x + 0.3f),
            Mathf.Clamp(unclampedFutureCameraPosition.y, initialCameraPosition.y - 0.3f, initialCameraPosition.y + 0.3f),
            Mathf.Clamp(unclampedFutureCameraPosition.z, initialCameraPosition.z, _plantPositionToTarget.z));
        if (_futurePosition != _camera.transform.position)
        {
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _futurePosition, Time.deltaTime * 5);
        }
    }
}



public class DetailViewUIManager
{

}



public class DetailViewPlantManager
{

}

public class DummyWiki
{
    public static string GetWikiEntry(string entryTitle)
    {
        return $"Das ist ein Enzyklopädieeintrag für {entryTitle}.";
    }
}

