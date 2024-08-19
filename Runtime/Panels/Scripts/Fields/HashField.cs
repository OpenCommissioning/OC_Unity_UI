using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class HashField : Hash128Field
    {
        public new class UxmlFactory : UxmlFactory<HashField, UxmlTraits> { }

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";

        public HashField() : this(""){}
        
        public HashField(string label, bool isReadOnly = false) : base(label)
        {
            this.isReadOnly = isReadOnly;
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);
        }
    }
}