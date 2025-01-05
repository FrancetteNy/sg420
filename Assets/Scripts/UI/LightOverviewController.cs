using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LightOverviewController : MonoBehaviour
{
    private VisualElement _root;
    public void Initialize(VisualElement root)
    {
        _root = root;
        SetupButtons();
        SetupLabels();
    }

    private void SetupLabels()
    {
        //throw new NotImplementedException();
    }

    private void SetupButtons()
    {
        var close_button = _root.Q<Button>("close-button");
        close_button.clicked += () => UIEvents.HideLightOverview.Invoke();
        close_button.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
    }
}