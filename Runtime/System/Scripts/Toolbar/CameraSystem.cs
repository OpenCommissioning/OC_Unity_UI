using OC.UI.Panel;
using UnityEngine;

namespace OC.UI.Toolbar
{
    public class CameraSystem : ToolbarSystemPanel
    {
        protected override void AddContent(SubsystemPanel subsystemPanel)
        {
            foreach (var cam in FindObjectsByType<Camera>(FindObjectsSortMode.InstanceID))
            {
                subsystemPanel.Add(new ToggleSlide(cam.name));
            }
        }
    }
} 

