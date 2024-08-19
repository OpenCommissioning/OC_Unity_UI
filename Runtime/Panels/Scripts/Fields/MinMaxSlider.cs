using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class MinMaxSlider : UnityEngine.UIElements.MinMaxSlider
    {
        public new class UxmlFactory : UxmlFactory<MinMaxSlider, UxmlTraits> { }

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";
        private const string USS_SLIDER = "panel-field-slider";

        public MinMaxSlider() : this(""){}
        
        public MinMaxSlider(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);
            AddToClassList(USS_SLIDER);
        }
    }
}