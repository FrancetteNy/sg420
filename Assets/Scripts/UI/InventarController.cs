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
    private Button tab1, tab2, tab3;
    private VisualElement content1, content2, content3;
    public void Initialize(VisualElement root)
    {
        _root = root;

        _root.style.display = DisplayStyle.None;

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideInventar.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        tab1 = _root.Q<Button>("tab1");
        tab2 = _root.Q<Button>("tab2");
        tab3 = _root.Q<Button>("tab3");

        
        content1 = _root.Q<VisualElement>("tab-content1");
        content2 = _root.Q<VisualElement>("tab-content2");
        content3 = _root.Q<VisualElement>("tab-content3");

        
        tab1.clicked += () => ShowTab(tab1, content1);
        tab1.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        tab2.clicked += () => ShowTab(tab2, content2);
        tab2.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        tab3.clicked += () => ShowTab(tab3, content3);
        tab3.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

       
        ShowTab(tab1, content1);
    }
    void ShowTab(Button selectedTab, VisualElement activeTab)
    {
        
        content1.style.display = DisplayStyle.None;
        content2.style.display = DisplayStyle.None;
        content3.style.display = DisplayStyle.None;

        tab1.RemoveFromClassList("active");
        tab2.RemoveFromClassList("active");
        tab3.RemoveFromClassList("active");

        selectedTab.AddToClassList("active");
        activeTab.style.display = DisplayStyle.Flex;
    }
}