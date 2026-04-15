using OC.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class PanelEntryDataField : VisualElement
    {
        public bool IsReadonly
        {
            get => _isReadonly;
            set
            {
                if (_isReadonly == value) return;
                SetReadonly(value);
            }
        }
        
        public EntryData EntryData
        {
            get => _entryData;
            set => SetValueWithoutNotify(value);
        }

        private const string USS = "StyleSheet/panel-field";
        private const string USS_ENTRY_DATA_FIELD = "panel-field-entrydata";

        private readonly PanelStringField _stringField;
        private EntryData _entryData;
        private bool _isReadonly;

        public PanelEntryDataField(EntryData entryData)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            AddToClassList(USS_ENTRY_DATA_FIELD);

            _entryData = entryData;
            
            var label = new Label(entryData.Type.ToString()); 
            _stringField = new PanelStringField(entryData.Key, entryData.Value);
            
            Add(label);
            Add(_stringField);

            _stringField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                if (entryData.Validate(_stringField.value))
                {
                    _stringField.SetValueWithoutNotify(evt.newValue);
                    _entryData.Value = evt.newValue;
                }
                else
                {
                    _stringField.SetValueWithoutNotify(evt.previousValue);
                }
            });

            _stringField.isDelayed = true;
            _stringField.multiline = false;
        }

        private void SetValueWithoutNotify(EntryData entryData)
        {
            _entryData = entryData;
            _stringField.SetValueWithoutNotify(entryData.Value);
        }

        private void SetReadonly(bool isReadonly)
        {
            _isReadonly = isReadonly;
            _stringField.isReadOnly = _isReadonly;
        }
    }
}