using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DetailViewController : MonoBehaviour
{
    public List<PlantController> PlantControllers;
    private int _currentPlantControllerIndex;
    private Action _closeAction;
    public Camera DetailViewCamera;
    public Camera MainCamera;
    private RenderTexture _cameraView;
    private VisualElement _background;
    private Dictionary<string, Label> _labelsWithName = new();
    private Button _previousPlantButton;
    private Button _nextPlantButton;
    private Vector3 _futureCameraPosition = Vector3.zero;
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
        _background.Q<Button>("start-sexing").clicked += () => Debug.Log("totaly starting the sexing minigame");
        _background.Q<Button>("close-button").clicked += DisableView;
        _previousPlantButton = _background.Q<Button>("previous-plant-button");
        _previousPlantButton.clicked += () => ShowOtherPlant(() => _currentPlantControllerIndex--);
        _nextPlantButton = _background.Q<Button>("next-plant-button");
        _nextPlantButton.clicked += () => ShowOtherPlant(() => _currentPlantControllerIndex++);
    }

    private void ShowOtherPlant(Action indexAction)
    {
        indexAction.Invoke();
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
        //_futureCameraPosition = PlantControllers[_currentPlantControllerIndex].gameObject.transform.position + new Vector3(0, 0.75f, -5);
        _closeAction = closeAction;
        ShowOtherPlant(() => _currentPlantControllerIndex = plantControllerIndex);
        DetailViewCamera.transform.position = _futureCameraPosition;
    }

    public void DisableView()
    {
        _background.style.display = DisplayStyle.None;
        //MainCamera.enabled = true;
        DetailViewCamera.enabled = false;
        _closeAction.Invoke();
    }

    private void PopulateData()
    {
        //DetailViewCamera.transform.position = PlantControllers[_currentPlantControllerIndex].gameObject.transform.position + new Vector3(0, 0.75f, -5);
        _futureCameraPosition = PlantControllers[_currentPlantControllerIndex].gameObject.transform.position + new Vector3(0, 0.75f, -5);
        
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
        if (_futureCameraPosition != DetailViewCamera.transform.position)
        {
            DetailViewCamera.transform.position = Vector3.Lerp(DetailViewCamera.transform.position, _futureCameraPosition, Time.deltaTime * 2);
        }
    }
}

public class DummyWiki
{
    public static string GetWikiEntry(string entryTitle)
    {
        return $"Das ist ein Enzyklopädieeintrag für {entryTitle}.";
    }
}