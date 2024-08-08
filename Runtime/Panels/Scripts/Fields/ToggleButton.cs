using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class ToggleButton : Label
    {
        public new class UxmlFactory : UxmlFactory<ToggleButton, UxmlTraits> { }
        public new class UxmlTraits : UnityEngine.UIElements.Label.UxmlTraits { }
        
        public sealed override string text
        {
            get => base.text;
            set => base.text = value.ToUpper();
        }
        
        public bool Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                SetValueWithoutNotify(value);
            }
        }
        
        private const string Uss = "StyleSheet/panel-field";
        private const string UssContainer = "panel-field-container";
        private const string UssButton = "panel-field-button";
        private const string UssUnityButton = "unity-button";
        private const string UssButtonActive = "panel-field-button_active";

        private bool _value;
        private readonly IProperty<bool> _property;

        public ToggleButton()
        {
            this.AddDefaultTheme();
            styleSheets.Add(Resources.Load<StyleSheet>(Uss));
            AddToClassList(UssContainer);
            AddToClassList(UssButton);
            AddToClassList(UssUnityButton);
        }
        
        public ToggleButton(string label, IProperty<bool> property, IProperty<bool> enable) : this()
        {
            if (!string.IsNullOrEmpty(label)) text = label;

            _property = property;
            
            _property.ValueChanged += SetValueWithoutNotify;
            enable.ValueChanged += SetEnabled;

            RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            
            Value = _property.Value;
            SetEnabled(enable.Value);
        }
        
        private void SetValueWithoutNotify(bool value)
        {
            _value = value;
            EnableInClassList(UssButtonActive, value);
        }
        
        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            evt.StopPropagation();
            if (_property == null) return;
            _property.Value = !_property.Value;
        }
    }
}