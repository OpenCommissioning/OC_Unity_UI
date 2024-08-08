using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class Vector3IntField : UnityEngine.UIElements.Vector3IntField
    {
        public new class UxmlFactory : UxmlFactory<Vector3IntField, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussVector = "panel-field-vector";
        private const string _ussVectorInputField = "panel-field-vector__input-field";
        private const string _ussTextInputReadOnly = "panel-field-container_readonly";

        public Vector3IntField() : this(""){}

        public Vector3IntField(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
            AddToClassList(_ussVector);

            foreach (UnityEngine.UIElements.IntegerField intField in this.Query<UnityEngine.UIElements.IntegerField>().ToList())
            {
                intField.AddToClassList(_ussContainer);
                intField.AddToClassList(_ussVectorInputField);
                intField.EnableInClassList(_ussTextInputReadOnly, isReadOnly);
                intField.isReadOnly = isReadOnly;
            }
        }

        public Vector3IntField(string label, IProperty<Vector3Int> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public Vector3IntField(string label, Property<Vector3Int> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public Vector3IntField(string label, Vector3Int value, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.value = value;
        }
    }
}