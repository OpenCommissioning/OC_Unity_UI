using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class SubsystemPanel : VisualElement, ICloseble
    {
        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable == value) return;
                EnableWithoutNotification(value);
                OnEnableChanged?.Invoke(value);
            }
        }

        public event Action<bool> OnEnableChanged;

        private bool _enable;

        private const string Uxml = "UXML/panel_subsystem";
        private const string StyleSheet = "StyleSheet/panel";
        private const string UssContainer = "panel-container";
        private const string UssContainerSybsystem = "panel-container_subsystem";
        
        private readonly VisualElement _content;
        
        public SubsystemPanel(string title)
        {
            Enable = true;
            this.AddDefaultTheme();
            styleSheets.Add(Resources.Load<StyleSheet>(StyleSheet));
            AddToClassList(UssContainer);
            AddToClassList(UssContainerSybsystem);
            
            var container = Resources.Load<VisualTreeAsset>(Uxml).Instantiate();
            var header = container.Q("header");
            _content = container.Q("content");
            
            hierarchy.Add(header);
            hierarchy.Add(_content);

            this.AddManipulator(new Moveable(header));
            
            header.Q<VisualElement>("close").RegisterCallback<ClickEvent>(_ => Close());
            header.Q<Label>("title").text = title;
            name = title.ToLower().Replace(" ", "_");
        }

        private void EnableWithoutNotification(bool enable)
        {
            _enable = enable;
            style.display = _enable ? DisplayStyle.Flex : DisplayStyle.None;

            if (enable)
            {
                UIManager.Instance.Register(this);
            }
            else
            {
                UIManager.Instance.Unregister(this);
            }
        }

        public new void Add(VisualElement visualElement)
        {
            _content.Add(visualElement);
        }

        public void Close()
        {
            Enable = false;
        }
    }
}
