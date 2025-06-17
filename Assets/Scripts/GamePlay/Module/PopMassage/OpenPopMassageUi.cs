using System.Collections.Generic;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Data;
using GamePlay.Globa;
using GamePlay.Main;
using Platform;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.PopMassage
{
    public class OpenPopMassageUi : MonoBehaviour
    {
        /** 前往订阅按钮 */
        private GameObject _btnGo;
        /** 领取奖励按钮 */
        private GameObject _btnGet;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            transform.Find("Mask").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            transform.Find("Follow/Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            _btnGet = transform.Find("Follow/BtnGet").gameObject;
            _btnGo = transform.Find("Follow/BtnGo").gameObject;
            _btnGet.GetComponent<Button>().onClick.AddListener(OnBtnGet);
            _btnGo.GetComponent<Button>().onClick.AddListener(OnBtnGo);
        }

        /// <summary>
        /// 打开弹窗
        /// </summary>
        internal void OpenPop()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);
            RefreshInfo();
        }

        /// <summary>
        /// 刷新页面
        /// </summary>
        private void RefreshInfo()
        {
            GameSdkManager._instance._sdkScript.CheckFeedSubscribeStatus(() =>
            {
                // 已经订阅
                _btnGo.SetActive(false);
                switch (DataHelper.CurUserInfoData.feedSubGet)
                {
                    case 2: // 订阅奖励已领取过
                        _btnGet.SetActive(false);
                        break;
                    case 1: // 订阅奖励未领取过
                        _btnGet.SetActive(true);
                        break;
                }
            }, () =>
            {
                // 尚未订阅 展示前往订阅按钮
                _btnGet.SetActive(false);
                _btnGo.SetActive(true);
            });
        }

        // --------------------------------------------------- 按钮 ---------------------------------------------------
        /// <summary>
        /// 按钮 关闭
        /// </summary>
        private void OnBtnClose()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            MainManager._instance.OnOpenPop_Massage(false);
        }

        /// <summary>
        /// 按钮 前往订阅
        /// </summary>
        private void OnBtnGo()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            GameSdkManager._instance._sdkScript.RequestFeedSubscribe(() =>
            {
                // 订阅成功
                if (DataHelper.CurUserInfoData.feedSubGet == 2)
                {
                    // 奖励已经领取过 订阅完成直接关闭弹窗
                    MainManager._instance.OnOpenPop_Massage(false);
                }
                else
                {
                    // 奖励尚未领取过
                    // 刷新订阅奖励领取记录
                    DataHelper.CurUserInfoData.feedSubGet = 1;
                    DataHelper.ModifyLocalData(new List<string>(1) { "feedSubGet" }, () => { });
                    // 刷新页面
                    RefreshInfo();
                    // 刷新主页面按钮
                    EventManager.Send(CustomEventType.RefreshBtnFeedSub);
                    // 刷新提示红点
                    EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);
                }
            }, () => { });
        }

        /// <summary>
        /// 按钮 领取奖励
        /// </summary>
        private void OnBtnGet()
        {
            List<string> modifyKeys = new List<string>();
            // 刷新订阅奖励领取记录
            DataHelper.CurUserInfoData.feedSubGet = 2;
            modifyKeys.Add("feedSubGet");
            // 领取奖励宝箱 打开宝箱
            int boxIdTmp = 200;
            int curLevelNum = DataHelper.CurUserInfoData.curLevelNum;
            if (curLevelNum >= 5) curLevelNum = 5;
            int boxId = boxIdTmp + (curLevelNum - 1);
            GameGlobalManager._instance.OpenBox(boxId);
            // 完成日常任务 打开X个部件宝箱 TaskID:4
            DataHelper.CompleteDailyTask(4, 1, 0);
            modifyKeys.Add("taskInfo1");
            // 完成成就任务 累计打开X个部件宝箱 TaskID:5
            DataHelper.CompleteGloalTask(5, 1);
            modifyKeys.Add("taskInfo2");
            DataHelper.ModifyLocalData(modifyKeys, () => { });
            // 刷新主页面按钮
            EventManager.Send(CustomEventType.RefreshBtnFeedSub);
            // 刷新提示红点
            EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);
            // 关闭弹窗
            MainManager._instance.OnOpenPop_Massage(false);
        }
    }
}