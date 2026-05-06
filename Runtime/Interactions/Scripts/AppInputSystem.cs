using System;
using System.Linq;
using Unity.Cinemachine;
using OC.UI.TransformHandles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.Interactions
{
    public class AppInputSystem : MonoBehaviourSingleton<AppInputSystem>
    {
        public bool IsPointerOverScreen => Utils.IsPointerOverScreen(Mouse.current.position.value);
        
        private bool _isValid;
        private bool _holdInput;

        private void OnEnable()
        {
            CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
        }

        private void OnDisable()
        {
            CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivated);
        }

        private void Update()
        {
            // if (_cameraControllerMaster != null && _cameraControllerMaster.IsBusy)
            // {
            //     DisableToolbarInput();
            //     SelectionManager.Instance.ResetHit();
            //     return;
            // }
            
            //TODO
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

            if (!AppUI.Instance.IsUIFieldSelected && RuntimeTransformHandle.Instance != null)
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

        private void OnCameraActivated(ICinemachineCamera.ActivationEventParams args)
        {
            if (args.IncomingCamera is CinemachineCamera cam)
            {
                // var master = cam.GetComponentInParent<CameraControllerMaster>();
                // if(master != null)
                // {
                //     //_cameraControllerMaster = master;
                // }
            }           
        }

        private void SelectionUserInputs()
        {
            if (AppUI.Instance.IsPointerOverUI) return;
            SelectionManager.Instance.Raycast();
            //SelectionManager.Instance.ProcessPointerEvents(0);

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
            //_toolTypeMoveAction.Enable();
            //_toolTypeRotationAction.Enable();
            //_toolTypeViewAction.Enable();
        }

        private void DisableToolbarInput()
        {
            //_toolTypeMoveAction.Disable();
            //_toolTypeRotationAction.Disable();
            //_toolTypeViewAction.Disable();
        }
    }
}
