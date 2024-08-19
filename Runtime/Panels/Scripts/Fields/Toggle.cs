using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class Toggle : UnityEngine.UIElements.Toggle
    {
        public new class UxmlFactory : UxmlFactory<Toggle, UxmlTraits> { }

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";
        private const string USS_TOGGLE_CHECKBOX = "panel-field-toggle";

        public Toggle() : this("") {}

        public Toggle(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);
            AddToClassList(USS_TOGGLE_CHECKBOX);
        }

        public Toggle(string label, Property<bool> property) : this(label) 
        {
            this.BindProperty(property);
        }
    }
}