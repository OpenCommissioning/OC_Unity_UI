using TMPro;
using UnityEngine;

namespace IOSEF.UI.Interactions
{
    public class Label : MonoBehaviour
    {
        [SerializeField]
        private string _label;
        [SerializeField]
        private bool _autoLabel;

        private void Start()
        {
            SetLabel();
        }

        private void OnValidate()
        {
            SetLabel();
        }

        private void SetLabel()
        {
            if (!_autoLabel) return;
            var textMeshPro = GetComponentInChildren<TextMeshPro>();
            if (textMeshPro == null) return;
            textMeshPro.text = string.IsNullOrWhiteSpace(_label) ? gameObject.name : _label;
        }
    }
}


