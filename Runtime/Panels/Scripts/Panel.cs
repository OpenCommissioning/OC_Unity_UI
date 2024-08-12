using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class Panel : VisualElement, ICloseble
    {
        public bool CanClosed
        {
            set => _closeButton.style.display = value ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex) : new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }
        
        public bool CanFocus
        {
            set => _focusButton.style.display = value ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex) : new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }
        
        public bool CanPinned
        {
            get => _canPinned;
            set
            {
                _canPinned = value;
                _pinButton.style.display = _canPinned ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex) : new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }

        private const string Uxml = "UXML/panel_component";
        private const string Uss = "StyleSheet/panel";
        private const string UssContainer = "panel-container";
        private const string UssButtonActive = "panel-header-button-active";
        private const string UssComponentPanel = "component-panel";

        private readonly VisualElement _content;
        private readonly UnityEngine.UIElements.Button _focusButton;
        private readonly UnityEngine.UIElements.Button _pinButton;
        private readonly UnityEngine.UIElements.Button _closeButton;
        
        private bool _isPinned;
        private bool _canPinned;

        public event Action OnClose;
        public event Action OnFocus;

        public Panel(string label, bool canClose = true, bool canPinned = false, bool canFocus = false)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(Uss));
            this.AddDefaultTheme();
            AddToClassList(UssComponentPanel);
            AddToClassList(UssContainer);

            var template = Resources.Load<VisualTreeAsset>(Uxml).CloneTree();
            var header = template.Q("header");
            _content = template.Q("content");
            
            hierarchy.Add(header);
            hierarchy.Add(_content);
            this.AddManipulator(new DragAndDrop(header, this));

            header.Q<Label>("title").text = label;

            _focusButton = header.Q<UnityEngine.UIElements.Button>("focus");
            _focusButton.clicked += () => { OnFocus?.Invoke(); };
            
            _closeButton = header.Q<UnityEngine.UIElements.Button>("close");
            _closeButton.clicked += () => { OnClose?.Invoke(); };

            _pinButton = header.Q<UnityEngine.UIElements.Button>("pin");
            _pinButton.clicked += PinOnclicked;

            InteractionsPanelManager.Instance.Register(this);
            InteractionsPanelManager.Instance.AddToSidebar(this);
            UIManager.Instance.Register(this);
            
            CanClosed = canClose;
            CanFocus = canFocus;
            CanPinned = canPinned;
        }

        private void PinOnclicked()
        {
            _isPinned = !_isPinned;
            _pinButton.EnableInClassList(UssButtonActive, _isPinned);
        }

        public new void Add(VisualElement visualElement)
        {
            _content.Add(visualElement);
        }

        public void Close()
        {
            if (!_canPinned) return;
            if (_isPinned) return;
            OnClose?.Invoke();
        }
        
        public void Delete()
        {
            RemoveFromHierarchy();
            InteractionsPanelManager.Instance.Unregister(this);
            UIManager.Instance.Unregister(this);
        }
    }
}