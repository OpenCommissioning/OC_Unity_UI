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
        [SerializeField] private Transform _followTarget;
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
                SetFollowTarget();
                _fpsCamera.Priority = 2;
                _cameraMode.Value = CameraMode.FPS;
            }
            else if(context.canceled)
            {
                _fpsCamera.Priority = 1;
                _cameraMode.Value = CameraMode.None;
            }
        }

        public void OnOrbit(InputAction.CallbackContext context)
        {
            if (ContextStartedAndNotOverUI(context))
            {
                SetPivot();
            }
            if (ContextPerformedAndNotOverUI(context))
            {
                
                _orbitCamera.Priority = 2;
                _cameraMode.Value = CameraMode.Orbit;
            }
            else if(context.canceled)
            {
                _orbitCamera.Priority = 1;
                _cameraMode.Value = CameraMode.None;
            }
        }

        public void OnPan(InputAction.CallbackContext context)
        {
            if (ContextStartedAndNotOverUI(context))
            {
                SetFollowTarget();
            }
            if (ContextPerformedAndNotOverUI(context))
            {
                _panCamera.Priority = 2;
                _cameraMode.Value = CameraMode.Pan;
            }
            else if(context.canceled)
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
                SetPivot();
                // Disable orbit input while zooming
                var axisControllers = _orbitCamera.GetComponent<CinemachineInputAxisController>().Controllers;
                foreach(var controller in axisControllers)
                {
                    if(controller.Name == "Look Orbit X" || controller.Name == "Look Orbit Y")
                    {
                        controller.Enabled = false;
                    }
                }
            }
            if (ContextPerformedAndNotOverUI(context))
            {
                _orbitCamera.Priority = 2;
                _cameraMode.Value = CameraMode.Zoom;
            }
            else if(context.canceled)
            {
                _distance = _orbitCamera.GetComponent<CinemachineOrbitalFollow>().RadialAxis.Value * _orbitCamera.GetComponent<CinemachineOrbitalFollow>().Radius;
                _orbitCamera.Priority = 1;
                _cameraMode.Value = CameraMode.None;
                // Enable orbit input while zooming
                var axisControllers = _orbitCamera.GetComponent<CinemachineInputAxisController>().Controllers;
                foreach(var controller in axisControllers)
                {
                    if(controller.Name == "Look Orbit X" || controller.Name == "Look Orbit Y")
                    {
                        controller.Enabled = true;
                    }
                }
            }
        }

        private void SetFollowTarget()
        {
            ICinemachineCamera activeCam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;
            Vector3 camPosition = activeCam.State.RawPosition;
            _followTarget.position = camPosition;
        }

        private void SetPivot()
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

        private bool ContextPerformedAndNotOverUI(InputAction.CallbackContext context)
        {
            return context.performed && !_isPointerOverUI;
        }

        private bool ContextStartedAndNotOverUI(InputAction.CallbackContext context)
        {
            return context.started && !_isPointerOverUI;
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
