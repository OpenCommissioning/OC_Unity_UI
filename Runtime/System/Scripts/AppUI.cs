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
        public bool IsPointerFocused => _isPointerFocused;
        public bool IsPointerInsideScreen => _isPointerInsideScreen;
        public bool IsPointerValidForAction => !IsPointerOverUI && IsPointerFocused && IsPointerInsideScreen;
        public VisualElement Root => _root;

        [Header("State")]
        [SerializeField]
        private bool _isPointerOverUI;
        [SerializeField]
        private bool _isPointerFocused;
        [SerializeField]
        private bool _isPointerInsideScreen;
        
        [Header("Settings")]
        [SerializeField]
        private bool _debug;
        
        [Header("Input Actions")]
        [SerializeField]
        private InputActionReference _cancel;
        [SerializeField]
        private InputActionReference _window;
        [SerializeField]
        private InputActionReference _delete;
        
        private VisualElement _root;
        private EventSystem _eventSystem;
        private ExitPopup _exitPopup;
        private readonly List<IFloatingPanel> _floatingPanels = new();
        
        private InputAction _cancelAction;
        private InputAction _windowAction;
        private InputAction _deleteAction;

        protected new void Awake()
        {
            base.Awake();
            Initialize();
            
            
        }

        private void OnEnable()
        {
            _deleteAction = _delete.action;
            _cancelAction = _cancel.action;
            _windowAction = _window.action;
            
            _deleteAction.started += CancelAction;
            _deleteAction.performed += CancelAction;
            _deleteAction.canceled += CancelAction;
            
            _cancelAction.started += CancelAction;
            _cancelAction.performed += CancelAction;
            _cancelAction.canceled += CancelAction;
            
            _windowAction.started += WindowAction;
            _windowAction.performed += WindowAction;
            _windowAction.canceled += WindowAction;
            
            _cancelAction?.Enable();
            _windowAction?.Enable();
            _deleteAction?.Enable();
        }

        private void OnDisable()
        {
            if (_deleteAction != null)
            {
                _deleteAction.started -= CancelAction;
                _deleteAction.performed -= CancelAction;
                _deleteAction.canceled -= CancelAction;
            }

            if (_cancelAction != null)
            {
                _cancelAction.started -= CancelAction;
                _cancelAction.performed -= CancelAction;
                _cancelAction.canceled -= CancelAction;
            }

            if (_windowAction != null)
            {
                _windowAction.started -= WindowAction;
                _windowAction.performed -= WindowAction;
                _windowAction.canceled -= WindowAction;
            }
        }

        private void Update()
        {
            _isPointerOverUI = _eventSystem.IsPointerOverGameObject();
            _isPointerInsideScreen = CheckPointerInsideScreen();
            _isPointerFocused = Application.isFocused;
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
        
        private void CancelAction(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if (_debug) Debug.Log("Cancel Action");
            
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
        
        private void WindowAction(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if(_debug) Debug.Log("Window Action");
            
            Screen.fullScreenMode = 
                Screen.fullScreenMode != FullScreenMode.Windowed ? 
                    FullScreenMode.Windowed : 
                    FullScreenMode.FullScreenWindow;
        }

        private void DeleteAction(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if (_debug) Debug.Log("Delete Action");

            foreach (var interaction in SelectionManager.Instance.SelectedInteractions)
            {
                //TODO
                Debug.Log($"Delete Action NOT IMPLEMENTED: {interaction}");
            }
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

        private bool CheckPointerInsideScreen()
        {
            if (Mouse.current == null) return false;

            var position = Mouse.current.position.ReadValue();
            
            return position.x >= 0 &&
                   position.y >= 0 &&
                   position.x < Screen.width &&
                   position.y < Screen.height;
        }
    }
}