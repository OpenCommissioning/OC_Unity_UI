using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace OC.UI
{
    [DefaultExecutionOrder(-500)]
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        public int MouseSensitivity
        {
            get => _mouseSensitivity;
            set
            {
                _mouseSensitivity = Mathf.Clamp(value, 1, 10);
                OnSettingsChanged?.Invoke();
            } 
        }
        
        public VisualConfig VisualConfig => _visualConfig;

        [Header("Interactions")]
        [SerializeField]
        private VisualConfig _visualConfig;

        [Header("Settings")]
        [SerializeField]
        private int _mouseSensitivity = 5;

        [Header("Collision")] 
        private bool _isWindowed;

        public UnityEvent OnSettingsChanged;
        
        private InputAction _actionWindow;
        
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            } 
            else 
            {
                Instance = this;
            }
        }
        
        private void Start()
        {
            Load();
        }

        private void OnDisable()
        {
            Save();
        }
        
        private void OnValidate()
        {
            OnSettingsChanged?.Invoke();
        }
        
        private void Load()
        {
            _mouseSensitivity = PlayerPrefs.GetInt("MouseSensitivity");
            OnSettingsChanged?.Invoke();
        }

        private void Save()
        {
            PlayerPrefs.SetInt("MouseSensitivity",_mouseSensitivity);
            PlayerPrefs.Save();
        }
    }
    
    public enum ViewModeType
    {
        Default,
        Transparent,
        Hide
    }

    public enum ViewModeState
    {
        Default,
        Warning,
        Error
    }
}
