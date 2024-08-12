using OC.UI.Panel;
using Button = OC.UI.Panel.Button;
using SliderInt = OC.UI.Panel.SliderInt;

namespace OC.UI.Toolbar
{
    public class SettingsSystem: ToolbarSystemPanel
    {
        private Panel.SliderInt _mouseSensitivity;

        protected override void AddContent(SubsystemPanel subsystemPanel)
        {
            _mouseSensitivity = new Panel.SliderInt("Mouse sensitivity")
            {
                showInputField = true,
                lowValue = 1,
                highValue = 10,
                value = SettingsManager.Instance.MouseSensitivity
            };

            var applyButton = new Panel.Button("Apply", ApplySettings);
            
            subsystemPanel.Add(_mouseSensitivity);
            subsystemPanel.Add(applyButton);
        }
        
        private void ApplySettings()
        {
            SettingsManager.Instance.MouseSensitivity = _mouseSensitivity.value;
        }
    }
} 