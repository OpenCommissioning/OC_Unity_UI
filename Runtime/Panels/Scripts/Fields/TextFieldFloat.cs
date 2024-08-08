using System.Globalization;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class TextFieldFloat : TextField<float>
    {
        public new class UxmlFactory : UxmlFactory<TextFieldFloat, UxmlTraits> { }
        
        public TextFieldFloat() : this(null,"", false) { }
        
        public TextFieldFloat(Property<float> property, string label, bool isReadOnly) : base(property, label, isReadOnly) {}

        protected override bool Validate(string newValue, out float validValue)
        {
            return float.TryParse(newValue, NumberStyles.Float, CultureInfo.CurrentCulture, out validValue);
        }
    }
}