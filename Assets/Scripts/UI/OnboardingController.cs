using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class OnboardingController : MonoBehaviour
{
    VisualElement _view;
    List<OnboardingData> _data;
    VisualElement _containerElement;
    VisualElement _informationElement;
    Button _nextButton;
    Button _previousButton;
    Button _closeButton;
    Label _title;
    Label _information;
    int _currentContainerElement = -1;
    internal void Initialize(VisualElement view)
    {
        _view = view;
        SetupButtonCallbacks();
        SetupLabels();
        _containerElement = _view.Q<VisualElement>("container-element");
        _informationElement = _view.Q<VisualElement>("information-element");
        _informationElement.RegisterCallback<GeometryChangedEvent>(UpdateInformationBoxPosition);
    }

    private void SetupLabels()
    {
        _title = _view.Q<Label>("title");
        _information = _view.Q<Label>("information");
    }

    private void SetupButtonCallbacks()
    {
        _nextButton = _view.Q<Button>("next-button");
        _nextButton.clicked += OnNextButtonClicked;
        _nextButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _previousButton = _view.Q<Button>("previous-button");
        _previousButton.clicked += OnPreviousButtonClicked;
        _previousButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
        _closeButton = _view.Q<Button>("close-button");
        _closeButton.clicked += StopOnboarding;
        _closeButton.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");
    }

    internal void SetData(List<OnboardingData> list)
    {
        _data = list;
    }
    StyleList<TimeValue> _storedInformationDurations;
    StyleList<TimeValue> _storedContainerDurations;
    internal void StartOnboarding()
    {
        _storedInformationDurations = new StyleList<TimeValue>(_informationElement.resolvedStyle.transitionDuration.ToList());
        _storedContainerDurations = new StyleList<TimeValue>(_containerElement.resolvedStyle.transitionDuration.ToList());
        _informationElement.style.transitionDuration = StyleKeyword.Initial;
        _containerElement.style.transitionDuration = StyleKeyword.Initial;
        _currentContainerElement = 0;
        UpdateOnboardingElements();
    }


    internal void StopOnboarding()
    {
        CleanUpFocusElement();
        UIEvents.HideOnboardingView();
    }

    private void OnNextButtonClicked()
    {
        CleanUpFocusElement();
        _currentContainerElement++;
        UpdateOnboardingElements();
    }


    private void OnPreviousButtonClicked()
    {
        CleanUpFocusElement();
        _currentContainerElement--;
        UpdateOnboardingElements();
    }

    private void UpdateOnboardingElements()
    {
        if (0 > _currentContainerElement || _currentContainerElement >= _data.Count)
        {
            StopOnboarding();
            return;
        }
        OnboardingData data = _data[_currentContainerElement];
        VisualElement focusedElement = data.FocusedElement;
        UpdateInformationText();
        UpdateButtons();
        focusedElement.RegisterCallback<GeometryChangedEvent>(UpdateFocusElement);
        UpdateFocusElement(null);
    }

    private void UpdateInformationText()
    {
        OnboardingData data = _data[_currentContainerElement];
        _title.text = data.Title;
        _information.text = data.Information;
    }

    private void UpdateButtons()
    {
        _nextButton.SetEnabled(_currentContainerElement < _data.Count - 1);
        _previousButton.SetEnabled(_currentContainerElement > 0);
    }
    private void UpdateFocusElement(GeometryChangedEvent evt)
    {
        OnboardingData data = _data[_currentContainerElement];
        VisualElement focusedElement = data.FocusedElement;
        var top = focusedElement.worldBound.yMin - Screen.height;
        var bottom = -focusedElement.worldBound.yMax;
        var left = focusedElement.worldBound.xMin - Screen.width;
        var right = - focusedElement.worldBound.xMax;
        _containerElement.style.top = top;
        _containerElement.style.bottom = bottom;
        _containerElement.style.left = left;
        _containerElement.style.right = right;
        UpdateInformationBoxPosition(null);
        if (evt != null)
        {
            ResetTransitionDurationTime();
        }
    }

    private void ResetTransitionDurationTime()
    {
        _informationElement.style.transitionDuration = _storedInformationDurations;
        _containerElement.style.transitionDuration = _storedContainerDurations;
    }

    private void CleanUpFocusElement()
    {
        if (0 > _currentContainerElement || _currentContainerElement >= _data.Count)
        {
            return;
        }
        OnboardingData data = _data[_currentContainerElement];
        VisualElement focusedElement = data.FocusedElement;
        focusedElement.UnregisterCallback<GeometryChangedEvent>(UpdateFocusElement);
    }

    private void UpdateInformationBoxPosition(GeometryChangedEvent evt)
    {
        var target = _data[_currentContainerElement].FocusedElement.worldBound;
        var size = _informationElement.worldBound.size;
        const float margin = 30f;

        var hCenter = target.center.x - size.x / 2;
        var vCenter = target.center.y - size.y / 2;
        var hFits = hCenter >= 0 && (hCenter + size.x) <= Screen.width;
        var vFits = vCenter >= 0 && (vCenter + size.y) <= Screen.height;

        var placements = new (Func<bool> check, Action pos)[]
        {
        //Position at the top
        (() => target.yMin >= size.y + margin && hFits,
            () => SetPos(target.yMin - size.y - margin, hCenter)),
        //Position at the Bottom
        (() => target.yMax + size.y + margin <= Screen.height && hFits,
            () => SetPos(target.yMax + margin, hCenter)),
        //Position to the left
        (() => target.xMin >= size.x + margin && vFits,
            () => SetPos(vCenter, target.xMin - size.x - margin)),
        //Position to the right
        (() => target.xMax + size.x + margin <= Screen.width && vFits,
            () => SetPos(vCenter, target.xMax + margin))
        };

        foreach (var placement in placements)
        {
            if (placement.check())
            {
                placement.pos();
                return;
            }
        }

        // Fallback to top-center with horizontal clamping
        SetPos(0, Mathf.Clamp(hCenter, 0, Screen.width - size.x));
    }

    private void SetPos(float top, float left)
    {
        _informationElement.style.top = top;
        _informationElement.style.left = left;
    }

}

public class OnboardingData
{
    public VisualElement FocusedElement;
    public string Title;
    public string Information;
    public OnboardingData(VisualElement focusedElement, string title, string information)
    {
        FocusedElement = focusedElement;
        Title = title;
        Information = information;
    }
}