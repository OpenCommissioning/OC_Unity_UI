using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class GroupContainer : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<GroupContainer, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _label =  new() { name = "Label", defaultValue = "Label" };
        
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is GroupContainer ate) ate.Label = _label.GetValueFromBag(bag, cc);
            }
        }

        public string Label
        {
            get => _label.text;
            set => _label.text = value.ToUpper();
        }
        
        private const string Uss = "StyleSheet/panel-field";
        private const string UssContainer = "panel-field-container";
        private const string UssGroup = "panel-field-group";
        private const string UssGroupHeader = "panel-field-group-header";
        private const string UssGroupHeaderOptions = "panel-field-group-header_options";

        private bool _collapsed;
        private readonly Label _label;
        private readonly VisualElement _header;
        private readonly VisualElement _headerOptions;
        private readonly VisualElement _content;

        public GroupContainer() : this("") { }
        
        public GroupContainer(string label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(Uss));
            AddToClassList(UssContainer);
            AddToClassList(UssGroup);

            var header = new VisualElement()
            {
                name = "header"
            };
            
            _headerOptions = new VisualElement()
            {
                name = "headerOptions"
            };
            
            header.AddToClassList(UssGroupHeader);
            _headerOptions.AddToClassList(UssGroupHeaderOptions);

            _label = new Label();
            if (!string.IsNullOrEmpty(label)) _label.text = label.ToUpper();
            
            header.Add(_label);
            header.Add(_headerOptions);

            var bar = new VisualElement
            {
                name = "bar"
            };

            _content = new VisualElement()
            {
                name = "content"
            };
            
            
            hierarchy.Add(header);
            hierarchy.Add(bar);
            hierarchy.Add(_content);
            
            _label.RegisterCallback<ClickEvent>(Collapse);
        }
        
        private void Collapse(ClickEvent evt)
        {
            _collapsed = !_collapsed;
            _label.style.unityFontStyleAndWeight = _collapsed ? FontStyle.Bold : FontStyle.Normal;
            _content.style.display = _collapsed ? DisplayStyle.None : DisplayStyle.Flex;
        }

        public new void Add(VisualElement visualElement)
        {
            _content.Add(visualElement);
        }

        public void AddToHeader(VisualElement visualElement)
        {
            _headerOptions.Add(visualElement);
        }
    }
}
