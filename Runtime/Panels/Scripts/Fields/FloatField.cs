using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class FloatField : UnityEngine.UIElements.FloatField
    {
        public new class UxmlFactory : UxmlFactory<FloatField, UxmlTraits> { }

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";
        private const string USS_TEXT_INPUT_READ_ONLY = "panel-field-container_readonly";

        public FloatField() : this(""){}

        public FloatField(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);
            onIsReadOnlyChanged += readOnly => EnableInClassList(USS_TEXT_INPUT_READ_ONLY, readOnly);
            this.isReadOnly = isReadOnly;
            formatString = "f3";
        }

        public FloatField(string label, IProperty<float> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public FloatField(string label, IPropertyReadOnly<float> property, bool isReadOnly = true) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public FloatField(string label, Property<float> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public FloatField(string label, IProperty<float> property, IProperty<bool> isReadOnly) : this(label)
        {
            this.BindProperty(property);
            this.BindReadOnlyProperty(isReadOnly);
        }

        public FloatField(string label, Property<float> property, IProperty<bool> isReadOnly) : this(label)
        {
            this.BindProperty(property);
            this.BindReadOnlyProperty(isReadOnly);
        }

        public FloatField(string label, float value, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.value = value;
        }
    }
}