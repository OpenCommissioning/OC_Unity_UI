using OC.Communication;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class LinkStatus : VisualElement
    {
        private const string Uss = "StyleSheet/panel-field";
        private const string UssBinaryStatus = "panel-field-binary-status_checkbox";
        private const string UssBoxIndicatorActive = "panel-field-progress-bar_indicator-active";

        public bool Status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                if (_status)
                {
                    _indicator.AddToClassList(UssBoxIndicatorActive);
                }
                else
                {
                    _indicator.RemoveFromClassList(UssBoxIndicatorActive);
                }
            }
        }

        private bool _status;
        private readonly VisualElement _indicator;
        
        public LinkStatus(Link link)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(Uss));
            
            var group = new GroupContainer("Communication");
            _indicator = new VisualElement();
            _indicator.AddDefaultTheme();
            _indicator.AddToClassList(UssBinaryStatus);
            group.AddToHeader(_indicator);

            var stringFieldType = new StringField("", link.Type);
            var stringFieldPath = new StringField("", link.Path);

            stringFieldType.SetTextInputAlign(TextAnchor.MiddleLeft);
            stringFieldPath.SetTextInputAlign(TextAnchor.MiddleLeft);

            group.Add(stringFieldType);
            group.Add(stringFieldPath);
            Add(group);

            link.IsConnected.ValueChanged += value => Status = value;

            Status = link.IsConnected.Value;
        }
    }
}