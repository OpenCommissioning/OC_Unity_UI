using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement("OCProgressBarField")]
    public partial class ProgressBarField : ProgressBar
    {
#else
    public class ProgressBarField : ProgressBar
    {
        public new class UxmlFactory : UxmlFactory<ProgressBarField, UxmlTraits> { }

        public new class UxmlTraits : UnityEngine.UIElements.ProgressBar.UxmlTraits { }
#endif

        private const string STYLE_SHEET = "StyleSheet/panel-field";
        private const string USS_PROGRESS_BAR_FIELD = "panel-field-progress-bar";

        public ProgressBarField() : base()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_PROGRESS_BAR_FIELD);
        }

        public ProgressBarField(string title, IProperty<float> property, float low = 0, float high = 1) : this(title, low, high)
        {
            value = property.Value;
            property.OnValueChanged += value => SetValueWithoutNotify(value);
            this.RegisterValueChangedCallback(evt => property.Value = evt.newValue);
        }

        public ProgressBarField(string title, IPropertyReadOnly<float> property, float low = 0, float high = 1) : this(title, low, high)
        {
            value = property.Value;
            property.OnValueChanged += Property_ValueChanged;
        }

        private void Property_ValueChanged(float value)
        {
            SetValueWithoutNotify(value);
        }

        public ProgressBarField(string title, IProperty<float> property, Vector2 limits) : this(title, limits)
        {
            value = property.Value;
            property.OnValueChanged += value => SetValueWithoutNotify(value);
            this.RegisterValueChangedCallback(evt => property.Value = evt.newValue);
        }

        public ProgressBarField(string title, IPropertyReadOnly<float> property, Vector2 limits) : this(title, limits)
        {
            value = property.Value;
            property.OnValueChanged += value => SetValueWithoutNotify(value);
        }

        private ProgressBarField(string title) : this()
        {
            this.title = title;
        }

        private ProgressBarField(string title, float low, float high) : this(title)
        {
            lowValue = low;
            highValue = high;
        }

        private ProgressBarField(string title, Vector2 limits) : this(title)
        {
            lowValue = limits.x;
            highValue = limits.y;
        }
    }
}