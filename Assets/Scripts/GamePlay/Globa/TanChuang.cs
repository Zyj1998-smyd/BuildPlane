using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Globa
{
    public class TanChuang : MonoBehaviour
    {
        protected internal TanChuangManager _tanChuangManager;
        private Animation tanChuanAni;
        private TanChuang tanChuangBack;
        [HideInInspector] public RectTransform tanChuanMask;
        
        /** 是否需要Mask */
        public bool showMask; 
        
        public virtual void OpenTanChuang()
        {
            GameObject tanChuangBackTmp = tanChuanMask.parent.GetChild(tanChuanMask.GetSiblingIndex() + 1).gameObject;
            if (tanChuangBackTmp.activeSelf)
            {
                tanChuangBack = tanChuangBackTmp.GetComponent<TanChuang>();
                if (tanChuangBack.transform.GetSiblingIndex()>transform.GetSiblingIndex())
                {
                    transform.SetSiblingIndex(tanChuangBack.transform.GetSiblingIndex() + 1);
                }
            }
            else
            {
                tanChuangBack = null;
            }

            gameObject.SetActive(true);

            
            if (tanChuanMask.GetSiblingIndex() < transform.GetSiblingIndex())
                tanChuanMask.SetSiblingIndex(Mathf.Max(0, transform.GetSiblingIndex() - 1));
            else
                tanChuanMask.SetSiblingIndex(Mathf.Max(0, transform.GetSiblingIndex()));
            
            
            if (showMask) tanChuanMask.gameObject.SetActive(true);
            else tanChuanMask.gameObject.SetActive(false);
            
            
            if (tanChuanAni.gameObject.activeSelf)
                tanChuanAni.Play();

            // AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioCurrencyOpen);

            _tanChuangManager.nowTanChuanTmp = this;
            
            // GameGlobalManager._instance.CloseGuide();
            CheckGuide();
        }
        
        public virtual void CloseTanChuang()
        {
            if (tanChuangBack)
            {
                if (tanChuanMask.GetSiblingIndex() < tanChuangBack.transform.GetSiblingIndex())
                    tanChuanMask.SetSiblingIndex(Mathf.Max(0, tanChuangBack.transform.GetSiblingIndex() - 1));
                else
                    tanChuanMask.SetSiblingIndex(Mathf.Max(0, tanChuangBack.transform.GetSiblingIndex()));
                
                
                _tanChuangManager.nowTanChuanTmp = tanChuangBack;
                
                if (tanChuangBack.showMask) tanChuanMask.gameObject.SetActive(true);
                else tanChuanMask.gameObject.SetActive(false);
            }
            else
            {
                tanChuanMask.gameObject.SetActive(false);
            }
            
            // AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioCurrencyClose);

            // GameGlobalManager._instance.CloseGuide();
            if (tanChuangBack)
                tanChuangBack.CheckGuide();
            else
            {
                // EventManager.Send(CustomEventType.CheckGuide);
            }
            
            gameObject.SetActive(false);
        }
        
        protected virtual void CheckGuide() { }
        
        public virtual void Initial ()
        {
            tanChuanAni = transform.GetComponent<Animation>();
            if (transform.Find("Tittle/BtnClose"))
            {
                transform.Find("Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(CloseTanChuang);
            }
            
            gameObject.SetActive(false);
        }
    }
}