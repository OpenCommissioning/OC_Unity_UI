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

        public List<Transform> Targets
        {
            get => _targets;
            set => _targets = value;
        }

        [SerializeField] 
        private Property<ToolType> _toolType = new (ToolType.View);
        [SerializeField] 
        private Property<PivotMode> _pivotMode = new (PivotMode.Pivot);
        [SerializeField] 
        private Property<CoordinateSpace> _coordinateSpace = new(CoordinateSpace.Local);

        [HideInInspector] 
        public Camera HandleCamera;
        [HideInInspector] 
        public float RotationSnap;
        [SerializeField] 
        private bool _autoScale = true;
        [SerializeField] 
        public float AutoScaleFactor = 1;
        [SerializeField] 
        private GameObject _positionHandle;
        [SerializeField] 
        private GameObject _rotationHandle;
        [SerializeField] 
        private List<Transform> _targets;
        [SerializeField]
        private InputActionProperty _inputActionPropertyClick;
        [SerializeField]
        private InputActionProperty _inputActionPropertyPointer;

        private Vector3 _previousMousePosition;
        private HandleBase _previousHandle;
        private HandleBase _draggingHandle;
        private bool _rotating;

        private Camera _camera;
        private InputAction _inputActionClick;
        private InputAction _inputActionPointer;

        private void Start()
        {
            _camera = Camera.main;
            
            if (HandleCamera == null) HandleCamera = Camera.main;
            if (_targets == null) _targets[0] = transform;

            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }

            SelectionManager.Instance.OnSelectionChanged += SelectedObjectsChanged;

            _inputActionClick = _inputActionPropertyClick.reference.action;
            _inputActionPointer = _inputActionPropertyPointer.reference.action;
        }

        private void OnDisable()
        {
            SelectionManager.Instance.OnSelectionChanged -= SelectedObjectsChanged;
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
            ManageHandles();
            ManageScale();

            HandleBase handle = null;
            Vector3 hitPoint = Vector3.zero;
            GetHandle(ref handle, ref hitPoint);

            HandleOverEffect(handle);
            ManageHandleTransform();

            if (_inputActionClick.IsPressed() && _draggingHandle != null)
            {
                _draggingHandle.Interact(_previousMousePosition);
                if (_draggingHandle is RotationAxis) _rotating = true;
            }

            if (_inputActionClick.IsPressed())
            {
                GetHandle(ref handle, ref hitPoint);
                if (handle != null)
                {
                    _draggingHandle = handle;
                    _draggingHandle.StartInteraction(hitPoint);
                }

                HandleOverEffect(_draggingHandle);
            }

            if (!_inputActionClick.IsPressed() && _draggingHandle != null)
            {
                _draggingHandle.EndInteraction();
                if (_draggingHandle is RotationAxis) _rotating = false;
                _draggingHandle = null;
            }
            
            _previousMousePosition = _inputActionPointer.ReadValue<Vector2>();
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
            }
        }

        private void ManageScale()
        {
            if (_autoScale)
                transform.localScale =
                    Vector3.one * (Vector3.Distance(HandleCamera.transform.position, transform.position) *
                                   AutoScaleFactor) / 15;
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

        private void HandleOverEffect(HandleBase handle)
        {
            if (_draggingHandle == null && _previousHandle != null && _previousHandle != handle)
            {
                _previousHandle.SetColor(_previousHandle._defaultColor);
            }

            if (handle != null && _draggingHandle == null)
            {
                handle.SetColor(Color.yellow);
            }

            _previousHandle = handle;
        }

        private void GetHandle(ref HandleBase handle, ref Vector3 hitPoint)
        {
            //TODO Need to refactor. The same Raycast is in the SelectionManager class
            
            var pointerPosition = _inputActionPointer.ReadValue<Vector2>();
            
            if (pointerPosition.sqrMagnitude > 1e16) return;
            
            var ray = _camera.ScreenPointToRay(pointerPosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length == 0) return;

            foreach (RaycastHit hit in hits)
            {
                handle = hit.collider.gameObject.GetComponentInParent<HandleBase>();

                if (handle != null)
                {
                    hitPoint = hit.point;
                    return;
                }
            }
        }

        public Vector3 GetCommonCenter()
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
            
            return new Vector3(x, y, z);;
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