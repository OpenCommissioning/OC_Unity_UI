using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class Slider : UnityEngine.UIElements.Slider
    {
        public new class UxmlFactory : UxmlFactory<Slider, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussSlider = "panel-field-slider";

        public Slider() : this(""){}
        
        public Slider(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
            AddToClassList(_ussSlider);
        }

        public Slider(string label, Property<float> property) : this(label)
        {
            this.BindProperty(property);
        }
    }
}