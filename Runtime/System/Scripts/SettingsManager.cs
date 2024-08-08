using UnityEngine;
using UnityEngine.Events;

namespace IOSEF.UI
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
        [SerializeField]
        private KeyCode _windowMode =  KeyCode.F12;

        [Header("Collision")] 
        private bool _isWindowed;

        public UnityEvent OnSettingsChanged;
        
        
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

        private void Update()
        {
            ScreenModeAction();
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
        
        private void ScreenModeAction()
        {
            if (!Input.GetKeyDown(_windowMode)) return;
            _isWindowed = !_isWindowed;
            SetScreenMode(_isWindowed);
        }
        
        private void SetScreenMode(bool window)
        {
            if (window)
            {
                Screen.SetResolution(Screen.currentResolution.width,Screen.currentResolution.height,FullScreenMode.Windowed);
            }
            else
            {
                Screen.SetResolution(Screen.currentResolution.width,Screen.currentResolution.height,FullScreenMode.FullScreenWindow);
            }
        }
    }
    
    public enum ViewModeType
    {
        Default,
        Transperent,
        Hide
    }

    public enum ViewModeState
    {
        Default,
        Warning,
        Error
    }
}
