using System;
using OC.UI.Inspector;
using OC.UI.TransformHandles;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI
{
    public class TransformComponent : VisualElement
    {
        public bool Visible
        {
            set => style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        private const string UXML = "UXML/TransformComponent";
        private const string USS = "StyleSheet/inspector-component";
        private RuntimeInspector _runtimeInspector;
        private Transform _transform;
        private readonly Vector3Field _positionField;
        private readonly Vector3Field _rotationField;
        private readonly Vector3Field _scaleField;

        private readonly IVisualElementScheduledItem _updateTask;

        public TransformComponent()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            this.AddDefaultTheme();
            Resources.Load<VisualTreeAsset>(UXML).CloneTree(this);
            
            _positionField = this.Q<Vector3Field>("position");
            _rotationField = this.Q<Vector3Field>("rotation");
            _scaleField = this.Q<Vector3Field>("scale");
            
            _updateTask = schedule.Execute(Refresh).Every(100);
            
            _positionField.SetEnabled(false);
            _rotationField.SetEnabled(false);
            _scaleField.SetEnabled(false);
            Visible = false;
        }
        
        public void Bind(Transform reference)
        {
            Visible = false;
            if (reference == null) return;
            if (!reference.TryGetComponent(out _runtimeInspector)) return;
            
            
            _transform = _runtimeInspector.transform;
            
            _updateTask.Resume();

            if (_runtimeInspector.TransformType.HasFlag(TransformType.Position))
            {
                _positionField.SetEnabled(true);
            }
            
            if (_runtimeInspector.TransformType.HasFlag(TransformType.Rotation))
            {
                _rotationField.SetEnabled(true);
            }
            
            if (_runtimeInspector.TransformType.HasFlag(TransformType.Scale))
            {
                _scaleField.SetEnabled(true);
            }
            
            _positionField.RegisterValueChangedCallback(OnPositionChanged);
            _rotationField.RegisterValueChangedCallback(OnPositionChanged);
            _scaleField.RegisterValueChangedCallback(OnPositionChanged);
            Visible = true;
        }

        public void Unbind()
        {
            _updateTask.Pause();
            _runtimeInspector = null;
            _transform = null;
            
            _positionField.SetEnabled(false);
            _rotationField.SetEnabled(false);
            _scaleField.SetEnabled(false);
            
            _positionField.UnregisterValueChangedCallback(OnPositionChanged);
            _rotationField.UnregisterValueChangedCallback(OnPositionChanged);
            _scaleField.UnregisterValueChangedCallback(OnPositionChanged);
        }

        private void Refresh()
        {
            if (_transform == null) return;

            switch (RuntimeTransformHandle.Instance.Coordinate.Value)
            {
                case CoordinateSpace.Local:
                    _positionField.SetValueWithoutNotify(_transform.localPosition);
                    _rotationField.SetValueWithoutNotify(_transform.localEulerAngles);
                    _scaleField.SetValueWithoutNotify(_transform.localScale);
                    break;
                case CoordinateSpace.World:
                    _positionField.SetValueWithoutNotify(_transform.position);
                    _rotationField.SetValueWithoutNotify(_transform.eulerAngles);
                    _scaleField.SetValueWithoutNotify(_transform.localScale);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnPositionChanged(ChangeEvent<Vector3> changeEvent)
        {
            _transform.position = changeEvent.newValue;
        }
        
        private void OnRotationChanged(ChangeEvent<Vector3> changeEvent)
        {
            throw new System.NotImplementedException();
        }
        
        private void OnScaleChanged(ChangeEvent<Vector3> changeEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}
