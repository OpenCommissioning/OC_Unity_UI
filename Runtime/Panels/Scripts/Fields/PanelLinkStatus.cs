using OC.Communication;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class PanelLinkStatus : VisualElement
    {
        private const string USS = "StyleSheet/panel-field";
        private const string USS_BINARY_STATUS = "panel-field-binary-status_checkbox";
        private const string USS_BOX_INDICATOR_ACTIVE = "panel-field-progress-bar_indicator-active";

        public bool Status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                if (_status)
                {
                    _indicator.AddToClassList(USS_BOX_INDICATOR_ACTIVE);
                }
                else
                {
                    _indicator.RemoveFromClassList(USS_BOX_INDICATOR_ACTIVE);
                }
            }
        }

        private bool _status;
        private readonly VisualElement _indicator;
        
        public PanelLinkStatus(Link link)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            
            var group = new PanelGroupContainer("Communication");
            _indicator = new VisualElement();
            _indicator.AddDefaultTheme();
            _indicator.AddToClassList(USS_BINARY_STATUS);
            group.AddToHeader(_indicator);

            var stringFieldType = new PanelStringField("", link.Type);
            var stringFieldPath = new PanelStringField("", link.ClientPath);

            stringFieldType.SetTextInputAlign(TextAnchor.MiddleLeft);
            stringFieldPath.SetTextInputAlign(TextAnchor.MiddleLeft);

            group.Add(stringFieldType);
            group.Add(stringFieldPath);
            Add(group);

            link.Connected.OnValueChanged += value => Status = value;

            Status = link.Connected.Value;
        }
    }
}