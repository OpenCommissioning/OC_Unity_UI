using System.Linq;
using OC.Interactions;
using OC.Interactions.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Industrial
{
    [RequireComponent(typeof(PanelSampler))]
    public class PanelSamplerUI : MonoBehaviour, IIndustrialPanel
    {
        [SerializeField]
        private string _name;

        public Component Component => this;
        public string Path => _panelSampler.Link.Path;

        private PanelSampler _panelSampler;

        private void Awake()
        {
            _panelSampler = GetComponent<PanelSampler>();
        }

        public VisualElement Create()
        {
            var groupName = string.IsNullOrEmpty(_name) ? gameObject.name : _name;
            var group = new ComponentsGroup(groupName);

            foreach (var visualElement in _panelSampler.Components.Select(Factory.Create))
            {
                group.Add(visualElement);
            }

            return group;
        }
    }
}

