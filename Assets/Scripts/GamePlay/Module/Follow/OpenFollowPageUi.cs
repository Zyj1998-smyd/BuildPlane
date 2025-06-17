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

namespace GamePlay.Module.Follow
{
    public class OpenFollowPageUi : MonoBehaviour
    {
        /** 进入侧边栏按钮 */
        private GameObject _btnGo;
        /** 领取奖励按钮 */
        private GameObject _btnGet;
        
        public void OpenTanChuang()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);
            
            _btnGo.SetActive(false);
            _btnGet.SetActive(false);

            GameSdkManager._instance._sdkScript.CheckIsAddedToMyMiniProgram(b =>
            {
                if (!b)
                {
                    // 未添加到我的小程序
                    _btnGet.SetActive(false);
                    _btnGo.SetActive(true);
                }
                else
                {
                    // 已添加到我的小程序
                    if (DataHelper.CurUserInfoData.addedToMyMiniProgramGet == 1)
                    {
                        // 已领取过收藏奖励
                        _btnGet.SetActive(false);
                        _btnGo.SetActive(true);
                    }
                    else
                    {
                        // 未领取过收藏奖励
                        _btnGet.SetActive(true);
                        _btnGo.SetActive(false);
                    }
                }
            }, () =>
            {
                _btnGo.SetActive(true);
                _btnGet.SetActive(false);
            }, () =>
            {
                _btnGo.SetActive(true);
                _btnGet.SetActive(false);
            });
        }

        private void CloseTanChuang()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            MainManager._instance.OnOpenPop_Follow(false);
        }
        
        // ----------------------------------------------- 按钮 -----------------------------------------------
        /// <summary>
        /// 按钮 我已知晓
        /// </summary>
        private void OnBtnGo()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            GameSdkManager._instance._sdkScript.NavigateToSideBar();
            CloseTanChuang();
        }

        /// <summary>
        /// 按钮 领取奖励
        /// </summary>
        private void OnBtnGet()
        {
            DataHelper.CurUserInfoData.addedToMyMiniProgramGet = 1;
            DataHelper.CurUserInfoData.diamond += GlobalValueManager.FollowRewardNum;
            DataHelper.ModifyLocalData(new List<string>(1) { "addedToMyMiniProgramGet", "diamond" }, () =>
            {
                EventManager.Send(CustomEventType.RefreshBtnFollow);
            });
            CloseTanChuang();

            // 弹窗 恭喜获得
            DataHelper.CurGetItem = new[] { 1, 200, GlobalValueManager.FollowRewardNum };
            GameGlobalManager._instance.OpenGetItem(true);
            
            // 刷新货币
            EventManager.Send(CustomEventType.RefreshMoney);
        }

        /// <summary>
        /// 初始化页面
        /// </summary>
        public void Initial()
        {
            transform.Find("Mask").GetComponent<Button>().onClick.AddListener(CloseTanChuang);
            transform.Find("Follow/Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(CloseTanChuang);

            _btnGo = transform.Find("Follow/BtnGo").gameObject;
            _btnGo.GetComponent<Button>().onClick.AddListener(OnBtnGo);
            
            _btnGet = transform.Find("Follow/BtnGet").gameObject;
            _btnGet.GetComponent<Button>().onClick.AddListener(OnBtnGet);
        }
    }
}