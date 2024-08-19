using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class EnumField : UnityEngine.UIElements.EnumField
    {
        public new class UxmlFactory : UxmlFactory<EnumField, UxmlTraits> { }

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";

        public EnumField() : this(""){}
        
        public EnumField(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);
        }
    }
}