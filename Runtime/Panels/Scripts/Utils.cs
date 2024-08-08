using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public static class Utils
    {
        public static Vector2 ClampInParent(VisualElement element, Vector2 position)
        {
            position.x = Mathf.Clamp(position.x, element.parent.worldBound.xMin, element.parent.worldBound.xMax - element.worldBound.width);
            position.y = Mathf.Clamp(position.y, element.parent.worldBound.yMin, element.parent.worldBound.yMax - element.worldBound.height);
            return position - element.layout.position;
        }
    }
}