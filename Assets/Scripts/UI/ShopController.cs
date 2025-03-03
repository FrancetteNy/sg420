using SG420UILibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UIElements;
 

public class ShopController: MonoBehaviour
{
    public static ShopController Instance { get; private set; }
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

    }
}

