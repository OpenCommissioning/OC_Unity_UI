using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement("OCToggle")]
    public partial class Toggle : UnityEngine.UIElements.Toggle
    {
#else
    public class Toggle : UnityEngine.UIElements.Toggle
    {
        public new class UxmlFactory : UxmlFactory<Toggle, UxmlTraits> { }
#endif

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