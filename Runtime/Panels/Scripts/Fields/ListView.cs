using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class ListView : UnityEngine.UIElements.ListView
    {
        public new class UxmlFactory : UxmlFactory<ListView, UxmlTraits> { }
        public new class UxmlTraits : UnityEngine.UIElements.ListView.UxmlTraits { }

        public Scroller VerticalScroller;
        public Scroller HorizontalScroller;

        private const string scrollViewStyleSheet = "StyleSheet/scrollview";
        private const string ussScrollView = "scrollview";

        public ListView() : base()
        {
            var scrollview = this.Q<UnityEngine.UIElements.ScrollView>();
            scrollview.styleSheets.Add(Resources.Load<StyleSheet>(scrollViewStyleSheet));
            scrollview.AddToClassList(ussScrollView);
            VerticalScroller = scrollview.verticalScroller;
            HorizontalScroller = scrollview.horizontalScroller;
            VerticalScroller.name = "scroller-vertical";
            HorizontalScroller.name = "scroller-horizontal";
        }
    }

}