using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OC.UI.Interactions;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.Interactions
{
    public class OrbitCameraController : CameraControllerBase
    {
        [SerializeField] private InputActionProperty _orbitActionProperty;
        [SerializeField] private InputActionProperty _zoomActionProperty;
        [SerializeField] private InputActionProperty _focusActionProperty;

        private InputAction _orbitAction;
        private InputAction _zoomAction;
        private InputAction _focusAction;

        private const float DEFAULT_DISTANCE = 3f;

        private float _distanceToPivot = 1f;
        private bool _isFocused = false;
        private GameObject _focusTarget;
        

        private void Start()
        {
            RegisterActions();
            _orbitAction.Enable();
            _zoomAction.Enable();
            _focusAction.Enable();
            SelectionManager.Instance.OnSelectionChanged += OnSelectionChanged;
        }

        private void OnEnable()
        {
            _orbitAction?.Enable();
            _zoomAction?.Enable();
            _focusAction?.Enable();
            SelectionManager.Instance.OnSelectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            _orbitAction?.Disable();
            _zoomAction?.Disable();
            _focusAction?.Disable();
            SelectionManager.Instance.OnSelectionChanged -= OnSelectionChanged;
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
            Enable();
        }

        private void OnOrbitCanceled(InputAction.CallbackContext context)
        {
            Disable();
        }

        private void OnZoomStarted(InputAction.CallbackContext context)
        {
            if(!UserInputSystem.Instance.IsPointerOverScreen) return;
            if(UIManager.Instance.IsUIFieldSelected) return;

            // Disable orbit input while zooming
            SetAxisControllerState("Look Orbit X", false);
            SetAxisControllerState("Look Orbit Y", false);

            Enable();
        }

        private void OnZoomCanceled(InputAction.CallbackContext context)
        {
            var orbitalFollowComponent = _camera.GetComponent<CinemachineOrbitalFollow>();
            _distanceToPivot = orbitalFollowComponent.RadialAxis.Value * orbitalFollowComponent.Radius;

            Disable();

            // Enable orbit input when not zooming
            SetAxisControllerState("Look Orbit X", true);
            SetAxisControllerState("Look Orbit Y", true);
        }

        private void OnFocusStarted(InputAction.CallbackContext context)
        {
            if(UIManager.Instance.IsUIFieldSelected) return;
            //if(!UIManager.Instance.IsPointerOverUI) return;
            if(_focusTarget == null) return;

            // Disable orbit input while focus
            SetAxisControllerState("Look Orbit X", false);
            SetAxisControllerState("Look Orbit Y", false);

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
                _distanceToPivot = bounds.extents.magnitude * DEFAULT_DISTANCE + _camera.Lens.NearClipPlane;
                ChangeTargetPosition(bounds.center);
                SetOrbitalFollowDistance(_distanceToPivot);
            }
            else
            {
                _distanceToPivot = DEFAULT_DISTANCE;
                ChangeTargetPosition(target.transform.position);
                SetOrbitalFollowDistance(_distanceToPivot);
            }
            
            StopAllCoroutines();
            StartCoroutine(WaitForCameraBlend(CinemachineCore.FindPotentialTargetBrain(_camera)));

            // Enable orbit input when focused
            SetAxisControllerState("Look Orbit X", true);
            SetAxisControllerState("Look Orbit Y", true);
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
            orbitalFollowComponent.Radius = DEFAULT_DISTANCE;
            _distanceToPivot = orbitalFollowComponent.RadialAxis.Value * orbitalFollowComponent.Radius;
        }

        private void SetAxisControllerState(string name, bool value)
        {
            foreach(var controller in _inputAxisController.Controllers)
            {
                if(controller.Name == name)
                {
                    controller.Enabled = value;
                }
            }
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

        private IEnumerator WaitForCameraBlend(CinemachineBrain brain)
        {
            yield return new WaitUntil(() => !brain.IsBlending);
            Disable();
        }

        private void SetPivotBeforeCamChange()
        {
            ICinemachineCamera activeCam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;
            Vector3 camPosition = activeCam.State.RawPosition;
            Vector3 camForward = activeCam.State.GetFinalOrientation() * Vector3.forward;
            Vector3 teleportPosition = camPosition + camForward * _distanceToPivot;

            _camera.PreviousStateIsValid = false;
            _camera.Target.TrackingTarget.position = teleportPosition;
        }
    }
}
