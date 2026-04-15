using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement("OCSliderInt")]
    public partial class SliderInt : UnityEngine.UIElements.SliderInt
    {
#else
    public class SliderInt : UnityEngine.UIElements.SliderInt
    {
        public new class UxmlFactory : UxmlFactory<SliderInt, UxmlTraits> { }
#endif

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";
        private const string USS_SLIDER = "panel-field-slider";

        public SliderInt() : this(""){}
        
        public SliderInt(string label) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);
            AddToClassList(USS_SLIDER);
        }

        public SliderInt(string label, Property<int> property) : this(label)
        {
            this.BindProperty(property);
        }
    }
}