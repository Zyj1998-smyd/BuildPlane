using System.Collections.Generic;
using Common.LoadRes;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace GamePlay.Globa
{
    public class TanChuangManager : MonoBehaviour
    {
        private RectTransform tangChuangMainRect;
        private RectTransform rectTranMask;
        
        internal TanChuang nowTanChuanTmp;

        private Dictionary<string, TanChuang> tanChuangDictionary = new Dictionary<string, TanChuang>();
        
        private UniversalAdditionalCameraData camMainPost;
        
        public void CreateMask()
        {
            tangChuangMainRect = GetComponent<RectTransform>();
                
            rectTranMask = new GameObject().AddComponent<RectTransform>();
            rectTranMask.transform.SetParent(tangChuangMainRect);
            rectTranMask.anchorMin = new Vector2(0, 0);
            rectTranMask.anchorMax = new Vector2(1, 1);
            rectTranMask.localPosition = Vector3.zero;
            rectTranMask.offsetMin = Vector2.zero;
            rectTranMask.offsetMax = Vector2.zero;
            rectTranMask.localRotation = Quaternion.identity;
            rectTranMask.localScale = Vector3.one;
            rectTranMask.name = "TanChuanMask";

            Image maskImageTmp = rectTranMask.gameObject.AddComponent<Image>();
            maskImageTmp.color = new Color(0, 0, 0, 0.8f);
            maskImageTmp.maskable = false;

            Button buttonTmp = rectTranMask.gameObject.AddComponent<Button>();
            buttonTmp.transition = Selectable.Transition.None;
            buttonTmp.onClick.AddListener(CloseNowTanChuan);

            rectTranMask.gameObject.SetActive(false);
        }

        public void OpenTanChuang(string tanChuangName)
        {
            if (tanChuangDictionary.ContainsKey(tanChuangName))
            {
                tanChuangDictionary[tanChuangName].OpenTanChuang();
                return;
            }

            // GameGlobalManager._instance.ShowWaitUi(true);
            LoadResources.XXResourcesLoad(tanChuangName, handleTmp =>
            {
                // GameGlobalManager._instance.ShowWaitUi(false);
                RectTransform rectTranTmp = Instantiate(handleTmp.GetComponent<RectTransform>(), tangChuangMainRect);

                TanChuang _tanChuang = rectTranTmp.GetComponent<TanChuang>();
                _tanChuang.tanChuanMask = rectTranMask;
                _tanChuang._tanChuangManager = this;
                _tanChuang.Initial();
                _tanChuang.name = tanChuangName;

                tanChuangDictionary.Add(tanChuangName, _tanChuang);

                tanChuangDictionary[tanChuangName].OpenTanChuang();
            });
        }

        public void CloseNowTanChuan()
        {
            if (nowTanChuanTmp)
                nowTanChuanTmp.CloseTanChuang();
        }

        public void DestroyTanChuang(GameObject tanChuanObj)
        {
            tanChuangDictionary.Remove(tanChuanObj.name);
            Destroy(tanChuanObj);
        }
    }
}