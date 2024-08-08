using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class IntegerField : UnityEngine.UIElements.IntegerField
    {
        public new class UxmlFactory : UxmlFactory<IntegerField, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussTextInputReadOnly = "panel-field-container_readonly";

        public IntegerField() : this ("") {}

        public IntegerField(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);

            onIsReadOnlyChanged += value => EnableInClassList(_ussTextInputReadOnly, value);
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