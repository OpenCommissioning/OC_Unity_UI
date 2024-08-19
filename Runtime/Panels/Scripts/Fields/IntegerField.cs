using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class IntegerField : UnityEngine.UIElements.IntegerField
    {
        public new class UxmlFactory : UxmlFactory<IntegerField, UxmlTraits> { }

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";
        private const string USS_TEXT_INPUT_READ_ONLY = "panel-field-container_readonly";

        public IntegerField() : this ("") {}

        public IntegerField(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);

            onIsReadOnlyChanged += readOnly => EnableInClassList(USS_TEXT_INPUT_READ_ONLY, readOnly);
            this.isReadOnly = isReadOnly;
        }

        public IntegerField(string label, IProperty<int> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public IntegerField(string label, Property<int> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public IntegerField(string label, int value, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.value = value;
        }
    }
}