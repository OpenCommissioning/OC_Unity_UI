using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.Interactions
{
    public class FPSCameraController : CameraControllerBase
    {
        [SerializeField] private InputActionProperty _fpsActionProperty;

        private MoveIn3DSpace _freeMover;

        private InputAction _fpsAction;


        protected override void Awake()
        {
            base.Awake();
            _freeMover = GetComponent<MoveIn3DSpace>();
        }

        private void Start()
        {
            RegisterActions();
            _fpsAction.Enable();
        }

        private void OnEnable()
        {
            _fpsAction?.Enable();
            _controllerMaster.MoveSpeed.Subscribe(OnMoveSpeedChangedAction);
            _controllerMaster.RotationGain.Subscribe(OnRatationGainChangedAction);
        }

        private void OnDisable()
        {
            _fpsAction?.Disable();
            _controllerMaster.MoveSpeed.Unsubscribe(OnMoveSpeedChangedAction);
            _controllerMaster.RotationGain.Unsubscribe(OnRatationGainChangedAction);
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
            if(_controllerMaster.Brain.IsBlending) return;
            Enable();
        }

        private void OnFPSCanceled(InputAction.CallbackContext context)
        {
            Disable();
        }

        private void OnMoveSpeedChangedAction(float value)
        {
            _freeMover.Speed = value;
        }

        private void OnRatationGainChangedAction(float value)
        {
            _inputAxisController.GetController("Look X (Pan)").Input.Gain = value;
            _inputAxisController.GetController("Look Y (Tilt)").Input.Gain = -value;
        }

        private void SetFollowTarget()
        {
            ICinemachineCamera activeCam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;
            Vector3 camPosition = activeCam.State.RawPosition;
            _camera.Target.TrackingTarget.position = camPosition;
        }
    }
}
