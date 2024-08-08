using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class MinMaxSlider : UnityEngine.UIElements.MinMaxSlider
    {
        public new class UxmlFactory : UxmlFactory<MinMaxSlider, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _usslider = "panel-field-slider";

        public MinMaxSlider() : this(""){}
        
        public MinMaxSlider(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
            AddToClassList(_usslider);
        }
    }
}