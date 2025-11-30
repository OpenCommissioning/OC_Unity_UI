using Unity.Cinemachine;
using UnityEngine;

namespace OC.UI.Interactions
{
    [RequireComponent(typeof(CinemachineCamera))]
    public abstract class CameraControllerBase : MonoBehaviour
    {
        public CinemachineCamera Camera => _camera;

        [SerializeField] private bool _cameraEnabledAtStart = false;

        protected CinemachineCamera _camera;
        protected CinemachineInputAxisController _inputAxisController;
        protected CameraControllerMaster _controllerMaster;
        

        protected void Awake()
        {
            _camera = GetComponent<CinemachineCamera>();   
            _camera.enabled = _cameraEnabledAtStart; 
            _inputAxisController = GetComponent<CinemachineInputAxisController>();
            _controllerMaster = GetComponentInParent<CameraControllerMaster>();
        }

        public virtual void Enable()
        {
            if(!_controllerMaster.Enabled) return;
            _camera.enabled = true;
        }

        public virtual void Disable()
        {
            if(!_controllerMaster.Enabled) return;
            _camera.enabled = false;
        }
    }
}
