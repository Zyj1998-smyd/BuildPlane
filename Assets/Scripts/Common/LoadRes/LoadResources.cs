using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Common.LoadRes
{
    public class LoadResources : MonoBehaviour
    {
        private static Dictionary<AssetsGroup, List<string>> assetsGroups
        {
            get
            {
                if (assetsGroupsTmp == null) assetsGroupsTmp = new Dictionary<AssetsGroup, List<string>>();
                return assetsGroupsTmp;
            }
        }

        private static Dictionary<AssetsGroup, List<string>> assetsGroupsTmp;
        public enum AssetsGroup
        {
            defaultAssect,
            spriteAssect,
            audioAssect,
            globa
        }

        public static void XXResourcesLoad(string assetName, Action<GameObject> onComplete, AssetsGroup assetsGroup = AssetsGroup.defaultAssect)
        {
            AddressablesManager.LoadAsset<GameObject>(assetName, (_, handleTmp) =>
            {
                if (!assetsGroups.ContainsKey(assetsGroup)) assetsGroups.Add(assetsGroup,new List<string>());
                assetsGroups[assetsGroup].Add(assetName);

                onComplete(handleTmp);
            }, (_) => Debug.LogError(new StringBuilder("[" + assetName + "]加载失败").ToString()));
        }

        public static void XXResourcesLoad(string assetName, Action<Sprite> onComplete)
        {
            AddressablesManager.LoadAsset<Sprite>(assetName, (_, handleTmp) =>
            {
                if (!assetsGroups.ContainsKey(AssetsGroup.spriteAssect)) assetsGroups.Add(AssetsGroup.spriteAssect,new List<string>());
                assetsGroups[AssetsGroup.spriteAssect].Add(assetName);
                
                onComplete(handleTmp);
            }, (_) => Debug.LogError(new StringBuilder("[" + assetName + "]加载失败").ToString()));
        }

        public static void XXResourcesLoad(string assetName, Action<AudioClip> onComplete, bool isBGM = false)
        {
            AddressablesManager.LoadAsset<AudioClip>(assetName, (_, handleTmp) =>
            {
                if (!isBGM)
                {
                    if (!assetsGroups.ContainsKey(AssetsGroup.audioAssect)) assetsGroups.Add(AssetsGroup.audioAssect,new List<string>());
                    assetsGroups[AssetsGroup.audioAssect].Add(assetName);
                }

                onComplete(handleTmp);
            }, (_) => Debug.LogError(new StringBuilder("[" + assetName + "]加载失败").ToString()));
        }
        
        public static void XXResourcesLoad(string assetName, Action<Font> onComplete, AssetsGroup assetsGroup = AssetsGroup.defaultAssect)
        {
            AddressablesManager.LoadAsset<Font>(assetName, (_, handleTmp) =>
            {
                if (!assetsGroups.ContainsKey(assetsGroup)) assetsGroups.Add(assetsGroup, new List<string>());
                assetsGroups[assetsGroup].Add(assetName);
            
                onComplete(handleTmp);
            }, (_) => Debug.LogError(new StringBuilder("[" + assetName + "]加载失败").ToString()));
        }

        public static void ReleaseOneAsset(string addressablesName)
        {
            AddressablesManager.ReleaseAsset(addressablesName);
        }

        public static void ReleaseGroupAsset(AssetsGroup assetsGroup)
        {
            for (int i = 0; i < assetsGroups[assetsGroup].Count; i++)
            {
                AddressablesManager.ReleaseAsset(assetsGroups[assetsGroup][i]);
            }
        }

        public static void ReleaseAllAsset()
        {
            foreach (var key in assetsGroups.Keys)
            {
                if (key != AssetsGroup.globa)
                {
                    for (int j = 0; j < assetsGroups[key].Count; j++)
                    {
                        AddressablesManager.ReleaseAsset(assetsGroups[key][j]);
                    }
                }
            }
        }

        /// <summary>
        /// 加载TextMesh字体
        /// </summary>
        public static void XXResourcesLoad(string assetName, Action<TMP_FontAsset> onComplete, AssetsGroup assetsGroup = AssetsGroup.defaultAssect)
        {
            AddressablesManager.LoadAsset<TMP_FontAsset>(assetName, (_, handleTmp) =>
            {
                if (!assetsGroups.ContainsKey(assetsGroup)) assetsGroups.Add(assetsGroup, new List<string>());
                assetsGroups[assetsGroup].Add(assetName);
            
                // Debug.LogError(new StringBuilder("[" + assetName + "]加载成功").ToString());
                onComplete(handleTmp);
            }, (_) => Debug.LogError(new StringBuilder("[" + assetName + "]加载失败").ToString()));
        }

        /// <summary>
        /// 加载TextMesh材质
        /// </summary>
        public static void XXResourcesLoad(string assetName, Action<Material> onComplete, AssetsGroup assetsGroup = AssetsGroup.defaultAssect)
        {
            AddressablesManager.LoadAsset<Material>(assetName, (_, handleTmp) =>
            {
                if (!assetsGroups.ContainsKey(assetsGroup)) assetsGroups.Add(assetsGroup, new List<string>());
                assetsGroups[assetsGroup].Add(assetName);
            
                // Debug.LogError(new StringBuilder("[" + assetName + "]加载成功").ToString());
                onComplete(handleTmp);
            }, (_) => Debug.LogError(new StringBuilder("[" + assetName + "]加载失败").ToString()));
        }
        
        /// <summary>
        /// 加载Json配置
        /// </summary>
        public static void XXResourcesLoad(string assetName, Action<TextAsset> onComplete, AssetsGroup assetsGroup = AssetsGroup.defaultAssect)
        {
            AddressablesManager.LoadAsset<TextAsset>(assetName, (_, handleTmp) =>
            {
                if (!assetsGroups.ContainsKey(assetsGroup)) assetsGroups.Add(assetsGroup, new List<string>());
                assetsGroups[assetsGroup].Add(assetName);
            
                // Debug.LogError(new StringBuilder("[" + assetName + "]加载成功").ToString());
                onComplete(handleTmp);
            }, (_) => Debug.LogError(new StringBuilder("[" + assetName + "]加载失败").ToString()));
        }

        public static void XXResourcesLoad(string assetName, Action<Texture> onComplete, AssetsGroup assetsGroup = AssetsGroup.defaultAssect)
        {
            AddressablesManager.LoadAsset<Texture>(assetName, (_, handleTmp) =>
            {
                if (!assetsGroups.ContainsKey(assetsGroup)) assetsGroups.Add(assetsGroup, new List<string>());
                assetsGroups[assetsGroup].Add(assetName);
            
                // Debug.LogError(new StringBuilder("[" + assetName + "]加载成功").ToString());
                onComplete(handleTmp);
            }, (_) => Debug.LogError(new StringBuilder("[" + assetName + "]加载失败").ToString()));
        }
    }
}