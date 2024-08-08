using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class LongField : UnityEngine.UIElements.LongField
    {
        public new class UxmlFactory : UxmlFactory<LongField, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussTextInputReadOnly = "panel-field-container_readonly";

        public LongField() : this(""){}
        
        public LongField(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);

            onIsReadOnlyChanged += value => EnableInClassList(_ussTextInputReadOnly, value);
            this.isReadOnly = isReadOnly;
        }

        public LongField(string label, IProperty<long> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public LongField(string label, Property<long> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public LongField(string label, long value, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.value = value;
        }
    }
}