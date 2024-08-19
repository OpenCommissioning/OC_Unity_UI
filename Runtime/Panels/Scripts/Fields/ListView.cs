using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class ListView : UnityEngine.UIElements.ListView
    {
        public new class UxmlFactory : UxmlFactory<ListView, UxmlTraits> { }
        public new class UxmlTraits : UnityEngine.UIElements.ListView.UxmlTraits { }

        public Scroller VerticalScroller;
        public Scroller HorizontalScroller;

        private const string SCROLL_VIEW_STYLE_SHEET = "StyleSheet/scrollview";
        private const string USS_SCROLL_VIEW = "scrollview";

        public ListView()
        {
            var scrollview = this.Q<UnityEngine.UIElements.ScrollView>();
            scrollview.styleSheets.Add(Resources.Load<StyleSheet>(SCROLL_VIEW_STYLE_SHEET));
            scrollview.AddToClassList(USS_SCROLL_VIEW);
            VerticalScroller = scrollview.verticalScroller;
            HorizontalScroller = scrollview.horizontalScroller;
            VerticalScroller.name = "scroller-vertical";
            HorizontalScroller.name = "scroller-horizontal";
        }
    }
}