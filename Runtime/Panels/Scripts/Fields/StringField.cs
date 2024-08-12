using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class StringField : TextField
    {
        public new class UxmlFactory : UxmlFactory<StringField, UxmlTraits> { }

        private const string Uss = "StyleSheet/panel-field";
        private const string UssContainer = "panel-field-container";
        private const string UssFieldString = "panel-field-string";
        private const string UssStringFieldAlt = "panel-field-stringfield_alt";
        private const string UssTextInputReadOnly = "panel-field-container_readonly";
        private const string UssUnityTextInput = "unity-text-input";
        
        public sealed override string value
        {
            get => base.value;
            set => base.value = value;
        }

        public StringField() : this(""){}

        public StringField(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(Uss));
            AddToClassList(UssContainer);
            AddToClassList(UssFieldString);

            onIsReadOnlyChanged += readOnly => EnableInClassList(UssTextInputReadOnly, readOnly);
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
            EnableInClassList(UssStringFieldAlt, toggle);
        }

        public void SetTextInputAlign(TextAnchor anchor)
        {
            this.Q(UssUnityTextInput).style.unityTextAlign = new StyleEnum<TextAnchor>(anchor);
        }
    }
}