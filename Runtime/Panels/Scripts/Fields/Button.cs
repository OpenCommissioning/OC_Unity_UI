using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Panel
{
    public class Button : UnityEngine.UIElements.Button
    {
        public new class UxmlFactory : UxmlFactory<Button, UxmlTraits> { }
        
        public sealed override string text
        {
            get => base.text;
            set => base.text = value.ToUpper();
        }
        
        private const string Uss = "StyleSheet/panel-field";
        private const string UssContainer = "panel-field-container";
        private const string UssButton = "panel-field-button";
        
        public Button() : this("", null) { }
        
        public Button(string label, Action clickEvent) : base(clickEvent)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(Uss));
            AddToClassList(UssContainer);
            AddToClassList(UssButton);
            if (!string.IsNullOrEmpty(label)) text = label;
        }
    }
}