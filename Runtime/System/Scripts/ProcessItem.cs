using System.Globalization;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IOSEF.UI
{
    public class ProcessItem : MonoBehaviour
    {
        public float Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                _progressBar.value = _progress;
                _progressText.text = ((int)(_progress * 100)).ToString(CultureInfo.InvariantCulture) + "%";
            }
        }

        [Header("Settings")] 
        [SerializeField] 
        private float _deleteDelay = 5;

        [Header("References")]
        [SerializeField]
        private TMP_Text _infoText;
        [SerializeField]
        private TMP_Text _progressText;
        [SerializeField]
        private Slider _progressBar;
        [SerializeField] 
        private Button _cancelButton;
        [SerializeField] 
        private Image _doneImage;
        [SerializeField] 
        private Image _progressColor;

        [Header("Collision")] 
        [SerializeField] 
        private float _progress;
        
        private CancellationTokenSource _cancellationTokenSource;

        private void OnEnable()
        {
            _cancelButton.onClick.AddListener(CancelButton);
        }

        private void OnDisable()
        {
            _cancelButton.onClick.RemoveListener(CancelButton);
        }

        private void CancelButton()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Cancel()
        {
            if (!_cancellationTokenSource.IsCancellationRequested) return;
            _cancelButton.interactable = false;
            _progressColor.color = Color.red;
            Destroy(gameObject,_deleteDelay);
        }

        public void Initialize(string text, CancellationTokenSource cancellationTokenSource)
        {
            _infoText.text = text;
            _cancellationTokenSource = cancellationTokenSource;
            _doneImage.enabled = false;
        }

        public void Done()
        {
            _progress = 1;
            _progressText.text = (_progress * 100).ToString(CultureInfo.InvariantCulture) + "%";
            _progressBar.value = _progress;

            _cancelButton.enabled = false;
            _progressText.enabled = false;
            _doneImage.enabled = true;
            
            Destroy(gameObject,_deleteDelay);
        }
    }
}
