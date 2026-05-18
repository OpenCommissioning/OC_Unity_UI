using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using OC.Interactions;
using OC.UI.Interactions;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(1000)]
    public class PanelManager : MonoBehaviourSingleton<PanelManager>
    {
        public VisualElement Sidebar => _sidebar;
        public VisualElement Screen => _appUiRoot;
        
        public float DockThreshold => _dockThreshold;
        
        [Header("Component Panels")]
        [SerializeField]
        private List<PanelHandler> _panelHandlers = new ();
        
        [Header("Settings")]
        [SerializeField]
        private float _dockThreshold = 10f;
        [SerializeField]
        private bool _debug;

        private VisualElement _appUiRoot;
        private ScrollView _sidebar;

        private const string UXML = "UXML/panel-sidebar";
        private const string STYLE_SHEET = "StyleSheet/panel-sidebar";
        private const string USS_SIDEBAR_ACTIVE = "sidebar-scrollView";
        private const string USS_SIDEBAR_DISABLED = "sidebar-scrollView_disabled";
        private const string USS_DOCKED_PANEL = "panel-container__docked";
        
        private readonly Dictionary<Type, PanelHandler> _factory = new ();
        private readonly List<IPanel> _cachedPanels = new ();
        private readonly List<IPanel> _activePanels = new ();
        private readonly List<IPanel> _removePanels = new ();

        private void OnEnable()
        {
            SelectionManager.Instance.OnSelectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            SelectionManager.Instance.OnSelectionChanged -= OnSelectionChanged;
        }

        private void Start()
        {
            _appUiRoot = AppUI.Instance.Root;
            _sidebar = Resources.Load<VisualTreeAsset>(UXML).Instantiate().Q<ScrollView>("sidebar");
            _sidebar.styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            _sidebar.AddToClassList(USS_SIDEBAR_ACTIVE);
            RefreshScrollViewStyle();
            _appUiRoot.Add(_sidebar);
            _sidebar.SendToBack();
            
            _factory.Clear();
            
            foreach (var panelHandler in _panelHandlers)
            {
                _factory.Add(panelHandler.ReferenceType, panelHandler);
            }
        }
        
        public void AddToScreen(VisualElement visualElement)
        {
            _appUiRoot.Add(visualElement);
            visualElement.style.position = Position.Absolute;
            visualElement.RemoveFromClassList(USS_DOCKED_PANEL);
            RefreshScrollViewStyle();
        }
        
        public void AddToSidebar(VisualElement visualElement)
        {
            _sidebar.Add(visualElement);
            visualElement.style.position = Position.Relative;
            visualElement.AddToClassList(USS_DOCKED_PANEL);
            RefreshScrollViewStyle();
        }

        [Button]
        public void CloseLastPanel()
        {
            for (var i = _activePanels.Count - 1; i >= 0; i--)
            {
                var panel = _activePanels[i];
                if (panel.Pinned) continue;
                DisablePanel(panel);
                _activePanels.RemoveAt(i);
                break;
            }
        }
        
        private void OnSelectionChanged(IReadOnlyList<Interaction> selections)
        {
            _removePanels.Clear();
            
            foreach (var activePanel in _activePanels)
            {
                if (activePanel.Pinned) continue;
                if (selections.Contains(activePanel.Interaction)) continue;
                _removePanels.Add(activePanel);
            }
            
            foreach (var panel in _removePanels)
            {
                DisablePanel(panel);
            }
            
            foreach (var interaction in selections)
            {
                EnablePanel(interaction);
            }
        }

        private void EnablePanel(Interaction interaction)
        {
            if (interaction == null) return;
            if (_activePanels.Find(panel => panel.Interaction == interaction) != null) return;
            
            if (interaction.Interactable == null) return;
            
            var panel = CreateOrGetPanel(interaction.Interactable.ReferenceType);
            
            _activePanels.Add(panel);
            
            panel.Enable = true;
            panel.Bind(interaction);
            AddToSidebar(panel.Root);
        }

        private void DisablePanel(IPanel panel)
        {
            _activePanels.Remove(panel);
            panel.Enable = false;
            panel.Unbind();
        }

        private IPanel CreateOrGetPanel(Type type)
        {
            var panel = _cachedPanels.FirstOrDefault(panel => 
                panel.Enable == false && panel.ReferenceType == type);

            if (panel != null) return panel;
            
            if (_factory.TryGetValue(type, out var panelHandler))
            {
                panel = panelHandler.Create();
                _cachedPanels.Add(panel);
            }

            return panel;
        }
        
        private void RefreshScrollViewStyle()
        {
            var visible = _activePanels.Any(panel => panel.Enable);
            _sidebar.EnableInClassList(USS_SIDEBAR_DISABLED, !visible);
        }
    }
}


