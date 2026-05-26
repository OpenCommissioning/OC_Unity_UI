using System;
using System.Collections.Generic;
using OC.Data;
using OC.MaterialFlow;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class PanelEntityTagContainer : VisualElement 
    {
        private const string USS = "StyleSheet/panel-field";

        private PayloadTag _entityTag;
        private int _directoryIndex;
        private List<EntryData> _entryData = new ();
        
        private readonly PanelGroupContainer _groupContainer ;
        private readonly Property<bool> _override = new (false);
        private readonly VisualElement _content;
        private readonly PanelButton _buttonWrite;
        private readonly PanelButton _buttonRead;
        private readonly List<PanelEntryDataField> _entryDataFields = new ();

        public PanelEntityTagContainer()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            Add(_groupContainer = new PanelGroupContainer());
            Add(new PanelToggleSlide("Override", _override));
            Add(_content = new VisualElement());
            Add(_buttonWrite = new PanelButton("Write", WriteData));
            Add(_buttonRead = new PanelButton("Read", ReadData));
            _override.Subscribe(OverrideOnValueChanged);
        }

        public void Bind(PayloadTag entityTag, int directoryIndex)
        {
            _override.Value = false;
            _entityTag = entityTag;
            _directoryIndex = directoryIndex;
            _groupContainer.Label = ProductDataDirectoryManager.Instance.ProductDataDirectories[directoryIndex].Name;
            _entryData = entityTag.GetProductDataContent(directoryIndex);

            foreach (var entryData in _entryData)
            {
                var entryDataFiled = new PanelEntryDataField(entryData);
                Add(entryDataFiled);
                _entryDataFields.Add(entryDataFiled);
            }
        }

        public void Unbind()
        {
            foreach (var dataField in _entryDataFields)
            {
                dataField.Unbind();
            }
            
            _entryDataFields.Clear();
            _content.Clear();
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