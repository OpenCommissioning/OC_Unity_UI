using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class SliderInt : UnityEngine.UIElements.SliderInt
    {
        public new class UxmlFactory : UxmlFactory<SliderInt, UxmlTraits> { }

        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussSlider = "panel-field-slider";

        public SliderInt() : this(""){}
        
        public SliderInt(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
            AddToClassList(_ussSlider);
        }

        public SliderInt(string label, Property<int> property) : this(label)
        {
            this.BindProperty(property);
        }
    }
}