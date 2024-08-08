using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    [RequireComponent(typeof(UIDocument))]
    [DisallowMultipleComponent]
    public class InteractionsPanelManager : MonoBehaviour
    {
        public static InteractionsPanelManager Instance { get; private set; }
        public VisualElement Sidebar => _sidebar;

        public float DockThreshold = 10f;

        private VisualElement _screen;
        private UnityEngine.UIElements.ScrollView _sidebar;
        private readonly List<Panel> _panels = new ();

        private const string _uxml = "UXML/panel-sidebar";
        private const string _styleSheet = "StyleSheet/panel-sidebar";
        private const string _ussSidebarActive = "sidebar-scrollView";
        private const string _ussSidebarDisabled = "sidebar-scrollView_disabled";
        private const string _ussDockedPanel = "panel-container__docked";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            _screen = uiDocument.rootVisualElement;
            _sidebar = Resources.Load<VisualTreeAsset>(_uxml).Instantiate().Q<UnityEngine.UIElements.ScrollView>("sidebar");
            _sidebar.styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            _sidebar.AddToClassList(_ussSidebarActive);
            RefreshScrollViewStyle();
            _screen.Add(_sidebar);
            _sidebar.SendToBack();
        }

        public void Register(Panel panel)
        {
            if (_panels.Contains(panel)) return; 
            _panels.Add(panel);
            RefreshScrollViewStyle();
        }
        
        public void Unregister(Panel panel)
        {
            if (!_panels.Contains(panel)) return;
            _panels.Remove(panel);
            RefreshScrollViewStyle();
        }
        
        public void AddToScreen(VisualElement visualElement)
        {
            _screen.Add(visualElement);
            visualElement.style.position = new StyleEnum<Position>(Position.Absolute);
            visualElement.RemoveFromClassList(_ussDockedPanel);
            RefreshScrollViewStyle();
        }
        
        public void AddToSidebar(VisualElement visualElement)
        {
            visualElement.style.position = new StyleEnum<Position>(Position.Relative);
            visualElement.AddToClassList(_ussDockedPanel);
            _sidebar.Add(visualElement);
            RefreshScrollViewStyle();
        }

        private void RefreshScrollViewStyle()
        {
            _sidebar.EnableInClassList(_ussSidebarDisabled, _sidebar.childCount == 0);
        }
    }
}


