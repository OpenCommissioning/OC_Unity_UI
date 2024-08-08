using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class ProgressBarField : UnityEngine.UIElements.ProgressBar
    {
        public new class UxmlFactory : UxmlFactory<ProgressBarField, UxmlTraits> { }

        public new class UxmlTraits : UnityEngine.UIElements.ProgressBar.UxmlTraits { }


        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussProgressBarField = "panel-field-progress-bar";

        public ProgressBarField() : base()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussProgressBarField);
        }

        public ProgressBarField(string title, IProperty<float> property, float low = 0, float high = 1) : this(title, low, high)
        {
            value = property.Value;
            property.ValueChanged += value => SetValueWithoutNotify(value);
            this.RegisterValueChangedCallback(evt => property.Value = evt.newValue);
        }

        public ProgressBarField(string title, IPropertyReadOnly<float> property, float low = 0, float high = 1) : this(title, low, high)
        {
            value = property.Value;
            property.ValueChanged += Property_ValueChanged;
        }

        private void Property_ValueChanged(float value)
        {
            SetValueWithoutNotify(value);
        }

        public ProgressBarField(string title, IProperty<float> property, Vector2 limits) : this(title, limits)
        {
            value = property.Value;
            property.ValueChanged += value => SetValueWithoutNotify(value);
            this.RegisterValueChangedCallback(evt => property.Value = evt.newValue);
        }

        public ProgressBarField(string title, IPropertyReadOnly<float> property, Vector2 limits) : this(title, limits)
        {
            value = property.Value;
            property.ValueChanged += value => SetValueWithoutNotify(value);
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