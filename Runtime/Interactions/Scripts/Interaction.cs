using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace IOSEF.UI.Interactions
{
    [AddComponentMenu("IOSEF/UI/Interaction")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider))]
    public class Interaction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject Target => _target == null ? gameObject : _target;
        public InteractionMode Mode => _mode;
        
        public bool IsDisabled
        {
            get => _isDisabled;
            set
            {
                if (_isDisabled == value) return;
                _isDisabled = value;
                ResetInteraction();
                _collider.enabled = !_isDisabled;
            }
        }
        
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnSelectionChanged?.Invoke(value);
                RefreshLayerMask();
            }
        }

        public bool IsHovered
        {
            get => _isHovered;
            set
            {
                if (_isHovered == value) return;
                _isHovered = value;
                OnHoverChanged?.Invoke(value);
                RefreshLayerMask();
            }
        }

        [Header("Settings")] 
        [SerializeField] 
        private InteractionMode _mode = InteractionMode.All;
        [SerializeField]
        protected GameObject _target;

        [Header("Status")] 
        [SerializeField] 
        protected bool _isDisabled;
        [SerializeField]
        [ReadOnly]
        protected bool _isSelected;
        [SerializeField]
        [ReadOnly]
        protected bool _isHovered;

        public event Action<bool> OnSelectionChanged;
        public event Action<bool> OnHoverChanged;

        public UnityEvent OnPointerClickEvent;
        public UnityEvent OnPointerDownEvent;
        public UnityEvent OnPointerUpEvent;

        private Collider _collider;
        private List<Renderer> _renderers;
        private const uint LayermaskHover = 2;
        private const uint LayermaskSelection = 4;

        protected void Awake()
        {
            _renderers = Target.GetComponentsInChildren<Renderer>().ToList();
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            gameObject.layer = 10;
        }

        protected void OnValidate()
        {
            if (_collider == null) _collider = GetComponent<Collider>();
            _collider.enabled = !_isDisabled;
        }

        protected void OnDestroy()
        {
            SelectionManager.Instance.Deselect(this);
        }

        protected void ResetInteraction()
        {
            _isSelected = false;
            _isHovered = false; 
            OnSelectionChanged?.Invoke(false);
            OnHoverChanged?.Invoke(false);
            RefreshLayerMask();
        }

        private void RefreshLayerMask()
        {
            if (!isActiveAndEnabled) return;

            if (_isSelected)
            {
                SetRenderLayerMask(LayermaskSelection);
                return;
            }

            if (_isHovered)
            {
                SetRenderLayerMask(LayermaskHover);
                return;
            }
            
            SetRenderLayerMask(1);
        }

        private void SetRenderLayerMask(uint layerMask)
        {
            foreach (var item in _renderers)
            {
                item.renderingLayerMask = layerMask;
            }
        }

        [Flags]
        public enum InteractionMode
        {
            Selection = 1,
            Hover = 2,
            Click = 4,
            PointerDown = 8,
            PoiterUp = 16,
            All = ~0
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isDisabled) return;
            if (_mode.HasFlag(InteractionMode.Hover)) IsHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isDisabled) return;
            if (_mode.HasFlag(InteractionMode.Hover)) IsHovered = false;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (_isDisabled) return;
            if (_mode.HasFlag(InteractionMode.Selection)) IsSelected = true;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (_isDisabled) return;
            if (_mode.HasFlag(InteractionMode.Selection)) IsSelected = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isDisabled) return;
            if (_mode.HasFlag(InteractionMode.Click)) OnPointerClickEvent?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isDisabled) return;
            if (_mode.HasFlag(InteractionMode.PointerDown)) OnPointerDownEvent?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDisabled) return;
            if (_mode.HasFlag(InteractionMode.PoiterUp)) OnPointerUpEvent?.Invoke();
        }
    }
}
