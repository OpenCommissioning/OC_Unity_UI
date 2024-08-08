using System;
using System.Collections.Generic;
using IOSEF.UI.Panel;
using UnityEngine;
using UnityEngine.Events;

namespace IOSEF.UI.Toolbar
{
    public class MacroSystem : ToolbarSystemPanel
    {
        [SerializeField]
        private List<Macros> _macros;

        protected override void AddContent(SubsystemPanel subsystemPanel)
        {
            foreach (var macro in _macros)
            {
                var button = new Button(macro.Name, () => macro.OnClick?.Invoke());
                subsystemPanel.Add(button);
            }
        }

        [Serializable]
        private class Macros
        {
            public string Name;
            public UnityEvent OnClick;
        }
    }
} 
