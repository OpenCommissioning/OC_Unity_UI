using IOSEF.UI.TransformHandles;
using System.Linq;
using UnityEngine;

namespace IOSEF.UI.Interactions
{
    public class UserInputSystem : MonoBehaviour
    {
        public static UserInputSystem Instance;

        public bool IsPointerOverScreen => Utils.IsPointerOverScreen(Input.mousePosition);

        private CameraController _cameraController;
        private bool _isValid;
        private bool _holdInput;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        private void Start()
        {
            if (Camera.main != null)
            {
                if (Camera.main.TryGetComponent(out _cameraController))
                {
                    _isValid = true;
                }
                else
                {
                    Debug.LogError("MovableCamera can't be found! Add MovableCamera to main camera object", this);
                }
            }
        }

        private void Update()
        {
            if (!_isValid) return;

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

            CameraUserInputs();

            if (_cameraController.IsBusy)
            {
                SelectionManager.Instance.ResetHit();
                return;
            }

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

        private void CameraUserInputs()
        {
            if (!IsPointerOverScreen)
            {
                _cameraController.Mode = CameraController.CameraMode.None;
                return;
            }

            if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Mouse2))
            {
                _cameraController.Mode = CameraController.CameraMode.None;
                return;
            }

            if (_cameraController.Mode == CameraController.CameraMode.Orbit)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    _cameraController.Mode = CameraController.CameraMode.None;
                    return;
                }

                if (!Input.GetKey(KeyCode.LeftAlt))
                {
                    _cameraController.Mode = CameraController.CameraMode.None;
                    return;
                }
            }

            if (_cameraController.Mode == CameraController.CameraMode.Zoom)
            {
                if (Input.mouseScrollDelta.y == 0)
                {
                    _cameraController.Mode = CameraController.CameraMode.None;
                    return;
                }
            }

            if (!UIManager.Instance.IsPointerOverUI)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    _cameraController.Mode = CameraController.CameraMode.FPS;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Mouse2))
                {
                    _cameraController.Mode = CameraController.CameraMode.Pan;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftAlt))
                {
                    _cameraController.Mode = CameraController.CameraMode.Orbit;
                    return;
                }

                if (Input.mouseScrollDelta.y != 0)
                {
                    _cameraController.Zoom();
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && !UIManager.Instance.IsUIFieldSelected)
            {
                _cameraController.Focus();
            }
        }
    }
}
