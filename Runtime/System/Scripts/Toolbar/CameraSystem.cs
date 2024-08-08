using IOSEF.UI.Panel;
using UnityEngine;

namespace IOSEF.UI.Toolbar
{
    public class CameraSystem : ToolbarSystemPanel
    {
        protected override void AddContent(SubsystemPanel subsystemPanel)
        {
            foreach (Camera camera in FindObjectsByType<Camera>(FindObjectsSortMode.InstanceID))
            {
                subsystemPanel.Add(new ToggleSlide(camera.name));
            }
        }
    }
} 

