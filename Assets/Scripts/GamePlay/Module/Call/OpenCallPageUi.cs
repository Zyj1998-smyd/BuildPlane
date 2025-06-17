using System.Collections.Generic;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Data;
using GamePlay.Globa;
using GamePlay.Main;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.Call
{
    public class OpenCallPageUi : MonoBehaviour
    {
        /** 邀请按钮 */
        private GameObject _btnGo;
        /** 领取奖励按钮 */
        private GameObject _btnGet;
        
        public void OpenTanChuang()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);
            _btnGo.SetActive(DataHelper.CurUserInfoData.callRewardGet == 0);
            _btnGet.SetActive(DataHelper.CurUserInfoData.callRewardGet == 1);
        }

        private void CloseTanChuang()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            MainManager._instance.OnOpenPop_Call(false);
        }
        
        // ----------------------------------------------- 按钮 -----------------------------------------------
        /// <summary>
        /// 按钮 去邀请
        /// </summary>
        private void OnBtnGo()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);

            GameSdkManager._instance._sdkScript.ShareControl(() =>
            {
                ConfigManager.Instance.ConsoleLog(0, "分享成功");
                DataHelper.CurUserInfoData.callRewardGet = 1;
                DataHelper.ModifyLocalData(new List<string>(1) { "callRewardGet" }, () =>
                {
                    EventManager.Send(CustomEventType.RefreshBtnCall);
                });
                _btnGo.SetActive(false);
                _btnGet.SetActive(true);
            });
        }

        /// <summary>
        /// 按钮 领取奖励
        /// </summary>
        private void OnBtnGet()
        {
            DataHelper.CurUserInfoData.callRewardGet = 2;
            DataHelper.CurUserInfoData.diamond += GlobalValueManager.CallRewardNum;
            DataHelper.ModifyLocalData(new List<string>(2) { "callRewardGet", "diamond" }, () =>
            {
                EventManager.Send(CustomEventType.RefreshBtnCall);
            });
            CloseTanChuang();

            // 弹窗 恭喜获得
            DataHelper.CurGetItem = new[] { 1, 200, GlobalValueManager.CallRewardNum };
            GameGlobalManager._instance.OpenGetItem(true);

            // 刷新货币
            EventManager.Send(CustomEventType.RefreshMoney);
        }

        public void Initial()
        {
            transform.Find("Mask").GetComponent<Button>().onClick.AddListener(CloseTanChuang);
            transform.Find("Call/Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(CloseTanChuang);

            _btnGo = transform.Find("Call/BtnGo").gameObject;
            _btnGet = transform.Find("Call/BtnGet").gameObject;
            _btnGo.GetComponent<Button>().onClick.AddListener(OnBtnGo);
            _btnGet.GetComponent<Button>().onClick.AddListener(OnBtnGet);

            _btnGo.SetActive(false);
            _btnGet.SetActive(false);
        }
    }
}