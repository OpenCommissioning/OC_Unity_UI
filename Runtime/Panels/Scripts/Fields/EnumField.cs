using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class EnumField : UnityEngine.UIElements.EnumField
    {
        public new class UxmlFactory : UxmlFactory<EnumField, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";

        public EnumField() : this(""){}
        
        public EnumField(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
        }
    }
}