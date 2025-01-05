using MyUILibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LightOverviewController : MonoBehaviour
{
    private VisualElement _root;
    private Label _lightLabel;
    private DropdownField _lightDropdownField;
    private Dictionary<GrowlightType, string> _lighttypeStrings = new Dictionary<GrowlightType, string> { { GrowlightType.NONE, "Kein Licht" }, { GrowlightType.LED, "LED Lampe" } };
    public void Initialize(VisualElement root)
    {
        _root = root;
        SetupButtons();
        SetupLabels();
        SetupDropDowns();
        SetupToggles();
    }

    private void SetupToggles()
    {
        var toggle = _root.Q<SlideToggle>("growthphase-slidetoggle");
        toggle.RegisterValueChangedCallback(OnGrowthPhaseChanged);
    }

    private void OnGrowthPhaseChanged(ChangeEvent<bool> evt)
    {
        SoundManagerSingleton.Instance.PlaySound("Click");
        GameStateManagerSingleton.Instance.GameState.Growlight.IsInFloweringGrowthMode = evt.newValue;
    }

    private void SetupDropDowns()
    {
        _lightDropdownField = _root.Q<DropdownField>("light-dropdown-field");
        _lightDropdownField.RegisterValueChangedCallback(OnLightTypeChanged);
        _lightDropdownField.choices = _lighttypeStrings.Values.ToList();
        _lightDropdownField.value = _lighttypeStrings[GameStateManagerSingleton.Instance.GameState.Growlight.Type];
        _lightDropdownField.RegisterCallback<PointerDownEvent>(_ =>
        {
            SoundManagerSingleton.Instance.PlaySound("Click");
        });
        _lightDropdownField.RegisterCallback<FocusEvent>(_ =>
        {
            StartCoroutine(WaitForDropdownMenu());
        });

    }
    private IEnumerator WaitForDropdownMenu()
    {
        var root = _lightDropdownField.panel.visualTree;

        VisualElement baseDropdown = null;
        while (baseDropdown == null)
        {
            baseDropdown = root.Q<VisualElement>(className: "unity-base-dropdown");
            yield return null;
        }
        var dropdownMenu = baseDropdown.Q<ScrollView>();

        dropdownMenu.RegisterCallback<PointerDownEvent>(_ =>
        {
            SoundManagerSingleton.Instance.PlaySound("Click");
        });
    }
    private void OnLightTypeChanged(ChangeEvent<string> evt)
    {
        GameStateManagerSingleton.Instance.GameState.Growlight.Type = _lighttypeStrings.FirstOrDefault(pair => pair.Value == evt.newValue).Key;
        _lightLabel.text = DummyWiki.GetWikiEntry(evt.newValue);
    }

    private void SetupLabels()
    {
        _lightLabel = _root.Q<Label>("light-label");
    }

    private void SetupButtons()
    {
        var close_button = _root.Q<Button>("close-button");
        close_button.clicked += () => UIEvents.HideLightOverview.Invoke();
        close_button.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
    }
}