using System;
using OC.UI.Panel;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI
{
    public class ExitPopup : VisualElement, ICloseble
    {
        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable == value) return;
                EnableWithoutNotification(value);
            }
        }

        public void Close()
        {
            Enable = false;
        }

        public event Action OnClose;

        private bool _enable;
        private const string Uxml = "UXML/popup-exit";
        private const string StyleSheet = "UXML/popup-exit";
        
        public ExitPopup()
        {
            _enable = true;
            var container = Resources.Load<VisualTreeAsset>(Uxml).Instantiate();
            hierarchy.Add(container);
            container.AddDefaultTheme();
            container.Q<UnityEngine.UIElements.Button>("cancel").clicked += () => Enable = false;
            container.Q<UnityEngine.UIElements.Button>("close").clicked += () => OnClose?.Invoke();
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
    }
}
