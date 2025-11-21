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
        

        protected void Awake()
        {
            _camera = GetComponent<CinemachineCamera>();   
            _camera.enabled = _cameraEnabledAtStart; 
            _inputAxisController = GetComponent<CinemachineInputAxisController>();
        }

        public virtual void Enable()
        {
            _camera.enabled = true;
        }

        public virtual void Disable()
        {
            _camera.enabled = false;
        }
    }
}
