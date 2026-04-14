using OC.Interactions;
using UnityEngine;

namespace OC.UI.Interactions
{
    [AddComponentMenu("Open Commissioning/UI/Outline")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(OC.Interactions.Interaction))]
    public class Outline : MonoBehaviour
    {
        [SerializeField]
        private OC.Interactions.Interaction _interaction;
        
        private const uint RENDER_LAYER_HOVER = 2;
        private const uint RENDER_LAYER_SELECTION = 4;

        private void Awake()
        {
            TryGetComponent(out _interaction);
        }
        
        private void OnEnable()
        {
            if (_interaction != null) _interaction.State.OnValueChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            if (_interaction != null) _interaction.State.OnValueChanged -= OnStateChanged;
        }

        private void Reset()
        {
            TryGetComponent(out _interaction);
        }

        private void OnStateChanged(InteractionState interactionState)
        {
            if (!isActiveAndEnabled) return;

            if (interactionState.HasFlag(InteractionState.Selected))
            {
                SetRenderLayerMask(_interaction, RENDER_LAYER_SELECTION);
                return;
            }
            
            if (interactionState.HasFlag(InteractionState.Hovered))
            {
                SetRenderLayerMask(_interaction, RENDER_LAYER_HOVER);
                return;
            }
            
            SetRenderLayerMask(_interaction, 1);
        }
        
        private static void SetRenderLayerMask(OC.Interactions.Interaction interaction, uint layerMask)
        {
            foreach (var item in interaction.Renderers)
            {
                item.renderingLayerMask = layerMask;
            }
        }
    }
}