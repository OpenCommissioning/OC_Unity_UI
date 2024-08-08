using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace IOSEF.UI.Interactions
{
    public class Clickable : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent onClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}
