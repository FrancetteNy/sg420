using SG420UILibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UIElements;
 

public class InventarController: MonoBehaviour
{
    public static InventarController Instance { get; private set; }
    private DetailViewUIManager _uiManager;
    private VisualElement _root;
    private Camera _inventarCamera;
    private Action _closeAction;
    private Button _tab1, _tab2, _tab3;
    private VisualElement _content1, _content2, _content3;
    private int _seed;
    private Label _label;
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
    }    public void Initialize(VisualElement root)
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

        selectedTab.AddToClassList("active");
        activeTab.style.display = DisplayStyle.Flex;
    }
    public bool UpdateSeedQuantity(string seedName)
    {   
        _label = _root.Q<Label>(seedName);
        _seed = int.Parse(_label.text);
        if (_seed > 0){
            _seed--;
            _label.text = _seed.ToString();
            return true;
        }
        else{
            return false;
        }
    }
}

public class SeedData
{
    public string Name { get; private set; }
    public int Quantity { get; private set; }

    public SeedData(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }
   
}