using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OC.UI.Interactions
{
    public class ColliderViewSystem : MonoBehaviour
    {
        public static ColliderViewSystem Singleton;

        private List<ColliderMaterial> _colliderViews;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(gameObject);
        }

        private void Start()
        {
#if UNITY_6000_3_OR_NEWER
            _colliderViews = FindObjectsByType<ColliderMaterial>().ToList();
#else
            _colliderViews = FindObjectsOfType<ColliderMaterial>().ToList();
#endif
            
            Show(false);
        }

        public void Show(bool show)
        {
            foreach (var colliderView in _colliderViews)
            {
                colliderView.EnableView(show);
            }
        }
    }
}
