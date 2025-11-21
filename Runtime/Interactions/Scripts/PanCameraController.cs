using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.Interactions
{
    public class PanCameraController : CameraControllerBase
    {
        [SerializeField] private InputActionProperty _panActionProperty;

        private InputAction _panAction;
        

        private void Start()
        {
            RegisterActions();
            _panAction.Enable();
        }

        private void OnEnable()
        {
            _panAction?.Enable();
        }

        private void OnDisable()
        {
            _panAction?.Disable();
        }

        public override void Enable()
        {
            SetFollowTarget();
            base.Enable();
        }

        private void RegisterActions()
        {
            _panAction = _panActionProperty.reference != null ? _panActionProperty.reference.action : _panActionProperty.action;
            _panAction.performed += OnPan;
            _panAction.canceled += OnPanCanceled;
        }

        private void OnPan(InputAction.CallbackContext context)
        {
            Enable();
        }

        private void OnPanCanceled(InputAction.CallbackContext context)
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
