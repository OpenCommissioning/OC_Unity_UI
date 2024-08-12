using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class BinaryStatusField : BaseField<bool>
    {
        public new class UxmlFactory : UxmlFactory<BinaryStatusField, UxmlTraits> { }

        public new class UxmlTraits : BaseField<bool>.UxmlTraits
        {
            UxmlBoolAttributeDescription _value =
                new UxmlBoolAttributeDescription { name = "Value", defaultValue = false };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as BinaryStatusField;

                ate.value = _value.GetValueFromBag(bag, cc);
            }
        }

        private VisualElement _checkMark;
        private const string _styleSheet = "StyleSheet/panel-field";
        private const string _ussContainer = "panel-field-container";
        private const string _ussBinaryStatusField = "panel-field-binary-status";
        private const string _ussBinaryStatusFieldCheckbox = "panel-field-binary-status_checkbox";
        private const string _ussBinaryStatusFieldCheckboxChecked = "panel-field-binary-status_checkbox__checked";

        public BinaryStatusField() : this("") { }

        public BinaryStatusField(string label) : base(label, (VisualElement)null)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(_styleSheet));
            AddToClassList(_ussContainer);
            AddToClassList(_ussBinaryStatusField);

            _checkMark = new VisualElement
            {
                name = "unity-checkmark",
                pickingMode = PickingMode.Ignore
            };

            _checkMark.AddToClassList(_ussBinaryStatusFieldCheckbox);

            hierarchy.Add(_checkMark);
        }

        public BinaryStatusField(string label, IProperty<bool> property) : this(label)
        {
            this.BindProperty(property);
        }

        public BinaryStatusField(string label, IPropertyReadOnly<bool> property) : this(label)
        {
            this.BindProperty(property);
        }

        public sealed override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);
            _checkMark.EnableInClassList(_ussBinaryStatusFieldCheckboxChecked, newValue);
        }
    }
}