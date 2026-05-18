using OC;
using System;
using UnityEngine.UIElements;

public static class BaseFieldExtension
{
    public static BaseField<T> Bind<T>(this BaseField<T> field, IProperty<T> property)
    {
        field.BindProperty(property);
        return field;
    }
    
    public static BaseField<T> Bind<T>(this BaseField<T> field, IPropertyReadOnly<T> property)
    {
        field.BindProperty(property);
        return field;
    }
    
    public static BaseField<T> Unbind<T>(this BaseField<T> field)
    {
        field.UnbindProperty();
        return field;
    }
    
    public static void BindProperty<T>(this BaseField<T> field, IProperty<T> property)
    {
        if (field.userData != null) field.UnbindProperty();

        field.value = property.Value;
        property.OnValueChanged += OnPropertyValueChange(field);
        field.RegisterValueChangedCallback(OnFieldValueChange);
        field.userData = property;
    }
    public static void BindProperty<T>(this BaseField<T> field, IPropertyReadOnly<T> property)
    {
        if (field.userData != null) field.UnbindProperty();

        field.value = property.Value;
        property.OnValueChanged += OnPropertyValueChange(field);
        field.userData = property;
    }

    public static void UnbindProperty<T>(this BaseField<T> field)
    {
        (field.userData as Property<T>).OnValueChanged -= OnPropertyValueChange(field);
        field.UnregisterValueChangedCallback(OnFieldValueChange);
        field.userData = null;
    }

    private static void OnFieldValueChange<T>(ChangeEvent<T> evt)
    {
        var property = (evt.target as BaseField<T>).userData as Property<T>;
        property.Value = evt.newValue;
    }

    private static Action<T> OnPropertyValueChange<T>(BaseField<T> field)
    {
        return value => field.SetValueWithoutNotify(value);
    }
}
