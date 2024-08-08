using System.Collections.Generic;
using System.Linq;
using IOSEF.MaterialFlow;
using UnityEngine;

namespace IOSEF.UI
{
    public static class PreviewManager 
    {
        public static GameObject CreatePreview(this Pool pool, ISource source)
        {
            return pool.PoolManager.IsTypeValid(source.TypeId) ? InstantiatePreviewEntity(pool, source) : null;
         
        }
        
        private static GameObject InstantiatePreviewEntity(Pool pool, ISource source)
        {
            var entities = new List<GameObject>();
            var newPreviewEntityObject = Object.Instantiate(pool.PoolManager.PayloadList[source.TypeId - 1].gameObject, source.gameObject.transform.position, source.gameObject.transform.rotation, source.gameObject.transform);
            entities.Add(newPreviewEntityObject);
            entities.AddRange(from Transform entity in newPreviewEntityObject.transform select entity.gameObject);

            foreach (var entity in entities)
            {
                Object.DestroyImmediate(entity.GetComponent<Payload>());
                Object.DestroyImmediate(entity.GetComponent<Rigidbody>());
                Object.DestroyImmediate(entity.GetComponent<BoxCollider>());
            }

            SetPreviewMaterial(newPreviewEntityObject);
            return newPreviewEntityObject;
        }
        
        private static void SetPreviewMaterial(GameObject gameObject)
        {
            var children = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in children)
            {
                var materials = new Material[renderer.materials.Length];
                for (var j = 0; j < renderer.materials.Length; j++)
                {
                    materials[j] = SettingsManager.Instance.VisualConfig.Preview;
                }
                renderer.materials = materials;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }
}
