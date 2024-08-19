using OC.UI.Panel;
using UnityEngine;
using UnityEngine.UIElements;
using FloatField = OC.UI.Panel.FloatField;
using IntegerField = OC.UI.Panel.IntegerField;
using Slider = OC.UI.Panel.Slider;
using SliderInt = OC.UI.Panel.SliderInt;
using Vector2Field = OC.UI.Panel.Vector2Field;
using Vector2IntField = OC.UI.Panel.Vector2IntField;
using Vector3Field = OC.UI.Panel.Vector3Field;
using Vector3IntField = OC.UI.Panel.Vector3IntField;

namespace OC.UI.Toolbar
{
    public class FieldsTestSystem : ToolbarSystemPanel
    {
        private const string UXML = "UXML/panel-field";

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
            var fields = Resources.Load<VisualTreeAsset>(UXML).Instantiate();
            subsystemPanel.Add(fields);

            fields.Q<ToggleSlide>().BindProperty(ToggleSlideValue);
            fields.Q<Panel.Toggle>().BindProperty(ToggleValue);
            fields.Q<BinaryStatusField>().BindProperty(BinaryStatusValue);
            fields.Q<StringField>().BindProperty(StringValue);
            fields.Q<FloatField>().BindProperty(FloatValue);
            fields.Q<IntegerField>().BindProperty(IntValue);
            fields.Q<Vector3Field>().BindProperty(Vector3Value);
            fields.Q<Vector3IntField>().BindProperty(Vector3IntValue);
            fields.Q<Vector2Field>().BindProperty(Vector2Value);
            fields.Q<Vector2IntField>().BindProperty(Vector2IntValue);
            fields.Q<SliderInt>().BindProperty(SliderIntValue);
            fields.Q<Slider>().BindProperty(SliderValue);
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
