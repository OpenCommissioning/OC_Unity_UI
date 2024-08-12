using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class HorizontalGroup : VisualElement
    {
        private const string Uss = "StyleSheet/panel-field";
        private const string UssContainer = "panel-field-group_horizontal";
        
        public HorizontalGroup()
        {
            this.AddDefaultTheme();
            styleSheets.Add(Resources.Load<StyleSheet>(Uss));
            AddToClassList(UssContainer);

            style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        }

        public new void Add(VisualElement visualElement)
        {
            base.Add(visualElement);
            visualElement.style.flexGrow = 1;
        }
    }
}