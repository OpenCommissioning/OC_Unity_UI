using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement("OCEnumField")]
    public partial class EnumField : UnityEngine.UIElements.EnumField
    {
#else
    public class EnumField : UnityEngine.UIElements.EnumField
    {
        public new class UxmlFactory : UxmlFactory<EnumField, UxmlTraits> { }
#endif

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