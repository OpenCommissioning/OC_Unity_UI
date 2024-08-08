using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Industrial
{
    public class ComponentsGroup : VisualElement
    {
        private const string StyleSheet = "StyleSheet/industrial-group";
        private const string UssContainer = "group-container"; 
        private const string UssLabel = "group-label";
        private const string UssContent = "group-content";

        private readonly VisualElement _content;

        public ComponentsGroup(string name)
        {
            this.AddDefaultTheme();
            styleSheets.Add(Resources.Load<StyleSheet>(StyleSheet));
            AddToClassList(UssContainer);
            
            var label = new Label(name.ToUpper());
            label.AddToClassList(UssLabel);
            hierarchy.Add(label);
            
            _content = new VisualElement();
            _content.AddToClassList(UssContent);
            hierarchy.Add(_content);
        }
        
        public new void Add(VisualElement element)
        {
            _content.Add(element);
        }
    }
}


