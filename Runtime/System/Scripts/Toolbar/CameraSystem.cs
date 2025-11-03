using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using OC.UI.Panel;
using UnityEngine;

namespace OC.UI.Toolbar
{
    public class CameraSystem : ToolbarSystemPanel
    {
        private List<CinemachineVirtualCamera> _cameras;
        private List<ToggleSlide> _toggles = new ();

        protected override void AddContent(SubsystemPanel subsystemPanel)
        {
            _cameras = FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.InstanceID).ToList();

            for (var i = 0; i < _cameras.Count; i++)
            {
                var toggle = new ToggleSlide(_cameras[i].name);
                var index = i;
                toggle.OnValueChanged += b =>
                {
                    SetVirtualCameraState(index, b);
                };
                _toggles.Add(toggle);
                subsystemPanel.Add(toggle);
            }

            DisalbeAll();
            SetVirtualCameraState(0, true);
        }

        private void SetVirtualCameraState(int index, bool enable)
        {
            DisalbeAll();
            _toggles[index].SetValueWithoutNotify(true);
            _cameras[index].Priority = enable ? 10 : 0;
        }

        private void DisalbeAll()
        {
            foreach (var virtualCamera in _cameras)
            {
                virtualCamera.Priority = 0;
            }

            foreach (var toggle in _toggles)
            {
                toggle.SetValueWithoutNotify(false);
            }
        }
    }
} 

