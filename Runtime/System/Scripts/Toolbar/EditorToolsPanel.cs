using System;
using IOSEF.UI.TransformHandles;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOSEF.UI.Toolbar
{
    [RequireComponent(typeof(UIDocument))]
    [DisallowMultipleComponent]
    public class EditorToolsPanel : MonoBehaviour
    {
        public static EditorToolsPanel Instance { get; private set; }
        
        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable == value) return;
                SetEnable(value);
            }
        }

        [SerializeField]
        private Sprite _viewIcon;
        [SerializeField]
        private Sprite _moveIcon;
        [SerializeField]
        private Sprite _rotateIcon;
        [SerializeField]
        private Sprite _centerDefaultIcon;
        [SerializeField]
        private Sprite _centerActiveIcon;
        [SerializeField]
        private Sprite _globalDefaultIcon;
        [SerializeField]
        private Sprite _globalActiveIcon;

        private bool _enable;
        
        private Toggle _view;
        private Toggle _move;
        private Toggle _rotate;
        private Toggle _center;
        private Toggle _global;

        private List<Toggle> _selectionGroup;

        private VisualElement _toolbar;
        private const string Uxml = "UXML/toolbar_editorTools";
        private const string StyleSheet = "StyleSheet/toolbar";

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }
        
        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            _toolbar = Resources.Load<VisualTreeAsset>(Uxml).Instantiate().Q("toolbar");
            _toolbar.AddDefaultTheme();
            _toolbar.styleSheets.Add(Resources.Load<StyleSheet>(StyleSheet));
            uiDocument.rootVisualElement.Add(_toolbar);

            _view = _toolbar.Q<Toggle>("view");
            _move = _toolbar.Q<Toggle>("move");
            _rotate = _toolbar.Q<Toggle>("rotate");
            _center = _toolbar.Q<Toggle>("center");
            _global = _toolbar.Q<Toggle>("global");

            _view.DefaultIcon = _viewIcon;
            _move.DefaultIcon = _moveIcon;
            _rotate.DefaultIcon = _rotateIcon;
            _center.DefaultIcon = _centerDefaultIcon;
            _center.ActiveIcon = _centerActiveIcon;
            _global.DefaultIcon = _globalDefaultIcon;
            _global.ActiveIcon = _globalActiveIcon;

            SetTool(RuntimeTransformHandle.Instance.ToolType);
            SetHandlePosition(RuntimeTransformHandle.Instance.HandlePosition);
            SetHandleRotation(RuntimeTransformHandle.Instance.HandleRotation);

            _view.RegisterCallback<ChangeEvent<bool>>(_ => RuntimeTransformHandle.Instance.ToolType = ToolType.View);
            _move.RegisterCallback<ChangeEvent<bool>>(_ => RuntimeTransformHandle.Instance.ToolType = ToolType.Move);
            _rotate.RegisterCallback<ChangeEvent<bool>>(_ => RuntimeTransformHandle.Instance.ToolType = ToolType.Rotation);
            _center.RegisterCallback<ChangeEvent<bool>>(evt => SetHandlePosition(evt.newValue));
            _global.RegisterCallback<ChangeEvent<bool>>(evt => SetHandleRotation(evt.newValue));

            RuntimeTransformHandle.Instance.OnToolChanged += SetTool;
            RuntimeTransformHandle.Instance.OnHandlePositionChanged += SetHandlePosition;
            RuntimeTransformHandle.Instance.OnHandleRotationChanged += SetHandleRotation;
            UserInteractionManager.Instance.OnInteractionEnableChanged += SetEnable;

            SetEnable(false);
        }

        private void OnDestroy()
        {
            RuntimeTransformHandle.Instance.OnToolChanged -= SetTool;
            RuntimeTransformHandle.Instance.OnHandlePositionChanged -= SetHandlePosition;
            RuntimeTransformHandle.Instance.OnHandleRotationChanged -= SetHandleRotation;
            UserInteractionManager.Instance.OnInteractionEnableChanged -= SetEnable;
        }

        private void SetEnable(bool value)
        {
            _enable = value;
            _toolbar.style.display = _enable
                ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex)
                : new StyleEnum<DisplayStyle>(DisplayStyle.None);
            if (!_enable) RuntimeTransformHandle.Instance.ToolType = ToolType.View;
        }

        private void SetHandlePosition(bool enable)
        {
            RuntimeTransformHandle.Instance.HandlePosition = enable ? HandlePosition.Center : HandlePosition.Pivot;
        }
        
        private void SetHandleRotation(bool enable)
        {
            RuntimeTransformHandle.Instance.HandleRotation = enable ? HandleRotation.World : HandleRotation.Local;
        }

        private void SetHandlePosition(HandlePosition handlePosition)
        {
            _center.SetValueWithoutNotify(handlePosition == HandlePosition.Center);
        }
        
        private void SetHandleRotation(HandleRotation handleRotation)
        {
            _global.SetValueWithoutNotify(handleRotation == HandleRotation.World);
        }

        private void SetTool(ToolType toolType)
        {
            _view.SetValueWithoutNotify(false);
            _move.SetValueWithoutNotify(false);
            _rotate.SetValueWithoutNotify(false);
            
            switch (toolType)
            {
                case ToolType.View:
                    _view.SetValueWithoutNotify(true);
                    break;
                case ToolType.Move:
                    _move.SetValueWithoutNotify(true);
                    break;
                case ToolType.Rotation:
                    _rotate.SetValueWithoutNotify(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(toolType), toolType, null);
            }
        }
    }
}