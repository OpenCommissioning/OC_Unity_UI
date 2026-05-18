using System;
using OC.Components;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace OC.UI
{
    [DefaultExecutionOrder(1000)]
    public class CameraController : MonoBehaviour
    {
        public bool IsBusy => _state.Value != CameraState.None;
        public IProperty<CameraState> State => _state;
        public CameraSettings Settings => _settings;
        
        private bool IsPointerValidForAction => AppUI.Instance.IsPointerValidForAction;

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
        private UpdateLoop _updateLoop = UpdateLoop.FixedUpdate;
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

        private EventSystem _eventSystem;
        
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

            if (_lookAction != null)
            {
                _lookAction.started += HandleLookAction;
                _lookAction.performed += HandleLookAction;
                _lookAction.canceled += HandleLookAction;
            }

            if (_orbitAction != null)
            {
                _orbitAction.started += HandleOrbitAction;
                _orbitAction.performed += HandleOrbitAction;
                _orbitAction.canceled += HandleOrbitAction;
            }

            if (_panAction != null)
            {
                _panAction.started += HandlePanAction;
                _panAction.performed += HandlePanAction;
                _panAction.canceled += HandlePanAction;
            }

            if (_zoomAction != null)
            {
                _zoomAction.started += HandleZoomAction;
                _zoomAction.performed += HandleZoomAction;
                _zoomAction.canceled += HandleZoomAction;
            }

            if (_scrollAction != null) _scrollAction.performed += HandleScrollAction;
            if (_focusAction != null) _focusAction.performed += HandleFocusAction;
            if (_followAction != null) _followAction.performed += HandleFollowAction;

            _state.Subscribe(OnStateChanged);
            _active.Subscribe(OnActiveChanged);
            _target.Subscribe(OnTargetChanged);
           
            Initialize();
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
            if (_updateLoop != UpdateLoop.Update) return;
            LocalUpdate(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            if (!_active.Value) return;
            if (_updateLoop != UpdateLoop.FixedUpdate) return;
            LocalUpdate(Time.fixedDeltaTime);
        }
        
        private void LateUpdate()
        {
            if (!_active.Value) return;
            if (_updateLoop != UpdateLoop.LateUpdate) return;
            LocalUpdate(Time.deltaTime);
        }

        private void LocalUpdate(float deltaTime)
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
            
            RefreshState();
        }

        private void RefreshState()
        {
            if (_isLockedToTarget.Value && _target.Value != null)
            {
                _targetPivotPosition = _target.Value.position;
            }
            
            transform.position = _targetPivotPosition + _targetPivotRotation * Vector3.back * _distance;
            _pivot.SetPositionAndRotation(_targetPivotPosition, _targetPivotRotation);
            transform.LookAt(_targetPivotPosition, Vector3.up);
            
            //PIVOT
            _previousPivotPosition = _targetPivotPosition;
            _previousPivotRotation = _targetPivotRotation;
            
            //CAMERA
            _previousCameraPosition = transform.position;
            _previousCameraRotation = transform.rotation;
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
            if (!IsBusy && ctx.performed && IsPointerValidForAction)
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
            if (!IsBusy && ctx.performed && IsPointerValidForAction)
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
            if (!IsBusy && ctx.performed && IsPointerValidForAction)
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
            if (!IsBusy && ctx.performed && IsPointerValidForAction)
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
            if (!IsBusy && ctx.performed && IsPointerValidForAction)
            {
                var scroll = _scrollAction.ReadValue<Vector2>().y;
                _distance -= scroll * _settings.ZoomSensitivity;
                _distance = Mathf.Clamp(_distance, _settings.MinDistance, _settings.MaxDistance);
            }
        }

        private void HandleFocusAction(InputAction.CallbackContext ctx)
        {
            if (!IsBusy && ctx.performed && IsPointerValidForAction)
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
            if (!IsBusy && ctx.performed && IsPointerValidForAction)
            {
                if (_target.Value == null) return;
                _isLockedToTarget.Value = true;
            }
        }

        private void FlyMode(float deltaTime)
        {
            _isLockedToTarget.Value = false;
            
            //MOVE WASD
            var speed = _settings.MoveSpeed;
            if (_sprintAction.inProgress) speed *= _settings.FastMoveMultiplier;
            var delta = _moveAction.ReadValue<Vector3>() * (speed * deltaTime);
            
            var cameraPosition = _previousCameraPosition;
            
            if (delta.magnitude > EPSILON) cameraPosition = _previousCameraPosition + _previousCameraRotation * delta;
            
            //LOOK
            var lookInput = _mouseAction.ReadValue<Vector2>() * _settings.LookSensitivity;
            var euler = _previousCameraRotation.eulerAngles;
            var yaw = euler.y + lookInput.x;
            var pitch = NormalizeAngle(euler.x - lookInput.y);
            pitch = Mathf.Clamp(pitch, -85f, 85f);
            var cameraRotation = Quaternion.Euler(pitch, yaw, 0f);
            
            _targetPivotPosition = cameraPosition + cameraRotation * Vector3.forward * _distance;
            _targetPivotRotation = cameraRotation;
            
            Mouse.current.WrapCursorOnScreen();
        }

        // ReSharper disable once UnusedParameter.Local
        private void OrbitMode(float deltaTime)
        {
            //LOOK
            var lookInput = _mouseAction.ReadValue<Vector2>() * _settings.LookSensitivity;
            var euler = _previousPivotRotation.eulerAngles;
            var yaw = euler.y + lookInput.x;
            var pitch = NormalizeAngle(euler.x - lookInput.y);
            
            _targetPivotRotation = Quaternion.Euler(pitch, yaw, 0f);
            
            Mouse.current.WrapCursorOnScreen();
        }

        // ReSharper disable once UnusedParameter.Local
        private void PanMode(float deltaTime)
        {
            _isLockedToTarget.Value = false;
            
            var cursorDelta = _mouseAction.ReadValue<Vector2>() * _settings.PanSensitivity;
            var positionDelta = transform.right * (cursorDelta.x * _worldPerPixelX) + transform.up * (cursorDelta.y * _worldPerPixelY);
            
            _targetPivotPosition = _previousPivotPosition - positionDelta;
            
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
            
            _targetPivotPosition = target.transform.position;
        }
        
        private static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            return angle;
        }

        public void Initialize()
        {
            _targetPivotPosition = transform.position + transform.rotation * Vector3.forward * _distance;
            _targetPivotRotation = transform.rotation;
            _previousPivotPosition = _targetPivotPosition;
            _previousPivotRotation = _targetPivotRotation;
            _previousCameraPosition = transform.position;
            _previousCameraRotation = transform.rotation;
            _focusedOnBounds = false;
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
#if UNITY_EDITOR
            UnityEditor.Handles.DoPositionHandle(_pivot.position, _pivot.rotation);
#endif
        }
    }
}
