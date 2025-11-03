using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.Interactions
{
    public class CameraController : MonoBehaviour
    {
        public bool IsBusy => _mode != CameraMode.None;
        
        public CameraMode Mode
        {
            get => _mode.Value;
            set => _mode.Value = value;
        }

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
        [SerializeField] 
        private UpdateLoop _updateLoop = UpdateLoop.Update;
        [SerializeField]
        private bool _showGizmos;
        
        [Header("Settings")]
        [SerializeField]
        private GameObject _focusTarget;

        private const float ROTATION_SPEED = 0.5f;
        private const float MOVE_SPEED = 1f;
        private const float SCROLL_SPEED = 0.04f;
        private const float FLY_ACCELERATION = 2f;
        private const float DEFAULT_DISTANCE = 3f;
        
        private readonly Property<CameraMode> _mode = new (CameraMode.None);
        private Camera _camera;
        private Transform _transform;
        private Vector3 _pivot;
        private Quaternion _rotation;
        private float _distance;
        private float _rotationSpeed;
        private float _moveSpeed;
        private float _scrollSpeed;

        private bool _isMoving;
        private float _flyTargetSpeed;
        private Vector3 _motionDirection;
        
        private bool _isFocused;
        private bool _isFocusedOnDefaultDistance;

        private Vector3 _flySpeed;

        private Texture2D _panIcon;
        private Texture2D _orbitIcon;
        private Texture2D _zoomIcon;
        private Texture2D _fpsIcon;

        [Header("Testing")]
        [SerializeField] private bool _isPointerOverUI = false;
        [SerializeField] private bool _pan;
        [SerializeField] private bool _sprint = false;
        [SerializeField] private bool _fps;
        [SerializeField] private Vector3 _moveInput;
        [SerializeField] private Vector2 _lookInput;
        [SerializeField] private Vector2 _zoomInput;
        [SerializeField] private Vector3 _lastMousePosition;

        [Header("Debug")]
        [SerializeField] private bool _debug = false;

        private void Start()
        {
            RefreshSettings();
            
            _transform = transform;
            var cameraBrain = CinemachineBrain.GetActiveBrain(0);
            _camera = cameraBrain.GetComponent<Camera>();

            _distance = DEFAULT_DISTANCE;
            _rotation = transform.rotation;
            _pivot = _transform.position + _rotation * Vector3.forward * _distance;
            
            _panIcon = Resources.Load<Texture2D>("Cursors/Pan");
            _orbitIcon = Resources.Load<Texture2D>("Cursors/Orbit");
            _fpsIcon = Resources.Load<Texture2D>("Cursors/FPS");
            
            SettingsManager.Instance.OnSettingsChanged.AddListener(RefreshSettings);
            SelectionManager.Instance.OnSelectionChanged += OnSelectionChanged;
            _mode.OnValueChanged += OnModeChanged;
        }

        private void OnDisable()
        {
            SettingsManager.Instance.OnSettingsChanged.RemoveListener(RefreshSettings);
            SelectionManager.Instance.OnSelectionChanged -= OnSelectionChanged;
            _mode.OnValueChanged -= OnModeChanged;
        }

        private void OnSelectionChanged(List<Interaction> selectedInteractions)
        {
            if (selectedInteractions == null || selectedInteractions.Count == 0)
            {
                _focusTarget = null;
                return;
            }

            _focusTarget = selectedInteractions.Last().Target;
        }
        
        private void OnModeChanged(CameraMode mode)
        {
            SetCursor(mode);
            if (_debug) Debug.Log($"Camera Mode changed to: {mode}");
        }

        private void OnValidate()
        {
            RefreshSettings();
        }

        private void Update()
        {
            if (_updateLoop == UpdateLoop.Update) LocalUpdate();
        }

        private void FixedUpdate()
        {
            if (_updateLoop == UpdateLoop.FixedUpdate) LocalUpdate();
        }

        private void LocalUpdate()
        {
            _motionDirection = Vector3.zero;

            if (_mode.Value != CameraMode.Pan) _pan = false;

            if (!Utils.IsPointerOverScreen(Mouse.current.position.ReadValue()))
            {
                _mode.Value = CameraMode.None;
            }

            switch (_mode.Value)
            {
                case CameraMode.None:
                    break;
                case CameraMode.FPS:
                    FPS();
                    break;
                case CameraMode.Pan:
                    Pan();
                    break;
                case CameraMode.Orbit:
                    Orbit();
                    break;
                case CameraMode.Zoom:
                    Zoom();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SetTransform();
            SetPointerPosition();
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!_fps)
            {
                _moveInput = Vector2.zero;
                return;
            }

            _moveInput = context.ReadValue<Vector3>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!_fps)
            {
                _lookInput = Vector2.zero;
                return;
            }

            _lookInput = context.ReadValue<Vector2>();
        }

        public void OnFPS(InputAction.CallbackContext context)
        {
            if (ContextPerformedAndNotOverUI(context))
            {
                _fps = true;
                _mode.Value = CameraMode.FPS;
            }
            else
            {
                _fps = false;
                _mode.Value = CameraMode.None;
            }
        }

        public void OnPan(InputAction.CallbackContext context)
        {
            if (ContextPerformedAndNotOverUI(context))
            {
                _mode.Value = CameraMode.Pan;
            }
            else
            {
                _mode.Value = CameraMode.None;
            }
        }
        
        public void OnZoom(InputAction.CallbackContext context)
        {
            if (ContextPerformedAndNotOverUI(context))
            {
                _mode.Value = CameraMode.Zoom;
            }
            else
            {
                _mode.Value = CameraMode.None;
            }

            _zoomInput = context.ReadValue<Vector2>();
        }

        public void OnFocus(InputAction.CallbackContext context)
        {
            if (_debug) Debug.Log($"Focus value: {context.performed}");
            
            // TO DO: proper implementation
        }

        public void OnOrbit(InputAction.CallbackContext context)
        {
            if (_debug) Debug.Log($"Orbit value: {context.performed}");

            if (ContextPerformedAndNotOverUI(context))
            {
                _mode.Value = CameraMode.Orbit;
            }
            else
            {
                _mode.Value = CameraMode.None;
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _sprint = true;
            }
            else
            {
                _sprint = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (!_showGizmos) return;
            if (!Application.isPlaying) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(_pivot, 0.04f);
            Gizmos.DrawLine(_transform.position, _pivot);
        }

        private void RefreshSettings()
        {
            _rotationSpeed = ROTATION_SPEED * _mouseSensitivity;
            _moveSpeed = MOVE_SPEED * _moveSensitivity;
            _scrollSpeed = SCROLL_SPEED * _scrollSensitivity;
        }

        private void SetTransform()
        {
            var position = _pivot - _rotation * Vector3.forward * _distance;
            position = Vector3.Lerp(_transform.position, position, 0.4f);
            var rotation = Quaternion.Slerp(_transform.rotation, _rotation, 0.4f);
            var eulerAngles = rotation.eulerAngles;
            eulerAngles.z = 0;
            _transform.SetPositionAndRotation(position, Quaternion.Euler(eulerAngles));
        }

        private Vector3 GetMovementDirection()
        {
            _isMoving = _motionDirection.sqrMagnitude > 0;
            var speedModifier = 1;

            if (_sprint)
            {
                speedModifier *= 5;
            }
            
            var speed = _isMoving ? _moveSpeed * speedModifier : 0f; 

            //_flySpeed.speed = 4;
            //_flySpeed.target = _motionDirection.normalized * (_flyTargetSpeed * speedModifier);
            
            _flySpeed = Vector3.MoveTowards(_flySpeed, _motionDirection.normalized * speed, FLY_ACCELERATION);
            return _flySpeed * GetDeltaTime();
        }
        
        private void FPS()
        {
            var delta = _lookInput;
            delta *= _rotationSpeed;
            
            // The reason we calculate the camera position from the pivot, rotation and distance,
            // rather than just getting it from the camera transform is that the camera transform
            // is the *output* of camera motion calculations. It shouldn't be input and output at the same time,
            // otherwise we easily get accumulated error.
            // We did get accumulated error before when we did this - the camera would continuously move slightly in FPS mode
            // even when not holding down any arrow/WASD keys or moving the mouse.
            var position = _pivot - _rotation * Vector3.forward * _distance;
            
            var rotation = _rotation;
            rotation = Quaternion.AngleAxis(-delta.y, rotation * Vector3.right) * rotation;
            rotation = Quaternion.AngleAxis(delta.x, Vector3.up) * rotation;
            _rotation = rotation;

            _pivot = position + rotation * Vector3.forward * _distance;

            if (_moveInput.z > 0)
            {
                _motionDirection = Vector3.up;
            }
            else if (_moveInput.z < 0)
            {
                _motionDirection = Vector3.down;
            }

            _motionDirection += _rotation * Vector3.forward * _moveInput.y;
            _motionDirection += _rotation * Vector3.right * _moveInput.x;
            _pivot += GetMovementDirection();
        }

        private Plane _dragPlane;
        
        private void Pan()
        {
            if (!_pan)
            {
                _lastMousePosition = Input.mousePosition;
                
                if (SelectionManager.Instance.HitGameObjects.Count > 0)
                {
                    _dragPlane = new Plane(-transform.forward,
                        SelectionManager.Instance.HitGameObjects.First().transform.position);
                }
                else
                {
                    var groundPlane = new Plane(Vector3.up, Vector3.zero);
                    var magnitudeMax = Mathf.Max(100, _distance);
                    var magnitudeMin = Mathf.Min(10, _distance);
                    if (groundPlane.Raycast(SelectionManager.Instance.Pointer, out float distance) &&
                        distance < magnitudeMax)
                    {
                        _dragPlane = new Plane(-transform.forward,
                            SelectionManager.Instance.Pointer.GetPoint(distance));
                    }
                    else
                    {
                        _dragPlane = new Plane(-transform.forward,
                            transform.position + transform.forward * magnitudeMin);
                    }
                }
                
                _pan = true;
                return;
            }
            
            if (GetPointOnPlane(_dragPlane, Input.mousePosition, out Vector3 pointOnDragPlane) && GetPointOnPlane(_dragPlane, _lastMousePosition, out Vector3 lastPointOnDragPlane))
            {
                _lastMousePosition = Input.mousePosition;
                var worldDelta = pointOnDragPlane - lastPointOnDragPlane;
                if (_sprint)
                {
                    worldDelta *= 4;
                }

                _pivot -= worldDelta;
            }
        }

        public void Zoom()
        {
            _distance -= _zoomInput.y * _scrollSpeed;
            _distance = Mathf.Min(_distance, 150f);
            
            if (_distance < 0.1f)
            {
                var delta = 0.1f - _distance;
                _distance = 0.1f;
                _pivot += transform.forward * delta;
            }
        }

        private void Orbit()
        {
            var delta = new Vector2(_lookInput.x, _lookInput.y);
            if (delta.sqrMagnitude < Mathf.Epsilon) return;
            delta *= _rotationSpeed;
            
            var rotation = _rotation;
            rotation = Quaternion.AngleAxis(-delta.y, rotation * Vector3.right) * rotation;
            rotation = Quaternion.AngleAxis(delta.x, Vector3.up) * rotation;
            _rotation = rotation;
        }

        public void Focus()
        {
            if (_focusTarget == null) return;
            
            StopAllCoroutines();

            if (!_isFocused)
            {
                FocusOn(_focusTarget, true);
                _isFocused = true;
            }
            else
            {
                FocusOn(_focusTarget);
                _isFocused = false;
            }
        }

        public void FocusOn(GameObject target, bool useBounds = false)
        {
            if (useBounds)
            {
                var bounds = GetBoundingBoxOfGameObject(target);
                _distance = bounds.extents.magnitude * DEFAULT_DISTANCE + _camera.nearClipPlane;
                _pivot = bounds.center;
            }
            else
            {
                _distance = DEFAULT_DISTANCE;
                _pivot = target.transform.position;
            }
        }

        private static Bounds GetBoundingBoxOfGameObject(GameObject gameObject)
        {
            Bounds bounds = new(gameObject.transform.position, Vector3.zero);
            foreach (var item in gameObject.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(item.bounds);
            }
            return bounds;
        }

        private bool ContextPerformedAndNotOverUI(InputAction.CallbackContext context)
        {
            return context.performed && !_isPointerOverUI;
        }
        
        private void SetPointerPosition()
        {
            _isPointerOverUI = UIManager.Instance.IsPointerOverUI;
        }

        private enum UpdateLoop
        {
            Update,
            FixedUpdate
        }
        
        public enum CameraMode
        {
            None,
            FPS,
            Pan,
            Orbit,
            Zoom
        }

        private float GetDeltaTime()
        {
            return _updateLoop switch
            {
                UpdateLoop.Update => Time.unscaledDeltaTime,
                UpdateLoop.FixedUpdate => Time.fixedUnscaledDeltaTime,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private void SetCursor(CameraMode type)
        {
            switch (type)
            {
                case CameraMode.None:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
                case CameraMode.Pan:
                    Cursor.SetCursor(_panIcon, Vector2.zero, CursorMode.Auto);
                    break;
                case CameraMode.Orbit:
                    Cursor.SetCursor(_orbitIcon, Vector2.zero, CursorMode.Auto);
                    break;
                case CameraMode.Zoom:
                    Cursor.SetCursor(_zoomIcon, Vector2.zero, CursorMode.Auto);
                    break;
                case CameraMode.FPS:
                    Cursor.SetCursor(_fpsIcon, Vector2.zero, CursorMode.Auto);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private bool GetPointOnPlane(Plane plane, Vector3 mouse, out Vector3 point)
        {
            var ray = _camera.ScreenPointToRay(mouse);
            if (plane.Raycast(ray, out var distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }
            
            point = Vector3.zero;
            return false;
        }
    }
}