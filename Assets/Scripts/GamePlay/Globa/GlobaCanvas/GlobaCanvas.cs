using System;
using Common.LoadRes;
using TMPro;
using UnityEngine;

namespace GamePlay.Globa.GlobaCanvas
{
    public class GlobaCanvas : MonoBehaviour
    {
        internal Canvas canvasMe;
        
        private GameObject tipsObj;
        private TextMeshProUGUI tipsText;
        private Animation tipsAni;
        
        private LoadScene _loadScene;

        private NoMoney _noMoneyUi;

        private GetItem _getItemUi;

        private void Awake()
        {
            canvasMe = GetComponent<Canvas>();
            
            tipsObj = transform.Find("Tips").gameObject;
            tipsText = tipsObj.transform.Find("Tips/Text").GetComponent<TextMeshProUGUI>();
            tipsAni = tipsObj.GetComponent<Animation>();
            tipsObj.SetActive(false);
            
            _loadScene = transform.Find("LoadScene").GetComponent<LoadScene>();
            _loadScene.gameObject.SetActive(false);

            _noMoneyUi = transform.Find("NoMoney").GetComponent<NoMoney>();
            _noMoneyUi.Initial();
            _noMoneyUi.gameObject.SetActive(false);

            _getItemUi = transform.Find("GetItem").GetComponent<GetItem>();
            _getItemUi.Initial();
            _getItemUi.gameObject.SetActive(false);
        }
        
        /** 弹出提示Tips */
        public void ShowTips(string textTmp)
        {
            tipsObj.SetActive(true);
            tipsText.text = textTmp;
            tipsAni["TipsOpen"].normalizedTime = 0;
            tipsAni.Play("TipsOpen");
        }
        
        /** 展示加载页面 */
        public void ShowLoadSceneStart()
        {
            _loadScene.gameObject.SetActive(true);
            _loadScene.StartLoad();
        }

        /** 关闭加载页面 */
        public void ShowLoadSceneEnd(int loadIngNumMaxTmp, Action callback)
        {
            _loadScene.EndLoad(loadIngNumMaxTmp, callback);
        }

        /// <summary>
        /// 打开/关闭 货币不足
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        /// <param name="type">类型 1: 金币 2: 钻石</param>
        public void OpenNoMoney(bool isOpen, int type)
        {
            _noMoneyUi.gameObject.SetActive(isOpen);
            if (isOpen)
            {
                _noMoneyUi.OpenNoMoney(type);
            }
        }

        /// <summary>
        /// 打开/关闭 恭喜获得
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        public void OpenGetItem(bool isOpen)
        {
            _getItemUi.gameObject.SetActive(isOpen);
            if (isOpen)
            {
                _getItemUi.OpenGetItem();
            }
        }

    }
}