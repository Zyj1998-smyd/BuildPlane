using System.Collections.Generic;
using UnityEngine;

namespace Common.Tool
{
    public class AssetPool<T> where T : MonoBehaviour
    {
        private readonly Transform parentTram;
        private readonly T prefab;
        private readonly List<T> pool;

        public AssetPool(T prefabTmp,Transform parentTramTep)
        {
            parentTram = parentTramTep;
            prefab = prefabTmp;
            pool = new List<T>();
        }

        public T GetAsset()
        {
            for (var index = 0; index < pool.Count; index++)
            {
                var obj = pool[index];
                if (obj.gameObject.activeSelf) continue;
                obj.gameObject.SetActive(true);
                return obj;
            }

            T newObj = Object.Instantiate(prefab,parentTram);
            pool.Add(newObj);
            return newObj;
        }
        
        public void CleanAsset()
        {
            foreach (var asset in pool)
            {
                if (asset != null)
                {
                    Object.Destroy(asset.gameObject);
                }
            }
            pool.Clear();
        }
        
    }
}
