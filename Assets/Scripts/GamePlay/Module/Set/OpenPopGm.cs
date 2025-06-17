using System.Collections.Generic;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Data;
using GamePlay.Globa;
using GamePlay.Main;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.Set
{
    public class OpenPopGm : MonoBehaviour
    {
        /** 开关 开 */
        private readonly GameObject[] _btnSwithOnUis = new GameObject[3];
        /** 开关 关 */
        private readonly GameObject[] _btnSwitchOffUis = new GameObject[3];
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            transform.Find("Mask").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            transform.Find("GM/Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            transform.Find("GM/GMFrame/GM1/AddGold").GetComponent<Button>().onClick.AddListener(OnBtnAddGold);
            transform.Find("GM/GMFrame/GM1/AddGem").GetComponent<Button>().onClick.AddListener(OnBtnAddGem);
            transform.Find("GM/GMFrame/GM5").GetComponent<Button>().onClick.AddListener(OnBtnAdvanceBoxTime);

            Transform btnSwitch_1 = transform.Find("GM/GMFrame/GM2/Switch");
            _btnSwithOnUis[0] = btnSwitch_1.Find("On").gameObject;
            _btnSwitchOffUis[0] = btnSwitch_1.Find("Off").gameObject;
            btnSwitch_1.GetComponent<Button>().onClick.AddListener(() => { OnBtnSwitch_1(1); });

            Transform btnSwitch_2 = transform.Find("GM/GMFrame/GM3/Switch");
            _btnSwithOnUis[1] = btnSwitch_2.Find("On").gameObject;
            _btnSwitchOffUis[1] = btnSwitch_2.Find("Off").gameObject;
            btnSwitch_2.GetComponent<Button>().onClick.AddListener(() => { OnBtnSwitch_2(1); });

            Transform btnSwitch_3 = transform.Find("GM/GMFrame/GM4/Switch");
            _btnSwithOnUis[2] = btnSwitch_3.Find("On").gameObject;
            _btnSwitchOffUis[2] = btnSwitch_3.Find("Off").gameObject;
            btnSwitch_3.GetComponent<Button>().onClick.AddListener(() => { OnBtnSwitch_3(1); });
        }

        /// <summary>
        /// 打开弹窗
        /// </summary>
        internal void OnOpenPop()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);

            DataHelper.GmSwitch_GetBox = !DataHelper.GmSwitch_GetBox;
            DataHelper.GmSwitch_UnlockAllMap = !DataHelper.GmSwitch_UnlockAllMap;
            DataHelper.GmSwitch_FreeVideo = !DataHelper.GmSwitch_FreeVideo;
            OnBtnSwitch_1(0);
            OnBtnSwitch_2(0);
            OnBtnSwitch_3(0);
        }

        // ---------------------------------------------- 按钮 ----------------------------------------------
        /// <summary>
        /// 按钮 关闭
        /// </summary>
        private void OnBtnClose()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            MainManager._instance.OnOpenPop_Gm(false);
        }

        /// <summary>
        /// 按钮 增加金币
        /// </summary>
        private void OnBtnAddGold()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioGetItem);
            DataHelper.CurUserInfoData.gold += 10000;
            DataHelper.ModifyLocalData(new List<string>(1) { "gold" }, () => { });
            EventManager.Send(CustomEventType.RefreshMoney);
        }

        /// <summary>
        /// 按钮 增加钻石
        /// </summary>
        private void OnBtnAddGem()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioGetItem);
            DataHelper.CurUserInfoData.diamond += 1000;
            DataHelper.ModifyLocalData(new List<string>(1) { "diamond" }, () => { });
            EventManager.Send(CustomEventType.RefreshMoney);
        }

        /// <summary>
        /// 按钮 缩短开箱时间
        /// </summary>
        private void OnBtnAdvanceBoxTime()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);

            List<int> modifyBoxKeys = new List<int>();
            List<long> modifyBoxTimes = new List<long>();
            foreach (KeyValuePair<int, string[]> boxData in DataHelper.CurUserInfoData.boxList)
            {
                if (boxData.Value != null)
                {
                    int boxId = int.Parse(boxData.Value[0]);
                    long boxTime = long.Parse(boxData.Value[1]);
                    if (boxTime > 0)
                    {
                        // 已经启动解锁
                        int unlockTime = ConfigManager.Instance.RewardBoxConfigDict[boxId].OpenTime * 60;
                        long curTime = ToolFunManager.GetCurrTime();
                        long nextTime = boxTime + unlockTime;
                        int subTime = (int)(nextTime - curTime);
                        if (subTime > 0)
                        {
                            // 解锁尚未完成
                            long newTime = (curTime + 10) - unlockTime;
                            modifyBoxKeys.Add(boxData.Key);
                            modifyBoxTimes.Add(newTime);
                        }
                    }
                }
            }

            if (modifyBoxKeys.Count > 0)
            {
                for (int i = 0; i < modifyBoxKeys.Count; i++)
                {
                    DataHelper.CurUserInfoData.boxList[modifyBoxKeys[i]][1] = modifyBoxTimes[i].ToString();
                }

                DataHelper.ModifyLocalData(new List<string>(1) { "boxsList" }, () => { });
                MainManager._instance.RefreshBox();
            }
            else
            {
                Debug.Log("当前没有正在解锁的宝箱");
            }
        }

        /// <summary>
        /// 按钮 开关 每次飞行都获得箱子
        /// </summary>
        private void OnBtnSwitch_1(int type)
        {
            if (type == 1) AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            DataHelper.GmSwitch_GetBox = !DataHelper.GmSwitch_GetBox;
            _btnSwithOnUis[0].SetActive(DataHelper.GmSwitch_GetBox);
            _btnSwitchOffUis[0].SetActive(!DataHelper.GmSwitch_GetBox);
        }

        /// <summary>
        /// 按钮 开关 解锁全部地图
        /// </summary>
        private void OnBtnSwitch_2(int type)
        {
            if (type == 1) AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            DataHelper.GmSwitch_UnlockAllMap = !DataHelper.GmSwitch_UnlockAllMap;
            _btnSwithOnUis[1].SetActive(DataHelper.GmSwitch_UnlockAllMap);
            _btnSwitchOffUis[1].SetActive(!DataHelper.GmSwitch_UnlockAllMap);
            DataHelper.CurUserInfoData.curLevelNum = DataHelper.GmSwitch_UnlockAllMap ? 10 : DataHelper.RealCurLevelNum;
            DataHelper.CurLevelNum = DataHelper.CurUserInfoData.curLevelNum;
            EventManager.Send(CustomEventType.RefreshMainPageMap);
        }

        /// <summary>
        /// 按钮 开关 直接跳过广告
        /// </summary>
        private void OnBtnSwitch_3(int type)
        {
            if (type == 1) AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            DataHelper.GmSwitch_FreeVideo = !DataHelper.GmSwitch_FreeVideo;
            _btnSwithOnUis[2].SetActive(DataHelper.GmSwitch_FreeVideo);
            _btnSwitchOffUis[2].SetActive(!DataHelper.GmSwitch_FreeVideo);
        }
    }
}