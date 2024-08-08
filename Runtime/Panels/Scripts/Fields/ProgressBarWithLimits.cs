using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class ProgressBarWithLimits : ProgressBar
    {
        public new class UxmlFactory : UxmlFactory<ProgressBarWithLimits, UxmlTraits> { }

        public new class UxmlTraits : UnityEngine.UIElements.ProgressBar.UxmlTraits
        {
            private readonly UxmlColorAttributeDescription _colorProgressBar = new() { name = "Color-Bar", defaultValue = Color.white };
            private readonly UxmlColorAttributeDescription _colorBackground = new() { name = "Color-Background", defaultValue = new Color(0.5f,0.5f,0.5f, 0.5f) };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not ProgressBarWithLimits progressBar) return;
                progressBar.ColorBar = _colorProgressBar.GetValueFromBag(bag, cc);
                progressBar.ColorBackground = _colorBackground.GetValueFromBag(bag, cc);
            }
        }

        public Color ColorBar
        {
            get => _colorBar;
            set
            {
                _progressBar.style.backgroundColor = new StyleColor(value);
                _colorBar = value;
            }
        }

        public Color ColorBackground
        {
            get => _colorBackground;
            set
            {
                _background.style.backgroundColor = new StyleColor(value);
                _colorBackground = value;
            }
        }

        public bool Min
        {
            get => _min;
            set
            {
                if (_min == value) return;
                _min = value;
                if (_min)
                {
                    _indicatorMin.AddToClassList(UssBoxIndicatorActive);
                }
                else
                {
                    _indicatorMin.RemoveFromClassList(UssBoxIndicatorActive);
                }
            }
        }
        
        public bool Max
        {
            get => _max;
            set
            {
                if (_max == value) return;
                _max = value;
                if (_max)
                {
                    _indicatorMax.AddToClassList(UssBoxIndicatorActive);
                }
                else
                {
                    _indicatorMax.RemoveFromClassList(UssBoxIndicatorActive);
                }
            }
        }

        private Color _colorBar;
        private Color _colorBackground;
        
        private readonly VisualElement _progressBar;
        private readonly VisualElement _background;
        private readonly VisualElement _indicatorMin;
        private readonly VisualElement _indicatorMax;

        private bool _min;
        private bool _max;
        
        private const string Uss = "StyleSheet/panel-field";
        private const string UssContainer = "panel-field-container";
        private const string UssProgressBarField = "panel-field-progress-bar";
        private const string UssBoxIndicator = "panel-field-progress-bar_indicator";
        private const string UssBoxIndicatorActive = "panel-field-progress-bar_indicator-active";


        public ProgressBarWithLimits()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(Uss));
            AddToClassList(UssContainer);
            AddToClassList(UssProgressBarField);
            
            _progressBar = this.Q<VisualElement>(className: progressUssClassName);
            _background = this.Q<VisualElement>(className: backgroundUssClassName);

            _indicatorMin = new VisualElement();
            _indicatorMin.AddToClassList(UssBoxIndicator);
            _indicatorMax = new VisualElement();
            _indicatorMax.AddToClassList(UssBoxIndicator);
            
            var container = this.Q<VisualElement>(className: containerUssClassName);
            container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

            container.Add(_indicatorMin);
            _indicatorMin.SendToBack();
            container.Add(_indicatorMax);
            _indicatorMax.BringToFront();
        }

        public ProgressBarWithLimits(string title, IPropertyReadOnly<float> property, float low = 0, float high = 1) : this()
        {
            this.title = title;
            lowValue = low;
            highValue = high;

            property.ValueChanged += OnPropertyValueChanged;
            OnPropertyValueChanged(property.Value);
        }

        private void OnPropertyValueChanged(float propertyValue)
        {
            value = propertyValue;
            SetValueWithoutNotify(propertyValue);
            UpdateIndicators();
        }

        private void UpdateIndicators()
        {
            Min = value <= lowValue;
            Max = value >= highValue;
        }
    }
}