using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace OC.UI.Interactions
{
    public class CameraControllerMaster : MonoBehaviour
    {
        public bool IsBusy => _mode != CameraMode.None;
        public bool Enabled => _defaultCamera.Priority != 0;
        public float DistanceToPivot => _orbitCameraController.DistanceToPivot;
        public CinemachineBrain Brain => CinemachineCore.FindPotentialTargetBrain(_defaultCamera);
        public IPropertyReadOnly<float> RotationGain => _rotationGain;
        public IPropertyReadOnly<float> ScrollGain => _scrollGain;
        public IPropertyReadOnly<float> MoveSpeed => _moveSpeed;

        public int MouseSensitivity
        {
            get => _mouseSensitivity;
            set
            {
                _mouseSensitivity = value;
                RefreshSettings();
            }
        }

        public int MoveSensitivity
        {
            get => _moveSensitivity;
            set
            {
                _moveSensitivity = value;
                RefreshSettings();
            }
        }

        public int ScrollSensitivity
        {
            get => _scrollSensitivity;
            set
            {
                _scrollSensitivity = value;
                RefreshSettings();
            }
        }

        [Header("State")]
        [SerializeField] private Property<CameraMode> _mode = new (CameraMode.None);

        [Header("Defaul Camera")]
        [SerializeField] private CinemachineCamera _defaultCamera;

        [Header("Camera Controllers")]
        [SerializeField] private OrbitCameraController _orbitCameraController;
        [SerializeField] private PanCameraController _panCameraController;
        [SerializeField] private FPSCameraController _fpsCameraController;

        [Header("Settings")]
        [Range(1,10)]
        [SerializeField]
        private int _mouseSensitivity = 5;
        [Range(1,10)]
        [SerializeField] 
        private int _moveSensitivity = 5;
        [Range(1, 10)] 
        [SerializeField] 
        private int _scrollSensitivity = 5;

        [Header("Cursor")]
        [SerializeField] private bool _warpCursor = false;


        private const float ROTATION_GAIN_DEFAULT = 0.15f;
        private const float SCROLL_GAIN_DEFAULT = 0.004f;
        private const float MOVE_SPEED_DEFAULT = 0.5f;

        private Property<float> _rotationGain = new(0f);
        private Property<float> _scrollGain = new(0f);
        private Property<float> _moveSpeed = new(0f);

        private CursorHandler _cursorHandler;


        private void Start()
        {
            _cursorHandler = new CursorHandler(_warpCursor);
            _mode.OnValueChanged += OnModeChanged;
            CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
        }

        private void OnDisable()
        {
            _mode.OnValueChanged -= OnModeChanged;
            CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivated);
        }

        private void OnValidate()
        {
            RefreshSettings();
        }

        private void Update()
        {
            _cursorHandler.Update();
        }

        private void OnModeChanged(CameraMode mode)
        {
            _cursorHandler.SetCursor(mode);
        }

        private void RefreshSettings()
        {
            _rotationGain.Value = ROTATION_GAIN_DEFAULT * _mouseSensitivity;
            _scrollGain.Value = SCROLL_GAIN_DEFAULT * _scrollSensitivity;
            _moveSpeed.Value = MOVE_SPEED_DEFAULT * _moveSensitivity;
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
                    if(_orbitCameraController.IsOrbiting)
                    {
                        _mode.Value = CameraMode.Orbit;
                    }
                    break;
                case CinemachineCamera c when ReferenceEquals(c, _fpsCameraController.Camera):
                    _mode.Value = CameraMode.FPS;
                    break;
                default:
                    _mode.Value = CameraMode.None;
                    break;
            }
        }

        public void Prioritize()
        {
            StopAllCoroutines();
            StartCoroutine(PrioritizeCoroutine());
        }

        public void Priority(int priority)
        {
            _defaultCamera.Priority = priority;
        }

        private IEnumerator PrioritizeCoroutine()
        {
            _defaultCamera.BlendHint = CinemachineCore.BlendHints.CylindricalPosition;
            _defaultCamera.Priority = 1;
            yield return new WaitUntil(() => !Brain.IsBlending);
            _defaultCamera.BlendHint = CinemachineCore.BlendHints.InheritPosition;
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
