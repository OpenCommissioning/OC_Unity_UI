using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OC.Interactions;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.Interactions
{
    public class OrbitCameraController : CameraControllerBase
    {
        public float DistanceToPivot => _distanceToPivot;
        public bool IsOrbiting => _isOrbiting; 
        
        [SerializeField] private InputActionProperty _orbitActionProperty;
        [SerializeField] private InputActionProperty _zoomActionProperty;
        [SerializeField] private InputActionProperty _focusActionProperty;

        private InputAction _orbitAction;
        private InputAction _zoomAction;
        private InputAction _focusAction;

        private const float DEFAULT_FOCUS_DISTANCE = 3f;
        private const float DEFAULT_PIVOT_DISTANCE = 3f;

        private float _distanceToPivot = 1f;
        private bool _isFocused = false;
        private bool _isOrbiting = false;
        private GameObject _focusTarget;

        [SerializeField] private float _zoomInputGain = 0;
        [SerializeField] private float _zoomInputGainMaster = 0;
        [SerializeField] private float _rotationGain = 0;
        

        private void Start()
        {
            RegisterActions();
            _orbitAction.Enable();
            _zoomAction.Enable();
            _focusAction.Enable();
            SelectionManager.Instance.OnSelectionChanged += OnSelectionChanged;
            _distanceToPivot = DEFAULT_PIVOT_DISTANCE;
        }

        private void OnEnable()
        {
            _orbitAction?.Enable();
            _zoomAction?.Enable();
            _focusAction?.Enable();
            SelectionManager.Instance.OnSelectionChanged += OnSelectionChanged;
            _controllerMaster.RotationGain.Subscribe(OnRotationGainChangedAction);
            _controllerMaster.ScrollGain.Subscribe(OnScrollGainChangedAction);
        }

        private void OnDisable()
        {
            _orbitAction?.Disable();
            _zoomAction?.Disable();
            _focusAction?.Disable();
            SelectionManager.Instance.OnSelectionChanged -= OnSelectionChanged;
            _controllerMaster.RotationGain.Unsubscribe(OnRotationGainChangedAction);
            _controllerMaster.ScrollGain.Unsubscribe(OnScrollGainChangedAction);
        }

        public override void Enable()
        {
            SetPivotBeforeCamChange();
            base.Enable();
        }

        private void RegisterActions()
        {
            _orbitAction = _orbitActionProperty.reference != null ? _orbitActionProperty.reference.action : _orbitActionProperty.action;
            _orbitAction.started += OnOrbitStarted;
            _orbitAction.canceled += OnOrbitCanceled;

            _zoomAction = _zoomActionProperty.reference != null ? _zoomActionProperty.reference.action : _zoomActionProperty.action;
            _zoomAction.started += OnZoomStarted;
            _zoomAction.canceled += OnZoomCanceled;

            _focusAction = _focusActionProperty.reference != null ? _focusActionProperty.reference.action : _focusActionProperty.action;
            _focusAction.started += OnFocusStarted;
        }

        private void OnOrbitStarted(InputAction.CallbackContext context)
        {
            if(_controllerMaster.Brain.IsBlending) return;
            _isOrbiting = true;
            Enable();
        }

        private void OnOrbitCanceled(InputAction.CallbackContext context)
        {
            _isOrbiting = false;
            Disable();
        }

        private void OnZoomStarted(InputAction.CallbackContext context)
        {
            if(_controllerMaster.Brain.IsBlending) return;
            if(!Utils.IsPointerOverScreen(Mouse.current.position.value)) return;
            if(UIManager.Instance.IsUIFieldSelected) return;

            SetZoomInputGain();

            // Disable orbit input while zooming
            StopAllCoroutines();
            StartCoroutine(DisableAxisControllerForSeconds("Look Orbit X", 0.1f));
            StartCoroutine(DisableAxisControllerForSeconds("Look Orbit Y", 0.1f));

            Enable();
        }

        private void OnZoomCanceled(InputAction.CallbackContext context)
        {
            var orbitalFollowComponent = _camera.GetComponent<CinemachineOrbitalFollow>();
            _distanceToPivot = orbitalFollowComponent.RadialAxis.Value * orbitalFollowComponent.Radius;

            Disable();
        }

        private void OnFocusStarted(InputAction.CallbackContext context)
        {
            if(_controllerMaster.Brain.IsBlending) return;
            if(UIManager.Instance.IsUIFieldSelected) return;
            //if(!UIManager.Instance.IsPointerOverUI) return;
            if(_focusTarget == null) return;

            // Disable orbit input while focus
            StopAllCoroutines();
            StartCoroutine(DisableAxisControllerForSeconds("Look Orbit X", 0.1f));
            StartCoroutine(DisableAxisControllerForSeconds("Look Orbit Y", 0.1f));

            Enable();

            if (!_isFocused)
            {
                FocusTarget(_focusTarget, true);
                _isFocused = true;
            }
            else
            {
                FocusTarget(_focusTarget);
                _isFocused = false;
            }
        }

        public void FocusTarget(GameObject target, bool useBounds = false)
        {
            if (useBounds)
            {
                var bounds = GetBoundingBoxOfGameObject(target);
                _distanceToPivot = bounds.extents.magnitude * DEFAULT_FOCUS_DISTANCE + _camera.Lens.NearClipPlane;
                ChangeTargetPosition(bounds.center);
                SetOrbitalFollowDistance(_distanceToPivot);
            }
            else
            {
                _distanceToPivot = DEFAULT_FOCUS_DISTANCE;
                ChangeTargetPosition(target.transform.position);
                SetOrbitalFollowDistance(_distanceToPivot);
            }

            // Wait for the blend from the OnFocusStared method and then disable the camera
            StartCoroutine(WaitForCameraBlend());
        }

        private void ChangeTargetPosition(Vector3 position)
        {
            //_camera.PreviousStateIsValid = false;
            _camera.Target.TrackingTarget.position = position;
        }

        private void SetOrbitalFollowDistance(float distance)
        {
            var orbitalFollowComponent = _camera.GetComponent<CinemachineOrbitalFollow>();
            orbitalFollowComponent.RadialAxis.Value = distance;
        }

        private IEnumerator DisableAxisControllerForSeconds(string name, float seconds)
        {
            _inputAxisController.GetController(name).Enabled = false;
            yield return new WaitForSeconds(seconds);
            _inputAxisController.GetController(name).Enabled = true;
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

        private static Bounds GetBoundingBoxOfGameObject(GameObject gameObject)
        {
            Bounds bounds = new(gameObject.transform.position, Vector3.zero);
            foreach (var item in gameObject.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(item.bounds);
            }
            return bounds;
        }

        private IEnumerator WaitForCameraBlend()
        {
            yield return new WaitUntil(() => !_controllerMaster.Brain.IsBlending);
            Disable();
        }

        private void OnRotationGainChangedAction(float value)
        {
            _rotationGain = value;
            _inputAxisController.GetController("Look Orbit X").Input.Gain = _rotationGain;
            _inputAxisController.GetController("Look Orbit Y").Input.Gain = -_rotationGain;
        }

        private void OnScrollGainChangedAction(float value)
        {
            _zoomInputGainMaster = value; 
        }

        private void SetPivotBeforeCamChange()
        {
            ICinemachineCamera activeCam = _controllerMaster.Brain.ActiveVirtualCamera;
            Vector3 camPosition = activeCam.State.RawPosition;
            Vector3 camForward = activeCam.State.GetFinalOrientation() * Vector3.forward;
            Vector3 teleportPosition = camPosition + camForward * _distanceToPivot;

            _camera.PreviousStateIsValid = false;
            _camera.Target.TrackingTarget.position = teleportPosition;
        }

        private void SetZoomInputGain()
        {
            _zoomInputGain = GetInputGainFromPivotDistance();
            _inputAxisController.GetController("Orbit Scale").Input.Gain = - _zoomInputGain;
        }

        private float GetInputGainFromPivotDistance()
        {
            return _distanceToPivot * _zoomInputGainMaster;
        }
    }
}
