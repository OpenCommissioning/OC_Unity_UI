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

        private bool _isValid;
        private bool _holdInput;


        private void Update()
        {
            //if (!_isValid) return;

            if (Input.GetKeyDown(KeyCode.Escape))
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
                if (Input.GetKeyDown(KeyCode.W)) RuntimeTransformHandle.Instance.ToolType = ToolType.Move;
                if (Input.GetKeyDown(KeyCode.E)) RuntimeTransformHandle.Instance.ToolType = ToolType.Rotation;
                if (Input.GetKeyDown(KeyCode.Q)) RuntimeTransformHandle.Instance.ToolType = ToolType.View;
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

        private void CameraUserInputs(CameraController cameraController)
        {
            if (!IsPointerOverScreen)
            {
                cameraController.Mode = CameraController.CameraMode.None;
                return;
            }

            if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Mouse2))
            {
                cameraController.Mode = CameraController.CameraMode.None;
                return;
            }

            if (cameraController.Mode == CameraController.CameraMode.Orbit)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    cameraController.Mode = CameraController.CameraMode.None;
                    return;
                }

                if (!Input.GetKey(KeyCode.LeftAlt))
                {
                    cameraController.Mode = CameraController.CameraMode.None;
                    return;
                }
            }

            if (cameraController.Mode == CameraController.CameraMode.Zoom)
            {
                if (Input.mouseScrollDelta.y == 0)
                {
                    cameraController.Mode = CameraController.CameraMode.None;
                    return;
                }
            }

            if (!UIManager.Instance.IsPointerOverUI)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    cameraController.Mode = CameraController.CameraMode.FPS;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Mouse2))
                {
                    cameraController.Mode = CameraController.CameraMode.Pan;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftAlt))
                {
                    cameraController.Mode = CameraController.CameraMode.Orbit;
                    return;
                }

                if (Input.mouseScrollDelta.y != 0)
                {
                    cameraController.Zoom();
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && !UIManager.Instance.IsUIFieldSelected)
            {
                cameraController.Focus();
            }
        }
    }
}
