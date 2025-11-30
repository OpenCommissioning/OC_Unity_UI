using System.Linq;
using Unity.Cinemachine;
using OC.UI.TransformHandles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.Interactions
{
    public class UserInputSystem : MonoBehaviourSingleton<UserInputSystem>
    {
        public bool IsPointerOverScreen => Utils.IsPointerOverScreen(Mouse.current.position.value);

        [SerializeField] private InputActionProperty _toolTypeMoveActionProperty;
        [SerializeField] private InputActionProperty _toolTypeRotationActionProperty;
        [SerializeField] private InputActionProperty _toolTypeViewActionProperty;
        [SerializeField] private InputActionProperty _escapeActionProperty;
        private bool _isValid;
        private bool _holdInput;

        private InputAction _toolTypeMoveAction;
        private InputAction _toolTypeRotationAction;
        private InputAction _toolTypeViewAction;
        private InputAction _escapeAction;

        private CameraControllerMaster _cameraControllerMaster;


        private void Start()
        {
            RegisterActions();
            _escapeAction.Enable();
            CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
        }

        private void RegisterActions()
        {
            _toolTypeMoveAction = _toolTypeMoveActionProperty.reference != null ? _toolTypeMoveActionProperty.reference.action : _toolTypeMoveActionProperty.action;
            _toolTypeMoveAction.started += OnToolTypeMoveAction;

            _toolTypeRotationAction = _toolTypeRotationActionProperty.reference != null ? _toolTypeRotationActionProperty.reference.action : _toolTypeRotationActionProperty.action;
            _toolTypeRotationAction.started += OnToolTypeRotationAction;

            _toolTypeViewAction = _toolTypeViewActionProperty.reference != null ? _toolTypeViewActionProperty.reference.action : _toolTypeViewActionProperty.action;
            _toolTypeViewAction.started += OnToolTypeViewAction;

            _escapeAction = _escapeActionProperty.reference != null ? _escapeActionProperty.reference.action : _escapeActionProperty.action;
            _escapeAction.started += OnEscapeAction;
        }

        private void Update()
        {
            if (_cameraControllerMaster != null && _cameraControllerMaster.IsBusy)
            {
                DisableToolbarInput();
                SelectionManager.Instance.ResetHit();
                return;
            }
            //if (!_isValid) return;

            // var cam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;

            // if (cam is MonoBehaviour camBehaviour && camBehaviour.TryGetComponent(out CameraController cameraController))
            // {
            //     CameraUserInputs(cameraController);
                
            //     if (cameraController.IsBusy)
            //     {
            //         SelectionManager.Instance.ResetHit();
            //         return;
            //     }
            // }

            SelectionUserInputs();

            if (!UIManager.Instance.IsUIFieldSelected && RuntimeTransformHandle.Instance != null)
            {
                EnableToolbarInput();
            }
            else
            {
                DisableToolbarInput();
            }
        }

        private void OnToolTypeMoveAction(InputAction.CallbackContext context)
        {
            RuntimeTransformHandle.Instance.ToolType = ToolType.Move;
        }

        private void OnToolTypeRotationAction(InputAction.CallbackContext context)
        {
            RuntimeTransformHandle.Instance.ToolType = ToolType.Rotation;
        }

        private void OnToolTypeViewAction(InputAction.CallbackContext context)
        {
            RuntimeTransformHandle.Instance.ToolType = ToolType.View;
        }

        private void OnEscapeAction(InputAction.CallbackContext context)
        {
            if (SelectionManager.Instance.SelectedInteractions.Any())
            {
                SelectionManager.Instance.Deselect(SelectionManager.Instance.SelectedInteractions.Last());
            }
            else
            {
                UIManager.Instance.CloseLast();
            }
        }

        private void OnCameraActivated(ICinemachineCamera.ActivationEventParams args)
        {
            if (args.IncomingCamera is CinemachineCamera cam)
            {
                var master = cam.GetComponentInParent<CameraControllerMaster>();
                if(master != null)
                {
                    _cameraControllerMaster = master;
                }
            }           
        }

        private void SelectionUserInputs()
        {
            if (UIManager.Instance.IsPointerOverUI) return;
            SelectionManager.Instance.Raycast();
            SelectionManager.Instance.ProcessPointerEvents(0);

            //TODO Box selection
            //if (((Input.GetMouseButton(0) && (SelectionManager.Instance._isDrawing))  || (Input.GetMouseButtonUp(0) && SelectionManager.Instance._isDrawing)) && !Input.GetKey(KeyCode.LeftAlt))
            //{
            //    SelectionManager.Instance.BoxSelection();
            //}
            //else
            //{
            //}
        }

        private void EnableToolbarInput()
        {
            _toolTypeMoveAction.Enable();
            _toolTypeRotationAction.Enable();
            _toolTypeViewAction.Enable();
        }

        private void DisableToolbarInput()
        {
            _toolTypeMoveAction.Disable();
            _toolTypeRotationAction.Disable();
            _toolTypeViewAction.Disable();
        }
    }
}
