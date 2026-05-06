using System.Collections.Generic;
using System.Linq;
using OC.UI.Panel;
using UnityEngine;
using OC.UI.Interactions;

namespace OC.UI.Toolbar
{
    public class CameraSystem : ToolbarSystemPanel
    {
        private List<CameraController> _cameraControllers;
        private List<PanelToggleSlide> _toggles = new ();

        protected override void AddContent(SubsystemPanel subsystemPanel)
        {
#if UNITY_6000_3_OR_NEWER
            _cameraControllers = FindObjectsByType<CameraController>(FindObjectsInactive.Exclude).ToList();
#else
            _cameraControllers = FindObjectsByType<CameraController>(FindObjectsSortMode.InstanceID).ToList();
#endif  

            for (var i = 0; i < _cameraControllers.Count; i++)
            {
                var toggle = new PanelToggleSlide(_cameraControllers[i].name);
                var index = i;
                toggle.OnValueChanged += b =>
                {
                    SetVirtualCameraState(index, b);
                };
                _toggles.Add(toggle);
                subsystemPanel.Add(toggle);
            }

            DisableAll();
            SetVirtualCameraState(0, true);
        }

        private void SetVirtualCameraState(int index, bool enable)
        {
            DisableAll();
            _toggles[index].SetValueWithoutNotify(true);
            _cameraControllers[index].enabled = enable;
        }

        private void DisableAll()
        {
            foreach (var cameraController in _cameraControllers)
            {
                cameraController.enabled = false; 
            }

            foreach (var toggle in _toggles)
            {
                toggle.SetValueWithoutNotify(false);
            }
        }
    }
} 

