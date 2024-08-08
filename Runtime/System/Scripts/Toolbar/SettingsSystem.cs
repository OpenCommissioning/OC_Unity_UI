using IOSEF.UI.Panel;
using Button = IOSEF.UI.Panel.Button;
using SliderInt = IOSEF.UI.Panel.SliderInt;

namespace IOSEF.UI.Toolbar
{
    public class SettingsSystem: ToolbarSystemPanel
    {
        private SliderInt _mouseSensitivity;

        protected override void AddContent(SubsystemPanel subsystemPanel)
        {
            _mouseSensitivity = new SliderInt("Mouse sensitivity")
            {
                showInputField = true,
                lowValue = 1,
                highValue = 10,
                value = SettingsManager.Instance.MouseSensitivity
            };

            var applyButton = new Button("Apply", ApplySettings);
            
            subsystemPanel.Add(_mouseSensitivity);
            subsystemPanel.Add(applyButton);
        }
        
        private void ApplySettings()
        {
            SettingsManager.Instance.MouseSensitivity = _mouseSensitivity.value;
        }
    }
} 