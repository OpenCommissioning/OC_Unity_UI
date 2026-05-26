using System.Collections.Generic;
using UnityEngine;

namespace OC.UI.TransformHandles
{
    public abstract class HandleBase : MonoBehaviour
    {
        public Color DefaultColor => _defaultColor;
        
        public Camera Camera => _camera;
        
        [SerializeField]
        protected bool _isInteracting;
        [SerializeField]
        protected List<Vector3> _targetStartPositions = new ();
        [SerializeField]
        protected List<Quaternion> _targetStartRotations = new ();
        [SerializeField]
        protected RuntimeTransformHandle _parentTransformHandle;
        [SerializeField]
        private Color _defaultColor;
        
        protected Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }
        
        public void SetColor(Color color)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.material.color = color;
            }
        }
        
        public virtual void StartInteraction(Vector3 mousePosition, Vector3 hitPoint)
        {
            _isInteracting = true;
        }
        
        public virtual void Interact(Vector3 mousePosition){}

        public virtual void EndInteraction(Vector3 mousePosition)
        {
            _isInteracting = false;
            SetColor(_defaultColor);
        }
    }
}