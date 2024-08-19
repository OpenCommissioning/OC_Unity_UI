using UnityEngine;

namespace OC.UI.TransformHandles
{
    public class PositionPlane : HandleBase
    {
        protected Vector3 _startPosition;
        [SerializeField]
        protected Vector3 _axis1;
        [SerializeField]
        protected Vector3 _axis2;
        [SerializeField]
        protected Vector3 _perpendicularAxis;
        protected Plane _plane;
        protected Vector3 _interactionOffset;
        private void Update()
        {
            Vector3 axis1 = _axis1;
            Vector3 raxis1 = _parentTransformHandle.HandleRotation == HandleRotation.Local ? _parentTransformHandle.transform.rotation * axis1 : axis1;
            float angle1 = Vector3.Angle(_parentTransformHandle.HandleCamera.transform.forward, raxis1);
            if (angle1 < 90) axis1 = -axis1;

            Vector3 axis2 = _axis2;
            Vector3 raxis2 = _parentTransformHandle.HandleRotation == HandleRotation.Local ? _parentTransformHandle.transform.rotation * axis2 : axis2;
            float angle2 = Vector3.Angle(_parentTransformHandle.HandleCamera.transform.forward, raxis2);
            if (angle2 < 90) axis2 = -axis2;

            transform.localPosition = (axis1 + axis2) * 0.25f;
        }
        public override void StartInteraction(Vector3 point)
        {
            Vector3 rperp = _parentTransformHandle.HandleRotation == HandleRotation.Local ? _parentTransformHandle.transform.rotation * _perpendicularAxis : _perpendicularAxis;

            _plane = new Plane(rperp, _parentTransformHandle.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _plane.Raycast(ray, out float p);

            Vector3 hitPoint = ray.GetPoint(p);
            _startPosition = _parentTransformHandle.transform.position;
            _interactionOffset = _startPosition - hitPoint;
            foreach (Transform target in _parentTransformHandle.Targets)
            {
                _targetStartPositions.Add(target.position);
            }
        }
        public override void Interact(Vector3 previousPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float d = 0.0f;
            _plane.Raycast(ray, out d);
            
            Vector3 hitPoint = ray.GetPoint(d);
            Vector3 offset = hitPoint + _interactionOffset - _startPosition;
            for (int i = 0; i < _parentTransformHandle.Targets.Count; i++)
            {
                _parentTransformHandle.Targets[i].transform.position = _targetStartPositions[i] + offset;
            }
        }
        public override void EndInteraction()
        {
            _targetStartPositions.Clear();
            base.EndInteraction();
        }
    }
}