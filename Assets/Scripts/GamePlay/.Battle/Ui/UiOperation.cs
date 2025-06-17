using System;
using System.Collections.Generic;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Data;
using Platform;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Ui
{
    public class UiOperation : MonoBehaviour
    {
        /** 道具索引 */
        private int _index;

        /** 道具使用页面列表 */
        private List<GameObject> _operationObjs;

        /** 使用道具按钮 观看视频标 */
        private GameObject _btnUseVideoImage;
        /** 使用道具按钮 分享标 */
        private GameObject _btnUseShareImage;

        /** 道具使用类型 -1: 免费 0: 视频 1: 分享 */
        private int _useType;
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        public void AwakeOnUi()
        {
            transform.Find("UseItemFrame/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            var btnUse = transform.Find("UseItemFrame/BtnUse");
            btnUse.GetComponent<Button>().onClick.AddListener(OnBtnUse);
            _operationObjs = new List<GameObject>(3);
            for (int i = 0; i < 3; i++)
            {
                var item = transform.Find("UseItemFrame/ItemFrame" + (i + 1));
                _operationObjs.Add(item.gameObject);
            }

            _btnUseVideoImage = btnUse.Find("Voide").gameObject;
            _btnUseShareImage = btnUse.Find("Share").gameObject;
        }

        private void OnEnable()
        {
            EventManager<int>.Add(CustomEventType.RunGuideUseItem, OnEventGuideUseItem);
        }

        private void OnDisable()
        {
            EventManager<int>.Remove(CustomEventType.RunGuideUseItem, OnEventGuideUseItem);
        }

        /// <summary>
        /// 打开道具使用页
        /// </summary>
        public void OpenOperationUi(int index)
        {
            _index = index;
            for (int i = 0; i < _operationObjs.Count; i++)
            {
                _operationObjs[i].SetActive(i == index);
            }

            if (UiBattle._instance._isClickOpenUseItem)
            {
                // 主动打开 分享/视频使用道具
                _btnUseVideoImage.SetActive(DataHelper.CurGameShareUsed);
                _btnUseShareImage.SetActive(!DataHelper.CurGameShareUsed);
                _useType = DataHelper.CurGameShareUsed ? 0 : 1;
            }
            else
            {
                // 新手引导触发调用 新手引导道具免费
                _btnUseVideoImage.SetActive(false);
                _btnUseShareImage.SetActive(false);
                _useType = -1;
            }
        }

        /** 道具使用回调 */
        private void GetCallBack()
        {
            // 使用道具
            ConfigManager.Instance.ConsoleLog(0, "进入道具使用状态...");
            BattleManager._instance.itemUsing = true;
            switch (_index)
            {
                case 0: // 清空备料杯
                    ConfigManager.Instance.ConsoleLog(0, "使用道具 清空备料");
                    BattleManager._instance.prepareCupClearType = 0; // 清空备料杯类型 使用道具
                    DataHelper.UseClearPrepareCups();
                    BattleManager._instance.OnClearPrepareCups();
                    break;
                case 1: // 更换订单
                    ConfigManager.Instance.ConsoleLog(0, "使用道具 更换订单");
                    DataHelper.UseRefreshOrder();
                    BattleManager._instance.OnRefreshOrderCup();
                    break;
                case 2: // 刷新原料
                    ConfigManager.Instance.ConsoleLog(0, "使用道具 刷新原料");
                    DataHelper.UseRefrshVessels();
                    BattleManager._instance.OnRefreshVessels();
                    break;
            }

            // 记录分享使用状态
            if (_useType == 1)
            {
                DataHelper.CurGameShareUsed = true;
            }

            // 关闭道具使用页
            UiBattle._instance.OnBtnOpenOperation(false, _index);
        }

        /// <summary>
        /// 全局事件回调 新手引导使用道具
        /// </summary>
        /// <param name="typeTmp">道具类型 0: 清空备料杯 1: 刷新订单杯 2: 刷新原料瓶</param>
        private void OnEventGuideUseItem(int typeTmp)
        {
            GetCallBack();
        }

        // ---------------------------------------------- 按钮 ----------------------------------------------
        /** 按钮 关闭 */
        private void OnBtnClose()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            UiBattle._instance.OnBtnOpenOperation(false, _index);
        }

        /// <summary>
        /// 按钮 使用
        /// </summary>
        private void OnBtnUse()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            switch (_useType)
            {
                case -1: // 使用道具 新手引导免费
                    GetCallBack();
                    break;
                case 0:  // 使用道具 观看视频
                    OnBtnGetVideo();
                    break;
                case 1:  // 使用道具 分享
                    OnBtnGetShare();
                    break;
            }
        }

        /** 按钮 观看视频获取 */
        private void OnBtnGetVideo()
        {
            switch (_index)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }

            // 播放激励视频
            GameSdkManager.Instance._sdkScript.VideoControl(GetCallBack, () => { });
        }

        /** 按钮 分享获取 */
        private void OnBtnGetShare()
        {
            switch (_index)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
            
            // 调用分享
            GameSdkManager.Instance._sdkScript.ShareControl(GetCallBack);
        }
    }
}