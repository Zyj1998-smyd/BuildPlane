using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using Data.ClassData;
using Data.ConfigData;
using GamePlay.Globa;
using Newtonsoft.Json;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Ui
{
    public class UiAccount : MonoBehaviour
    {
        /** 页面动画组件 */
        private Animation _animation;
        
        /** 点击继续按钮 */
        private GameObject _btnNext;
        /** 得分1(个人)/2(地区) */
        private GameObject _scoreObj_1, _scoreObj_2;
        /** 奖励 */
        private GameObject _rewardObj;
        /** 宝箱存储已满弹窗 */
        private GameObject _boxFullObj;
        /** 排行榜1(个人)/2(地区) */
        private GameObject _rankObj_1, _rankObj_2;

        /** 得分1(个人) 城市进度 */
        private Image _scoreOrderValue;
        /** 得分1(个人) 城市名称 当前站/下一站 */
        private TextMeshProUGUI _scoreOrderText_1, _scoreOrderText_2;
        /** 得分1(个人) 距离 */
        private TextMeshProUGUI _scoreDisNumText;
        /** 得分1(个人) 新纪录 */
        private GameObject _scoreNewRecord;

        /** 得分2(地区) 地区名称 */
        private TextMeshProUGUI _scoreCityNameText;
        /** 得分2(地区) 地区距离 */
        private TextMeshProUGUI _scoreCityDisNumText;

        /** 奖励 宝箱 背光 */
        private readonly GameObject[] _rewardBoxLights = new GameObject[3];
        /** 奖励 宝箱 图标 */
        private Image _rewardBoxImage;
        /** 奖励 宝箱 获取进度条 */
        private Image _rewardBoxBar;
        /** 奖励 宝箱 获取进度值 */
        private TextMeshProUGUI _rewardBoxTipText;

        /** 奖励 金币 */
        private readonly GameObject[] _rewardGolds = new GameObject[5];
        /** 奖励 金币 金币数量/倍率 */
        private readonly TextMeshProUGUI[] _rewardGoldNumTexts = new TextMeshProUGUI[5];
        /** 奖励 金币 超级加倍 */
        private GameObject _rewardGoldMultiple;
        /** 奖励 金币 超级加倍按钮 */
        private GameObject _rewardGoldBtnMultiple;
        /** 奖励 金币 超级加倍箭头 */
        private Transform _rewardGoldMultipleArrow;
        /** 奖励 金币 超级加倍按钮金币数量 */
        private TextMeshProUGUI _rewardGoldBtnMultipleNumText;
        /** 奖励 金币 超级加倍箭头动画组件 */
        private Animation _rewardGoldMultipleArrowAni;

        /** 宝箱存储已满弹窗 背光 */
        private readonly GameObject[] _boxFullBoxLights = new GameObject[3];
        /** 宝箱存储已满弹窗 宝箱图标 */
        private Image _boxFullBoxImage;
        /** 宝箱存储已满弹窗 立即打开消耗钻石数量 */
        private TextMeshProUGUI _boxFullBtnOpenNumText;

        /** 个人排行榜 */
        private readonly GameObject[] _rankItemUis = new GameObject[5];
        /** 个人排行榜 我的背景 */
        private readonly GameObject[] _rankItemMeUis = new GameObject[5];
        /** 个人排行榜 头像 */
        private readonly Image[] _rankItemHeadUis = new Image[5];
        /** 个人排行榜 昵称 */
        private readonly Text[] _rankItemNameUis = new Text[5];
        /** 个人排行榜 最远距离 */
        private readonly TextMeshProUGUI[] _rankItemScoreUis = new TextMeshProUGUI[5];

        /** 个人排行榜 我的排名 */
        private TextMeshProUGUI _rankMeNumText;
        /** 个人排行榜 我的头像 */
        private Image _rankMeHead;
        /** 个人排行榜 我的昵称 */
        private Text _rankMeName;
        /** 个人排行榜 我的最远距离 */
        private TextMeshProUGUI _rankMeScoreText;

        /** 地区排行榜 */
        private readonly GameObject[] _rankItemUis_city = new GameObject[5];
        /** 地区排行榜 我的背景 */
        private readonly GameObject[] _rankItemMeUis_city = new GameObject[5];
        /** 地区排行榜 地区名称 */
        private readonly TextMeshProUGUI[] _rankItemNameUis_city = new TextMeshProUGUI[5];
        /** 地区排行榜 地区最远距离 */
        private readonly TextMeshProUGUI[] _rankItemScoreUis_city = new TextMeshProUGUI[5];

        /** 地区排行榜 我的地区排名 */
        private TextMeshProUGUI _rankMeNumText_city;
        /** 地区排行榜 我的地区名称 */
        private TextMeshProUGUI _rankMeName_city;
        /** 地区排行榜 我的地区最远距离 */
        private TextMeshProUGUI _rankMeScoreText_city;

        /** 按钮 再飞一次/下个城市 */
        private GameObject nextBtnText1, nextBtnText2;
        
        /** UniTask异步信标 */
        private CancellationTokenSource ctsWaitUniTask;
        /** UniTask异步信标 金币超级加倍 */
        private CancellationTokenSource ctsWaitUniTask_goldMult;

        /** 点击继续按钮 点击次数 */
        private int _nextNum;

        /** 金币数量 */
        private float _goldNum;
        /** 总金币数量 */
        private int _totalGoldNum;

        /** 获得新宝箱 */
        private int _newBoxGet;
        /** 宝箱背光 */
        private int _showLight;
        /** 宝箱获取进度距离目标剩余进度 */
        private int _showProgress;

        /** 宝箱存储已满 立即打开消耗钻石数量 */
        private int _boxFullOpenGemNum;

        /** 金币超级加倍已经领取 */
        private bool _rewardGoldMultGetOk;

        /** 本次推进了关卡 */
        private bool _curLevelNumAdd;

        /** 是否进入下个城市 */
        private bool _isNextLevel;

        private void OnEnable()
        {
            EventManager.Add(CustomEventType.BoxOpenDone, BoxOpenComplete);
        }

        private void OnDisable()
        {
            EventManager.Remove(CustomEventType.BoxOpenDone, BoxOpenComplete);
            ctsWaitUniTask?.Cancel();
            ctsWaitUniTask?.Dispose();
            ctsWaitUniTask = null;
            ctsWaitUniTask_goldMult?.Cancel();
            ctsWaitUniTask_goldMult?.Dispose();
            ctsWaitUniTask_goldMult = null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _animation = gameObject.GetComponent<Animation>();
            
            _btnNext = transform.Find("BtnNext").gameObject;
            _btnNext.SetActive(false);
            _btnNext.GetComponent<Button>().onClick.AddListener(OnBtnNext);
            _scoreObj_1 = transform.Find("Score1").gameObject;
            _scoreObj_1.SetActive(false);
            _scoreObj_2 = transform.Find("Score2").gameObject;
            _scoreObj_2.SetActive(false);
            _rewardObj = transform.Find("Reward").gameObject;
            _rewardObj.SetActive(false);
            _boxFullObj = transform.Find("BoxFull").gameObject;
            _boxFullObj.SetActive(false);
            GameObject rankObj = transform.Find("Rank").gameObject;
            rankObj.SetActive(false);
            _rankObj_1 = rankObj.transform.Find("Rank1").gameObject;
            _rankObj_1.SetActive(false);
            _rankObj_2 = rankObj.transform.Find("Rank2").gameObject;
            _rankObj_2.SetActive(false);

            _scoreOrderValue = _scoreObj_1.transform.Find("Frame3/OrderFrame/Value").GetComponent<Image>();
            _scoreOrderText_1 = _scoreObj_1.transform.Find("Frame3/OrderFrame/Text1").GetComponent<TextMeshProUGUI>();
            _scoreOrderText_2 = _scoreObj_1.transform.Find("Frame3/OrderFrame/Text2").GetComponent<TextMeshProUGUI>();
            _scoreDisNumText = _scoreObj_1.transform.Find("Num").GetComponent<TextMeshProUGUI>();
            _scoreNewRecord = _scoreObj_1.transform.Find("NewRecord").gameObject;

            _scoreCityNameText = _scoreObj_2.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            _scoreCityDisNumText = _scoreObj_2.transform.Find("Num").GetComponent<TextMeshProUGUI>();

            for (int i = 0; i < 3; i++)
            {
                _rewardBoxLights[i] = _rewardObj.transform.Find("Box/Light/Light" + (i + 1)).gameObject;
                _boxFullBoxLights[i] = _boxFullObj.transform.Find("Box/Light/Light" + (i + 1)).gameObject;
                
                _rewardBoxLights[i].SetActive(false);
            }

            _rewardBoxImage = _rewardObj.transform.Find("Box/Image").GetComponent<Image>();
            _rewardBoxTipText = _rewardObj.transform.Find("Box/Progress/Progress/Tip").GetComponent<TextMeshProUGUI>();
            _rewardBoxBar = _rewardObj.transform.Find("Box/Progress/Progress/Value").GetComponent<Image>();

            for (int i = 0; i < 5; i++)
            {
                _rewardGolds[i] = _rewardObj.transform.Find("Gold/Gold" + (i + 1)).gameObject;
                _rewardGoldNumTexts[i] = _rewardObj.transform.Find("Gold/Gold" + (i + 1) + "/Num").GetComponent<TextMeshProUGUI>();
            }

            _rewardGoldMultiple = _rewardObj.transform.Find("Gold/Multiple").gameObject;
            _rewardGoldBtnMultiple = _rewardObj.transform.Find("Gold/BtnMultiple").gameObject;
            _rewardGoldBtnMultiple.GetComponent<Button>().onClick.AddListener(OnBtnMultiple);
            _rewardGoldMultipleArrow = _rewardGoldMultiple.transform.Find("Arrow");
            _rewardGoldBtnMultipleNumText = _rewardGoldBtnMultiple.transform.Find("Num").GetComponent<TextMeshProUGUI>();
            _rewardGoldMultipleArrowAni = _rewardGoldMultiple.GetComponent<Animation>();

            Transform btnHome = _rewardObj.transform.Find("Button/BtnHome");
            Transform btnAgain = _rewardObj.transform.Find("Button/BtnAgain");
            btnHome.GetComponent<Button>().onClick.AddListener(OnBtnHome);
            btnAgain.GetComponent<Button>().onClick.AddListener(OnBtnAgain);
            if (DataHelper.CurUserInfoData.isNewUser == 0) btnAgain.gameObject.SetActive(false);
            nextBtnText1 = btnAgain.Find("Text1").gameObject;
            nextBtnText2 = btnAgain.Find("Text2").gameObject;
            
            _boxFullBoxImage = _boxFullObj.transform.Find("Box/Image").GetComponent<Image>();

            Transform btnDiscard = _boxFullObj.transform.Find("Button/BtnDiscard");
            Transform btnOpen = _boxFullObj.transform.Find("Button/BtnOpen");
            btnDiscard.GetComponent<Button>().onClick.AddListener(OnBtnDiscard);
            btnOpen.GetComponent<Button>().onClick.AddListener(OnBtnOpen);

            _boxFullBtnOpenNumText = btnOpen.Find("Frame/Gem").GetComponent<TextMeshProUGUI>();

            for (int i = 0; i < 5; i++)
            {
                _rankItemUis[i] = _rankObj_1.transform.Find("RankFrame/RankSub" + (i + 1)).gameObject;
                _rankItemMeUis[i] = _rankItemUis[i].transform.Find("Me").gameObject;
                _rankItemHeadUis[i] = _rankItemUis[i].transform.Find("Head/Mask/Image").GetComponent<Image>();
                _rankItemNameUis[i] = _rankItemUis[i].transform.Find("Name").GetComponent<Text>();
                _rankItemScoreUis[i] = _rankItemUis[i].transform.Find("Score").GetComponent<TextMeshProUGUI>();
                
                _rankItemUis_city[i] = _rankObj_2.transform.Find("RankFrame/RankSub" + (i + 1)).gameObject;
                _rankItemMeUis_city[i] = _rankItemUis_city[i].transform.Find("Me").gameObject;
                _rankItemNameUis_city[i] = _rankItemUis_city[i].transform.Find("Name").GetComponent<TextMeshProUGUI>();
                _rankItemScoreUis_city[i] = _rankItemUis_city[i].transform.Find("Score").GetComponent<TextMeshProUGUI>();
            }

            Transform rankMeObj = _rankObj_1.transform.Find("RankFrame/RankSubMe");
            _rankMeNumText = rankMeObj.Find("Num").GetComponent<TextMeshProUGUI>();
            _rankMeHead = rankMeObj.Find("Head/Mask/Image").GetComponent<Image>();
            _rankMeName = rankMeObj.Find("Name").GetComponent<Text>();
            _rankMeScoreText = rankMeObj.Find("Score").GetComponent<TextMeshProUGUI>();
            
            Transform rankMeObj_city = _rankObj_2.transform.Find("RankFrame/RankSubMe");
            _rankMeNumText_city = rankMeObj_city.Find("Num").GetComponent<TextMeshProUGUI>();
            _rankMeName_city = rankMeObj_city.Find("Name").GetComponent<TextMeshProUGUI>();
            _rankMeScoreText_city = rankMeObj_city.Find("Score").GetComponent<TextMeshProUGUI>();
            
            // 获取系统字体
            GameSdkManager._instance._sdkScript.GetSystemFont((font) =>
            {
                _rankMeName.font = font;
                for (int i = 0; i < _rankItemNameUis.Length; i++)
                {
                    _rankItemNameUis[i].font = font;
                }
            }, () => { });
        }

        /// <summary>
        /// 打开结算弹窗
        /// </summary>
        internal void OnOpenAccount()
        {
            // 上报自定义分析数据 事件: 通过关卡
            GameSdkManager._instance._sdkScript.ReportAnalytics("LevelComplete", "levelId", DataHelper.CurLevelNum);
            GameSdkManager._instance._sdkScript.LongVibrateControl();
            
            AudioHandler._instance.PlayAudio(BattleManager._instance.audioAccount);
            _rewardGoldMultGetOk = false;
            _curLevelNumAdd = false;

            _scoreDisNumText.text = BattleManager._instance._uiBattle.distanceNumText.text;
            _scoreNewRecord.SetActive(BattleManager._instance._uiBattle.disNewRecord.activeSelf);
            _scoreOrderValue.fillAmount = BattleManager._instance._uiBattle.infoOrderValue.fillAmount;
            _scoreOrderText_1.text = BattleManager._instance._uiBattle.cityNameNow.text;
            _scoreOrderText_2.text = BattleManager._instance._uiBattle.cityNameNew.text;
            if (BattleManager._instance.gameWin)
            {
                nextBtnText1.SetActive(false);
                nextBtnText2.SetActive(true);
                _isNextLevel = true;
            }
            else
            {
                nextBtnText1.SetActive(true);
                nextBtnText2.SetActive(false);
                _isNextLevel = false;
            }

            float curDistance = BattleManager._instance.scoreDistance + BattleManager._instance.endDis * (DataHelper.CurLevelNum - 1);

            List<string> modifyKeys = new List<string>();
            // 当前不是新手引导流程 正常保存数据
            if (DataHelper.CurUserInfoData.isNewUser != 0)
            {
                // 新记录(每日刷新的记录)
                if (curDistance > DataHelper.CurUserInfoData.scoreDistanceMax)
                {
                    DataHelper.CurUserInfoData.scoreDistanceMax = curDistance;
                    modifyKeys.Add("scoreDistanceMax");
                }
                // 新纪录(不会刷新的记录)
                if (curDistance > DataHelper.CurUserInfoData.distanceRecord)
                {
                    DataHelper.CurUserInfoData.distanceRecord = curDistance;
                    modifyKeys.Add("distanceRecord");
                }
                // 打卡地标
                bool isAddLandMark = false;
                for (int i = 0; i < BattleManager._instance.getLandmarks.Count; i++)
                {
                    int landMarkId = BattleManager._instance.getLandmarks[i];
                    if (!DataHelper.CurUserInfoData.landMarkInfo.ContainsKey(landMarkId))
                    {
                        DataHelper.CurUserInfoData.landMarkInfo.Add(landMarkId, 0);
                        if (!isAddLandMark) isAddLandMark = true;
                    }
                }
                if (isAddLandMark) modifyKeys.Add("landMarkInfo");
                
                // 有效飞行 起飞次数+1
                if (BattleManager._instance.scoreDistance > 0)
                {
                    DataHelper.CurUserInfoData.flyNum += 1;
                    modifyKeys.Add("flyNum");
                
                    // 完成日常任务 完成X次飞行 TaskID:1
                    DataHelper.CompleteDailyTask(1, 1, 0);
                    if (!modifyKeys.Contains("taskInfo1")) modifyKeys.Add("taskInfo1");
                    // 完成成就任务 累计完成X次飞行 TaskID:2
                    DataHelper.CompleteGloalTask(2, 1);
                    // 完成成就任务 飞行高度达到XM TaskID:13
                    DataHelper.CompleteGloalTask(13, BattleManager._instance.maxHeight);
                    // 完成成就任务 飞行速度达到XM/H TaskID:15
                    DataHelper.CompleteGloalTask(15, BattleManager._instance.maxSpeed);
                    // 完成成就任务 飞行距离达到XM TaskID:14
                    DataHelper.CompleteGloalTask(14, Mathf.FloorToInt(curDistance));
                    // 完成成就任务 累计完成X个地标打卡 TaskID:12
                    DataHelper.CompleteGloalTask(12, DataHelper.CurUserInfoData.landMarkInfo.Count);
                    if (!modifyKeys.Contains("taskInfo2")) modifyKeys.Add("taskInfo2");
                }
                // 宝箱获取累计飞行距离
                DataHelper.CurUserInfoData.boxGetFlyDis += Mathf.FloorToInt(BattleManager._instance.scoreDistance);
                if (!modifyKeys.Contains("boxGetFlyDis")) modifyKeys.Add("boxGetFlyDis");
                // 宝箱
                // 宝箱品质
                int boxGetLevelTmp = DataHelper.CurUserInfoData.boxGetLevel;
                if (boxGetLevelTmp >= 5) boxGetLevelTmp = 5;
                int[] curQualitys = GlobalValueManager.RewardBoxGetQualitys[boxGetLevelTmp - 1];
                int index = DataHelper.CurUserInfoData.boxGetNum % curQualitys.Length;
                _showLight = curQualitys[index] - 1;
                // 宝箱获取进度
                int targetNum = ConfigManager.Instance.ShopConfigDict[9999].Limit;
                int subNum = targetNum - DataHelper.CurUserInfoData.boxGetFlyDis;
                _showProgress = subNum <= 0 ? 0 : subNum;
                if (DataHelper.GmSwitch_GetBox) _showProgress = 0; // GM模式 每次飞行都获得宝箱
                if (_showProgress == 0)
                {
                    // 本次获得了宝箱
                    DataHelper.CurUserInfoData.boxGetFlyDis = 0;
                    if (!modifyKeys.Contains("boxGetFlyDis")) modifyKeys.Add("boxGetFlyDis");
                    DataHelper.CurUserInfoData.boxGetNum += 1;
                    if (!modifyKeys.Contains("boxGetNum")) modifyKeys.Add("boxGetNum");
                }

                // 通过关卡
                if (BattleManager._instance.gameWin)
                {
                    if (DataHelper.CurLevelNum == DataHelper.CurUserInfoData.curLevelNum)
                    {
                        // 当前选择的关卡就是当前最高关卡
                        if (DataHelper.CurUserInfoData.curLevelNum < 10)
                        {
                            // 未到达关卡上限 推进关卡
                            _curLevelNumAdd = true;
                            DataHelper.CurUserInfoData.curLevelNum += 1;
                            modifyKeys.Add("curLevelNum");
                            // 关卡推进 宝箱领取重置
                            if (_showProgress == 0)
                            {
                                DataHelper.CurUserInfoData.boxGetFlyDis = 0;
                                if (!modifyKeys.Contains("boxGetFlyDis")) modifyKeys.Add("boxGetFlyDis");
                                DataHelper.CurUserInfoData.boxGetNum = 0;
                                if (!modifyKeys.Contains("boxGetNum")) modifyKeys.Add("boxGetNum");
                                DataHelper.CurUserInfoData.boxGetLevel = DataHelper.CurUserInfoData.curLevelNum;
                                modifyKeys.Add("boxGetLevel");
                            }
                        }
                    }
                }
            }
            
            // 保存数据
            if (modifyKeys.Count > 0)
            {
                DataHelper.ModifyLocalData(modifyKeys, () =>
                {
                    // 本次不是新手引导
                    if (DataHelper.CurUserInfoData.isNewUser != 0)
                    {
                        // 产生新记录或飞行距离相同 上报全国飞行距离排行榜
                        // if (curDistance >= DataHelper.CurUserInfoData.scoreDistanceMax)
                        // {
                            GameSdkManager._instance._serverScript.UpdateRankAll(2);
                        // }

                        // 上报省份排行榜 ==> 本次是有效飞行(飞行距离大于0)
                        if (BattleManager._instance.scoreDistance > 0)
                        {
                            DataHelper.CurUpdataProvinceNum = Mathf.FloorToInt(BattleManager._instance.scoreDistance);
                            GameSdkManager._instance._serverScript.UpdateGeneralRank();
                        }
                    }
                });
            }
            
            _nextNum = 0;
            _ = AccountStep_1();
        }

        /// <summary>
        /// 结算步骤 1
        /// </summary>
        async UniTask AccountStep_1()
        {
            ctsWaitUniTask = new CancellationTokenSource();
            
            // 延迟10帧 设置排行榜数据
            float timeTmp0 = (10f / 60 * 1000);
            await UniTask.Delay((int)timeTmp0, cancellationToken: ctsWaitUniTask.Token);
            
            SetPersonalRank();
            SetCityRank();
        }

        /// <summary>
        /// 结算步骤 2
        /// </summary>
        async UniTask AccountStep_2()
        {
            _animation.Play("AccountOpenStep2");
            AudioHandler._instance.PlayAudio(BattleManager._instance.audioAccountNext);
            
            ctsWaitUniTask?.Cancel();
            ctsWaitUniTask?.Dispose();
            ctsWaitUniTask = new CancellationTokenSource();
            
            float timeTmp1 = (10f / 60 * 1000);
            await UniTask.Delay((int)timeTmp1, cancellationToken: ctsWaitUniTask.Token);
        }

        /// <summary>
        /// 结算步骤 3
        /// </summary>
        async UniTask AccountStep_3()
        {
            _animation.Play("AccountOpenStep3");
            
            ctsWaitUniTask?.Cancel();
            ctsWaitUniTask?.Dispose();
            ctsWaitUniTask = new CancellationTokenSource();

            if (GameGlobalManager._instance)
            {
                int imageId = 300 + _showLight;
                GameGlobalManager._instance.SetImage(_rewardBoxImage, new StringBuilder("IconImage" + imageId).ToString());
                GameGlobalManager._instance.SetImage(_boxFullBoxImage, new StringBuilder("IconImage" + imageId).ToString());
            }

            if (DataHelper.CurUserInfoData.isNewUser == 0)
            {
                // 新手引导流程
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioGetItem);
                _rewardBoxTipText.text = "恭喜获得宝箱!!!";
                _rewardBoxBar.fillAmount = 1;
                for (int i = 0; i < _rewardBoxLights.Length; i++)
                {
                    _rewardBoxLights[i].SetActive(i == 0);
                    _boxFullBoxLights[i].SetActive(i == 0);
                }

                _newBoxGet = -1;
            }
            else
            {
                // 正常游戏流程
                if (_showProgress == 0)
                {
                    AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioGetItem);
                    _rewardBoxTipText.text = "恭喜获得宝箱!!!";
                    _rewardBoxBar.fillAmount = 1;
                    for (int i = 0; i < _rewardBoxLights.Length; i++)
                    {
                        _rewardBoxLights[i].SetActive(i == _showLight);
                        _boxFullBoxLights[i].SetActive(i == _showLight);
                    }
                }
                else
                {
                    AudioHandler._instance.PlayAudio(BattleManager._instance.audioAccountNext);
                    _rewardBoxTipText.text = new StringBuilder("Fly more " + _showProgress + " M Can Get Reward").ToString();
                    _rewardBoxBar.fillAmount = 1 - ((float)_showProgress / ConfigManager.Instance.ShopConfigDict[9999].Limit);
                }

                _newBoxGet = -1;
                if (_showProgress == 0)
                {
                    int curLevelNum = DataHelper.CurUserInfoData.curLevelNum;
                    if (curLevelNum >= 5) curLevelNum = 5;
                    _newBoxGet = (_showLight + 1) * 100 + (curLevelNum - 1);
                }
            }

            _boxFullOpenGemNum = _newBoxGet == -1 ? 0 : ConfigManager.Instance.RewardBoxConfigDict[_newBoxGet].OpenGem;
            _boxFullBtnOpenNumText.text = _boxFullOpenGemNum.ToString();
            
            // 金币增幅
            int additionLv = DataHelper.CurUserInfoData.additions[4];       // 金币增幅等级
            int addNumTmp = GlobalValueManager.TrainBaseNums[3] + (additionLv - 1);
            
            int baseGoldNum = Mathf.FloorToInt(BattleManager._instance.scoreDistance);
            int pickGoldNum = Mathf.FloorToInt(BattleManager._instance.scoreGetGold);
            float additionValue = (100 + addNumTmp) / 100f;
            _goldNum = (baseGoldNum + pickGoldNum) * additionValue;
            _totalGoldNum = Mathf.FloorToInt(_goldNum);

            _rewardGoldNumTexts[0].text = new StringBuilder("+" + ToolFunManager.GetText(baseGoldNum, false)).ToString();
            _rewardGoldNumTexts[1].text = new StringBuilder("+" + ToolFunManager.GetText(pickGoldNum, false)).ToString();
            _rewardGoldNumTexts[2].text = new StringBuilder("X" + additionValue.ToString("0.0")).ToString();

            _ = RefreshBtnMult();
            
            // 完成日常任务 飞行中捡取X枚金币 TaskID:10
            DataHelper.CompleteDailyTask(10, pickGoldNum, 0);
            // 完成成就任务 飞行中累计捡取X枚金币 TaskID:19
            DataHelper.CompleteGloalTask(19, 1);
            // 完成成就任务 飞行中累计触发X次推进环 TaskID:20
            DataHelper.CompleteGloalTask(20, Mathf.FloorToInt(BattleManager._instance.scoreGetRing));
            
            float timeTmp1 = (10f / 60 * 1000);
            await UniTask.Delay((int)timeTmp1, cancellationToken: ctsWaitUniTask.Token);
        }

        /// <summary>
        /// 刷新超级加倍金币数量
        /// </summary>
        async UniTask RefreshBtnMult()
        {
            ctsWaitUniTask_goldMult?.Cancel();
            ctsWaitUniTask_goldMult?.Dispose();
            ctsWaitUniTask_goldMult = new CancellationTokenSource();

            while (!_rewardGoldMultGetOk)
            {
                int ratio = DataHelper.GetAccountMultRatio(_rewardGoldMultipleArrow.transform.localPosition.x);
                float numTmp = _goldNum * ratio;
                int num = Mathf.FloorToInt(numTmp);
                _rewardGoldBtnMultipleNumText.text = new StringBuilder("+" + (ToolFunManager.GetText(num, false))).ToString();

                await UniTask.Delay(125, true, cancellationToken: ctsWaitUniTask_goldMult.Token);
            }
        }

        /// <summary>
        /// 设置个人排行榜
        /// </summary>
        private void SetPersonalRank()
        {
            GameSdkManager._instance._serverScript.GetRankAll(2, () =>
            {
                string myRankKey = GameSdkManager._instance._serverScript.GetRankAllJudgeKey();
                string rankData = DataHelper.RankAllSort(2);
                List<RankDisUserData> levelUserDatas = JsonConvert.DeserializeObject<List<RankDisUserData>>(rankData);

                int rankMeIndex = -1;
                string rankNumTmp = "---";
                string rankNameTmp = DataHelper.CurUserInfoData.userName;
                rankNameTmp = rankNameTmp == "" ? "未授权用户" : rankNameTmp;
                float rankScoreTmp = DataHelper.CurUserInfoData.scoreDistanceMax;
                string rankHeadTmp = "";
                for (int i = 0; i < levelUserDatas.Count; i++)
                {
                    if (levelUserDatas[i].openId == myRankKey)
                    {
                        rankMeIndex = i;
                        rankNumTmp = (i + 1).ToString();
                        rankNameTmp = levelUserDatas[i].nickName;
                        rankScoreTmp = levelUserDatas[i].distance;
                        rankHeadTmp = levelUserDatas[i].userAvatar;
                        break;
                    }
                }

                for (int i = 0; i < _rankItemUis.Length; i++)
                {
                    if (i < levelUserDatas.Count)
                    {
                        _rankItemUis[i].SetActive(true);
                        _rankItemMeUis[i].SetActive(i == rankMeIndex);
                        _rankItemNameUis[i].color = i == rankMeIndex ? Color.black : Color.white;
                        _rankItemNameUis[i].text = levelUserDatas[i].nickName;
                        _rankItemScoreUis[i].text = new StringBuilder(ToolFunManager.GetText(levelUserDatas[i].distance, true) + " M").ToString();
                        if (levelUserDatas[i].userAvatar != "")
                        {
                            int iTmp = i;
                            StartCoroutine(ServerGetData.GetRemoteImg(levelUserDatas[i].userAvatar, sprite => _rankItemHeadUis[iTmp].sprite = sprite));
                        }
                    }
                    else
                    {
                        _rankItemUis[i].SetActive(false);
                    }
                }

                _rankMeNumText.text = rankNumTmp;
                _rankMeName.text = ToolFunManager.LongStrDeal(rankNameTmp, 16, "...");
                _rankMeScoreText.text = new StringBuilder(ToolFunManager.GetText(rankScoreTmp, true) + " M").ToString();
                if (rankHeadTmp != "")
                {
                    StartCoroutine(ServerGetData.GetRemoteImg(rankHeadTmp, sprite => _rankMeHead.sprite = sprite));
                }
            });
        }

        /// <summary>
        /// 设置地区排行榜
        /// </summary>
        private void SetCityRank()
        {
            GameSdkManager._instance._serverScript.GetGeneralRank(() =>
            {
                for (int i = 0; i < DataHelper.GeneralRanks.Count; i++)
                {
                    KeyValuePair<string, int> rankTmp = DataHelper.GeneralRanks[i];
                    if (DataHelper.ProvinceRanks.ContainsKey(rankTmp.Key))
                    {
                        DataHelper.ProvinceRanks[rankTmp.Key] = rankTmp.Value;
                    }
                }
                
                List<string> provinceSort = DataHelper.GetProvinceSort();
                List<string> provinceNames = JsonConvert.DeserializeObject<List<string>>(provinceSort[0]);
                List<int> provinceNums = JsonConvert.DeserializeObject<List<int>>(provinceSort[1]);
                
                string provinceName = DataHelper.CurUserInfoData.userProvince;
                int rankMeIndex = -1;
                if (provinceName != "")
                {
                    for (int i = 0; i < provinceNames.Count; i++)
                    {
                        if (provinceNames[i] == provinceName)
                        {
                            rankMeIndex = i;
                            break;
                        }
                    }
                }

                for (int i = 0; i < _rankItemUis_city.Length; i++)
                {
                    _rankItemMeUis_city[i].SetActive(i == rankMeIndex);
                    _rankItemNameUis_city[i].color = i == rankMeIndex ? Color.black : Color.white;
                    _rankItemNameUis_city[i].text = provinceNames[i];
                    _rankItemScoreUis_city[i].text = new StringBuilder(ToolFunManager.GetText(provinceNums[i], true) + " M").ToString();
                }

                _rankMeNumText_city.text = rankMeIndex == -1 ? "---" : (rankMeIndex + 1).ToString();
                _rankMeName_city.text = rankMeIndex == -1 ? "UnKnown" : provinceNames[rankMeIndex];
                _rankMeScoreText_city.text = rankMeIndex == -1
                    ? "0 M"
                    : new StringBuilder(ToolFunManager.GetText(provinceNums[rankMeIndex], true) + " M").ToString();

                _scoreCityNameText.text = _rankMeName_city.text;
                _scoreCityDisNumText.text = _rankMeScoreText_city.text;
            });
        }

        // ---------------------------------------------------------- 按钮 ----------------------------------------------------------
        /// <summary>
        /// 按钮 点击继续
        /// </summary>
        private void OnBtnNext()
        {
            if (_nextNum == 0)
            {
                _nextNum = 1;
                // _ = AccountStep_2();
                _ = AccountStep_3();
            }
            // else
            // {
            //     _nextNum = 2;
            //     _ = AccountStep_3();
            // }
        }

        /// <summary>
        /// 按钮 金币超级加倍
        /// </summary>
        private void OnBtnMultiple()
        {
            _rewardGoldMultipleArrowAni["Multiple"].speed = 0;
            int ratio = DataHelper.GetAccountMultRatio(_rewardGoldMultipleArrow.transform.localPosition.x);
            _rewardGoldMultGetOk = true;
            
            void successCb()
            {
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioGetItem);
                _goldNum *= ratio;
                _totalGoldNum = Mathf.FloorToInt(_goldNum);
                _rewardGoldNumTexts[3].text = new StringBuilder("X" + ratio).ToString();
                _rewardGoldNumTexts[4].text = new StringBuilder("+"+(ToolFunManager.GetText(_totalGoldNum, false))).ToString();

                _animation.Play("AccountOpenStep4");
                // Debug.Log("************ = " + _rewardGoldMultipleArrow.transform.localPosition.x);
                // Debug.Log("************ = " + ratio);
            }

            if (GameSdkManager._instance)
            {
                DataHelper.CurReportDf_adScene = "AccountGoldMultiple";
                GameSdkManager._instance._sdkScript.VideoControl("结算金币加倍", () =>
                {
                    successCb();
                    // 完成日常任务 观看X次广告 TaskID:2
                    DataHelper.CompleteDailyTask(2, 1, 0);
                    // 完成成就任务 累计观看X次广告 TaskID:3
                    DataHelper.CompleteGloalTask(3, 1);

                    // 上报自定义分析数据 事件: 金币加倍
                    GameSdkManager._instance._sdkScript.ReportAnalytics("MoreCoins", "levelId", DataHelper.CurLevelNum);
                }, () =>
                {
                    _rewardGoldMultipleArrowAni["Multiple"].speed = 1;
                    _rewardGoldMultGetOk = false;
                    _ = RefreshBtnMult();
                });
            }
            else
            {
                successCb();
            }
        }

        /// <summary>
        /// 按钮 回主界面
        /// </summary>
        private void OnBtnHome()
        {
            DataHelper.nextSceneName = "MainScene";
            DataHelper.CurUserInfoData.gold += _totalGoldNum;
            if (_newBoxGet != -1)
            {
                // 本局获得新宝箱
                int boxSlot = DataHelper.GetEmptyBoxSlot();
                if (boxSlot != -1)
                {
                    // 宝箱列表未满
                    List<string> modifyKeys = new List<string> { "gold", "boxsList", "taskInfo1", "taskInfo2" };
                    DataHelper.CurUserInfoData.boxList[boxSlot] = new[] { _newBoxGet.ToString(), "0" };
                    
                    // 本次结算推进了关卡 获得宝箱 更新宝箱获取所属关卡ID
                    if (_curLevelNumAdd)
                    {
                        if (DataHelper.CurUserInfoData != null && DataHelper.CurUserInfoData.boxGetLevel != DataHelper.CurUserInfoData.curLevelNum)
                        {
                            DataHelper.CurUserInfoData.boxGetLevel = DataHelper.CurUserInfoData.curLevelNum;
                            modifyKeys.Add("boxGetLevel");
                        }
                    }
                    
                    DataHelper.ModifyLocalData(modifyKeys, () => { });
                    
                    if (GameGlobalManager._instance)
                        GameGlobalManager._instance.LoadScene("MainScene");
                }
                else
                {
                    // 宝箱列表已满
                    _rewardObj.SetActive(false);
                    AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioError);
                    _boxFullObj.SetActive(true);
                }
                // 上报自定义分析数据 事件: 关卡获得宝箱
                GameSdkManager._instance._sdkScript.ReportAnalytics("GetBox", "levelId", DataHelper.CurLevelNum);
            }
            else
            {
                // 本局未获得宝箱 直接执行返回主界面
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                DataHelper.ModifyLocalData(new List<string>(3) { "gold", "taskInfo1", "taskInfo2" }, () => { });
                if (GameGlobalManager._instance)
                    GameGlobalManager._instance.LoadScene("MainScene");
            }
        }

        /// <summary>
        /// 按钮 再飞一次
        /// </summary>
        private void OnBtnAgain()
        {
            if (_isNextLevel)
            {
                // 本次通过关卡 进入下一关卡
                DataHelper.CurLevelNum += 1;
                if (DataHelper.CurLevelNum >= 10) DataHelper.CurLevelNum = 10;
            }
            DataHelper.nextSceneName = "BattleScene";
            DataHelper.CurUserInfoData.gold += _totalGoldNum;
            if (_newBoxGet != -1)
            {
                // 本局获得新宝箱
                int boxSlot = DataHelper.GetEmptyBoxSlot();
                if (boxSlot != -1)
                {
                    // 宝箱列表未满
                    List<string> modifyKeys = new List<string> { "gold", "boxsList", "taskInfo1", "taskInfo2" };
                    DataHelper.CurUserInfoData.boxList[boxSlot] = new[] { _newBoxGet.ToString(), "0" };
                    
                    // 本次结算推进了关卡 获得宝箱 更新宝箱获取所属关卡ID
                    if (_curLevelNumAdd)
                    {
                        if (DataHelper.CurUserInfoData != null && DataHelper.CurUserInfoData.boxGetLevel != DataHelper.CurUserInfoData.curLevelNum)
                        {
                            DataHelper.CurUserInfoData.boxGetLevel = DataHelper.CurUserInfoData.curLevelNum;
                            modifyKeys.Add("boxGetLevel");
                        }
                    }

                    DataHelper.ModifyLocalData(modifyKeys, () => { });
                    
                    if (GameGlobalManager._instance)
                        GameGlobalManager._instance.LoadScene("BattleScene");
                }
                else
                {
                    // 宝箱列表已满
                    _rewardObj.SetActive(false);
                    AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioError);
                    _boxFullObj.SetActive(true);
                }
                // 上报自定义分析数据 事件: 关卡获得宝箱
                GameSdkManager._instance._sdkScript.ReportAnalytics("GetBox", "levelId", DataHelper.CurLevelNum);
            }
            else
            {
                // 本局未获得宝箱 直接执行再飞一次
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                DataHelper.ModifyLocalData(new List<string>(1) { "gold", "taskInfo1", "taskInfo2" }, () => { });
                if (GameGlobalManager._instance)
                    GameGlobalManager._instance.LoadScene("BattleScene");
            }
        }

        /// <summary>
        /// 按钮 丢弃
        /// </summary>
        private void OnBtnDiscard()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            List<string> modifyKeys = new List<string> { "gold", "taskInfo1", "taskInfo2" };
            
            // 本次结算推进了关卡 获得宝箱 更新宝箱获取所属关卡ID
            if (_curLevelNumAdd)
            {
                if (DataHelper.CurUserInfoData != null && DataHelper.CurUserInfoData.boxGetLevel != DataHelper.CurUserInfoData.curLevelNum)
                {
                    DataHelper.CurUserInfoData.boxGetLevel = DataHelper.CurUserInfoData.curLevelNum;
                    modifyKeys.Add("boxGetLevel");
                }
            }
            
            DataHelper.ModifyLocalData(modifyKeys, () => { });
            if (GameGlobalManager._instance)
                GameGlobalManager._instance.LoadScene(DataHelper.nextSceneName);
        }

        /// <summary>
        /// 按钮 立即打开
        /// </summary>
        private void OnBtnOpen()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            if (DataHelper.CurUserInfoData.diamond < _boxFullOpenGemNum)
            {
                GameGlobalManager._instance.OpenNoMoney(true, 2);
                return;
            }

            List<string> modifyKeys = new List<string> { "gold", "diamond", "taskInfo1", "taskInfo2" };
            // 消耗钻石
            DataHelper.CurUserInfoData.diamond -= _boxFullOpenGemNum;
            
            // 完成日常任务 打开X个部件宝箱 TaskID:4
            DataHelper.CompleteDailyTask(4, 1, 0);
            // 完成成就任务 累计打开X个部件宝箱 TaskID:5
            DataHelper.CompleteGloalTask(5, 1);

            // 本次结算推进了关卡 获得宝箱 更新宝箱获取所属关卡ID
            if (_curLevelNumAdd)
            {
                if (DataHelper.CurUserInfoData != null && DataHelper.CurUserInfoData.boxGetLevel != DataHelper.CurUserInfoData.curLevelNum)
                {
                    DataHelper.CurUserInfoData.boxGetLevel = DataHelper.CurUserInfoData.curLevelNum;
                    modifyKeys.Add("boxGetLevel");
                }
            }
            
            // 保存数据
            DataHelper.ModifyLocalData(modifyKeys, () => { });
            
            // 打开宝箱
            GameGlobalManager._instance.OpenBox(_newBoxGet);
        }

        /// <summary>
        /// 立即打开宝箱执行完成
        /// </summary>
        private void BoxOpenComplete()
        {
            if (GameGlobalManager._instance)
                GameGlobalManager._instance.LoadScene(DataHelper.nextSceneName);
        }
    }
}