using UnityEngine;

namespace IOSEF.UI
{
    [CreateAssetMenu(fileName = "VisualConfig", menuName = "IOSEF/Visual Config")]
    public class VisualConfig : ScriptableObject
    {
        [Header("Collision Materials")] 
        public Material Preview;
        public Material Transperent;
        public Material Enable;
        public Material Warning;
        public Material Error;
    }
}
