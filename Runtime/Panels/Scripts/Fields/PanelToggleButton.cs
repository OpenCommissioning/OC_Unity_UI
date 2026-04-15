using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement("PanelToggleButton")]
    public partial class PanelToggleButton : Label
    {
#else
    public class PanelToggleButton : Label
    {
        public new class UxmlFactory : UxmlFactory<PanelToggleButton, UxmlTraits> { }
        public new class UxmlTraits : UnityEngine.UIElements.Label.UxmlTraits { }
#endif

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
        
        private const string USS = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";
        private const string USS_BUTTON = "panel-field-button";
        private const string USS_UNITY_BUTTON = "unity-button";
        private const string USS_BUTTON_ACTIVE = "panel-field-button_active";

        private bool _value;
        private readonly IProperty<bool> _property;

        public PanelToggleButton()
        {
            this.AddDefaultTheme();
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            AddToClassList(USS_CONTAINER);
            AddToClassList(USS_BUTTON);
            AddToClassList(USS_UNITY_BUTTON);
        }
        
        public PanelToggleButton(string label, IProperty<bool> property, IProperty<bool> enable) : this()
        {
            if (!string.IsNullOrEmpty(label)) text = label;

            _property = property;
            
            _property.OnValueChanged += SetValueWithoutNotify;
            enable.OnValueChanged += SetEnabled;

            RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            
            Value = _property.Value;
            SetEnabled(enable.Value);
        }
        
        private void SetValueWithoutNotify(bool value)
        {
            _value = value;
            EnableInClassList(USS_BUTTON_ACTIVE, value);
        }
        
        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            evt.StopPropagation();
            if (_property == null) return;
            _property.Value = !_property.Value;
        }
    }
}