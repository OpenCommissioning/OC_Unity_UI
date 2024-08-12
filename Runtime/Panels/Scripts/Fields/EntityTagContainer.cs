using System;
using System.Collections.Generic;
using OC.Data;
using OC.MaterialFlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class EntityTagContainer : VisualElement 
    {
        private const string Uss = "StyleSheet/panel-field";

        private readonly PayloadTag _entityTag;
        private readonly int _directoryIndex;
        private readonly Property<bool> _override = new (false);
        private readonly List<EntryDataField> _entryDataFields = new ();
        private readonly Button _buttonWrite;

        public EntityTagContainer(PayloadTag entityTag, int directoryIndex)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(Uss));

            _entityTag = entityTag;
            _directoryIndex = directoryIndex;
            
            var entryData = entityTag.GetProductDataContent(directoryIndex);

            Add(new GroupContainer(ProductDataDirectoryManager.Instance.ProductDataDirectories[directoryIndex].Name));
            Add(new ToggleSlide("Override", _override));
            foreach (var data in entryData)
            {
                var entryDataFiled = new EntryDataField(data);
                Add(entryDataFiled);
                _entryDataFields.Add(entryDataFiled);
            }

            _buttonWrite = new Button("Write", WriteData);

            Add(_buttonWrite);
            Add(new Button("Read", ReadData));

            OverrideOnValueChanged(false);
            _override.ValueChanged += OverrideOnValueChanged;
        }

        private void OverrideOnValueChanged(bool value)
        {
            foreach (var dataField in _entryDataFields)
            {
                dataField.IsReadonly = !value;
            }
            
            _buttonWrite.SetEnabled(value);
        }

        private void WriteData()
        {
            var content = new List<EntryData>();
            foreach (var dataField in _entryDataFields)
            {
                content.Add(dataField.EntryData);
            }
            
            _entityTag.OverwriteProductData(_directoryIndex, content);
        }

        private void ReadData()
        {
            var entryData = _entityTag.GetProductDataContent(_directoryIndex);
            if (entryData.Count != _entryDataFields.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(entryData));
            }

            for (var i = 0; i < entryData.Count; i++)
            {
                _entryDataFields[i].EntryData = entryData[i];
            }
        }
    }
}