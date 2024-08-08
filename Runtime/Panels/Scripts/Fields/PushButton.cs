using IOSEF.Interactions.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class PushButton : Label
    {
        public new class UxmlFactory : UxmlFactory<PushButton, UxmlTraits> { }
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
        
        private readonly MouseEvents _mouseEvents;
        
        private bool _value;
        

        public PushButton()
        {
            this.AddDefaultTheme();
            styleSheets.Add(Resources.Load<StyleSheet>(Uss));
            AddToClassList(UssContainer);
            AddToClassList(UssButton);
            AddToClassList(UssUnityButton);
            
            _mouseEvents = new MouseEvents()
            {
                target = this
            };
        }
        
        public PushButton(string label, IProperty<bool> property, IProperty<bool> enable) : this()
        {
            if (!string.IsNullOrEmpty(label)) text = label;

            _mouseEvents.Up += () => property.Value = false;
            _mouseEvents.Down += () => property.Value = true;
            
            property.ValueChanged += SetValueWithoutNotify;
            enable.ValueChanged += SetEnabled;

            Value = property.Value;
            SetEnabled(enable.Value);
        }
        
        private void SetValueWithoutNotify(bool value)
        {
            _value = value;
            EnableInClassList(UssButtonActive, value);
        }
    }
}