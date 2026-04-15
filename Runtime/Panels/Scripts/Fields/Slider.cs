using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement("OCSlider")]
    public partial class Slider : UnityEngine.UIElements.Slider
    {
#else
    public class Slider : UnityEngine.UIElements.Slider
    {
        public new class UxmlFactory : UxmlFactory<Slider, UxmlTraits> { }
#endif

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";
        private const string USS_SLIDER = "panel-field-slider";

        public Slider() : this(""){}
        
        public Slider(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);
            AddToClassList(USS_SLIDER);
        }

        public Slider(string label, Property<float> property) : this(label)
        {
            this.BindProperty(property);
        }
    }
}