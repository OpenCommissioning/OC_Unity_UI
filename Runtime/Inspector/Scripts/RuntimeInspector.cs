using System;
using UnityEngine;

namespace OC.UI.Inspector
{
    public class RuntimeInspector : MonoBehaviour
    {
        public TransformType TransformType => _transformType;
        public bool CanDestroyed => _canDestroyed;
        public bool CanDisabled => _canDisabled;
        public bool CanDuplicated => _canDuplicated;
        
        [Header("Settings")]
        [SerializeField]
        private TransformType _transformType = TransformType.Position | TransformType.Rotation;
        [SerializeField]
        private bool _canDestroyed;
        [SerializeField]
        private bool _canDisabled;
        [SerializeField]
        private bool _canDuplicated;
    }

    [Flags]
    public enum TransformType
    {
        Position = 1,
        Rotation = 2,
        Scale = 4,
        All = ~0
    }
}