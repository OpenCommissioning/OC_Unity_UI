using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class DropdownField : UnityEngine.UIElements.DropdownField
    {
        public new class UxmlFactory : UxmlFactory<DropdownField, UxmlTraits> { }

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";

        public DropdownField() : this(""){}

        public DropdownField(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);
        }
    }
}