using System.Collections.Generic;
using UnityEngine;

namespace OC.UI.TransformHandles
{
    public abstract class HandleBase : MonoBehaviour
    {
        [SerializeField]
        protected bool _isInteracting;
        [SerializeField]
        protected List<Vector3> _targetStartPositions = new ();
        [SerializeField]
        protected List<Quaternion> _targetStartRotations = new ();
        [SerializeField]
        protected RuntimeTransformHandle _parentTransformHandle;
        [SerializeField]
        public Color _defaultColor;
        
        protected Material _material;
        protected Vector3 _hitPoint;
        
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