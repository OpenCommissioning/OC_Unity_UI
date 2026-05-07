using System.Collections.Generic;
using System.Linq;
using OC.UI.Interactions;
using OC.UI.Panel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace OC.UI
{
    [RequireComponent(typeof(UIDocument))]
    [DisallowMultipleComponent]
    public class AppUI : MonoBehaviourSingleton<AppUI>
    {
        public bool IsPointerOverUI => _isPointerOverUI;
        public bool IsUIFieldSelected => UIFieldSelected();

        [Header("State")]
        [SerializeField]
        private bool _isPointerOverUI;
        
        [Header("Debug")]
        [SerializeField]
        private bool _debug;
        
        private InputAction _actionCancel;
        private InputAction _actionWindow;
        

        private VisualElement _root;
        private EventSystem _eventSystem;
        private ExitPopup _exitPopup;
        private readonly List<IFloatingPanel> _floatingPanels = new();

        protected new void Awake()
        {
            base.Awake();
            Initialize();
            
            _actionCancel = InputSystem.actions.FindAction("Cancel");
            _actionWindow = InputSystem.actions.FindAction("Window");
        }

        private void OnEnable()
        {
            _actionCancel.performed += ActionClose;
            _actionWindow.performed += ActionWindow;
        }

        private void OnDisable()
        {
            _actionCancel.performed -= ActionClose;
            _actionWindow.performed -= ActionWindow;
        }

        private void Update()
        {
            _isPointerOverUI = _eventSystem.IsPointerOverGameObject();
        }

        private void Initialize()
        {
            _eventSystem = EventSystem.current;
            _root = GetComponent<UIDocument>().rootVisualElement;
            _root.style.alignItems = new StyleEnum<Align>(Align.Center);
            _root.style.justifyContent = new StyleEnum<Justify>(Justify.Center);

            _exitPopup = new ExitPopup
            {
                Enable = false
            };
            
            _exitPopup.AddDefaultTheme();

            _root.Add(_exitPopup);
            _exitPopup.BringToFront();
            _exitPopup.OnClose += Application.Quit;
        }

        private bool UIFieldSelected()
        {
            if (_eventSystem.currentSelectedGameObject == null) return false;
            return _eventSystem.currentSelectedGameObject.TryGetComponent<PanelEventHandler>(out var _);
        }

        public void Register(IFloatingPanel panel)
        {
            if (_floatingPanels.Contains(panel)) return;
            _floatingPanels.Add(panel);
        }

        public void Unregister(IFloatingPanel panel)
        {
            if (!_floatingPanels.Contains(panel)) return;
            _floatingPanels.Remove(panel);
        }

        private void ActionClose(InputAction.CallbackContext context)
        {
            if (_debug)
            {
                Debug.Log("ActionClose", this);
            }

            if (SelectionManager.Instance.SelectedInteractions.Any())
            {
                SelectionManager.Instance.Deselect(SelectionManager.Instance.SelectedInteractions.Last());
                return;
            }
            
            if (_floatingPanels.Count > 0)
            {
                _floatingPanels.Last().Close();
                return;
            }
            
            _exitPopup.Enable = !_exitPopup.Enable;
        }

        private void ActionWindow(InputAction.CallbackContext context)
        {
            if (_debug)
            {
                Debug.Log("ActionWindow", this);
            }

            Screen.fullScreenMode = Screen.fullScreenMode != FullScreenMode.Windowed ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;
        }
    }
}