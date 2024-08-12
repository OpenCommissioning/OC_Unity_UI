using UnityEngine;

namespace OC.UI.TransformHandles
{
    public class RotationAxis : HandleBase
    {
        [SerializeField]
        private Vector3 _axis;
        private Vector3 _rotatedAxis;
        private Plane _axisPlane;
        private Vector3 _tangent;
        private Vector3 _biTangent;
        [SerializeField]
        private Quaternion _startRotation;

        public override void Interact(Vector3 p_previousPosition)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!_axisPlane.Raycast(cameraRay, out float hitT))
            {
                return;
            }
            Vector3 hitPoint = cameraRay.GetPoint(hitT);
            Vector3 hitDirection = (hitPoint - _parentTransformHandle.transform.position).normalized;
            float x = Vector3.Dot(hitDirection, _tangent);
            float y = Vector3.Dot(hitDirection, _biTangent);
            float angleRadians = Mathf.Atan2(y, x);
            float angleDegrees = angleRadians * Mathf.Rad2Deg;

            if (_parentTransformHandle.RotationSnap != 0)
            {
                angleDegrees = Mathf.Round(angleDegrees / _parentTransformHandle.RotationSnap) * _parentTransformHandle.RotationSnap;
                angleRadians = angleDegrees * Mathf.Deg2Rad;
            }
            
            if (_parentTransformHandle.HandleRotation == HandleRotation.Local)
            {
                for (int i = 0; i < _parentTransformHandle.Targets.Count; i++)
                {
                    _parentTransformHandle.Targets[i].transform.rotation = _targetStartRotations[i] * Quaternion.AngleAxis(angleDegrees, _axis);
                }
            }
            else
            {
                for (int i = 0; i < _parentTransformHandle.Targets.Count; i++)
                {
                    _parentTransformHandle.Targets[i].transform.RotateAround(_parentTransformHandle.transform.position, _axis, angleDegrees);
                }
                CalculateTangents(hitPoint);
            }
        }

        public override void StartInteraction(Vector3 hitPoint)
        {
            base.StartInteraction(hitPoint);
            SetStartRotations();
            CalculateTangents(hitPoint);
        }

        public override void EndInteraction()
        {
            base.EndInteraction();
            _targetStartRotations.Clear();
        }

        private void CalculateTangents(Vector3 hitPoint)
        {
            _axisPlane = new Plane(_rotatedAxis, _parentTransformHandle.transform.position);
            Vector3 startHitPoint;
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (_axisPlane.Raycast(cameraRay, out float lenghtOfPlaneEnterpoint))
            {
                startHitPoint = cameraRay.GetPoint(lenghtOfPlaneEnterpoint);
            }
            else
            {
                startHitPoint = _axisPlane.ClosestPointOnPlane(hitPoint);
            }
            _tangent = (startHitPoint - _parentTransformHandle.transform.position).normalized;
            _biTangent = Vector3.Cross(_rotatedAxis, _tangent);
        }

        public void SetStartRotations()
        {
            _startRotation = _parentTransformHandle.HandleRotation == HandleRotation.Local ? _parentTransformHandle.transform.localRotation : _parentTransformHandle.transform.rotation;

            foreach (Transform target in _parentTransformHandle.Targets)
            {
                _targetStartRotations.Add(target.rotation);
            }
            
            if (_parentTransformHandle.HandleRotation == HandleRotation.Local)
            {
                _rotatedAxis = _startRotation * _axis;
            }
            else
            {
                _rotatedAxis = _axis;
            }
        }
    }
}