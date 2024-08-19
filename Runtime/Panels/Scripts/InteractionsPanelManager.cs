using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
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

        private const string UXML = "UXML/panel-sidebar";
        private const string STYLE_SHEET = "StyleSheet/panel-sidebar";
        private const string USS_SIDEBAR_ACTIVE = "sidebar-scrollView";
        private const string USS_SIDEBAR_DISABLED = "sidebar-scrollView_disabled";
        private const string USS_DOCKED_PANEL = "panel-container__docked";

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
            _sidebar = Resources.Load<VisualTreeAsset>(UXML).Instantiate().Q<UnityEngine.UIElements.ScrollView>("sidebar");
            _sidebar.styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            _sidebar.AddToClassList(USS_SIDEBAR_ACTIVE);
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
            visualElement.RemoveFromClassList(USS_DOCKED_PANEL);
            RefreshScrollViewStyle();
        }
        
        public void AddToSidebar(VisualElement visualElement)
        {
            visualElement.style.position = new StyleEnum<Position>(Position.Relative);
            visualElement.AddToClassList(USS_DOCKED_PANEL);
            _sidebar.Add(visualElement);
            RefreshScrollViewStyle();
        }

        private void RefreshScrollViewStyle()
        {
            _sidebar.EnableInClassList(USS_SIDEBAR_DISABLED, _sidebar.childCount == 0);
        }
    }
}


