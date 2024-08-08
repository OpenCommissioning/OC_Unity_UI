using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IOSEF.UI.Interactions
{
    public class Geometry : MaterialChanger
    {
        public ViewModeType ViewModeType
        {
            get => _viewModeType;
            set => SetViewMode(value);
        }

        public ViewModeState ViewModeState
        {
            get => _viewModeState;
            set => SetViewState(value);
        }

        [Header("Settings")] 
        [SerializeField] 
        private ViewModeType _viewModeType;
        [SerializeField] 
        private ViewModeState _viewModeState;

        private ViewModeType _lastViewModeType;
        private ViewModeState _lastViewModeState;
        private List<Interaction> _interactions;

        private new void OnEnable()
        {
            base.OnEnable();
            _interactions = GetComponentsInChildren<Interaction>().ToList();
        }

        private void SetViewMode(ViewModeType type)
        {
            if (type == _lastViewModeType) return;
            _lastViewModeType = type;

            switch (type)
            {
                case ViewModeType.Default:
                    Hide(false);
                    SetOriginalMaterials();
                    DisableInteraction(false);
                    break;
                case ViewModeType.Transperent:
                    Hide(false);
                    SetMaterial(SettingsManager.Instance.VisualConfig.Transperent);
                    DisableInteraction(true);
                    break;
                case ViewModeType.Hide:
                    Hide(true);
                    DisableInteraction(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void SetViewState(ViewModeState state)
        {
            if (state == _lastViewModeState) return;
            _lastViewModeState = state;

            switch (state)
            {
                case ViewModeState.Default:
                    ResetColor();
                    break;
                case ViewModeState.Warning:
                    SetColor(Color.yellow, 0.4f);
                    break;
                case ViewModeState.Error:
                    SetColor(Color.red, 0.4f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void DisableInteraction(bool disable)
        {
            foreach (var interaction in _interactions)
            {
                interaction.IsDisabled = disable;
            }
        }
    }
}
