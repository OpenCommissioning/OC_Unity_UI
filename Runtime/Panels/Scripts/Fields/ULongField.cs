using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class ULongField : UnityEngine.UIElements.UnsignedLongField
    {
        public new class UxmlFactory : UxmlFactory<ULongField, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussTextInputReadOnly = "panel-field-container_readonly";


        public ULongField() : this("") { }
        
        public ULongField(string label, bool isReadonly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);

            onIsReadOnlyChanged += value => EnableInClassList(_ussTextInputReadOnly, value);
            this.isReadOnly = isReadonly;
        }

        public ULongField(string label, IProperty<ulong> property, bool isReadonly = false) : this(label, isReadonly)
        {
            this.BindProperty(property);
        }

        public ULongField(string label, IPropertyReadOnly<ulong> property, bool isReadonly = true) : this(label, isReadonly)
        {
            this.BindProperty(property);
        }

        public ULongField(string label, Property<ulong> property, bool isReadonly = false) : this(label, isReadonly)
        {
            this.BindProperty(property);
        }

        public ULongField(string label, IProperty<ulong> property, IProperty<bool> isReadOnly) : this(label)
        {
            this.BindProperty(property);
            this.BindReadOnlyProperty(isReadOnly);
        }

        public ULongField(string label, Property<ulong> property, IProperty<bool> isReadOnly) : this(label)
        {
            this.BindProperty(property);
            this.BindReadOnlyProperty(isReadOnly);
        }

        public ULongField(string label, ulong value, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.value = value;
        }
    }
}