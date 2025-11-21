using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace OC.UI.Interactions
{
    public class NavigationController : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private Property<CameraMode> _mode = new (CameraMode.None);

        [Header("Camera Controllers")]
        [SerializeField] private List<CameraControllerBase> _cameraControllerList;

        [Header("Testing")]
        [SerializeField] private bool _debug = false;

        private CursorHandler _cursorHandler;


        private void Start()
        {
            _cursorHandler = new CursorHandler();
            _mode.OnValueChanged += OnModeChanged;
            CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
        }

        private void OnDisable()
        {
            _mode.OnValueChanged -= OnModeChanged;
            CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivated);
        }

        private void OnModeChanged(CameraMode mode)
        {
            _cursorHandler.SetCursor(mode);
        }

        private void OnCameraActivated(ICinemachineCamera.ActivationEventParams args)
        {
            string camName = args.IncomingCamera.Name;
            var controller = _cameraControllerList.FirstOrDefault(c => c.Camera != null && c.Camera.Name == camName);
            
            switch(controller)
            {
                case PanCameraController:
                    _mode.Value = CameraMode.Pan;
                    break;
                case OrbitCameraController:
                    _mode.Value = CameraMode.Orbit;
                    break;
                case FPSCameraController:
                    _mode.Value = CameraMode.FPS;
                    break;
                default:
                    _mode.Value = CameraMode.None;
                    break;
            }
        }
    }

    public enum CameraMode
    {
        None,
        FPS,
        Pan,
        Orbit,
        Zoom
    }
}
