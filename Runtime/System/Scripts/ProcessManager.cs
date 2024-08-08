using System.Threading;
using UnityEngine;

namespace IOSEF.UI
{
    public class ProcessManager : MonoBehaviour
    {
        public static ProcessManager Singleton;
        
        [SerializeField]
        private RectTransform _contentRoot;

        [SerializeField] 
        private ProcessItem _processItemPrefab;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(gameObject);
        }

        public ProcessItem CreateProcessItem(string text, CancellationTokenSource cancellationTokenSource)
        {
            var item = Instantiate(_processItemPrefab, _contentRoot);
            item.Initialize(text, cancellationTokenSource);
            return item;
        }
    }
}
