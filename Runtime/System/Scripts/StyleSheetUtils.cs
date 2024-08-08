using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI
{
    public static class StyleSheetUtils
    {
        private const string ThemeStyleSheet = "StyleSheet/default-theme";
        
        public static void AddDefaultTheme(this VisualElement visualElement)
        {
            visualElement.styleSheets.Add(Resources.Load<StyleSheet>(ThemeStyleSheet));
        }
    }
}