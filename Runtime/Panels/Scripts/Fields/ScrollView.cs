using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class ScrollView : UnityEngine.UIElements.ScrollView
    {
        public new class UxmlFactory : UxmlFactory<ScrollView, UxmlTraits> { }
        public new class UxmlTraits : UnityEngine.UIElements.ScrollView.UxmlTraits { }

        private const string SCROLL_VIEW_STYLE_SHEET = "StyleSheet/scrollview";
        private const string USS_SCROLL_VIEW = "scrollview";

        public ScrollView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(SCROLL_VIEW_STYLE_SHEET));
            AddToClassList(USS_SCROLL_VIEW);
            verticalScroller.name = "scroller-vertical";
            horizontalScroller.name = "scroller-horizontal";
        }
    }
}