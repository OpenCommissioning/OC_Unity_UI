using System.Globalization;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class TextFieldUlong : TextField<ulong>
    {
        public new class UxmlFactory : UxmlFactory<TextFieldUlong, UxmlTraits> { }
        
        public TextFieldUlong() : this(null,"", false) { }
        
        public TextFieldUlong(Property<ulong> property, string label, bool isReadOnly) : base(property, label, isReadOnly) {}
        
        protected override bool Validate(string newValue, out ulong validValue)
        {
            return ulong.TryParse(newValue, NumberStyles.Integer, CultureInfo.CurrentCulture, out validValue);
        }
    }
}