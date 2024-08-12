using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OC.UI.Interactions
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
