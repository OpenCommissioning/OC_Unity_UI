using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class Vector2Field : UnityEngine.UIElements.Vector2Field
    {
        public new class UxmlFactory : UxmlFactory<Vector2Field, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussVector = "panel-field-vector";
        private const string _ussVectorInputField = "panel-field-vector__input-field";
        private const string _ussTextInputReadOnly = "panel-field-container_readonly";

        public Vector2Field() : this(""){}

        public Vector2Field(string label, bool isReadOnly = false) : base(label)
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

        public Vector2Field(string label, IProperty<Vector2> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public Vector2Field(string label, Property<Vector2> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public Vector2Field(string label, Vector2 value, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.value = value;
        }
    }
}