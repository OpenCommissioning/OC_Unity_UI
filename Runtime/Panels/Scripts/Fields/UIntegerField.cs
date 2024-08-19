using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class UIntegerField : UnsignedIntegerField
    {
        public new class UxmlFactory : UxmlFactory<UIntegerField, UxmlTraits> { }

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";
        private const string USS_TEXT_INPUT_READ_ONLY = "panel-field-container_readonly";


        public UIntegerField() : this(""){}
        
        public UIntegerField(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);

            onIsReadOnlyChanged += readOnly => EnableInClassList(USS_TEXT_INPUT_READ_ONLY, readOnly);
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