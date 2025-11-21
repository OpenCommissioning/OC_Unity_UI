using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.Interactions
{
    public class FPSCameraController : CameraControllerBase
    {
        [SerializeField] private InputActionProperty _fpsActionProperty;

        private InputAction _fpsAction;
        

        private void Start()
        {
            RegisterActions();
            _fpsAction.Enable();
        }

        private void OnEnable()
        {
            _fpsAction?.Enable();
        }

        private void OnDisable()
        {
            _fpsAction?.Disable();
        }

        public override void Enable()
        {
            SetFollowTarget();
            base.Enable();
        }

        private void RegisterActions()
        {
            _fpsAction = _fpsActionProperty.reference != null ? _fpsActionProperty.reference.action : _fpsActionProperty.action;
            _fpsAction.performed += OnFPS;
            _fpsAction.canceled += OnFPSCanceled;
        }

        private void OnFPS(InputAction.CallbackContext context)
        {
            Enable();
        }

        private void OnFPSCanceled(InputAction.CallbackContext context)
        {
            Disable();
        }

        private void SetFollowTarget()
        {
            ICinemachineCamera activeCam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;
            Vector3 camPosition = activeCam.State.RawPosition;
            _camera.Target.TrackingTarget.position = camPosition;
        }
    }
}
