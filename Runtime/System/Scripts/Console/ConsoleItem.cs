using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Console
{
    public class ConsoleItem : VisualElement
    {
        public string Message
        {
            get => _label.text;
            set => _label.text = value;
        }

        public Label Label => _label;
        public VisualElement Icon => _icon;

        private const string StyleResource = "StyleSheet/console";
        private const string UssConsoleItem = "console-item";
        private const string UssConsoleItemLabel = "console-item__label";
        private const string UssConsoleItemIconContainer = "console-item__icon-container";
        private const string UssConsoleItemIcon = "console-item__icon";
        
        private readonly Label _label;
        private readonly VisualElement _icon;

        public ConsoleItem()
        {
            var container = new VisualElement();
            container.AddToClassList(UssConsoleItemIconContainer);

            _icon = new VisualElement();
            _icon.AddToClassList(UssConsoleItemIcon);

            container.Add(_icon);

            styleSheets.Add(Resources.Load<StyleSheet>(StyleResource));
            AddToClassList(UssConsoleItem);

            _label = new Label();
            _label.AddToClassList(UssConsoleItemLabel);

            Add(container);
            Add(_label);
        }
    } 
}
