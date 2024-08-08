using System.Globalization;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class TextFieldInt : TextField<int>
    {
        public new class UxmlFactory : UxmlFactory<TextFieldInt, UxmlTraits> { }
        
        public TextFieldInt() : this(null,"", false) { }
        
        public TextFieldInt(Property<int> property, string label, bool isReadOnly) : base(property, label, isReadOnly) {}
        
        protected override bool Validate(string newValue, out int validValue)
        {
            return int.TryParse(newValue, NumberStyles.Integer, CultureInfo.CurrentCulture, out validValue);
        }
    }
}