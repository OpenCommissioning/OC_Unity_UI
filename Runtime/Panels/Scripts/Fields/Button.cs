using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public class Button : UnityEngine.UIElements.Button
    {
        public new class UxmlFactory : UxmlFactory<Button, UxmlTraits> { }
        
        public sealed override string text
        {
            get => base.text;
            set => base.text = value.ToUpper();
        }
        
        private const string USS = "StyleSheet/panel-field";
        private const string USS_CONTAINER = "panel-field-container";
        private const string USS_BUTTON = "panel-field-button";
        
        public Button() : this("", null) { }
        
        public Button(string label, Action clickEvent) : base(clickEvent)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            AddToClassList(USS_CONTAINER);
            AddToClassList(USS_BUTTON);
            if (!string.IsNullOrEmpty(label)) text = label;
        }
    }
}