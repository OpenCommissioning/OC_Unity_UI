using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace OC.UI.Interactions
{
    public class NavigationController : MonoBehaviour
    {
        public float DistanceToPivot => _orbitCameraController.DistanceToPivot;

        [Header("State")]
        [SerializeField] private Property<CameraMode> _mode = new (CameraMode.None);

        [Header("Camera Controllers")]
        [SerializeField] private OrbitCameraController _orbitCameraController;
        [SerializeField] private PanCameraController _panCameraController;
        [SerializeField] private FPSCameraController _fpsCameraController;

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
            var cam = args.IncomingCamera;
            
            switch(cam)
            {
                case CinemachineCamera c when ReferenceEquals(c, _panCameraController.Camera):
                    _mode.Value = CameraMode.Pan;
                    break;
                case CinemachineCamera c when ReferenceEquals(c, _orbitCameraController.Camera):
                    _mode.Value = CameraMode.Orbit;
                    break;
                case CinemachineCamera c when ReferenceEquals(c, _fpsCameraController.Camera):
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
