using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement("OCButton")]
    public partial class Button : UnityEngine.UIElements.Button
    {
#else
    public class Button : UnityEngine.UIElements.Button
    {
        public new class UxmlFactory : UxmlFactory<Button, UxmlTraits> { }
#endif

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