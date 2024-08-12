using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OC.UI.Interactions.SceneGizmo
{
    public class SceneGizmoRenderer : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private RawImage _imageHolder;
        private RectTransform _imageHolderTR;

        [SerializeField]
        public SceneGizmoController controller;

        [SerializeField]
        private bool highlightHoveredComponents = true;
        private PointerEventData hoveringPointer;

        [SerializeField]
        private UnityEvent<GizmoComponent> _onComponentClicked;

        public static SceneGizmoRenderer Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;

            _imageHolderTR = (RectTransform)_imageHolder.transform;
            _imageHolder.texture = controller.TargetTexture;
        }

        private void OnEnable()
        {
            if (controller != null && !controller.Equals(null))
                controller.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (controller != null && !controller.Equals(null))
                controller.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (hoveringPointer != null)
                controller.OnPointerHover(GetNormalizedPointerPosition(hoveringPointer));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging)
                return;

            GizmoComponent hitComponent = controller.Raycast(GetNormalizedPointerPosition(eventData));
            if (hitComponent != GizmoComponent.None)
                _onComponentClicked.Invoke(hitComponent);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        private Vector3 GetNormalizedPointerPosition(PointerEventData eventData)
        {
            Vector2 localPos;
            Vector2 size = _imageHolderTR.rect.size;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_imageHolderTR, eventData.position, null, out localPos);

            return new Vector3(1f + localPos.x / size.x, 1f + localPos.y / size.y, 0f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (highlightHoveredComponents)
                hoveringPointer = eventData;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hoveringPointer != null)
            {
                controller.OnPointerHover(new Vector3(-10f, -10f, 0f));
                hoveringPointer = null;
            }
        }
    }
}