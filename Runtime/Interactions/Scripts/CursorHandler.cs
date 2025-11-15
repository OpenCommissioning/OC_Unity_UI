using System;
using UnityEngine;

namespace OC.UI.Interactions
{
    public class CursorHandler
    {
        private Texture2D _panIcon;
        private Texture2D _orbitIcon;
        private Texture2D _zoomIcon;
        private Texture2D _fpsIcon;

        public CursorHandler()
        {
            Initialize();
        }

        private void Initialize()
        {
            _panIcon = Resources.Load<Texture2D>("Cursors/Pan");
            _orbitIcon = Resources.Load<Texture2D>("Cursors/Orbit");
            _fpsIcon = Resources.Load<Texture2D>("Cursors/FPS");
        }

        public void SetCursor(CameraMode type)
        {
            switch (type)
            {
                case CameraMode.None:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
                case CameraMode.Pan:
                    Cursor.SetCursor(_panIcon, Vector2.zero, CursorMode.Auto);
                    break;
                case CameraMode.Orbit:
                    Cursor.SetCursor(_orbitIcon, Vector2.zero, CursorMode.Auto);
                    break;
                case CameraMode.Zoom:
                    Cursor.SetCursor(_zoomIcon, Vector2.zero, CursorMode.Auto);
                    break;
                case CameraMode.FPS:
                    Cursor.SetCursor(_fpsIcon, Vector2.zero, CursorMode.Auto);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
