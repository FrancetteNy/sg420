using UnityEngine;
using UnityEngine.UIElements;
//https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-slide-toggle.html
namespace MyUILibrary
{
    // Derives from BaseField<bool> base class. Represents a container for its input part.
    [UxmlElement]
    public partial class SlideToggle : BaseField<bool>
    {
        [UxmlAttribute]
        public string RightLabelValue
        {
            get => _rightLabel?.text;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _rightLabel.style.display = DisplayStyle.Flex;
                    _rightLabel.text = value;
                }
                else if (_rightLabel != null)
                {
                    _rightLabel.style.display = DisplayStyle.None;
                }
            }
        }
        // In the spirit of the BEM standard, the SlideToggle has its own block class and two element classes. It also
        // has a class that represents the enabled state of the toggle.
#pragma warning disable IDE1006 // Naming Styles
        public static readonly new string ussClassName = "slide-toggle";
        public static readonly new string inputUssClassName = "slide-toggle__input";
        public static readonly string inputKnobUssClassName = "slide-toggle__input-knob";
        public static readonly string inputCheckedUssClassName = "slide-toggle__input--checked";
#pragma warning restore IDE1006 // Naming Styles

        VisualElement _input;
        VisualElement _knob;
        Label _leftLabel;   
        Label _rightLabel;

        // Custom controls need a default constructor. This default constructor calls the other constructor in this
        // class.
        public SlideToggle() : this(null, null) { }

        // This constructor allows users to set the contents of the label.
        public SlideToggle(string leftLabelText, string rightLabelText) : base(leftLabelText, null)
        {
            // Style the control overall.
            AddToClassList(ussClassName);

            // Get the BaseField's visual input element and use it as the background of the slide.
            _input = this.Q(className: BaseField<bool>.inputUssClassName);
            _input.AddToClassList(inputUssClassName);
            _input.name = "input";

            // Create a "knob" child element for the background to represent the actual slide of the toggle.
            _knob = new();
            _knob.AddToClassList(inputKnobUssClassName);
            _knob.name = "knob";
            _input.Add(_knob);

            _leftLabel = this.labelElement;
            _leftLabel.name = "left-label";
            _leftLabel.RemoveFromClassList(BaseField<bool>.labelUssClassName);
            if (rightLabelText != null)
            {
                RightLabelValue = rightLabelText;
            }
            _rightLabel = new();
            _rightLabel.name = "right-label";
            _rightLabel.text = rightLabelText;
            if (string.IsNullOrEmpty(RightLabelValue))
            {
                _rightLabel.style.display = DisplayStyle.None;
            }
            this.Add(_rightLabel);
            // There are three main ways to activate or deactivate the SlideToggle. All three event handlers use the
            // static function pattern described in the Custom control best practices.

            // ClickEvent fires when a sequence of pointer down and pointer up actions occurs.
            RegisterCallback<ClickEvent>(evt => OnClick(evt));
            // KeydownEvent fires when the field has focus and a user presses a key.
            RegisterCallback<KeyDownEvent>(evt => OnKeydownEvent(evt));
            // NavigationSubmitEvent detects input from keyboards, gamepads, or other devices at runtime.
            RegisterCallback<NavigationSubmitEvent>(evt => OnSubmit(evt));
        }

        static void OnClick(ClickEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;
            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        static void OnSubmit(NavigationSubmitEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;
            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        static void OnKeydownEvent(KeyDownEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;

            // NavigationSubmitEvent event already covers keydown events at runtime, so this method shouldn't handle
            // them.
            if (slideToggle.panel?.contextType == ContextType.Player)
                return;

            // Toggle the value only when the user presses Enter, Return, or Space.
            if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
            {
                slideToggle.ToggleValue();
                evt.StopPropagation();
            }
        }

        // All three callbacks call this method.
        void ToggleValue()
        {
            value = !value;
        }

        // Because ToggleValue() sets the value property, the BaseField class dispatches a ChangeEvent. This results in a
        // call to SetValueWithoutNotify(). This example uses it to style the toggle based on whether it's currently
        // enabled.
        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);

            //This line of code styles the input element to look enabled or disabled.
            _input.EnableInClassList(inputCheckedUssClassName, newValue);
        }
    }
}