using UnityEngine.UIElements;

namespace IOSEF.UI.Industrial
{
    public interface IIndustrialPanel
    {
        public string Path { get; }
        public VisualElement Create();
    }
}