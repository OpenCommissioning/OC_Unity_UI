using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class DropdownField : UnityEngine.UIElements.DropdownField
    {
        public new class UxmlFactory : UxmlFactory<DropdownField, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";

        public DropdownField() : this(""){}
        
        public DropdownField(string label) : base()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
        }
    }
}