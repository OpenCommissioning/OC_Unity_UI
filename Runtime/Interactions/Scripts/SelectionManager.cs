using System;
using System.Collections.Generic;
using System.Linq;
using OC.Interactions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace OC.UI.Interactions
{
    [DefaultExecutionOrder(-1000)]
    public class SelectionManager : MonoBehaviourSingleton<SelectionManager>
    {
        public IEnumerable<Interaction> SelectedInteractions => _selectedInteractions;
        public List<GameObject> HitGameObjects => _hitGameObjects;
        
        public event Action<List<Interaction>> OnSelectionChanged;

        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable == value) return;
                _enable = value;
                if (!_enable)
                {
                    ResetHit();
                    ClearSelection();
                }
            }
        }

        [Header("State")] 
        [SerializeField] 
        private bool _enable;
        [SerializeField] 
        private List<GameObject> _hitGameObjects = new ();
        [SerializeField]
        private List<Interaction> _selectedInteractions = new();
        
        [Header("Settings")] 
        [SerializeField]
        private LayerMask _layerMask;

        [Header("Input Actions")]
        [SerializeField]
        private InputActionReference _click;
        [SerializeField]
        private InputActionReference _pointer;
        
        [Header("Debug")] 
        [SerializeField]
        private bool _debug;

        private const float MAX_DISTANCE = 500;
        
        private readonly RaycastHit[] _raycastHits = new RaycastHit[10];
        private int _hitsCount;
        
        private Camera _camera;
        private GameObject _closestHitGameObject;
        private bool _hitHandle;
        
        private InputAction _clickAction;
        private InputAction _pointerAction;

        private void OnEnable()
        {
            ResetHit();
            ClearSelection();
            _camera = Camera.main;

            _clickAction = _click.action;
            _pointerAction = _pointer.action;
            
            _clickAction?.Enable();
            _pointerAction?.Enable();

            if (_clickAction != null)
            {
                _clickAction.started += HandleClickAction;
                _clickAction.performed += HandleClickAction;
                _clickAction.canceled += HandleClickAction;
            }
        }

        private void OnDisable()
        {
            _clickAction.started -= HandleClickAction;
            _clickAction.performed -= HandleClickAction;
            _clickAction.canceled -= HandleClickAction;
        }

        private void Update()
        {
            if (!_enable) return;
            if (AppUI.Instance.IsPointerOverUI) return;
            HandleRaycastHits();
        }

        private void HandleClickAction(InputAction.CallbackContext context)
        {
            if (!_enable) return;
            if (_hitHandle) return;
            if (AppUI.Instance.IsPointerOverUI) return;
            
            if (_debug) Debug.Log($"Handle Click action: {context.phase}");
            
            if (context.performed)
            {
                if (_hitGameObjects.Count > 0)
                {
                    var modifier = Keyboard.current.leftCtrlKey.wasPressedThisFrame;
                    
                    if (_selectedInteractions.Count > 0)
                    {
                        var index = (_hitGameObjects.IndexOf(_selectedInteractions.First().gameObject) + 1) % _hitGameObjects.Count;
                        var nextHitGameObject = _hitGameObjects[index];
                        Select(nextHitGameObject, modifier);
                        PointerDownEvent(nextHitGameObject);
                    }
                    else
                    {
                        Select(_closestHitGameObject, modifier);
                        PointerDownEvent(_closestHitGameObject);
                    }
                }
                else
                {
                    ResetHit();
                    ClearSelection();
                }
                
                return;
            }

            if (context.canceled)
            {
                if (_hitGameObjects.Count > 0)
                {
                    PointerClickEvent(_closestHitGameObject);
                    PointerUpEvent(_closestHitGameObject);
                }
            }
        }
        
        public void HandleRaycastHits()
        {
            Array.Clear(_raycastHits, 0, _hitsCount);
            _hitGameObjects.Clear();
            _hitHandle = false;
            
            var mousePosition = _pointerAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(mousePosition);
            _hitsCount = Physics.RaycastNonAlloc(ray.origin, ray.direction, _raycastHits, MAX_DISTANCE, _layerMask);

            if (_hitsCount == 0)
            {
                ResetHit();
                return;
            }

            var hits = _raycastHits.OrderBy(hit => hit.distance);
            foreach (var raycast in hits)
            {
                if (raycast.distance < OC.Utils.TOLERANCE) continue;
                if (raycast.collider.gameObject.CompareTag($"Handles"))
                {
                    _hitHandle = true;
                    continue;
                }
                _hitGameObjects.Add(raycast.collider.gameObject);
            }

            if (_hitGameObjects.Count < 1)
            {
                ResetHit();
                return; 
            }
            
            if (_hitGameObjects.First() == _closestHitGameObject) return;
            if (_closestHitGameObject != null) PointerExitEvent(_closestHitGameObject);

            _closestHitGameObject = _hitGameObjects.First();
            PointerEnterEvent(_closestHitGameObject);
        }

        public void ResetHit()
        {
            PointerUpEvent(_closestHitGameObject);
            PointerExitEvent(_closestHitGameObject);
            _hitGameObjects.Clear();
            _closestHitGameObject = null;
        }

        public void Deselect(Interaction interaction) => RemoveSelection(interaction);

        private void Select(GameObject go, bool multiple)
        {
            if (!go.TryGetComponent<Interaction>(out var interaction)) return;
            if (interaction.State.Value.HasFlag(InteractionState.Disabled)) return;
            if (!interaction.Mode.HasFlag(Interaction.InteractionMode.Selection)) return;

            if (multiple)
            {
                TrySelection(interaction);
            }
            else
            {
                ClearSelection();
                TrySelection(interaction);
            }
        }
        
        private void TrySelection(Interaction interaction)
        {
            if (_selectedInteractions.Contains(interaction))
            {
                RemoveSelection(interaction);
            }
            else
            {
                AddSelection(interaction);
            }
        }

        private void AddSelection(Interaction interaction)
        {
            if (_selectedInteractions.Contains(interaction)) return;
            _selectedInteractions.Add(interaction);
            OnSelect(interaction.gameObject);
            OnSelectionChanged?.Invoke(_selectedInteractions);
        }
        
        private void RemoveSelection(Interaction interaction)
        {
            if (!_selectedInteractions.Contains(interaction)) return;
            _selectedInteractions.Remove(interaction);
            OnDeselect(interaction.gameObject);
            OnSelectionChanged?.Invoke(_selectedInteractions);
        }
        
        private void ClearSelection()
        {
            foreach (var selection in _selectedInteractions)
            {
                OnDeselect(selection.gameObject);
            }
            _selectedInteractions.Clear();
            OnSelectionChanged?.Invoke(_selectedInteractions);
        }
        
        private void PointerEnterEvent(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
        }
        
        private void PointerExitEvent(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
        }

        private void OnSelect(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.selectHandler);
        }
        
        private void OnDeselect(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.deselectHandler);
        }

        private void PointerClickEvent(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
        
        private void PointerDownEvent(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
        }
        
        private void PointerUpEvent(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
        }
    }
}
