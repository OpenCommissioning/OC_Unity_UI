using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class DragAndDrop : Manipulator
    {
        private bool _enabled;
        private bool _inMotion;
        
        private Vector3 _targetStartPosition;
        private Vector3 _pointerStartPosition;
        private const float THRESHOLD = 10;

        private readonly VisualElement _handle;
        private readonly Panel _panel;
        private readonly InteractionsPanelManager _interactionsPanelManager;

        public DragAndDrop(VisualElement handler, Panel panel)
        {
            _handle = handler;
            _panel = panel;
            _interactionsPanelManager = InteractionsPanelManager.Instance;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            _handle.RegisterCallback<PointerDownEvent>(DownEvent);
            _handle.RegisterCallback<PointerMoveEvent>(MoveEvent);
            _handle.RegisterCallback<PointerUpEvent>(UpEvent);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            _handle.UnregisterCallback<PointerDownEvent>(DownEvent);
            _handle.UnregisterCallback<PointerMoveEvent>(MoveEvent);
            _handle.UnregisterCallback<PointerUpEvent>(UpEvent);
        }

        private void DownEvent(PointerDownEvent evt)
        {
            _pointerStartPosition = evt.position;
            _handle.CapturePointer(evt.pointerId);
            _enabled = true;
        }
        
        private void MoveEvent(PointerMoveEvent evt)
        {
            if (!_enabled) return;
            if (!_handle.HasPointerCapture(evt.pointerId)) return;

            var delta = evt.position - _pointerStartPosition;

            if (_inMotion)
            {
                Move(delta);
            }
            else
            {
                if (!(delta.sqrMagnitude > THRESHOLD * THRESHOLD)) return;
                _inMotion = true;
                _targetStartPosition = target.worldBound.position;
                _interactionsPanelManager.AddToScreen(target);
                _panel.CanPinned = false;
                Move(delta);
            }
        }

        private void Move(Vector3 delta)
        {
            var position = _targetStartPosition + delta;
            
#if UNITY_6000_3_OR_NEWER
            target.style.translate = Utils.ClampInParent(target, position);
#else
            target.transform.position = Utils.ClampInParent(target, position);
#endif
        }

        private void UpEvent(PointerUpEvent evt)
        {
            if (!_enabled || !_handle.HasPointerCapture(evt.pointerId)) return;
            _handle.ReleasePointer(evt.pointerId);
            if (_inMotion)
            {
                var localPointerPosition = _interactionsPanelManager.Sidebar.WorldToLocal(evt.position);
                if (_interactionsPanelManager.Sidebar.ContainsPoint(localPointerPosition) 
                    || Math.FastApproximately(
                        target.worldBound.x + target.worldBound.width,
                        target.parent.worldBound.width,
                        InteractionsPanelManager.Instance.DockThreshold)
                    )
                {
                    _interactionsPanelManager.AddToSidebar(target);
                    _panel.CanPinned = true;
                    
#if UNITY_6000_3_OR_NEWER
                    target.style.translate = new Vector3();
#else
                    target.transform.position = new Vector3();
#endif
                }
                _inMotion = false;
            }

            _enabled = false;
        }
    }
}