using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IOSEF.UI.Interactions
{
    [RequireComponent(typeof(Camera))]
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

        private const float RotationSpeed = 0.5f;
        private const float MoveSpeed = 1f;
        private const float ScrollSpeed = 0.04f;
        private const float FlyAcceleration = 2f;
        private const float DefaultDistance = 3f;
        
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

        private bool _pan;
        private Vector3 _lastMousePosition;

        private void Start()
        {
            RefreshSettings();
            
            _transform = transform;
            _camera = GetComponent<Camera>();

            _distance = DefaultDistance;
            _rotation = transform.rotation;
            _pivot = _transform.position + _rotation * Vector3.forward * _distance;
            
            _panIcon = Resources.Load<Texture2D>("Cursors/Pan");
            _orbitIcon = Resources.Load<Texture2D>("Cursors/Orbit");
            _fpsIcon = Resources.Load<Texture2D>("Cursors/FPS");
            
            SettingsManager.Instance.OnSettingsChanged.AddListener(RefreshSettings);
            SelectionManager.Instance.OnSelectionChanged += OnSelectionChanged;
            _mode.ValueChanged += OnModeChanged;
        }

        private void OnDisable()
        {
            SettingsManager.Instance.OnSettingsChanged.RemoveListener(RefreshSettings);
            SelectionManager.Instance.OnSelectionChanged -= OnSelectionChanged;
            _mode.ValueChanged -= OnModeChanged;
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
            _rotationSpeed = RotationSpeed * _mouseSensitivity;
            _moveSpeed = MoveSpeed * _moveSensitivity;
            _scrollSpeed = ScrollSpeed * _scrollSensitivity;
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

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speedModifier *= 5;
            }
            
            var speed = _isMoving ? _moveSpeed * speedModifier : 0f; 

            //_flySpeed.speed = 4;
            //_flySpeed.target = _motionDirection.normalized * (_flyTargetSpeed * speedModifier);
            
            _flySpeed = Vector3.MoveTowards(_flySpeed, _motionDirection.normalized * speed, FlyAcceleration);
            return _flySpeed * GetDeltaTime();
        }
        
        private void FPS()
        {
            var delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
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

            if (Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Q))
            {
                _motionDirection = Vector3.up;
            }
            else if (!Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.Q))
            {
                _motionDirection = Vector3.down;
            }

            _motionDirection += _rotation * Vector3.forward * Input.GetAxisRaw("Vertical");
            _motionDirection += _rotation * Vector3.right * Input.GetAxisRaw("Horizontal");
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
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    worldDelta *= 4;
                }

                _pivot -= worldDelta;
            }
        }

        public void Zoom()
        {
            _distance -= Input.mouseScrollDelta.y * _scrollSpeed;
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
            var delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
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
                _distance = bounds.extents.magnitude * DefaultDistance + _camera.nearClipPlane;
                _pivot = bounds.center;
            }
            else
            {
                _distance = DefaultDistance;
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