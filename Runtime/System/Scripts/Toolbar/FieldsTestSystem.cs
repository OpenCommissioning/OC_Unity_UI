using IOSEF.UI.Interactions;
using System.Collections.Generic;
using System.Linq;
using IOSEF.UI.Panel;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Toolbar
{
    public class FieldsTestSystem : ToolbarSystemPanel
    {
        private const string _uxml = "UXML/panel-field";

        public Property<bool> ToggleSlideValue = false;
        public Property<bool> ToggleValue = false;
        public Property<bool> BinaryStatusValue = true;
        public Property<string> StringValue = "test";
        public Property<float> FloatValue = 0.3f;
        public Property<int> IntValue = 12;
        public Property<Vector3> Vector3Value;
        public Property<Vector3Int> Vector3IntValue;
        public Property<Vector2> Vector2Value;
        public Property<Vector2Int> Vector2IntValue;
        public Property<int> SliderIntValue = 12;
        public Property<float> SliderValue = 0.3f;

        protected override void AddContent(SubsystemPanel subsystemPanel)
        {
            var fields = Resources.Load<VisualTreeAsset>(_uxml).Instantiate();
            subsystemPanel.Add(fields);

            fields.Q<ToggleSlide>().BindProperty(ToggleSlideValue);
            fields.Q<IOSEF.UI.Panel.Toggle>().BindProperty(ToggleValue);
            fields.Q<IOSEF.UI.Panel.BinaryStatusField>().BindProperty(BinaryStatusValue);
            fields.Q<IOSEF.UI.Panel.StringField>().BindProperty(StringValue);
            fields.Q<IOSEF.UI.Panel.FloatField>().BindProperty(FloatValue);
            fields.Q<IOSEF.UI.Panel.IntegerField>().BindProperty(IntValue);
            fields.Q<IOSEF.UI.Panel.Vector3Field>().BindProperty(Vector3Value);
            fields.Q<IOSEF.UI.Panel.Vector3IntField>().BindProperty(Vector3IntValue);
            fields.Q<IOSEF.UI.Panel.Vector2Field>().BindProperty(Vector2Value);
            fields.Q<IOSEF.UI.Panel.Vector2IntField>().BindProperty(Vector2IntValue);
            fields.Q<IOSEF.UI.Panel.SliderInt>().BindProperty(SliderIntValue);
            fields.Q<IOSEF.UI.Panel.Slider>().BindProperty(SliderValue);

        }

        private void OnValidate()
        {
            ToggleSlideValue.OnValidate();
            ToggleValue.OnValidate();
            BinaryStatusValue.OnValidate();
            StringValue.OnValidate();
            FloatValue.OnValidate();
            IntValue.OnValidate();
            Vector3Value.OnValidate();
            Vector3IntValue.OnValidate();
            Vector2Value.OnValidate();
            Vector2IntValue.OnValidate();
            SliderIntValue.OnValidate();
            SliderValue.OnValidate();
        }
    }
}
