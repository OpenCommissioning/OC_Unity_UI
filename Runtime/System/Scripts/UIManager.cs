using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using IOSEF.UI.Panel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace IOSEF.UI
{
    [RequireComponent(typeof(UIDocument))]
    [DisallowMultipleComponent]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public bool IsPointerOverUI => _eventSystem.IsPointerOverGameObject();
        public bool IsUIFieldSelected => UIFieldSelected();

        private VisualElement _root;
        private EventSystem _eventSystem;
        private ExitPopup _exitPopup;
        private readonly List<ICloseble> _closeblesElements = new();

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

            _eventSystem = EventSystem.current;
            _root = GetComponent<UIDocument>().rootVisualElement;
            _root.style.alignItems = new StyleEnum<Align>(Align.Center);
            _root.style.justifyContent = new StyleEnum<Justify>(Justify.Center);

            _exitPopup = new ExitPopup
            {
                Enable = false
            };

            _root.Add(_exitPopup);
            _exitPopup.BringToFront();
            _exitPopup.OnClose += Application.Quit;
        }

        private bool UIFieldSelected()
        {
            if (_eventSystem.currentSelectedGameObject == null) return false;
            return _eventSystem.currentSelectedGameObject.TryGetComponent<PanelEventHandler>(out var handler);
        }

        public void Register(ICloseble panel)
        {
            if (_closeblesElements.Contains(panel)) return;
            _closeblesElements.Add(panel);
        }

        public void Unregister(ICloseble panel)
        {
            if (!_closeblesElements.Contains(panel)) return;
            _closeblesElements.Remove(panel);
        }

        [Button]
        public void CloseLast()
        {
            if (_closeblesElements.Count > 0)
            {
                _closeblesElements.Last().Close();
            }
            else
            {
                _exitPopup.Enable = true;
            }
        }
    }
}