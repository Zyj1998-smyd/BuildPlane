using System;
using System.Collections.Generic;
using System.Text;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Data;
using Data.ClassData;
using Data.ConfigData;
using GamePlay.Globa;
using GamePlay.Main;
using Newtonsoft.Json;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.Round.Sign
{
    public class OpenSignPageUi : MonoBehaviour
    {
        /** 品质框 */
        public Sprite[] qualityFrames;

        /** 签到奖励品质框 */
        private readonly Image[][] _rewardQualitys = new Image[7][];
        /** 签到奖励图标 */
        private readonly Image[][] _rewardImages = new Image[7][];
        /** 签到奖励数量 */
        private readonly TextMeshProUGUI[][] _rewardNumTexts = new TextMeshProUGUI[7][];
        /** 签到完成 */
        private readonly GameObject[] _getOkUis = new GameObject[7];
        /** 今日签到 */
        private readonly GameObject[] _todayUis = new GameObject[7];

        /** 签到按钮 单倍奖励 */
        private GameObject _btnSign;
        /** 签到按钮 双倍奖励 */
        private GameObject _btnSignDouble;
        /** 签到按钮 再领一份奖励 */
        private GameObject _btnSignAgain;
        /** 已签到提示 */
        private GameObject _signGeted;
        
        /** 签到状态列表 -2: 签到时间已过 -1: 签到时间未到 0: 未签到 1: 已签到(单倍) 2: 已签到(双倍) */
        private List<int> _signCanGetList;
        /** 今天是第几天 */
        private int _todayNum;

        /** 奖励ID */
        private readonly int[][] _rewardIds = new int[7][];
        /** 奖励数量 */
        private readonly int[][] _rewardNums = new int[7][];

        /** 当前获得的物品列表 */
        private readonly List<int[]> _curGetRewards = new List<int[]>();

        private void OnEnable()
        {
            EventManager.Add(CustomEventType.GetItemClose, PopGetItem);
        }

        private void OnDisable()
        {
            EventManager.Remove(CustomEventType.GetItemClose, PopGetItem);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            transform.Find("Mask").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            transform.Find("Frame/Close").GetComponent<Button>().onClick.AddListener(OnBtnClose);

            Transform signObj = transform.Find("Frame/Sign");
            for (int i = 0; i < 7; i++)
            {
                Transform dayItem = signObj.Find("Day_" + (i + 1));
                _getOkUis[i] = dayItem.Find("GetOk").gameObject;
                _todayUis[i] = dayItem.Find("ToDay").gameObject;
                if (i != 6)
                {
                    _rewardQualitys[i] = new[] { dayItem.Find("Item1").GetComponent<Image>() };
                    _rewardImages[i] = new[] { dayItem.Find("Item1/Item").GetComponent<Image>() };
                    _rewardNumTexts[i] = new[] { dayItem.Find("Item1/Num").GetComponent<TextMeshProUGUI>() };
                }
                else
                {
                    _rewardQualitys[i] = new[]
                    {
                        dayItem.Find("Item1").GetComponent<Image>(),
                        dayItem.Find("Item2").GetComponent<Image>()
                    };
                    _rewardImages[i] = new[]
                    {
                        dayItem.Find("Item1/Item").GetComponent<Image>(),
                        dayItem.Find("Item2/Item").GetComponent<Image>()
                    };
                    _rewardNumTexts[i] = new[]
                    {
                        dayItem.Find("Item1/Num").GetComponent<TextMeshProUGUI>(),
                        dayItem.Find("Item2/Num").GetComponent<TextMeshProUGUI>()
                    };
                }
            }

            _btnSign = transform.Find("Frame/Btn1").gameObject;
            _btnSign.GetComponent<Button>().onClick.AddListener(OnBtnSign);
            
            _btnSignDouble = transform.Find("Frame/Btn2").gameObject;
            _btnSignDouble.GetComponent<Button>().onClick.AddListener(OnBtnSignDouble);
            
            _btnSignAgain = transform.Find("Frame/Btn3").gameObject;
            _btnSignAgain.GetComponent<Button>().onClick.AddListener(OnBtnSignAgain);

            _signGeted = transform.Find("Frame/Geted").gameObject;
        }

        /// <summary>
        /// 打开弹窗 签到
        /// </summary>
        internal void OpenPop()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);
            RefreshSignReward();
            RefreshSign();
        }

        /// <summary>
        /// 关闭弹窗
        /// </summary>
        private void ClosePop()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            MainManager._instance.OnOpenPop_Sign(false);
        }

        /// <summary>
        /// 刷新签到奖励
        /// </summary>
        private void RefreshSignReward()
        {
            for (int i = 0; i < 7; i++)
            {
                SignConfig config = ConfigManager.Instance.SignConfigs[i];
                List<int> rewardIds = ToolFunManager.GetNumFromStrNew(config.ID);
                if (i != 6)
                {
                    _rewardNumTexts[i][0].text = config.Num == 1 ? "" : config.Num.ToString();
                    GameGlobalManager._instance.SetImage(_rewardImages[i][0], new StringBuilder("IconImage" + rewardIds[0]).ToString());
                    _rewardIds[i] = new[] { rewardIds[0] };
                    _rewardNums[i] = new[] { config.Num };
                    if (ConfigManager.Instance.ShopConfigDict.ContainsKey(rewardIds[0]))
                    {
                        _rewardQualitys[i][0].sprite = qualityFrames[2];
                    }
                    else
                    {
                        ComponentConfig configTmp = ConfigManager.Instance.ComponentConfigDict[rewardIds[0]];
                        _rewardQualitys[i][0].sprite = qualityFrames[configTmp.Quality];
                        _rewardNumTexts[i][0].text = configTmp.Name;
                    }
                }
                else
                {
                    _rewardNumTexts[i][0].text = config.Num == 1 ? "" : config.Num.ToString();
                    _rewardNumTexts[i][1].text = config.Num == 1 ? "" : config.Num.ToString();
                    GameGlobalManager._instance.SetImage(_rewardImages[i][0], new StringBuilder("IconImage" + rewardIds[0]).ToString());
                    GameGlobalManager._instance.SetImage(_rewardImages[i][1], new StringBuilder("IconImage" + rewardIds[1]).ToString());
                    _rewardIds[i] = new[] { rewardIds[0], rewardIds[1] };
                    _rewardNums[i] = new[] { config.Num, config.Num };
                    for (int j = 0; j < 2; j++)
                    {
                        if (ConfigManager.Instance.ShopConfigDict.ContainsKey(rewardIds[j]))
                        {
                            _rewardQualitys[i][j].sprite = qualityFrames[2];
                        }
                        else
                        {
                            ComponentConfig configTmp = ConfigManager.Instance.ComponentConfigDict[rewardIds[j]];
                            _rewardQualitys[i][j].sprite = qualityFrames[configTmp.Quality];
                            _rewardNumTexts[i][j].text = configTmp.Name;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刷新签到
        /// </summary>
        private void RefreshSign()
        {
            SignInfoData signInfo = JsonConvert.DeserializeObject<SignInfoData>(DataHelper.CurUserInfoData.signInfo);
            _todayNum = signInfo.day;
            _signCanGetList = new List<int>(7) { 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < 7; i++)
            {
                if (i == signInfo.day - 1)
                {
                    // 是今天
                    _todayUis[i].SetActive(true);
                    switch (signInfo.isSign)
                    {
                        case 2:  // 已领取全部
                            _getOkUis[i].SetActive(true);
                            _signCanGetList[i] = 2;
                            break;
                        case 1:  // 仅领取单倍
                            _getOkUis[i].SetActive(false);
                            _signCanGetList[i] = 1;
                            break;
                        default: // 未领取
                            _getOkUis[i].SetActive(false);
                            _signCanGetList[i] = 0;
                            break;
                    }
                }
                else
                {
                    // 非今天
                    _todayUis[i].SetActive(false);
                    if (i < signInfo.day - 1)
                    {
                        // 已过日期 已领取
                        _getOkUis[i].SetActive(true);
                        _signCanGetList[i] = -2;
                    }
                    else
                    {
                        // 未到日期 不可领取
                        _getOkUis[i].SetActive(false);
                        _signCanGetList[i] = -1;
                    }
                }
            }

            _btnSign.SetActive(_signCanGetList[_todayNum - 1] == 0);
            _btnSignDouble.SetActive(_signCanGetList[_todayNum - 1] == 0);
            _btnSignAgain.SetActive(_signCanGetList[_todayNum - 1] == 1);
            _signGeted.SetActive(_signCanGetList[_todayNum - 1] == 2);
        }

        /// <summary>
        /// 领取签到奖励
        /// </summary>
        /// <param name="num">0: 单倍奖励 1: 双倍奖励 2: 再领一次奖励</param>
        private void GetSignReward(int num)
        {
            List<string> modifyKeys = new List<string>();
            // 刷新签到信息
            SignInfoData dataTmp = JsonConvert.DeserializeObject<SignInfoData>(DataHelper.CurUserInfoData.signInfo);
            dataTmp.isSign = num == 0 ? 1 : 2;
            DataHelper.CurUserInfoData.signInfo = JsonConvert.SerializeObject(dataTmp);
            modifyKeys.Add("signInfo");
            
            // 领取奖励
            _curGetRewards.Clear();
            for (int i = 0; i < _rewardIds[_todayNum - 1].Length; i++)
            {
                int rewardId = _rewardIds[_todayNum - 1][i];
                int rewardNum = _rewardNums[_todayNum - 1][i];
                if (rewardId < 1000)
                {
                    switch (rewardId / 100)
                    {
                        case 1: // 金币
                            if (num == 1) rewardNum += rewardNum; // 直接双倍领取 奖励加倍
                            DataHelper.CurUserInfoData.gold += rewardNum;
                            modifyKeys.Add("gold");
                            _curGetRewards.Add(new[] { 1, rewardId, rewardNum });
                            break;
                        case 2: // 钻石
                            if (num == 1) rewardNum += rewardNum; // 直接双倍领取 奖励加倍
                            DataHelper.CurUserInfoData.diamond += rewardNum;
                            modifyKeys.Add("diamond");
                            _curGetRewards.Add(new[] { 1, rewardId, rewardNum });
                            break;
                    }
                }
                else
                {
                    void GetRewardEquipment()
                    {
                        bool isNew;
                        if (DataHelper.CurUserInfoData.equipments.ContainsKey(rewardId))
                        {
                            // 老部件 转化为碎片
                            if (DataHelper.CurUserInfoData.equipmentChips.ContainsKey(rewardId))
                                DataHelper.CurUserInfoData.equipmentChips[rewardId] += 10;
                            else
                                DataHelper.CurUserInfoData.equipmentChips.Add(rewardId, 10);
                            if (!modifyKeys.Contains("equipmentChips")) modifyKeys.Add("equipmentChips");
                            isNew = false;
                        }
                        else
                        {
                            // 新部件
                            DataHelper.CurUserInfoData.equipments.Add(rewardId, 1);
                            if (!modifyKeys.Contains("equipments")) modifyKeys.Add("equipments");
                            isNew = true;
                        }

                        _curGetRewards.Add(new[] { 2, rewardId, isNew ? 1 : 10 });
                    
                        // 完成成就任务 累计获得X个部件 TaskID:17
                        DataHelper.CompleteGloalTask(17, 1);
                        // 完成成就任务 累计获得X个红色部件 TaskID:18
                        if (ConfigManager.Instance.ComponentConfigDict[rewardId].Quality >= 4) DataHelper.CompleteGloalTask(18, 1);
                        if (!modifyKeys.Contains("taskInfo2")) modifyKeys.Add("taskInfo2");
                    
                        // 上报自定义分析数据 事件: 获得新部件
                        GameSdkManager._instance._sdkScript.ReportAnalytics("GetNewEquipment", "equipmentNum", DataHelper.CurUserInfoData.equipments.Count);
                    }
                    
                    if (num == 1) GetRewardEquipment(); // 直接双倍领取 奖励加倍
                    GetRewardEquipment();
                }
            }
            
            if (modifyKeys.Count > 0) DataHelper.ModifyLocalData(modifyKeys, () => { });

            // 刷新货币数量
            EventManager.Send(CustomEventType.RefreshMoney);
            // 刷新提示红点
            EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);
            
            // 刷新签到
            RefreshSign();

            // 弹出恭喜获得
            PopGetItem();
        }

        /// <summary>
        /// 弹出恭喜获得
        /// </summary>
        private void PopGetItem()
        {
            if (_curGetRewards.Count <= 0) return;
            DataHelper.CurGetItem = new[] { _curGetRewards[0][0], _curGetRewards[0][1], _curGetRewards[0][2] };
            GameGlobalManager._instance.OpenGetItem(true);
            _curGetRewards.Remove(_curGetRewards[0]);
        }

        // ----------------------------------------------- 按钮 -----------------------------------------------
        /// <summary>
        /// 按钮 关闭弹窗
        /// </summary>
        private void OnBtnClose()
        {
            ClosePop();
        }

        /// <summary>
        /// 按钮 签到 单倍奖励
        /// </summary>
        private void OnBtnSign()
        {
            GetSignReward(0);
        }

        /// <summary>
        /// 按钮 签到 双倍奖励
        /// </summary>
        private void OnBtnSignDouble()
        {
            DataHelper.CurReportDf_adScene = "SignDouble";
            GameSdkManager._instance._sdkScript.VideoControl("双倍领取签到奖励", () => { GetSignReward(1); }, () => { });
        }

        /// <summary>
        /// 按钮 签到 再领一份奖励
        /// </summary>
        private void OnBtnSignAgain()
        {
            DataHelper.CurReportDf_adScene = "SignAgain";
            GameSdkManager._instance._sdkScript.VideoControl("再领一份签到奖励", () => { GetSignReward(2); }, () => { });
        }

        /// <summary>
        /// 按钮 签到
        /// </summary>
        /// <param name="day">签到索引</param>
        private void OnBtnSignDay(int day)
        {
            // switch (_signCanGetList[day])
            // {
            //     case -1: // 已签到
            //         GameGlobalManager._instance.ShowTips(_isSign ? "今日已签到，明天再来吧!" : "签到奖励已领取!");
            //         break;
            //     case 0:  // 不可签到
            //         GameGlobalManager._instance.ShowTips("签到时间未到!");
            //         break;
            //     case 1:  // 可签到
            //         GetSignReward(1);
            //         break;
            // }
        }
    }
}