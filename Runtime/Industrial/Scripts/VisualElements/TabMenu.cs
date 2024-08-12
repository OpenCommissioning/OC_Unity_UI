using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Industrial
{
    public class TabMenu : VisualElement
    {
        private const string _styleSheet = "StyleSheet/industrial-tabs";
        private const string _ussTabMenu = "tab-menu";
        private const string _ussTabBarContainer = "tab-bar-container";
        private const string _ussTabBar = "tab-bar";
        private const string _ussTabLabel = "tab-label";
        private const string _ussTabContent = "tab-content";
        private const string _ussTabSlected = "tab-selected";
        private const string _ussTabContentUnselected = "tab-content-unselected";

        private readonly VisualElement _tabBar;
        private readonly VisualElement _tabsContainer;
        private readonly List<Label> _labels;
        private readonly List<VisualElement> _tabs;

        public TabMenu()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussTabMenu);

            var tabsContainer = new VisualElement();
            tabsContainer.AddToClassList(_ussTabBarContainer);

            _tabBar = new VisualElement();
            
            _tabBar.AddToClassList(_ussTabBar);
            tabsContainer.Add(_tabBar);
            
            _tabsContainer = new VisualElement();
            
            hierarchy.Add(tabsContainer);
            hierarchy.Add(_tabsContainer);
            _labels = new List<Label>();
            _tabs = new List<VisualElement>();
        }
        
        public VisualElement GetOrCreate(string tabName)
        {
            foreach (var tab in _tabs.Where(tab => tab.name == tabName))
            {
                return tab;
            }

            return CreateTab(tabName);
        }

        private VisualElement CreateTab(string tabName)
        {
            var label = new Label(tabName.ToUpper());
            label.AddToClassList(_ussTabLabel);
            _labels.Add(label);
            _tabBar.Add(label);
            
            var tab = new VisualElement()
            {
                name = tabName
            };
            tab.AddToClassList(_ussTabContent);
            _tabs.Add(tab);
            _tabsContainer.Add(tab);

            label.RegisterCallback<ClickEvent>(_ => ChangeTabState(label, tab));
            UnselectAll();
            return tab;
        }

        private void ChangeTabState(VisualElement label, VisualElement tab)
        {
            var isSelected = label.ClassListContains(_ussTabSlected);
            UnselectAll();
            
            if (!isSelected)
            {
                label.AddToClassList(_ussTabSlected);
                tab.RemoveFromClassList(_ussTabContentUnselected);
            }
            else
            {
                label.RemoveFromClassList(_ussTabSlected);
                tab.AddToClassList(_ussTabContentUnselected);
            }
        }

        private void SetTabState(int index, bool select)
        {
            if (select)
            {
                _labels[index].AddToClassList(_ussTabSlected);
                _tabs[index].RemoveFromClassList(_ussTabContentUnselected);
            }
            else
            {
                _labels[index].RemoveFromClassList(_ussTabSlected);
                _tabs[index].AddToClassList(_ussTabContentUnselected);
            }
        }

        private void UnselectAll()
        {
            for (var i = 0; i < _tabs.Count; i++)
            {
                SetTabState(i,false);
            }
        }
    }
}
