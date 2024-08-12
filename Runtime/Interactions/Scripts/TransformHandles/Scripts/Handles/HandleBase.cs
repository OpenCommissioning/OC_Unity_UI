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
        protected bool _isInteracting = false;

        [SerializeField]
        public Color _defaultColor;
        public void SetColor(Color p_color)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = p_color;
            }
        }
        public virtual void StartInteraction(Vector3 p_hitPoint)
        {
            _isInteracting = true;
        }
        public virtual void Interact(Vector3 p_previousPosition)
        {
        }

        public virtual void EndInteraction()
        {
            _isInteracting = false;
            SetColor(_defaultColor);
        }
    }
}