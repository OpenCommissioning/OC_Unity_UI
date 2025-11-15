using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace OC.UI
{
    public class PanMover : MonoBehaviour, IInputAxisOwner
    {
        [Header("Input Axes")]
        public InputAxis Horizontal = InputAxis.DefaultMomentary;
        public InputAxis Vertical = InputAxis.DefaultMomentary;

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
        }

        private void OnValidate()
        {
            Horizontal.Validate();
            Vertical.Validate();
        }

        private void Update()
        {
            if (!_cam.IsLive) return;
            Pan();
        }
        
        private void Pan()
        {
            var movement = _trackingTarget.rotation * new Vector3(Horizontal.Value, Vertical.Value, 0);
            _trackingTarget.SetPositionAndRotation(_trackingTarget.position + 1 * Time.deltaTime * movement, _cam.State.RawOrientation);
        }
    }
}
