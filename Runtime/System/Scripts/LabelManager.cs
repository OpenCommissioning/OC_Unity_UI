using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OC.UI.Interactions
{
    /// <summary>
    /// Global on/off for every <see cref="Label"/> in the scene. Wired to the AppUI Label Tool.
    /// Labels start hidden; toggling the tool calls <see cref="Show"/>.
    /// </summary>
    public class LabelManager : MonoBehaviour
    {
        public static LabelManager Singleton;

        private List<Label> _labels;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (Singleton == this) Singleton = null;
        }

        private void Start()
        {
            RefreshLabels();
            Show(false);
        }

        public void Show(bool value)
        {
            RefreshLabels();
            foreach (var label in _labels)
            {
                if (label != null) label.SetMasterVisible(value);
            }
        }

        private void RefreshLabels()
        {
            _labels = Utils.FindObjectsByType<Label>().ToList();
        }
    }
}
