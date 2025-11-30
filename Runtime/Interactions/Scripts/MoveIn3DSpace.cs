using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace OC.UI
{
    public class MoveIn3DSpace : MonoBehaviour, IInputAxisOwner
    {
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        [Tooltip("Default FPS speed")]
        [SerializeField] private float _speed = 1f;
        [Tooltip("Speed when sprinting")]
        [SerializeField] private float _sprintSpeed = 4;

        [Header("Input Axes")]
        public InputAxis Horizontal = InputAxis.DefaultMomentary;
        public InputAxis Vertical = InputAxis.DefaultMomentary;
        public InputAxis Perpendicular = InputAxis.DefaultMomentary;
        public InputAxis Sprint = InputAxis.DefaultMomentary;

        private bool _isSprinting;

        private CinemachineCamera _cam;
        private Transform _trackingTarget;

        private void Start()
        {
            _cam = GetComponent<CinemachineCamera>();
            _trackingTarget = _cam.Target.TrackingTarget;
        }

        public void GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new() { DrivenAxis = () => ref Horizontal, Name = "Horizontal", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new() { DrivenAxis = () => ref Vertical, Name = "Vertical", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
            axes.Add(new() { DrivenAxis = () => ref Perpendicular, Name = "Perpendicular", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
            axes.Add(new () { DrivenAxis = () => ref Sprint, Name = "Sprint" });
        }

        private void OnValidate()
        {
            Horizontal.Validate();
            Vertical.Validate();
            Perpendicular.Validate();
            Sprint.Validate();
        }

        private void Update()
        {
            if (!_cam.IsLive) return;
            FPS();
        }
        
        private void FPS()
        {
            _isSprinting = Sprint.Value > 0.5f;
            var desiredVelocity = _isSprinting ? _sprintSpeed : _speed;
            var movement = _trackingTarget.rotation * new Vector3(Horizontal.Value, Perpendicular.Value, Vertical.Value);
            _trackingTarget.SetPositionAndRotation(_trackingTarget.position + desiredVelocity * Time.deltaTime * movement, _cam.State.RawOrientation);
        }
    }
}
