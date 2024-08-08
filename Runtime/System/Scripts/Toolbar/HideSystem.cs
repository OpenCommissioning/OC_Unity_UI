using IOSEF.UI.Interactions;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using IOSEF.UI.Panel;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Toolbar
{
    public class HideSystem : ToolbarSystemPanel
    {
        private List<HideGroup> _hideGroups;
        private List<ToggleIcon> _toggleIcons;

        private void Start()
        {
            _hideGroups = FindObjectsOfType<HideGroup>().ToList().OrderBy(x => x.Name).ToList();
        }

        protected override void AddContent(SubsystemPanel subsystemPanel)
        {
            _toggleIcons = new List<ToggleIcon>();
            
            foreach (var group in _hideGroups)
            {
                var toggle = new ToggleIcon(group.name, _defaultIcon, _activeIcon, Color.white, new Color(1f, 0.33f, 0.29f));
                toggle.RegisterValueChangedCallback( evt =>
                {
                    group.Transparent = evt.newValue;
                });

                toggle.value = group.Transparent;

                _toggleIcons.Add(toggle);

                subsystemPanel.Add(toggle);
            }
            
            var hoizontalGroup = new HorizontalGroup();
            hoizontalGroup.Add(new Panel.Button("Hide all", HideAll));
            hoizontalGroup.Add(new Panel.Button("Show all", ShowAll));
            subsystemPanel.Add(hoizontalGroup);
        }

        private void HideAll()
        {
            foreach (var toggle in _toggleIcons)
            {
                toggle.value = true;
            }
        }

        private void ShowAll()
        {
            foreach (var toggle in _toggleIcons)
            {
                toggle.value = false;
            }
        }

        [Button]
        public void HideVisual()
        {
            foreach (var group in _hideGroups)
            {
                group.Transparent = true;
            }
        }
        
        [Button]
        public void ShowVisual()
        {
            foreach (var group in _hideGroups)
            {
                group.Transparent = false;
            }
        }
    }
}
