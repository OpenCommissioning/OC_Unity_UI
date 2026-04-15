using OC.Interactions.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement]
    public partial class PanelPushButton : Label
    {
#else
    public class PanelPushButton : Label
    {
        public new class UxmlFactory : UxmlFactory<PanelPushButton, UxmlTraits> { }
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
        
        private readonly MouseEvents _mouseEvents;
        
        private bool _value;
        

        public PanelPushButton()
        {
            this.AddDefaultTheme();
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            AddToClassList(USS_CONTAINER);
            AddToClassList(USS_BUTTON);
            AddToClassList(USS_UNITY_BUTTON);
            
            _mouseEvents = new MouseEvents()
            {
                target = this
            };
        }
        
        public PanelPushButton(string label, IProperty<bool> property, IProperty<bool> enable) : this()
        {
            if (!string.IsNullOrEmpty(label)) text = label;

            _mouseEvents.Up += () => property.Value = false;
            _mouseEvents.Down += () => property.Value = true;
            
            property.OnValueChanged += SetValueWithoutNotify;
            enable.OnValueChanged += SetEnabled;

            Value = property.Value;
            SetEnabled(enable.Value);
        }
        
        private void SetValueWithoutNotify(bool value)
        {
            _value = value;
            EnableInClassList(USS_BUTTON_ACTIVE, value);
        }
    }
}