using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class UIntegerField : UnityEngine.UIElements.UnsignedIntegerField
    {
        public new class UxmlFactory : UxmlFactory<UIntegerField, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussTextInputReadOnly = "panel-field-container_readonly";


        public UIntegerField() : this(""){}
        
        public UIntegerField(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);

            onIsReadOnlyChanged += value => EnableInClassList(_ussTextInputReadOnly, value);
            this.isReadOnly = isReadOnly;
        }

        public UIntegerField(string label, IProperty<uint> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public UIntegerField(string label, IPropertyReadOnly<uint> property, bool isReadOnly = true) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public UIntegerField(string label, Property<uint> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public UIntegerField(string label, uint value, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.value = value;
        }
    }
}