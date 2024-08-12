using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public abstract class TextField<T> : TextField
    {
    
    
        public T ValidValue
        {
            get => _validValue;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_validValue, value))
                {
                    return;
                }
                _validValue = value;
                SetValueWithoutNotify(value.ToString());
            }
        }
        
        private T _validValue;
        private string _text;

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussLabel = "panel-field-label";
        private const string _ussInputText = "panel-field-text_input";
        

        protected TextField(Property<T> property, string label, bool isReadOnly) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
            
            labelElement.AddToClassList(_ussLabel);
            textInputBase.AddToClassList(_ussInputText);

            this.isReadOnly = isReadOnly;
            isDelayed = true;

            if (property == null) return;
            RegisterCallback<ChangeEvent<string>>(evt => ValidateInput(property, evt.newValue));
            property.ValueChanged += i => ValidValue = i;
            SetValueWithoutNotify(property.Value.ToString());
        }

        public sealed override void SetValueWithoutNotify(string newValue)
        {
            base.SetValueWithoutNotify(newValue);
        }

        private void ValidateInput(Property<T> property, string newText)
        {
            if (Validate(newText, out T validValue))
            {
                property.Value = validValue;
                _text = newText;
            }
            else
            {
                SetValueWithoutNotify(_text);
            }
        }
        
        protected abstract bool Validate(string text, out T validValue);
    } 
}


