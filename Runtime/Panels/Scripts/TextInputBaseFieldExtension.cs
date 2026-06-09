using OC;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public static class TextInputBaseFieldExtension
    {
        field.isReadOnly = !readOnlyProperty.Value;
        readOnlyProperty.OnValueChanged += value => field.isReadOnly = !value;
    }
}
