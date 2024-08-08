using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class Toggle : UnityEngine.UIElements.Toggle
    {
        public new class UxmlFactory : UxmlFactory<Toggle, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussToggleCheckbox = "panel-field-toggle";

        public Toggle() : this("") {}

        public Toggle(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
            AddToClassList(_ussToggleCheckbox);
        }

        public Toggle(string label, Property<bool> property) : this(label) 
        {
            this.BindProperty(property);
        }
    }
}