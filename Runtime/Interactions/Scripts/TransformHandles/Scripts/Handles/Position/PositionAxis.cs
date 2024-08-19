using UnityEngine;

namespace OC.UI.TransformHandles
{
    public class PositionAxis : HandleBase
    {
        [SerializeField]
        protected Vector3 _axis;
        protected Vector3 _startPosition;
        private Vector3 _interactionOffset;
        private Ray _axisRay;
        public override void StartInteraction(Vector3 hitPoint)
        {
            base.StartInteraction(hitPoint);
            _startPosition = _parentTransformHandle.transform.position;
            foreach (Transform target in _parentTransformHandle.Targets)
            {
                _targetStartPositions.Add(target.position);
            }
            Vector3 raxis = _parentTransformHandle.HandleRotation == HandleRotation.Local ? _parentTransformHandle.transform.rotation * _axis : _axis;
            _axisRay = new Ray(_startPosition, raxis);
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float closestPoint = HandleMathUtils.ClosestPointOnRay(_axisRay, cameraRay);
            Vector3 axisHitPoint = _axisRay.GetPoint(closestPoint);
            _interactionOffset = _startPosition - axisHitPoint;
        }
        public override void Interact(Vector3 previousPosition)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float clostestPoint = HandleMathUtils.ClosestPointOnRay(_axisRay, cameraRay);
            Vector3 hitPoint = _axisRay.GetPoint(clostestPoint);
            Vector3 offset = hitPoint + _interactionOffset - _startPosition;
            Vector3 position = _startPosition + offset;
            _parentTransformHandle.transform.position = position;

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