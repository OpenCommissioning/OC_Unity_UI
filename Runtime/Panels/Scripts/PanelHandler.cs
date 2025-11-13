using Cinemachine;
using OC.UI.Interactions;
using UnityEngine;

namespace OC.UI.Panel
{
    [AddComponentMenu("Open Commissioning/UI/Panel Handler")]
    [DisallowMultipleComponent]
    public class PanelHandler : Interaction
    {
        public Component Component => _component;

        private Component _component;
        protected Panel _panel;

        private void Start()
        {
            if (!Target.TryGetComponent<IInteractable>(out var interactable))
            {
                Debug.LogWarning("Can't find IInteractable on Component", this);
                return;
            }
            _component = (Component)interactable;
            OnSelectionChanged += ShowPanel;
        }

        private new void OnDestroy()
        {
            base.OnDestroy();
            Delete();
            OnSelectionChanged -= ShowPanel;
        }

        private void ShowPanel(bool show)
        {
            if (show)
            {
                Create();
            }
            else
            {
                Close();
            }
        }

        protected virtual void Create()
        {
            _panel ??= PanelFactory.Create(this);
        }

        public void Close()
        {
            _panel?.Close();
        }

        public void Delete()
        {
            _panel?.Delete();
            _panel = null;
        }

        public void Focus()
        {
            if (CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.VirtualCameraGameObject.TryGetComponent(out CameraController cameraController))
            {
                cameraController.FocusOn(Target.gameObject, true);
            }
        }
    }
}
