using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class StringField : TextField
    {
        public new class UxmlFactory : UxmlFactory<StringField, UxmlTraits> { }

        private const string USS = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";
        private const string USS_FIELD_STRING = "panel-field-string";
        private const string USS_STRING_FIELD_ALT = "panel-field-stringfield_alt";
        private const string USS_TEXT_INPUT_READ_ONLY = "panel-field-container_readonly";
        private const string USS_UNITY_TEXT_INPUT = "unity-text-input";
        
        public sealed override string value
        {
            get => base.value;
            set => base.value = value;
        }

        public StringField() : this(""){}

        public StringField(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            AddToClassList(USS_CONTAINER);
            AddToClassList(USS_FIELD_STRING);

            onIsReadOnlyChanged += readOnly => EnableInClassList(USS_TEXT_INPUT_READ_ONLY, readOnly);
            multiline = true;
            this.isReadOnly = isReadOnly;
        }

        public StringField(string label, IProperty<string> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public StringField(string label, IPropertyReadOnly<string> property) : this(label, true)
        {
            this.BindProperty(property);
        }

        public StringField(string label, string text) : this(label, true)
        {
            value = text;
        }
        
        public void ToggleStringFieldAltStyle(bool toggle)
        {
            EnableInClassList(USS_STRING_FIELD_ALT, toggle);
        }

        public void SetTextInputAlign(TextAnchor anchor)
        {
            this.Q(USS_UNITY_TEXT_INPUT).style.unityTextAlign = new StyleEnum<TextAnchor>(anchor);
        }
    }
}