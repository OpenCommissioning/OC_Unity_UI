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

        private CursorHandler _cursorHandler;


        private void Start()
        {
            _cursorHandler = new CursorHandler();
            _cameraMode.OnValueChanged += OnModeChanged;
        }
        
        private void OnModeChanged(CameraMode mode)
        {
            _cursorHandler.SetCursor(mode);
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
            if(!UserInputSystem.Instance.IsPointerOverScreen) return;

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
