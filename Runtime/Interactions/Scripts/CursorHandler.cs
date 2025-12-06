using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OC.UI.Interactions
{
    public class CursorHandler
    {
        private Texture2D _panIcon;
        private Texture2D _orbitIcon;
        private Texture2D _fpsIcon;

        private bool _warpCursor = false;
        private bool _isWarping = false;
        private float _warpThreshold = 5f;

        private CameraMode _cameraMode = CameraMode.None;

        public CursorHandler(bool warpCursor)
        {
            _warpCursor = warpCursor;
            Initialize();
        }

        private void Initialize()
        {
            _panIcon = Resources.Load<Texture2D>("Cursors/Pan");
            _orbitIcon = Resources.Load<Texture2D>("Cursors/Orbit");
            _fpsIcon = Resources.Load<Texture2D>("Cursors/FPS");
        }

        public void Update()
        {
            CheckMousePosition();
        }

        public void SetCursor(CameraMode type)
        {
            switch (type)
            {
                case CameraMode.None:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    _cameraMode = CameraMode.None;
                    break;
                case CameraMode.Pan:
                    Cursor.SetCursor(_panIcon, Vector2.zero, CursorMode.Auto);
                    _cameraMode = CameraMode.Pan;
                    break;
                case CameraMode.Orbit:
                    Cursor.SetCursor(_orbitIcon, Vector2.zero, CursorMode.Auto);
                    _cameraMode = CameraMode.Orbit;
                    break;
                case CameraMode.Zoom:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    _cameraMode = CameraMode.Zoom;
                    break;
                case CameraMode.FPS:
                    Cursor.SetCursor(_fpsIcon, Vector2.zero, CursorMode.Auto);
                    _cameraMode = CameraMode.FPS;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void CheckMousePosition()
        {
            if(Utils.IsPointerOverScreen(Mouse.current.position.ReadValue())) return;

            // Only warp, when cam state is not none
            if(_cameraMode == CameraMode.None) return;

            if(!_isWarping && _warpCursor)
            {
                _isWarping = true;
                WarpCursor();
            }
        }

        private void WarpCursor()
        {
            // Get position on other side
            var oppositePosition = GetOppositeMousePosition();
            Mouse.current.WarpCursorPosition(oppositePosition);
            _isWarping = false;
        }

        private Vector2 GetOppositeMousePosition()
        {
            var lastPosition = Mouse.current.position.ReadValue();
            var oppositePosition = Vector2.zero;
            
            if(lastPosition.x > Screen.width)
            {
                oppositePosition = new Vector2(0 + _warpThreshold, lastPosition.y);
            }
            else if(lastPosition.y > Screen.height)
            {
                oppositePosition = new Vector2(lastPosition.x, 0 + _warpThreshold);
            }
            else if(lastPosition.x < 0)
            {
                oppositePosition = new Vector2(Screen.width - _warpThreshold, lastPosition.y);
            }
            else if(lastPosition.y < 0)
            {
                oppositePosition = new Vector2(lastPosition.x, Screen.height - _warpThreshold);
            }

            return oppositePosition;
        }
    }
}
