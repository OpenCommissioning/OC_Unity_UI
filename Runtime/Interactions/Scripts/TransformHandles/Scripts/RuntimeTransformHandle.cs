using System;
using System.Collections.Generic;
using System.Linq;
using OC.Interactions;
using OC.UI.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.TransformHandles
{
    public class RuntimeTransformHandle : MonoBehaviourSingleton<RuntimeTransformHandle>
    {
        public IProperty<ToolType> Tool => _toolType;
        public IProperty<PivotMode> Pivot => _pivotMode;
        public IProperty<CoordinateSpace> Coordinate => _coordinateSpace;
        
        public Camera Camera => _camera;
        
        public float RotationSnap => _rotationSnap;

        public List<Transform> Targets
        {
            get => _targets;
            set => _targets = value;
        }

        [Header("State")]
        [SerializeField] 
        private bool _dragged;
        [SerializeField] 
        private List<Transform> _targets = new ();
        [SerializeField] 
        private Property<ToolType> _toolType = new (ToolType.View);
        [SerializeField] 
        private Property<PivotMode> _pivotMode = new (PivotMode.Pivot);
        [SerializeField] 
        private Property<CoordinateSpace> _coordinateSpace = new(CoordinateSpace.Local);
        
        [Header("Settings")]
        [SerializeField] 
        private bool _autoScale = true;
        [SerializeField] 
        private float _autoScaleFactor = 1;
        [SerializeField] 
        private float _rotationSnap;
        [SerializeField] 
        private GameObject _positionHandle;
        [SerializeField] 
        private GameObject _rotationHandle;
        
        [Header("Debug")]
        [SerializeField] 
        private bool _debug;
        
        [Header("Input Actions")]
        [SerializeField]
        private InputActionReference _click;
        [SerializeField]
        private InputActionReference _pointer;

        private Vector3 _previousMousePosition;
        private HandleBase _previousHandle;
        private HandleBase _draggingHandle;
        private bool _rotating;

        private Camera _camera;
        private InputAction _clickAction;
        private InputAction _pointerAction;
        private HandleRaycastHit _handleRaycastHit;

        private void Start()
        {
            _camera = Camera.main;

            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }

        private void OnEnable()
        {
            SelectionManager.Instance.OnSelectionChanged += SelectedObjectsChanged;
            
            _clickAction = _click.action;
            _pointerAction = _pointer.action;
            
            _clickAction?.Enable();
            _pointerAction?.Enable();

            if (_clickAction != null)
            {
                _clickAction.started += HandleClickAction;
                _clickAction.performed += HandleClickAction;
                _clickAction.canceled += HandleClickAction;
            }
        }
        
        private void OnDisable()
        {
            SelectionManager.Instance.OnSelectionChanged -= SelectedObjectsChanged;
            
            _clickAction.started -= HandleClickAction;
            _clickAction.performed -= HandleClickAction;
            _clickAction.canceled -= HandleClickAction;
        }
        
        private void SelectedObjectsChanged(List<Interaction> selectedObjects)
        {
            if (selectedObjects.Count == 0)
            {
                _targets.Clear();
                return;
            }

            _targets = selectedObjects.Select(x => x.Target.transform).ToList();
        }

        private void Update()
        {
            _handleRaycastHit = SelectionManager.Instance.HandleRaycastHit;
            
            ManageHandles();
            ManageScale();
            ManageHandleTransform();
            HandleOverEffect();

            if (_dragged && _draggingHandle is not null)
            {
                if (_debug) Debug.Log($"Drag Action");
                
                _draggingHandle.Interact(_pointerAction.ReadValue<Vector2>());
                if (_draggingHandle is RotationAxis) _rotating = true;
            }
        }
        
        private void HandleClickAction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (AppUI.Instance.IsPointerOverUI) return;
                if (!_handleRaycastHit.Hit) return;
                
                if (_debug) Debug.Log($"Handle Click action: {context.phase}");
                
                _draggingHandle = _handleRaycastHit.HitHandle;
                _draggingHandle.StartInteraction(_pointerAction.ReadValue<Vector2>(), _handleRaycastHit.RaycastHit.point);
                _dragged = true;
            }

            if (context.performed)
            {
                
            }
            
            if (context.canceled)
            {
                if (_draggingHandle == null) return;
                
                if (_debug) Debug.Log($"Handle Click action: {context.phase}");
                
                _draggingHandle.EndInteraction(_pointerAction.ReadValue<Vector2>());
                if (_draggingHandle is RotationAxis) _rotating = false;
                _draggingHandle = null;
                _dragged = false;
            }
        }

        public void AddGameObjectToTargets(GameObject go)
        {
            _targets.Add(go.transform);
            _targets = _targets.Distinct().ToList();
        }

        public void RemoveGameObjectFromTargets(GameObject go)
        {
            _targets.Remove(go.transform);
            _targets = _targets.Distinct().ToList();
        }

        public void ClearTargets()
        {
            _targets.Clear();
            _targets = _targets.Distinct().ToList();

        }

        private void ManageHandles()
        {
            if (_targets.Count == 0)
            {
                _positionHandle.SetActive(false);
                _rotationHandle.SetActive(false);
                return;
            }

            switch (_toolType.Value)
            {
                case ToolType.View:
                    _positionHandle.SetActive(false);
                    _rotationHandle.SetActive(false);
                    break;
                case ToolType.Move:
                    _positionHandle.SetActive(true);
                    _rotationHandle.SetActive(false);
                    break;
                case ToolType.Rotation:
                    _positionHandle.SetActive(false);
                    _rotationHandle.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ManageScale()
        {
            if (_autoScale)
            {
                transform.localScale = Vector3.one * (Vector3.Distance(Camera.transform.position, transform.position) * _autoScaleFactor) / 15;
            }
        }

        private void ManageHandleTransform()
        {
            if (_targets.Count > 0)
            {
                if (_pivotMode == PivotMode.Center && !_rotating)
                {
                    transform.position = GetCommonCenter();
                    transform.rotation = _targets.Last().rotation;
                }
                else if (_pivotMode == PivotMode.Pivot)
                {
                    transform.position = _targets.Last().position;
                    transform.rotation = _targets.Last().rotation;
                }
            }

            if (_coordinateSpace == CoordinateSpace.World)
            {
                transform.rotation = Quaternion.identity;
            }
        }

        private void ActivateHandle(HandleBase handle)
        {
            if (handle is not null)
            {
                if (_previousHandle is not null && _previousHandle != handle)
                {
                    _previousHandle.SetColor(_previousHandle.DefaultColor);
                }
                
                handle.SetColor(Color.yellow);
            }
            else
            {
                _previousHandle?.SetColor(_previousHandle.DefaultColor);
            }
            
            _previousHandle = handle;
        }

        private void HandleOverEffect()
        {
            ActivateHandle(_draggingHandle ?? _handleRaycastHit.HitHandle);
        }

        private Vector3 GetCommonCenter()
        {
            var lowX = _targets.Min(t => t.position.x);
            var highX = _targets.Max(t => t.position.x);

            var lowY = _targets.Min(t => t.position.y);
            var highY = _targets.Max(t => t.position.y);

            var lowZ = _targets.Min(t => t.position.z);
            var highZ = _targets.Max(t => t.position.z);

            var x = lowX + (highX - lowX) / 2;
            var y = lowY + (highY - lowY) / 2;
            var z = lowZ + (highZ - lowZ) / 2;
            
            return new Vector3(x, y, z);
        }
    }
}


public enum ToolType
{
    View,
    Move,
    Rotation
}

public enum PivotMode
{
    Pivot,
    Center
}

public enum CoordinateSpace
{
    Local,
    World
}