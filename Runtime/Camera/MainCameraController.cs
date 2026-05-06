using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI
{
    public class MainCameraController : MonoBehaviour
    {
        public bool IsBusy => _state.Value != CameraState.None;
        public IProperty<CameraState> State => _state;
        
        public CameraSettings Settings => _settings;
        
        protected const float EPSILON = UnityVectorExtensions.Epsilon;
        
        [Header("State")]
        [SerializeField]
        private Property<bool> _active = new(false);
        [SerializeField]
        private Property<CameraState> _state = new(CameraState.None);
        [SerializeField]
        private Property<bool> _isLockedToTarget = new(false);
        [SerializeField]
        private float _distance = 8;
        
        [Header("Settings")]
        [SerializeField]
        private CameraSettings _settings;
        [SerializeField]
        private bool _debug;
        
        [Header("References")]
        [SerializeField]
        private Transform _pivot;
        [SerializeField]
        private Property<Transform> _target = new(null);
        [SerializeField]
        private CinemachineCamera _camera;
        
        [Header("Input Actions")]
        [SerializeField]
        private InputActionReference _move;
        [SerializeField]
        private InputActionReference _mouse;
        [SerializeField]
        private InputActionReference _scroll;
        [SerializeField]
        private InputActionReference _look;
        [SerializeField]
        private InputActionReference _orbit;
        [SerializeField]
        private InputActionReference _pan;
        [SerializeField]
        private InputActionReference _zoom;
        [SerializeField]
        private InputActionReference _sprint;
        [SerializeField]
        private InputActionReference _focus;
        [SerializeField]
        private InputActionReference _follow;
        
        private InputAction _moveAction;
        private InputAction _mouseAction;
        private InputAction _scrollAction;
        
        private InputAction _lookAction;
        private InputAction _orbitAction;
        private InputAction _panAction;
        private InputAction _zoomAction;
        private InputAction _sprintAction;
        
        private InputAction _focusAction;
        private InputAction _followAction;
        
        private Vector3 _targetCameraPosition;
        private Quaternion _targetCameraRotation;
        private Vector3 _previousCameraPosition;
        private Quaternion _previousCameraRotation;
        
        private Vector3 _targetPivotPosition;
        private Quaternion _targetPivotRotation;
        private Vector3 _previousPivotPosition;
        private Quaternion _previousPivotRotation;

        private float _worldPerPixelX;
        private float _worldPerPixelY;
        private bool _focusedOnBounds;

        private void OnEnable()
        {
            _moveAction = _move.action;
            _mouseAction = _mouse.action;
            _scrollAction = _scroll.action;
            
            _lookAction = _look.action;
            _orbitAction = _orbit.action;
            _panAction = _pan.action;
            _zoomAction = _zoom.action;
            _sprintAction = _sprint.action;
            _focusAction = _focus.action;
            _followAction = _follow.action;
            
            _moveAction?.Enable();
            _mouseAction?.Enable();
            _scrollAction?.Enable();
            _lookAction?.Enable();
            _orbitAction?.Enable();
            _panAction?.Enable();
            _zoomAction?.Enable();
            _sprintAction?.Enable();
            _focusAction?.Enable();
            _followAction?.Enable();
            
            _lookAction.started += HandleLookAction;
            _lookAction.performed += HandleLookAction;
            _lookAction.canceled += HandleLookAction;
            
            _orbitAction.started += HandleOrbitAction;
            _orbitAction.performed += HandleOrbitAction;
            _orbitAction.canceled += HandleOrbitAction;
            
            _panAction.started += HandlePanAction;
            _panAction.performed += HandlePanAction;
            _panAction.canceled += HandlePanAction;
            
            _zoomAction.started += HandleZoomAction;
            _zoomAction.performed += HandleZoomAction;
            _zoomAction.canceled += HandleZoomAction;

            _scrollAction.performed += HandleScrollAction;

            _focusAction.performed += HandleFocusAction;
            _followAction.performed += HandleFollowAction;
            
            _state.Subscribe(OnStateChanged);
            _active.Subscribe(OnActiveChanged);
            _target.Subscribe(OnTargetChanged);
            
            //INITIALIZE
            _targetCameraPosition = transform.position;
            _targetCameraRotation = transform.rotation;
            _previousCameraPosition = transform.position;
            _previousCameraRotation = transform.rotation;
            _focusedOnBounds = false;
            
            RefreshPivotFromCamera();
        }

        private void OnDisable()
        {
            _lookAction.started -= HandleLookAction;
            _lookAction.performed -= HandleLookAction;
            _lookAction.canceled -= HandleLookAction;
            
            _orbitAction.started -= HandleOrbitAction;
            _orbitAction.performed -= HandleOrbitAction;
            _orbitAction.canceled -= HandleOrbitAction;
            
            _panAction.started -= HandlePanAction;
            _panAction.performed -= HandlePanAction;
            _panAction.canceled -= HandlePanAction;
            
            _zoomAction.started -= HandleZoomAction;
            _zoomAction.performed -= HandleZoomAction;
            _zoomAction.canceled -= HandleZoomAction;
            
            _scrollAction.performed -= HandleScrollAction;
            
            _focusAction.performed -= HandleFocusAction;
            _followAction.performed -= HandleFollowAction;
            
            _state.Unsubscribe(OnStateChanged);
            _active.Unsubscribe(OnActiveChanged);
            _target.Unsubscribe(OnTargetChanged);
        }

        private void Update()
        {
            if (!_active.Value) return;
            
            Pipeline(Time.deltaTime);
        }

        private void Pipeline(float deltaTime)
        {
            switch (_state.Value)
            {
                case CameraState.None:
                    break;
                case CameraState.Fly:
                    FlyMode(deltaTime);
                    break;
                case CameraState.Pan:
                    PanMode(deltaTime);
                    break;
                case CameraState.Orbit:
                    OrbitMode(deltaTime);
                    break;
                case CameraState.Zoom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var cameraDirection = (_targetPivotPosition - _targetCameraPosition).normalized;
            
            
            
            
            
            //PIVOT
            _previousPivotPosition = _targetPivotPosition;
            _previousPivotRotation = _targetPivotRotation;
            
            //CAMERA
            _previousCameraPosition = _targetCameraPosition;
            _previousCameraRotation = _targetCameraRotation;
            
            transform.SetPositionAndRotation(_targetCameraPosition, _targetCameraRotation);
            _pivot.SetPositionAndRotation(_targetPivotPosition, _targetPivotRotation);
        }

        private void RefreshCameraPose()
        {
            _previousCameraPosition = _targetCameraPosition;
            _previousCameraRotation = _targetCameraRotation;
            
        }

        private void RefreshPivotFromCamera()
        {
            var position = _targetCameraPosition + _targetCameraRotation * Vector3.forward * _distance;
            _pivot.position = position;
        }

        private void RefreshCameraFromPivot()
        {
            var position = _pivot.position + _pivot.rotation * Vector3.back * _distance;
            _targetCameraPosition = position;
        }

        private void OnStateChanged(CameraState state)
        {
            if (_debug) Debug.Log($"Camera state changed to {state}");
        }

        private void OnActiveChanged(bool isActive)
        {
            if (_debug) Debug.Log($"Active state changed to {isActive}");
        }
        
        private void OnTargetChanged(Transform target)
        {
            _focusedOnBounds = false;
        }
        
        private void HandleLookAction(InputAction.CallbackContext ctx)
        {
            if (!IsBusy && ctx.performed)
            {
                _state.Value = CameraState.Fly;
            }

            if (ctx.canceled)
            {
                _state.Value = CameraState.None;
            }
        }
        
        private void HandleOrbitAction(InputAction.CallbackContext ctx)
        {
            if (!IsBusy && ctx.performed)
            {
                _state.Value = CameraState.Orbit;
            }

            if (ctx.canceled)
            {
                _state.Value = CameraState.None;
            }
        }
        
        private void HandlePanAction(InputAction.CallbackContext ctx)
        {
            if (!IsBusy && ctx.performed)
            {
                _state.Value = CameraState.Pan;
                InitializeCameraFrustum(out _worldPerPixelX, out _worldPerPixelY);
            }

            if (ctx.canceled)
            {
                _state.Value = CameraState.None;
            }
        }
        
        private void HandleZoomAction(InputAction.CallbackContext ctx)
        {
            if (!IsBusy && ctx.performed)
            {
                _state.Value = CameraState.Zoom;
            }
            
            if (ctx.canceled)
            {
                _state.Value = CameraState.None;
            }
        }

        private void HandleScrollAction(InputAction.CallbackContext ctx)
        {
            if (!IsBusy && ctx.performed)
            {
                var scroll = _scrollAction.ReadValue<Vector2>().y;
                _distance -= scroll * _settings.ZoomSensitivity;
                _distance = Mathf.Clamp(_distance, _settings.MinDistance, _settings.MaxDistance);
                
                RefreshCameraFromPivot();
                RefreshCameraPose();
                RefreshPivotFromCamera();
            }
        }

        private void HandleFocusAction(InputAction.CallbackContext ctx)
        {
            if (!IsBusy && ctx.performed)
            {
                if (_target.Value == null) return;

                if (_focusedOnBounds)
                {
                    Focus(_target.Value);
                    _focusedOnBounds = false; 
                }
                else
                {
                    Focus(_target.Value, true);
                    _focusedOnBounds = true;
                }
            }
        }

        private void HandleFollowAction(InputAction.CallbackContext ctx)
        {
            if (!IsBusy && ctx.performed)
            {
                
            }
        }

        private void FlyMode(float deltaTime)
        {
            _isLockedToTarget.Value = false;
            
            //MOVE WASD
            var speed = _settings.MoveSpeed;
            if (_sprintAction.inProgress) speed *= _settings.FastMoveMultiplier;
            var delta = _moveAction.ReadValue<Vector3>() * (speed * deltaTime);
            if (delta.magnitude > EPSILON) _targetCameraPosition = _previousCameraPosition + _previousCameraRotation * delta;
            
            //LOOK
            var lookInput = _mouseAction.ReadValue<Vector2>() * _settings.LookSensitivity;
            var euler = _previousCameraRotation.eulerAngles;
            var yaw = euler.y + lookInput.x;
            var pitch = NormalizeAngle(euler.x - lookInput.y);
            pitch = Mathf.Clamp(pitch, -85f, 85f);
            _targetCameraRotation = Quaternion.Euler(pitch, yaw, 0f);

            RefreshCameraPose();
            RefreshPivotFromCamera();
            Mouse.current.WrapCursorOnScreen();
        }

        private void OrbitMode(float deltaTime)
        {
            //LOOK
            var lookInput = _mouseAction.ReadValue<Vector2>() * _settings.LookSensitivity;
            var euler = _previousCameraRotation.eulerAngles;
            var yaw = euler.y + lookInput.x;
            var pitch = NormalizeAngle(euler.x - lookInput.y);
                
            _targetCameraRotation = Quaternion.Euler(pitch, yaw, 0f);

            var direction = _targetCameraRotation * Vector3.back;
            _targetCameraPosition = _pivot.position + direction * _distance;
            
            RefreshCameraPose();
            Mouse.current.WrapCursorOnScreen();
        }

        private void PanMode(float deltaTime)
        {
            var cursorDelta = _mouseAction.ReadValue<Vector2>() * _settings.PanSensitivity;
            var positionDelta = transform.right * (cursorDelta.x * _worldPerPixelX) + transform.up * (cursorDelta.y * _worldPerPixelY);
            _targetCameraPosition = _previousCameraPosition - positionDelta;
                
            RefreshCameraPose();
            Mouse.current.WrapCursorOnScreen();
        }

        public void Focus(Transform target, bool useBounds = false)
        {
            if (useBounds)
            {
                var bounds = GetBoundingBox(target);
                _distance = bounds.extents.magnitude * _settings.FocusMinDistance;
            }
            else
            {
                _distance = _settings.FocusMinDistance;
            }
            
            _pivot.position = target.transform.position;
            
            RefreshCameraFromPivot();
            RefreshCameraPose();
            RefreshPivotFromCamera();
        }
        
        private static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            return angle;
        }

        private void InitializeCameraFrustum(out float worldPerPixelX, out float worldPerPixelY)
        {
            float frustumHeight;
            float frustumWidth;
            
            var lens = _camera.Lens;
            
            if (lens.Orthographic)
            {
                frustumHeight = lens.OrthographicSize * 2f;
                frustumWidth = frustumHeight * lens.Aspect;
            }
            else
            {
                frustumHeight = 2f * _distance * Mathf.Tan(lens.FieldOfView * 0.5f * Mathf.Deg2Rad);
                frustumWidth = frustumHeight * lens.Aspect;
            }

            worldPerPixelX = frustumWidth / Screen.width;
            worldPerPixelY = frustumHeight / Screen.height;
        }
        
        private static Bounds GetBoundingBox(Transform transform)
        {
            Bounds bounds = new(transform.position, Vector3.zero);
            var renderers = transform.GetComponentsInChildren<Renderer>();
            foreach (var item in renderers)
            {
                bounds.Encapsulate(item.bounds);
            }
            return bounds;
        } 
        
        public enum CameraState
        {
            None,
            Fly,
            Pan,
            Orbit,
            Zoom
        }

        private void OnDrawGizmos()
        {
            if (!_debug) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _pivot.position);
            UnityEditor.Handles.DoPositionHandle(_pivot.position, _pivot.rotation);
        }
    }
}
