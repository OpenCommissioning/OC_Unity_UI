using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement("OCMinMaxSlider")]
    public partial class MinMaxSlider : UnityEngine.UIElements.MinMaxSlider
    {
#else
    public class MinMaxSlider : UnityEngine.UIElements.MinMaxSlider
    {
        public new class UxmlFactory : UxmlFactory<MinMaxSlider, UxmlTraits> { }
#endif

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