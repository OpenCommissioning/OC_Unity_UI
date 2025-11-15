using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.Interactions
{
    public class TestCameraController : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private Property<CameraMode> _cameraMode = new (CameraMode.None);

        [Header("Camera References")]
        [SerializeField] private CinemachineCamera _fpsCamera;
        [SerializeField] private CinemachineCamera _orbitCamera;
        [SerializeField] private CinemachineCamera _panCamera;

        [Header("Testing")]
        [SerializeField] private bool _debug = false;
        [SerializeField] private bool _isPointerOverUI = false;
        [SerializeField] private Transform _pivot;
        [SerializeField] private float _distance = 5f;

        private Texture2D _panIcon;
        private Texture2D _orbitIcon;
        private Texture2D _zoomIcon;
        private Texture2D _fpsIcon;


        private void Start()
        {
            _panIcon = Resources.Load<Texture2D>("Cursors/Pan");
            _orbitIcon = Resources.Load<Texture2D>("Cursors/Orbit");
            _fpsIcon = Resources.Load<Texture2D>("Cursors/FPS");

            _cameraMode.OnValueChanged += OnModeChanged;
        }
        
        private void OnModeChanged(CameraMode mode)
        {
            SetCursor(mode);
            if (_debug) Debug.Log($"Camera Mode changed to: {mode}");
        }

        public void OnFPS(InputAction.CallbackContext context)
        {
            if (ContextPerformedAndNotOverUI(context))
            {
                SetFPSFollowTarget();
                _fpsCamera.Priority = 2;
                _cameraMode.Value = CameraMode.FPS;
            }
            else
            {
                _fpsCamera.Priority = 1;
                _cameraMode.Value = CameraMode.None;
            }
        }

        public void OnOrbit(InputAction.CallbackContext context)
        {
            if (ContextStartedAndNotOverUI(context))
            {
                SetOrbitPivot();
            }
            if (ContextPerformedAndNotOverUI(context))
            {
                
                _orbitCamera.Priority = 2;
                _cameraMode.Value = CameraMode.Orbit;
            }
            else
            {
                _orbitCamera.Priority = 1;
                _cameraMode.Value = CameraMode.None;
            }
        }

        public void OnPan(InputAction.CallbackContext context)
        {
            if (ContextStartedAndNotOverUI(context))
            {
                SetOrbitPivot();
            }
            if (ContextPerformedAndNotOverUI(context))
            {
                _panCamera.GetComponent<CinemachineFollow>().FollowOffset.z = - _distance;
                _panCamera.Priority = 2;
                _cameraMode.Value = CameraMode.Pan;
            }
            else
            {
                _panCamera.Priority = 1;
                _cameraMode.Value = CameraMode.None;
            }
        }
        
        public void OnZoom(InputAction.CallbackContext context)
        {
            if (ContextStartedAndNotOverUI(context))
            {
                SetOrbitPivot();
            }
            if (ContextPerformedAndNotOverUI(context))
            {
                _orbitCamera.Priority = 2;
                _cameraMode.Value = CameraMode.Zoom;
            }
            else
            {
                _distance = _orbitCamera.GetComponent<CinemachineOrbitalFollow>().RadialAxis.Value * _orbitCamera.GetComponent<CinemachineOrbitalFollow>().Radius;
                _orbitCamera.Priority = 1;
                _cameraMode.Value = CameraMode.None;
            }
        }

        private void SetFPSFollowTarget()
        {
            ICinemachineCamera activeCam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;
            Vector3 camPosition = activeCam.State.RawPosition;
            _fpsCamera.Target.TrackingTarget.position = camPosition;
        }

        private void SetOrbitPivot()
        {
            ICinemachineCamera activeCam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;
            Vector3 camPosition = activeCam.State.RawPosition;
            Vector3 camForward = activeCam.State.GetFinalOrientation() * Vector3.forward;
            Vector3 teleportPosition = camPosition + camForward * _distance;

            var delta = teleportPosition - _pivot.position;
            _orbitCamera.PreviousStateIsValid = false;
            _orbitCamera.OnTargetObjectWarped(_pivot, delta);

            _pivot.position = teleportPosition;
        }
        
        private void SetPivot()
        {
            ICinemachineCamera activeCam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;
            Vector3 camPosition = activeCam.State.RawPosition;
            Vector3 camForward = activeCam.State.GetFinalOrientation() * Vector3.forward;

            _orbitCamera.Target.TrackingTarget.position = camPosition + camForward * 5;
        }

        private bool ContextPerformedAndNotOverUI(InputAction.CallbackContext context)
        {
            return context.performed && !_isPointerOverUI;
        }

        private bool ContextStartedAndNotOverUI(InputAction.CallbackContext context)
        {
            return context.started && !_isPointerOverUI;
        }

        private void SetPointerPosition()
        {
            _isPointerOverUI = UIManager.Instance.IsPointerOverUI;
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
        
        public enum CameraMode
        {
            None,
            FPS,
            Pan,
            Orbit,
            Zoom
        }
    }
}
