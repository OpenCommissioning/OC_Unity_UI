using System;
using UnityEngine;

namespace OC.UI
{
    [RequireComponent(typeof(MainCameraController))]
    public class CursorIconController : MonoBehaviour
    {
        [SerializeField]
        private Texture2D _iconPan;
        [SerializeField]
        private Texture2D _iconOrbit;
        [SerializeField]
        private Texture2D _iconFree;
        [SerializeField]
        private MainCameraController _mainCameraController;

        private void OnEnable()
        {
            _mainCameraController.State.Subscribe(OnCameraStateChanged);
        }
        
        private void OnDisable()
        {
            OnCameraStateChanged(MainCameraController.CameraState.None);
            _mainCameraController.State.Unsubscribe(OnCameraStateChanged);
        }

        private void Reset()
        {
            TryGetComponent(out _mainCameraController);
        }

        private void OnCameraStateChanged(MainCameraController.CameraState state)
        {
            switch (state)
            {
                case MainCameraController.CameraState.None:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
                case MainCameraController.CameraState.Fly:
                    Cursor.SetCursor(_iconFree, Vector2.zero, CursorMode.Auto);
                    break;
                case MainCameraController.CameraState.Pan:
                    Cursor.SetCursor(_iconPan, Vector2.zero, CursorMode.Auto);
                    break;
                case MainCameraController.CameraState.Orbit:
                    Cursor.SetCursor(_iconOrbit, Vector2.zero, CursorMode.Auto);
                    break;
                case MainCameraController.CameraState.Zoom:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}