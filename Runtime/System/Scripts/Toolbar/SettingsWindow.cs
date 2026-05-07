using OC.UI.Panel;

namespace OC.UI.Toolbar
{
    public class SettingsWindow: ToolbarWindow
    {
        private PanelSliderInt _mouseSensitivity;

        protected override void AddContent(SubsystemPanel subsystemPanel)
        {
            _mouseSensitivity = new PanelSliderInt("Mouse sensitivity")
            {
                showInputField = true,
                lowValue = 1,
                highValue = 10,
                value = SettingsManager.Instance.MouseSensitivity
            };

            var applyButton = new PanelButton("Apply", ApplySettings);
            
            subsystemPanel.Add(_mouseSensitivity);
            subsystemPanel.Add(applyButton);
        }
        
        private void ApplySettings()
        {
            SettingsManager.Instance.MouseSensitivity = _mouseSensitivity.value;
        }
    }
} 