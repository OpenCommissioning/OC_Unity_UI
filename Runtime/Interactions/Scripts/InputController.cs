using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace OC.UI
{
    public class InputController : MonoBehaviour, Unity.Cinemachine.IInputAxisOwner
    {
        [Header("Cams")]
        [SerializeField] private CinemachineCamera _movingCamera;
        [SerializeField] private CinemachineCamera _rotateCamera;

        [Header("Input Axes")]
        public InputAxis Horizontal = InputAxis.DefaultMomentary;
        public InputAxis Vertical = InputAxis.DefaultMomentary;
        public InputAxis Perpendicular = InputAxis.DefaultMomentary;

        private Vector3 _position;

        public void GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new() { DrivenAxis = () => ref Horizontal, Name = "Horizontal" });
            axes.Add(new() { DrivenAxis = () => ref Vertical, Name = "Vertical" });
            axes.Add(new() { DrivenAxis = () => ref Perpendicular, Name = "Perpendicular" });
        }

        private void OnValidate()
        {
            Horizontal.Validate();
            Vertical.Validate();
            Perpendicular.Validate();
        }

        private void Update()
        {
            
        }
    }
}
