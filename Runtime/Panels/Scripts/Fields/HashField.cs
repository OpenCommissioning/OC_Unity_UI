using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class HashField : UnityEngine.UIElements.Hash128Field
    {
        public new class UxmlFactory : UxmlFactory<HashField, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";

        public HashField() : this(""){}
        
        public HashField(string label, bool isReadOnly = false) : base(label)
        {
            this.isReadOnly = isReadOnly;
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
        }
    }
}