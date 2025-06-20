using System.Collections.Generic;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Data;
using Data.ConfigData;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Globa.GlobaCanvas
{
    public class NoMoney : MonoBehaviour
    {
        /** 标题 金币不足 */
        private GameObject _tittle_NoGold;
        /** 标题 钻石不足 */
        private GameObject _tittle_NoDiamond;
        /** 内容 金币不足 */
        private GameObject _image_NoGold;
        /** 内容 钻石不足 */
        private GameObject _image_NoDiamond;
        /** 金币/钻石数量 */
        private TextMeshProUGUI _numText;

        /** 类型 1: 金币 2: 钻石 */
        private int _type;

        /** 免费金币数量 */
        private int _freeGold;
        /** 免费钻石数量 */
        private int _freeDiamond;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _tittle_NoGold = transform.Find("Frame/Tittle/Text1").gameObject;
            _tittle_NoDiamond = transform.Find("Frame/Tittle/Text2").gameObject;
            _image_NoGold = transform.Find("Frame/Image1").gameObject;
            _image_NoDiamond = transform.Find("Frame/Image2").gameObject;
            _numText = transform.Find("Frame/Num").GetComponent<TextMeshProUGUI>();

            transform.Find("Frame/Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            gameObject.GetComponent<Button>().onClick.AddListener(OnBtnClose);

            transform.Find("Frame/BtnGet").GetComponent<Button>().onClick.AddListener(OnBtnGet);
        }

        /// <summary>
        /// 打开 货币不足
        /// </summary>
        /// <param name="type">类型 1: 金币 2: 钻石</param>
        internal void OpenNoMoney(int type)
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);
            _type = type;

            _tittle_NoGold.SetActive(_type == 1);
            _image_NoGold.SetActive(_type == 1);

            _tittle_NoDiamond.SetActive(_type == 2);
            _image_NoDiamond.SetActive(_type == 2);

            ShopConfig config_NoGold = ConfigManager.Instance.ShopConfigDict[501];
            ShopConfig config_NoDiamond = ConfigManager.Instance.ShopConfigDict[500];
            _freeGold = ToolFunManager.GetNumFromStrNew(config_NoGold.Num)[1];
            _freeDiamond = ToolFunManager.GetNumFromStrNew(config_NoDiamond.Num)[1];

            _numText.text = _type == 1
                ? _freeGold.ToString()
                : _freeDiamond.ToString();
            
            // 上报自定义分析数据 事件: 提示金币不足/提示钻石不足
            string eventName = _type == 1 ? "NoMoney_Gold" : "Nomoney_Diamond";
            GameSdkManager._instance._sdkScript.ReportAnalytics(eventName, "", "");
        }

        /// <summary>
        /// 获取回调
        /// </summary>
        private void GetCallBack()
        {
            List<string> modifyKeys = new List<string>();
            switch (_type)
            {
                case 1:
                    DataHelper.CurUserInfoData.gold += _freeGold;
                    modifyKeys.Add("gold");
                    DataHelper.CurGetItem = new[] { 1, 100, _freeGold };
                    break;
                case 2:
                    DataHelper.CurUserInfoData.diamond += _freeDiamond;
                    modifyKeys.Add("diamond");
                    DataHelper.CurGetItem = new[] { 1, 200, _freeDiamond };
                    break;
            }

            // 完成日常任务 观看X次视频 TaskID:2
            DataHelper.CompleteDailyTask(2, 1, 0);
            modifyKeys.Add("taskInfo1");
            // 完成成就任务 累计观看X次视频 TaskID:3
            DataHelper.CompleteGloalTask(3, 1);
            modifyKeys.Add("taskInfo2");

            if (modifyKeys.Count > 0)
            {
                DataHelper.ModifyLocalData(modifyKeys, () => { });
                EventManager.Send(CustomEventType.RefreshMoney);
            }

            // 上报自定义分析数据 事件: 广告获得金币/广告获得钻石
            string eventName = _type == 1 ? "GetFreeGold" : "GetFreeDiamond";
            GameSdkManager._instance._sdkScript.ReportAnalytics(eventName, "", "");
            
            GameGlobalManager._instance.OpenNoMoney(false, _type);
            GameGlobalManager._instance.OpenGetItem(true);
        }

        // ------------------------------------------------- 按钮 -------------------------------------------------
        /// <summary>
        /// 按钮 关闭
        /// </summary>
        private void OnBtnClose()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            GameGlobalManager._instance.OpenNoMoney(false, _type);
        }

        /// <summary>
        /// 按钮 获取
        /// </summary>
        private void OnBtnGet()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            DataHelper.CurReportDf_adScene = "GetFreeMoney";
            GameSdkManager._instance._sdkScript.VideoControl("Get Free Coin", GetCallBack, () => { });
        }
    }
}