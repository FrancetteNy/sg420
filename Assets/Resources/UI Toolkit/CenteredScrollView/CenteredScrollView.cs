using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace SG420UILibrary
{
    [UxmlElement]
    public partial class CenteredScrollView : VisualElement
    {

        private readonly List<LabelWithResolvedHeight> _labels = new List<LabelWithResolvedHeight>();
        private readonly List<VisualElement> _carouselPoints = new List<VisualElement>();
        private int _selectedIndex;
        public int SelectedIndex => _selectedIndex;
        public float Spacing = 10f;
        private List<string> _texts;
        public event Action<int> Textchosen;

        private VisualElement _answerContainer;
        private VisualElement _carouselContainer;

        public CenteredScrollView()
        {
            var styleSheet = Resources.Load<StyleSheet>("UI Toolkit/CenteredScrollView/CenteredScrollView");
            styleSheets.Add(styleSheet);
            style.flexDirection = FlexDirection.Row;

            _answerContainer = new VisualElement { name = "answer-container" };
            _answerContainer.AddToClassList("answer-container");
            Add(_answerContainer);

            _carouselContainer = new VisualElement { name = "carousel-container" };
            _carouselContainer.AddToClassList("carousel-container");
            Add(_carouselContainer);

            var previousButton = CreateChoiceButton("choice-button", "choice-button--rotate-left");
            previousButton.clicked += PreviousIndex;
            _carouselContainer.Add(previousButton);
            var nextButton = CreateChoiceButton("choice-button", "choice-button--rotate-right");
            nextButton.clicked += NextIndex;
            _carouselContainer.Add(nextButton);

            RegisterCallback<WheelEvent>(OnScroll);
        }

        private void CreateCarouselPoint(int i)
        {
            var point = new VisualElement { name = "carousel-point" };
            point.AddToClassList("carousel-point");
            point.style.display = DisplayStyle.None;
            var index = i;
            point.RegisterCallback<MouseDownEvent>((_) =>
            {
                if (_selectedIndex < index)
                {
                    this.schedule.Execute(NextIndex).Every(75).Until(() => index <= _selectedIndex);
                }
                else
                {
                    this.schedule.Execute(PreviousIndex).Every(75).Until(() => index >= _selectedIndex);
                }
            });
            _carouselPoints.Add(point);
            _carouselContainer.Insert(_carouselPoints.Count, point);
        }

        public void NextIndex()
        {
            if (_selectedIndex < _labels.Count - 1)
            {
                _selectedIndex = Mathf.Clamp(_selectedIndex + 1, 0, _labels.Count - 1);
                UpdateVisuals();
            }
        }

        public void PreviousIndex()
        {
            if (_selectedIndex > 0)
            {
                _selectedIndex = Mathf.Clamp(_selectedIndex - 1, 0, _labels.Count - 1);
                UpdateVisuals();
            }
        }

        private Button CreateChoiceButton(string baseClass, string rotationClass)
        {
            var button = new Button();
            button.AddToClassList(baseClass);
            button.AddToClassList(rotationClass);
            return button;
        }

        public void SetTexts(List<string> texts)
        {
            // Hide all carousel points.
            foreach (var point in _carouselPoints)
            {
                point.style.display = DisplayStyle.None;
            }
            // Create Points if needed
            for (int i = _carouselPoints.Count; i < texts.Count; i++)
            {
                CreateCarouselPoint(i);
            }
            // Show only the points corresponding to texts.
            for (int i = 0; i < texts.Count; i++)
            {
                _carouselPoints[i].style.display = DisplayStyle.Flex;
            }
            // Remove previous labels.
            foreach (var labelEntry in _labels)
            {
                _answerContainer.Remove(labelEntry.Label);
            }
            _labels.Clear();

            _texts = texts;
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            if (_texts == null)
                return;

            StyleList<TimeValue> transitionDuration = null;
            for (int i = 0; i < _texts.Count; i++)
            {
                var label = new Label(_texts[i]);
                label.AddToClassList("picker-item");
                int index = i;
                label.RegisterCallback<MouseDownEvent>(_ =>
                {
                    if (index != _selectedIndex)
                    {
                        _selectedIndex = index;
                        UpdateVisuals();
                    }
                    else
                    {
                        Textchosen?.Invoke(index);
                    }
                });
                if (transitionDuration == null)
                {
                    transitionDuration = label.style.transitionDuration;
                }
                label.style.transitionDuration = StyleKeyword.Initial;
                _labels.Add(new LabelWithResolvedHeight(label, 0));
                _answerContainer.Add(label);
            }
            _selectedIndex = _labels.Count / 2;
            style.opacity = 0f;
            schedule.Execute(() =>
            {
                UpdateResolvedHeights();
                UpdateVisuals();
                style.opacity = 1f;
                foreach (var entry in _labels)
                {
                    entry.Label.style.transitionDuration = transitionDuration;
                }
            }).ExecuteLater(10);
        }

        private void UpdateResolvedHeights()
        {
            foreach (var entry in _labels)
            {
                entry.Height = entry.Label.resolvedStyle.height;
            }
        }

        private void OnScroll(WheelEvent evt)
        {
            evt.StopPropagation();
            if (evt.delta.y > 0)
                NextIndex();
            else
                PreviousIndex();
        }

        private void UpdateVisuals()
        {
            if (_labels == null || _labels.Count == 0)
                return;
            float containerCenter = resolvedStyle.height / 2f;
            var selectedEntry = _labels[_selectedIndex];
            var selectedLabel = selectedEntry.Label;
            float selectedHeight = selectedEntry.Height;
            float selectedTop = containerCenter - selectedHeight * 0.5f;

            // Update carousel points.
            foreach (var point in _carouselPoints)
            {
                point.RemoveFromClassList("carousel-point--selected");
            }
            _carouselPoints[_selectedIndex].AddToClassList("carousel-point--selected");

            // Update labels.
            foreach (var entry in _labels)
            {
                entry.Label.RemoveFromClassList("picker-item--selected");
            }
            selectedLabel.style.top = selectedTop;
            selectedLabel.AddToClassList("picker-item--selected");
            selectedLabel.style.opacity = 1f;
            selectedLabel.style.scale = new Scale(new Vector2(1, 1));
            selectedLabel.BringToFront();

            // Update labels above.
            float lastPosition = selectedTop;
            for (int i = _selectedIndex - 1; i >= 0; i--)
            {
                lastPosition -= (_labels[i].Height + Spacing) * 0.8f;
                UpdateLabelStyle(_labels[i].Label, lastPosition, i);
            }

            // Update labels below.
            lastPosition = selectedTop;
            for (int i = _selectedIndex + 1; i < _labels.Count; i++)
            {
                lastPosition += (_labels[i - 1].Height + Spacing) * 0.8f;
                UpdateLabelStyle(_labels[i].Label, lastPosition, i);
            }
        }

        private void UpdateLabelStyle(Label label, float topPosition, int index)
        {
            label.style.top = topPosition;
            float alpha = 1f - (Mathf.Abs(index - _selectedIndex) * 0.3f);
            label.style.opacity = Mathf.Clamp01(alpha);
            label.style.scale = new Scale(new Vector2(alpha, alpha));
        }

        private class LabelWithResolvedHeight
        {
            public Label Label;
            public float Height;
            public LabelWithResolvedHeight(Label label, float height)
            {
                this.Label = label;
                this.Height = height;
            }
        }
    }
}