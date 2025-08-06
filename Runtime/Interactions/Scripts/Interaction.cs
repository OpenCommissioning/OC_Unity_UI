using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OC.UI.Interactions
{
    [AddComponentMenu("Open Commissioning/UI/Interaction")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider))]
    public class Interaction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject Target
        {
            get
            {
                if (_target == null)
                {
                    _target = gameObject;
                }

                return _target;
            }
            set => _target = value;
        }

        public InteractionMode Mode
        {
            get => _mode;
            set => _mode = value;
        }
        
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
        private InteractionMode _mode = InteractionMode.Hover | InteractionMode.Click;
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
        private const uint LAYERMASK_HOVER = 2;
        private const uint LAYERMASK_SELECTION = 4;

        protected void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>().ToList();
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _collider.enabled = !_isDisabled;
            gameObject.layer = 10;
        }

        // ReSharper disable once Unity.RedundantEventFunction
        protected void OnValidate()
        {
            
        }
        
        public void Reset()
        {
            BoundBoxColliderSize();
            gameObject.layer = (int)DefaultLayers.Interactions;
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
                SetRenderLayerMask(LAYERMASK_SELECTION);
                return;
            }

            if (_isHovered)
            {
                SetRenderLayerMask(LAYERMASK_HOVER);
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
        
        [ContextMenu("Bound Box Collider Size", false, 100)]
        public void BoundBoxColliderSize()
        {
            if (OC.Utils.TryBoundBoxColliderSize(gameObject, out var boxCollider))
            {
                boxCollider.isTrigger = true;
            }
        }
    }
}
