using SG420UILibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
 

public class InventarController : MonoBehaviour
{
    private DetailViewUIManager _uiManager;
    private VisualElement _root;
    private Camera _inventarCamera;
    private Action _closeAction;
    private Button _tab1, _tab2, _tab3;
    private VisualElement _content1, _content2, _content3;
    List<(string name, int quantity)> _seeds = new List<(string, int)>();
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

        Content1Get();
        ShowTab(_tab1, _content1);
    }
      public List<(string seedName, int seedQuantity)> Content1Get(){
        var seedElements = _content1.Children();

        foreach (var seedElement in seedElements)
        {
            var seedName = seedElement.Q<Label>("seedName").text;
            var seedQuantity = int.Parse(seedElement.Q<Label>("seedQuantity").text);
            _seeds.Add((seedName, seedQuantity));
        }
        return _seeds;
    }
    
    void ShowTab(Button selectedTab, VisualElement activeTab)
    {
        
        _content1.style.display = DisplayStyle.None;
        _content2.style.display = DisplayStyle.None;
        _content3.style.display = DisplayStyle.None;

        _tab1.RemoveFromClassList("active");
        _tab2.RemoveFromClassList("active");
        _tab3.RemoveFromClassList("active");

        selectedTab.AddToClassList("active");
        activeTab.style.display = DisplayStyle.Flex;
    }
}

public class SeedData
{
    public string Name { get; private set; }
    public int Quantity { get; private set; }

    // Constructeur pour initialiser les valeurs
    public SeedData(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }
}