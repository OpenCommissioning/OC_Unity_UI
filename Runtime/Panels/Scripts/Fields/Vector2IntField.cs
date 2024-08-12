using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class Vector2IntField : UnityEngine.UIElements.Vector2IntField
    {
        public new class UxmlFactory : UxmlFactory<Vector2IntField, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussVector = "panel-field-vector";
        private const string _ussVectorInputField = "panel-field-vector__input-field";
        private const string _ussTextInputReadOnly = "panel-field-container_readonly";

        public Vector2IntField() : this(""){}

        public Vector2IntField(string label, bool isReadOnly = false) : base(label)
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

        public Vector2IntField(string label, IProperty<Vector2Int> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public Vector2IntField(string label, Property<Vector2Int> property, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.BindProperty(property);
        }

        public Vector2IntField(string label, Vector2Int value, bool isReadOnly = false) : this(label, isReadOnly)
        {
            this.value = value;
        }
    }
}