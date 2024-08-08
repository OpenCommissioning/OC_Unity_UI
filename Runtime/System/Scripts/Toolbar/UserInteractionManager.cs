using System;
using UnityEngine;

namespace IOSEF.UI.Toolbar
{
    [DefaultExecutionOrder(1000)]
    public class UserInteractionManager : MonoBehaviour
    {
        public static UserInteractionManager Instance { get; private set; }

        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable == value) return;
                _enable = value;
                OnInteractionEnableChanged?.Invoke(_enable);
            }
        }

        [SerializeField]
        private bool _enable;
        
        public event Action<bool> OnInteractionEnableChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }
    }
}