using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class Vector3Field : UnityEngine.UIElements.Vector3Field
    {
        public new class UxmlFactory : UxmlFactory<Vector3Field, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussVector = "panel-field-vector";
        private const string _ussVectorInputField = "panel-field-vector__input-field";
        private const string _ussTextInputReadOnly = "panel-field-container_readonly";

        public Vector3Field() : this(""){}

        public Vector3Field(string label, bool isReadOnly = false) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
            AddToClassList(_ussVector);

            foreach (UnityEngine.UIElements.FloatField floatField in this.Query<UnityEngine.UIElements.FloatField>().ToList())
            {
                floatField.AddToClassList(_ussContainer);
                floatField.AddToClassList(_ussVectorInputField);
                floatField.EnableInClassList(_ussTextInputReadOnly, isReadOnly);
                floatField.isReadOnly = isReadOnly;
            }
        }

        public Vector3Field(string label, IProperty<Vector3> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public Vector3Field(string label, Property<Vector3> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public Vector3Field(string label, Vector3 value, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.value = value;
        }
    }
}