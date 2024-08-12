using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class FloatField : UnityEngine.UIElements.FloatField
    {
        public new class UxmlFactory : UxmlFactory<FloatField, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussTextInputReadOnly = "panel-field-container_readonly";

        public FloatField() : this(""){}

        public FloatField(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
            onIsReadOnlyChanged += value => EnableInClassList(_ussTextInputReadOnly, value);
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