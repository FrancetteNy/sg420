using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DetailViewController : MonoBehaviour
{
    public List<PlantController> PlantControllers;
    private int _currentPlantControllerIndex = -1;
    private Action _closeAction;
    public Camera DetailViewCamera;
    public Camera MainCamera;
    private RenderTexture _cameraView;
    private VisualElement _background;
    private Dictionary<string, Label> _labelsWithName = new();
    private Button _previousPlantButton;
    private Button _nextPlantButton;
    private Vector3 _futureCameraPosition = Vector3.zero;
    private Quaternion _savedPlantRotation;
    private Vector3 _rotateVector;
    private Dictionary<int, Quaternion> _plantsToReset = new();
    private Vector3 _cameraMoveVector;
    private void Start()
    {
        _background = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("background");
        //Configure Camera
        var render_texture = _background.Q<Image>("plant-view");
        _cameraView = new RenderTexture(Screen.height, Screen.height, 24);
        DetailViewCamera.targetTexture = _cameraView;
        render_texture.image = _cameraView;
        _futureCameraPosition = DetailViewCamera.transform.position;


        Label wikiText = _background.Q<Label>("wiki-text");
        //Collect all Label that need to be filled
        foreach (var labelName in new string[] { "water", "nutrients", "potsize", "strain", "sex", "age", "growthStage" })
        {
            VisualElement rowOfDetailView = _background.Q<VisualElement>(labelName);
            _labelsWithName[labelName] = rowOfDetailView.Q<Label>("value");
            rowOfDetailView.RegisterCallback<MouseEnterEvent>((evt) => wikiText.text = DummyWiki.GetWikiEntry(labelName));
        }
        //Add Button Callbacks
        _background.Q<Button>("start-sexing").clicked += () => Debug.Log("totaly starting the sexing minigame");
        _background.Q<Button>("close-button").clicked += DisableView;
        _rotateVector = Vector3.zero;
        var rotate_control_buttons = _background.Q<VisualElement>("rotate-control-buttons");
        if (rotate_control_buttons != null)
        {
            foreach (var (button_name, rotate_axis) in new Dictionary<string, Vector3> {
                { "rotate-up-button", Vector3.left },
                { "rotate-down-button", Vector3.right },
                { "rotate-left-button", Vector3.up },
                { "rotate-right-button", Vector3.down },
            })
            {
                var button = rotate_control_buttons.Q<Button>(button_name);
                if (button != null)
                {
                    button.RegisterCallback<PointerDownEvent>((evt) => _rotateVector += (rotate_axis / 2) / (Mathf.Abs(_initialCameraPosition.z / DetailViewCamera.transform.position.z)), TrickleDown.TrickleDown);
                    button.RegisterCallback<PointerUpEvent>((evt) => _rotateVector = Vector3.zero);
                }
            }
        }
        _cameraMoveVector = Vector3.zero;
        var camera_control_buttons = _background.Q<VisualElement>("camera-control-buttons");
        if (camera_control_buttons != null)
        {
            foreach (var (button_name, rotate_axis) in new Dictionary<string, Vector3> {
                { "move-up-button", Vector3.up },
                { "move-down-button", Vector3.down},
                { "move-left-button", Vector3.left },
                { "move-right-button", Vector3.right },
            })
            {
                var button = camera_control_buttons.Q<Button>(button_name);
                if (button != null)
                {
                    button.RegisterCallback<PointerDownEvent>((evt) => _cameraMoveVector += (rotate_axis / 2) / (Mathf.Abs(_initialCameraPosition.z / DetailViewCamera.transform.position.z)), TrickleDown.TrickleDown);
                    button.RegisterCallback<PointerUpEvent>((evt) => _cameraMoveVector = Vector3.zero);
                }
            }
        }

        var zoom_control_buttons = _background.Q<VisualElement>("zoom-control-buttons");
        if (zoom_control_buttons != null)
        {
            foreach (var (button_name, rotate_axis) in new Dictionary<string, Vector3> {
                { "zoom-in-button", Vector3.forward },
                { "zoom-out-button", Vector3.back},
            })
            {
                var button = zoom_control_buttons.Q<Button>(button_name);
                if (button != null)
                {
                    button.RegisterCallback<PointerDownEvent>((evt) => _cameraMoveVector += rotate_axis, TrickleDown.TrickleDown);
                    button.RegisterCallback<PointerUpEvent>((evt) => _cameraMoveVector = Vector3.zero);
                }
            }
        }

        _previousPlantButton = _background.Q<Button>("previous-plant-button");
        _previousPlantButton.clicked += () => ShowOtherPlant(() => _currentPlantControllerIndex--);
        _nextPlantButton = _background.Q<Button>("next-plant-button");
        _nextPlantButton.clicked += () => ShowOtherPlant(() => _currentPlantControllerIndex++);
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
        //MainCamera.enabled = false;
        DetailViewCamera.enabled = true;
        _closeAction = closeAction;
        SafeTransform();
        ShowOtherPlant(() => _currentPlantControllerIndex = plantControllerIndex);
        DetailViewCamera.transform.position = _futureCameraPosition;
    }

    public void DisableView()
    {
        _background.style.display = DisplayStyle.None;
        //MainCamera.enabled = true;
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
        _futureCameraPosition = PlantControllers[_currentPlantControllerIndex].gameObject.transform.position + _cameraOffsetVector;
        _initialCameraPosition = _futureCameraPosition;
        var data = PlantControllers[_currentPlantControllerIndex].DataDictionary();
        foreach (var (name, label) in _labelsWithName)
        {
            if (data[name] == null)
                continue;
            label.text = data[name].ToString();
        }
    }
    private void Update()
    {
        if (_currentPlantControllerIndex < 0)
            return;
        var unclampedFutureCameraPosition = _futureCameraPosition + _cameraMoveVector * Time.deltaTime;
        _futureCameraPosition = new Vector3(
            Mathf.Clamp(unclampedFutureCameraPosition.x, _initialCameraPosition.x - 0.3f, _initialCameraPosition.x + 0.3f),
            Mathf.Clamp(unclampedFutureCameraPosition.y, _initialCameraPosition.y - 0.3f, _initialCameraPosition.y + 0.3f),
            Mathf.Clamp(unclampedFutureCameraPosition.z, _initialCameraPosition.z, PlantControllers[_currentPlantControllerIndex].gameObject.transform.position.z));
        if (_futureCameraPosition != DetailViewCamera.transform.position)
        {
            DetailViewCamera.transform.position = Vector3.Lerp(DetailViewCamera.transform.position, _futureCameraPosition, Time.deltaTime * 5);
        }
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
}

public class DummyWiki
{
    public static string GetWikiEntry(string entryTitle)
    {
        return $"Das ist ein Enzyklopädieeintrag für {entryTitle}.";
    }
}

