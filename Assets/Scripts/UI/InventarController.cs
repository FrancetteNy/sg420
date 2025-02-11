using SG420UILibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class InventarController : MonoBehaviour
{

    private VisualElement _root;
    
    public void Initialize(VisualElement root)
    {
        _root = root;

        _root.style.display = DisplayStyle.None;

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideInventar.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");


    }
}