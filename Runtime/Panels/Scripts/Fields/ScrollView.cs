using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement("OCScrollView")]
    public partial class ScrollView : UnityEngine.UIElements.ScrollView
    {
#else
    public class ScrollView : UnityEngine.UIElements.ScrollView
    {
        public new class UxmlFactory : UxmlFactory<ScrollView, UxmlTraits> { }
        public new class UxmlTraits : UnityEngine.UIElements.ScrollView.UxmlTraits { }
#endif

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