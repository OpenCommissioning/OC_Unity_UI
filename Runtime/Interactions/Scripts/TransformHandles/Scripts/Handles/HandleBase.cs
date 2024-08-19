using System.Collections.Generic;
using UnityEngine;

namespace OC.UI.TransformHandles
{
    public abstract class HandleBase : MonoBehaviour
    {

        [SerializeField]
        protected List<Vector3> _targetStartPositions = new List<Vector3>();
        protected List<Quaternion> _targetStartRotations = new List<Quaternion>();
        [SerializeField]
        protected RuntimeTransformHandle _parentTransformHandle;
        protected Material _material;
        protected Vector3 _hitPoint;
        [SerializeField]
        protected bool _isInteracting;

        [SerializeField]
        public Color _defaultColor;
        public void SetColor(Color color)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.material.color = color;
            }
        }
        public virtual void StartInteraction(Vector3 hitPoint)
        {
            _isInteracting = true;
        }
        public virtual void Interact(Vector3 previousPosition)
        {
        }

        public virtual void EndInteraction()
        {
            _isInteracting = false;
            SetColor(_defaultColor);
        }
    }
}