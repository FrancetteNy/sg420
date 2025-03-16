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
    const int _screenWidth = 1920;
    const int _screenHeight = 1080;
    bool _firstScreen = true;

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
        _firstScreen = true;
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
        focusedElement?.RegisterCallback<GeometryChangedEvent>(UpdateFocusElement);
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
        if (!_firstScreen)
        {
            ResetTransitionDurationTime();
        }
        else
        {
            _firstScreen = false;
        }
        OnboardingData data = _data[_currentContainerElement];
        VisualElement focusedElement = data.FocusedElement;
        Single top = focusedElement?.worldBound.yMin - _screenHeight ?? 0;
        Single bottom = -focusedElement?.worldBound.yMax ?? 0;
        Single left = focusedElement?.worldBound.xMin - _screenWidth ?? 0;
        Single right = - focusedElement?.worldBound.xMax ?? 0;
        _containerElement.style.top = top;
        _containerElement.style.bottom = bottom;
        _containerElement.style.left = left;
        _containerElement.style.right = right;
        UpdateInformationBoxPosition(null);
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
        focusedElement?.UnregisterCallback<GeometryChangedEvent>(UpdateFocusElement);
    }

    private void UpdateInformationBoxPosition(GeometryChangedEvent evt)
    {
        var target = _data[_currentContainerElement].FocusedElement?.worldBound ?? new Rect(new(_screenWidth / 2, _screenHeight / 2), new(0,0));
        var size = _informationElement.worldBound.size;
        const float margin = 30f;

        var hCenter = target.center.x - size.x / 2;
        var vCenter = target.center.y - size.y / 2;
        var hFits = hCenter >= 0 && (hCenter + size.x) <= _screenWidth;
        var vFits = vCenter >= 0 && (vCenter + size.y) <= _screenHeight;

        var fitsTop = target.yMin - size.y - margin >= 0;
        var fitsBelow = target.yMax + size.y + margin <= _screenHeight;
        var fitsRight = target.xMax + size.x + margin <= _screenWidth;
        var fitsLeft = target.xMin - size.x - margin >= 0;

        var placements = new (Func<bool> check, Action pos)[]
        {
        //Position at the top
        (() => fitsTop && hFits,
            () => SetPos(target.yMin - size.y - margin, hCenter)),
        //Position at the Bottom
        (() => fitsBelow && hFits,
            () => SetPos(target.yMax + margin, hCenter)),
        //Position to the left
        (() => fitsLeft && vFits,
            () => SetPos(vCenter, target.xMin - size.x - margin)),
        //Position to the right
        (() => fitsRight && vFits,
            () => SetPos(vCenter, target.xMax + margin)),
        //Position to the top, but shifted to the middle in horizontal
        (() => fitsTop,
            () => SetPos(target.yMin - size.y - margin, Mathf.Clamp(hCenter, 0, _screenWidth - size.x))),
        //Position at the Bottom, but shifted to the middle in horizontal
        (() => fitsBelow,
            () => SetPos(target.yMax + margin, Mathf.Clamp(hCenter, 0, _screenWidth - size.x))),
        //Position to the left, but shifted to the middle in vertical
        (() => fitsLeft && vFits,
            () => SetPos(Mathf.Clamp(vCenter, 0, _screenHeight - size.x), target.xMin - size.x - margin)),
        //Position to the right, but shifted to the middle in vertical
        (() => fitsRight && vFits,
            () => SetPos(Mathf.Clamp(vCenter, 0, _screenHeight - size.x), target.xMax + margin)),
        };

        foreach (var placement in placements)
        {
            if (placement.check())
            {
                placement.pos();
                return;
            }
        }

        // Fallback to middle
        SetPos(_screenHeight/2 - size.y/2, _screenWidth / 2 - size.x/2);
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
