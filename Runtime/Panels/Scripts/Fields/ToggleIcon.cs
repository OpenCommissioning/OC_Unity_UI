using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class ToggleIcon : BaseBoolField
    {
        public new class UxmlFactory : UxmlFactory<ToggleIcon, UxmlTraits> { }
        public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription> { }

        private const string StyleSheet = "StyleSheet/panel-field";
        private const string USSContainer = "panel-field-container";
        private const string USSToggle = "panel-field-toggle";
        private const string USSLabel = "panel-field-label";
        private const string USSIcon = "panel-field-icon";

        private readonly VisualElement _icon;
        private readonly StyleBackground _defaultIcon;
        private readonly StyleBackground _activeIcon;
        private readonly StyleColor _defaultColor;
        private readonly StyleColor _activeColor;

        public override bool value
        {
            get => base.value;
            set 
            {
                base.value = value;
                SetToggle(base.value);
            }
        }

        public ToggleIcon() : this("",null, null, Color.white, Color.yellow) { }

        public ToggleIcon(string label, Sprite defaultIcon, Sprite activeIcton, Color defaultColor, Color activeColor) : base(label)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(StyleSheet));
            AddToClassList(USSContainer);
            AddToClassList(USSToggle);
            labelElement.AddToClassList(USSLabel);

            focusable = false;

            _defaultIcon = new StyleBackground(defaultIcon);
            _activeIcon = new StyleBackground(activeIcton);
            _defaultColor = new StyleColor(defaultColor);
            _activeColor = new StyleColor(activeColor);

            _icon = this.Q<VisualElement>("unity-checkmark");
            _icon.AddToClassList(USSIcon);
            _icon.style.backgroundImage = _defaultIcon;
        }

        private void SetToggle(bool toggle)
        {
            _icon.style.backgroundImage = toggle ? _activeIcon : _defaultIcon;
            _icon.style.unityBackgroundImageTintColor = toggle ? _activeColor : _defaultColor;
        }
    }
}
