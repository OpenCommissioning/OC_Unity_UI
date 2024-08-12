using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class ScrollView : UnityEngine.UIElements.ScrollView
    {
        public new class UxmlFactory : UxmlFactory<ScrollView, UxmlTraits> { }
        public new class UxmlTraits : UnityEngine.UIElements.ScrollView.UxmlTraits { }

        private const string _scrollViewStyleSheet = "StyleSheet/scrollview";
        private const string _ussScrollView = "scrollview";

        public ScrollView() : base()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_scrollViewStyleSheet));
            AddToClassList(_ussScrollView);
            verticalScroller.name = "scroller-vertical";
            horizontalScroller.name = "scroller-horizontal";
        }
    }

}