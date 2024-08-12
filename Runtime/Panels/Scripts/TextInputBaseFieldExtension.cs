using OC;
using UnityEngine.UIElements;

public static class TextInputBaseFieldExtension
{
    public static void BindReadOnlyProperty<T>(this TextInputBaseField<T> field, IProperty<bool> readOnlyProperty)
    {
        field.isReadOnly = !readOnlyProperty.Value;
        readOnlyProperty.ValueChanged += value => field.isReadOnly = !value;

    }
}
