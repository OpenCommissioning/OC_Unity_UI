using UnityEngine;

namespace OC.UI.Interactions
{
    [DefaultExecutionOrder(1000)]
    [RequireComponent(typeof(Interaction))]
    public class Tooltip : MonoBehaviour, ITooltip
    {
        public string Name => _name;
        public string Description => _description;

        [SerializeField]
        protected string _name;
        [SerializeField]
        protected string _description;

        protected Interaction _interaction;
        
        private void Awake()
        {
            _interaction = GetComponent<Interaction>();
            _interaction.OnHoverChanged += SetActive;
            if (string.IsNullOrEmpty(_name)) _name = _interaction.Target.name;
        }

        private void OnDestroy()
        {
            _interaction.OnHoverChanged -= SetActive;
        }

        private void SetActive(bool isActive)
        {
            if (isActive)
            {
                TooltipManager.Instance.Show(this);
            }
            else
            {
                TooltipManager.Instance.Hide();
            }
        }
    }
}
