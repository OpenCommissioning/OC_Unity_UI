using IOSEF.MaterialFlow;
using IOSEF.UI.Interactions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IOSEF.UI
{
    [DefaultExecutionOrder(-100)]
    public class InteractionSystem : MonoBehaviour
    {
        public static InteractionSystem Instance { get; private set; }
        public uint HoverLayerMask => _hoverLayerMask;
        public uint SelectionLayerMask => _selectionLayerMask;
        public bool IsToolValid => _isToolValid;

        [Header("Settings")] 
        [SerializeField]
        private KeyCode _deleteKey = KeyCode.Delete;
        [SerializeField]
        private PointerEventData.InputButton _pointerEventButton;
        [SerializeField]
        private uint _hoverLayerMask = 2;
        [SerializeField]
        private uint _selectionLayerMask = 4;

        private bool _isToolValid;
        
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
            _isToolValid = true;
        }

        private void OnDestroy()
        {
        }

        private void LateUpdate()
        {
            DeleteAction();
        }

        public bool IsPointerValid(PointerEventData eventData)
        {
            return eventData.button == _pointerEventButton;
        }

        private void DeleteAction()
        {
            if (!Input.GetKeyDown(_deleteKey)) return;
            if (SelectionManager.Instance.SelectedInteractions == null) return;
            foreach (var item in SelectionManager.Instance.SelectedInteractions)
            {
                var entity = item.gameObject.GetComponentInParent<Payload>();
                if (entity !=null)
                {
                    Pool.Instance.PoolManager.Destroy(entity);
                }
            }
        }

        private void OnToolChanged()
        {
        }

  
        public void EnalbeInputs(bool value)
        {
            
        }
    }
}
