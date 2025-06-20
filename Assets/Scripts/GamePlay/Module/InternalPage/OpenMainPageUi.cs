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
using GamePlay.Main;
using Newtonsoft.Json;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage
{
    public class OpenMainPageUi : InternalPageScript
    {
        /** 我的排名 */
        private TextMeshProUGUI _myRankText;
        /** 我的地区排名 */
        private TextMeshProUGUI _myCityRankText;
        
        /** 邀请按钮 */
        private GameObject _btnCall;
        /** 邀请按钮提示红点 */
        private GameObject _btnCallRedPoint;
        
        /** 收藏奖励按钮 */
        private GameObject _btnFollow;
        /** 收藏奖励提示红点 */
        private GameObject _btnFollowRedPoint;

        /** 金币 */
        private TextMeshProUGUI _goldNumText;
        /** 钻石 */
        private TextMeshProUGUI _diamondNumText;

        /** 签到按钮/提示红点 */
        private GameObject _btnSign, _btnSignRedPoint;
        /** 转盘按钮/提示红点 */
        private GameObject _btnRaffle, _btnRaffleRedPoint;
        /** 任务按钮/提示红点 */
        private GameObject _btnTask, _btnTaskRedPoint;
        /** 地标打卡按钮/提示红点 */
        private GameObject _btnClockIn, _btnClockInRedPoint;
        /** 订阅按钮/提示红点 */
        private GameObject _btnMassage, _btnMassageRedPoint;
        /** 游戏圈按钮 */
        private GameObject _btnGameClub;

        /** UniTask异步信标 */
        private readonly CancellationTokenSource[] _cancellationToken = new CancellationTokenSource[5];

        /** 最远飞行距离 */
        private TextMeshProUGUI _distanceNumText;

        /** 宝箱获取进度条 */
        private Image _rewardBoxBar;
        /** 宝箱获取进度值 */
        private TextMeshProUGUI _rewardBoxValueText;
        /** 下一个获取宝箱图标 */
        private readonly GameObject[] _rewardBoxIcons = new GameObject[3];

        /** 城市选择图片 */
        private Image _mapImage;
        /** 城市选择图片动画 */
        private Animation _mapImageAni;
        /** 城市选择按钮可用状态 左 */
        private GameObject _btnMapOnL;
        /** 城市选择按钮可用状态 右 */
        private GameObject _btnMapOnR;
        /** 城市选择按钮不可用状态 左 */
        private GameObject _btnMapOffL;
        /** 城市选择按钮不可用状态 右 */
        private GameObject _btnMapOffR;

        /** 宝箱挂载节点 */
        private RectTransform _mainBoxFrameRect;
        /** 宝箱 信息 */
        private readonly GameObject[] _mainBoxTouchUis = new GameObject[5];
        /** 宝箱 点击解锁 */
        private readonly GameObject[] _mainBoxTouchUnlockUis = new GameObject[5];
        /** 宝箱 点击打开 */
        private readonly GameObject[] _mainBoxTouchOpenUis = new GameObject[5];
        /** 宝箱 提前打开 */
        private readonly GameObject[] _mainBoxTouchAdvanceUis = new GameObject[5];
        /** 宝箱 剩余时间 */
        private readonly GameObject[] _mainBoxTouchTimeUis = new GameObject[5];
        /** 宝箱 剩余时间 */
        private readonly TextMeshProUGUI[] _mainBoxTouchTimeTexts = new TextMeshProUGUI[5];

        /** 宝箱ID */
        private readonly int[] _boxIds = new int[5];
        /** 倒计时 */
        private readonly int[] _timeNum = new int[5];
        /** 宝箱可点击状态列表 */
        private readonly bool[] _boxTouchs = new bool[5];

        private bool isLoading;
        
        private void OnEnable()
        {
            EventManager.Add(CustomEventType.RefreshMoney, RefreshMoney);
            EventManager.Add(CustomEventType.BoxOpenDone, RefreshBoxTouch);
            EventManager.Add(CustomEventType.RefreshBtnFollow, RefreshBtnFollow);
            EventManager.Add(CustomEventType.RefreshBtnCall, RefreshBtnCall);
            EventManager.Add(CustomEventType.RefreshBtnFeedSub, RefreshBtnMassage);
            EventManager<int>.Add(CustomEventType.GuideComplete, GuideComplete);
            EventManager<int>.Add(CustomEventType.RefreshRedPoint, RefreshRedPoint);
            EventManager.Add(CustomEventType.RefreshMainPageMap, RefreshMapSelect);
        }

        private void OnDisable()
        {
            EventManager.Remove(CustomEventType.RefreshMoney, RefreshMoney);
            EventManager.Remove(CustomEventType.BoxOpenDone, RefreshBoxTouch);
            EventManager.Remove(CustomEventType.RefreshBtnFollow, RefreshBtnFollow);
            EventManager.Remove(CustomEventType.RefreshBtnCall, RefreshBtnCall);
            EventManager.Remove(CustomEventType.RefreshBtnFeedSub, RefreshBtnMassage);
            EventManager<int>.Remove(CustomEventType.GuideComplete, GuideComplete);
            EventManager<int>.Remove(CustomEventType.RefreshRedPoint, RefreshRedPoint);
            EventManager.Remove(CustomEventType.RefreshMainPageMap, RefreshMapSelect);
            for (int i = 0; i < 5; i++)
            {
                _cancellationToken[i]?.Cancel();
                _cancellationToken[i]?.Dispose();
                _cancellationToken[i] = null;
            }
        }

        public override void Initial()
        {
            base.Initial();

            _goldNumText = transform.Find("Info/Money/Gold/Num").GetComponent<TextMeshProUGUI>();
            _diamondNumText = transform.Find("Info/Money/Gem/Num").GetComponent<TextMeshProUGUI>();
            transform.Find("Info/BtnSet").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                MainManager._instance.OnOpenPop_Set(true);
            });

            Transform btnStart = transform.Find("BtnStart");
            btnStart.GetComponent<Button>().onClick.AddListener(OnBtnStart);

            Transform mainBoxTouchTran = MainManager._instance.mainBoxCam.transform.parent.Find("CanvasUi3D_MainBox");
            
            _distanceNumText = btnStart.Find("Score/Num").GetComponent<TextMeshProUGUI>();

            _rewardBoxBar = btnStart.Find("Reward/Progress/Value").GetComponent<Image>();
            _rewardBoxValueText = btnStart.Find("Reward/Progress/Num").GetComponent<TextMeshProUGUI>();
            _rewardBoxIcons[0] = btnStart.Find("Reward/RewardIcon1").gameObject;
            _rewardBoxIcons[1] = btnStart.Find("Reward/RewardIcon2").gameObject;
            _rewardBoxIcons[2] = btnStart.Find("Reward/RewardIcon3").gameObject;
            
            for (int i = 0; i < 5; i++)
            {
                Transform mainBoxTouch = mainBoxTouchTran.Find("BoxInfo" + (i + 1));
                _mainBoxTouchUis[i] = mainBoxTouch.gameObject;
                _mainBoxTouchUnlockUis[i] = mainBoxTouch.Find("Unlock").gameObject;
                _mainBoxTouchOpenUis[i] = mainBoxTouch.Find("Opne").gameObject;
                _mainBoxTouchAdvanceUis[i] = mainBoxTouch.Find("AdvanceOpne").gameObject;
                _mainBoxTouchTimeUis[i] = mainBoxTouch.Find("Time").gameObject;
                _mainBoxTouchTimeTexts[i] = mainBoxTouch.Find("Time/Num").GetComponent<TextMeshProUGUI>();
            }

            _mainBoxFrameRect = mainBoxTouchTran.GetComponent<RectTransform>();

            Transform boxTouchTran = transform.Find("MainBox/Frame");
            string[] paths = { "BoxTouchsB/BoxTouch", "BoxTouchsB/BoxTouch", "BoxTouchsB/BoxTouch", "BoxTouchsA/BoxTouch", "BoxTouchsA/BoxTouch" };
            for (int i = 0; i < paths.Length; i++)
            {
                Transform mainBoxTouchButton = boxTouchTran.Find(paths[i] + (i + 1));
                int index = i;
                mainBoxTouchButton.GetComponent<Button>().onClick.AddListener(() => { OnBtnBoxTouch(index); });
            }
            
            _myRankText = transform.Find("MyRank/Rank2").GetComponent<TextMeshProUGUI>();
            _myCityRankText = transform.Find("MyRank/Rank1").GetComponent<TextMeshProUGUI>();

            _btnFollow = transform.Find("BtnRs/BtnFollow").gameObject;
            _btnFollowRedPoint = _btnFollow.transform.Find("RedPoint").gameObject;
            _btnFollow.SetActive(false);
            
            _btnCall = transform.Find("BtnRs/BtnCall").gameObject;
            _btnCallRedPoint = _btnCall.transform.Find("RedPoint").gameObject;

            _btnSign = transform.Find("BtnLs/BtnSign").gameObject;
            _btnSignRedPoint = _btnSign.transform.Find("RedPoint").gameObject;
            _btnSign.GetComponent<Button>().onClick.AddListener(OnBtnSign);
            
            _btnRaffle = transform.Find("BtnRs/BtnRaffle").gameObject;
            _btnRaffleRedPoint = _btnRaffle.transform.Find("BtnRaffle/RedPoint").gameObject;
            _btnRaffle.GetComponent<Button>().onClick.AddListener(OnBtnRaffle);
            
            _btnTask = transform.Find("BtnRs/BtnTask").gameObject;
            _btnTaskRedPoint = _btnTask.transform.Find("RedPoint").gameObject;
            _btnTask.GetComponent<Button>().onClick.AddListener(OnBtnTask);
            
            _btnFollow.GetComponent<Button>().onClick.AddListener(OnBtnFollow);
            _btnCall.GetComponent<Button>().onClick.AddListener(OnBtnCall);

            _mapImage = btnStart.Find("Map/MapImage").GetComponent<Image>();
            _mapImageAni = _mapImage.gameObject.GetComponent<Animation>();
            
            Transform btnMapLeft = btnStart.Find("Map/BtnJianTouL");
            btnMapLeft.GetComponent<Button>().onClick.AddListener(OnBtnMapLeft);
            _btnMapOnL = btnMapLeft.Find("On").gameObject;
            _btnMapOffL = btnMapLeft.Find("Off").gameObject;

            Transform btnMapRight = btnStart.Find("Map/BtnJianTouR");
            btnMapRight.GetComponent<Button>().onClick.AddListener(OnBtnMapRight);
            _btnMapOnR = btnMapRight.Find("On").gameObject;
            _btnMapOffR = btnMapRight.Find("Off").gameObject;

            _btnClockIn = transform.Find("BtnLs/BtnClockIn").gameObject;
            _btnClockInRedPoint = _btnClockIn.transform.Find("RedPoint").gameObject;
            _btnClockIn.GetComponent<Button>().onClick.AddListener(OnBtnClockIn);

            _btnMassage = transform.Find("BtnRs/BtnMassage").gameObject;
            _btnMassageRedPoint = _btnMassage.transform.Find("RedPoint").gameObject;
            _btnMassage.GetComponent<Button>().onClick.AddListener(OnBtnMassage);

            _btnGameClub = transform.Find("BtnRs/BtnCircle").gameObject;
            _btnGameClub.SetActive(false);

            // 刷新宝箱位置点 游戏进行中不会切换屏幕分辨率只需要执行一次
            RefreshBoxTouchPoint();
        }

        /// <summary>
        /// 初始化宝箱
        /// </summary>
        private void InitialMainBoxTouch()
        {
            for (int i = 0; i < _mainBoxTouchUis.Length; i++)
            {
                _mainBoxTouchUis[i].gameObject.SetActive(false);
                _mainBoxTouchUnlockUis[i].SetActive(false);
                _mainBoxTouchOpenUis[i].SetActive(false);
                _mainBoxTouchAdvanceUis[i].SetActive(false);
                _mainBoxTouchTimeUis[i].SetActive(false);
                _boxTouchs[i] = false;
            }
        }

        public override void OpenInternalPage()
        {
            base.OpenInternalPage();

            // 打开主页面 默认选择当前最高关卡
            DataHelper.CurLevelNum = DataHelper.CurUserInfoData.curLevelNum;
            
            MainManager._instance.SetMainPlaneAni("PlaneMain");
            MainManager._instance.SetMainCamAniTrigger(-1);
            
            InitialMainBoxTouch();
            
            MainManager._instance.SetBoxCamActive(true);
            
            RefreshMoney();
            RefreshInfo();

            // 不是启动场景默认打开的主页面 是切换页签打开的主页面 刷新宝箱倒计时
            if (!MainManager._instance._startRefreshBox)
            {
                MainManager._instance.RefreshBox();
                AudioHandler._instance.PlayAudio(MainManager._instance.audioPageClose);
            }

            // 刷新收藏游戏(侧边栏)按钮
            RefreshBtnFollow();
            // 刷新邀请好友按钮
            RefreshBtnCall();
            // 刷新直玩订阅按钮
            RefreshBtnMassage();

            // 刷新提示红点
            RefreshRedPoint(2);

            // 刷新城市选择
            RefreshMapSelect();

            // 隐藏游戏圈入口按钮
            _btnGameClub.SetActive(false);
        }

        public override void CloseInternalPage()
        {
            MainManager._instance.SetBoxCamActive(false);
            base.CloseInternalPage();
        }

        /// <summary>
        /// 刷新货币
        /// </summary>
        private void RefreshMoney()
        {
            _goldNumText.text = ToolFunManager.GetText(DataHelper.CurUserInfoData.gold, false);
            _diamondNumText.text = ToolFunManager.GetText(DataHelper.CurUserInfoData.diamond, false);
        }

        /// <summary>
        /// 刷新收藏游戏(侧边栏)按钮
        /// </summary>
        private void RefreshBtnFollow()
        {
            _btnFollow.SetActive(false);
            _btnFollowRedPoint.SetActive(false);

            if (DataHelper.CurUserInfoData.addedToMyMiniProgramGet == 0)
            {
                // GameSdkManager._instance._sdkScript.CheckSideBarStartGame(_btnFollow);
            }
        }

        /// <summary>
        /// 刷新邀请好友按钮
        /// </summary>
        private void RefreshBtnCall()
        {
            _btnCall.SetActive(DataHelper.CurUserInfoData.callRewardGet == 0 || DataHelper.CurUserInfoData.callRewardGet == 1);
            _btnCallRedPoint.SetActive(DataHelper.CurUserInfoData.callRewardGet == 0 || DataHelper.CurUserInfoData.callRewardGet == 1);
        }

        /// <summary>
        /// 刷新直玩订阅按钮
        /// </summary>
        private void RefreshBtnMassage()
        {
            DataHelper.CurUserInfoData.feedSubGet = 2;
            switch (DataHelper.CurUserInfoData.feedSubGet)
            {
                case 0:
                    _btnMassage.SetActive(true);
                    _btnMassageRedPoint.SetActive(true);
                    // GameSdkManager._instance._sdkScript.CheckFeedSubscribeStatus(() => { _btnMassageRedPoint.SetActive(true); }, () => { });
                    break;
                case 1:
                    _btnMassage.SetActive(true);
                    _btnMassageRedPoint.SetActive(true);
                    break;
                case 2:
                    _btnMassage.SetActive(false);
                    _btnMassageRedPoint.SetActive(false);
                    break;
            }
        }

        /// <summary>
        /// 刷新信息
        /// </summary>
        private void RefreshInfo()
        {
            _distanceNumText.text = new StringBuilder("Most Fly Distance：" + ToolFunManager.GetText(DataHelper.CurUserInfoData.distanceRecord, true) + "M").ToString();

            ShopConfig shopConfig = ConfigManager.Instance.ShopConfigDict[9999];
            int targetNum = shopConfig.Limit;

            int subNum = targetNum - DataHelper.CurUserInfoData.boxGetFlyDis;
            _rewardBoxValueText.text = new StringBuilder("Fly More " + subNum + "M Can Get Rewards").ToString();
            _rewardBoxBar.fillAmount = 1 - ((float)subNum / targetNum);

            int boxGetLevelTmp = DataHelper.CurUserInfoData.boxGetLevel;
            if (boxGetLevelTmp >= 5) boxGetLevelTmp = 5;
            int[] curQualitys = GlobalValueManager.RewardBoxGetQualitys[boxGetLevelTmp - 1];
            int index = DataHelper.CurUserInfoData.boxGetNum % curQualitys.Length;
            for (int i = 0; i < _rewardBoxIcons.Length; i++)
            {
                _rewardBoxIcons[i].SetActive(curQualitys[index] - 1 == i);
            }

            RefreshRankCity();
            RefreshRankPersonal();

            SignInfoData signInfoData = JsonConvert.DeserializeObject<SignInfoData>(DataHelper.CurUserInfoData.signInfo);
            _btnSign.SetActive(signInfoData.day <= 7);
        }

        /// <summary>
        /// 刷新宝箱点位
        /// </summary>
        private void RefreshBoxTouchPoint()
        {
            // 获取宝箱的点位
            Vector3[] boxPoints = new Vector3[5];
            for (int i = 0; i < 5; i++)
            {
                boxPoints[i] = MainManager._instance.GetBoxPoint(i);
            }
            // 转换坐标系
            for (int i = 0; i < boxPoints.Length; i++)
            {
                // Vector2 screenPos = MainManager._instance.mainBoxCam.WorldToScreenPoint(boxPoints[i]);
                // RectTransformUtility.ScreenPointToLocalPointInRectangle(_mainBoxFrameRect, screenPos, MainManager._instance.mainBoxUi3DCam, out var localPositionTmp);
                // _mainBoxTouchUis[i].transform.localPosition  = localPositionTmp;
                _mainBoxTouchUis[i].transform.position = boxPoints[i];
            }
        }

        /// <summary>
        /// 刷新宝箱
        /// </summary>
        private void RefreshBoxTouch()
        {
            InitialMainBoxTouch();
            
            long[] boxTimes = new long[5];
            int[] countDownTimes = new int[5];
            int emptySlotNum = 0;
            foreach (KeyValuePair<int, string[]> boxData in DataHelper.CurUserInfoData.boxList)
            {
                if (boxData.Value == null)
                {
                    _boxIds[boxData.Key] = -1;
                    boxTimes[boxData.Key] = -1;
                    countDownTimes[boxData.Key] = -1;
                    emptySlotNum += 1;
                }
                else
                {
                    int boxId = int.Parse(boxData.Value[0]);
                    long boxTime = long.Parse(boxData.Value[1]);
                    _boxIds[boxData.Key] = boxId;
                    boxTimes[boxData.Key] = boxTime;
                    long boxTimeTmp = boxTime == 0 ? ToolFunManager.GetCurrTime() : boxTime;
                    long nextTime = boxTimeTmp + ConfigManager.Instance.RewardBoxConfigDict[boxId].OpenTime * 60;
                    int subTime = (int)(nextTime - ToolFunManager.GetCurrTime());
                    countDownTimes[boxData.Key] = subTime <= 0 ? 0 : subTime;
                }
            }

            // Debug.Log(JsonConvert.SerializeObject(_boxIds));
            // Debug.Log(JsonConvert.SerializeObject(boxTimes));
            // Debug.Log(JsonConvert.SerializeObject(countDownTimes));

            // 没有宝箱
            if (emptySlotNum >= 5) return;
            // 有宝箱
            // 找到倒计时结束的
            int clickOpenIndex = -1;
            for (int i = 0; i < countDownTimes.Length; i++)
            {
                if (countDownTimes[i] == 0)
                {
                    clickOpenIndex = i;
                    break;
                }
            }

            // 剩余倒计时时间最短但未启动倒计时
            int clickUnlockIndex = -1;
            int clickUnlockTimeTmp = 0;
            for (int i = 0; i < boxTimes.Length; i++)
            {
                if (boxTimes[i] == 0)
                {
                    if (clickOpenIndex == i) continue;
                    if (clickUnlockTimeTmp == 0)
                    {
                        clickUnlockTimeTmp = countDownTimes[i];
                        clickUnlockIndex = i;
                    }
                    else
                    {
                        if (countDownTimes[i] < clickUnlockTimeTmp)
                        {
                            clickUnlockTimeTmp = countDownTimes[i];
                            clickUnlockIndex = i;
                        }
                    }
                }
            }
            
            // 剩余倒计时时间最短且已启动倒计时
            int clickAdvanceIndex = -1;
            int clickAdvanceTimeTmp = 0;
            for (int i = 0; i < boxTimes.Length; i++)
            {
                if (boxTimes[i] != 0)
                {
                    if (clickOpenIndex == i) continue;
                    if (clickAdvanceTimeTmp == 0)
                    {
                        clickAdvanceTimeTmp = countDownTimes[i];
                        clickAdvanceIndex = i;
                    }
                    else
                    {
                        if (countDownTimes[i] < clickAdvanceTimeTmp)
                        {
                            clickAdvanceTimeTmp = countDownTimes[i];
                            clickAdvanceIndex = i;
                        }
                    }
                }
            }

            for (int i = 0; i < _mainBoxTouchUis.Length; i++)
            {
                if (boxTimes[i] != -1)
                {
                    // 当前槽位有宝箱
                    _mainBoxTouchUis[i].gameObject.SetActive(true);
                  
                    _boxTouchs[i] = true;
                    int minute = countDownTimes[i] / 60 % 60;
                    int second = countDownTimes[i] % 60;
                    _mainBoxTouchTimeTexts[i].text = $"{minute:D2}:{second:D2}";
                    _timeNum[i] = countDownTimes[i];
                    
                    _mainBoxTouchOpenUis[i].SetActive(i == clickOpenIndex);
                    _mainBoxTouchUnlockUis[i].SetActive(i == clickUnlockIndex);
                    _mainBoxTouchAdvanceUis[i].SetActive(i == clickAdvanceIndex);
                    if (boxTimes[i] != 0)
                    {
                        // 当前槽位宝箱已启动解锁
                        if (_timeNum[i] > 0)
                        {
                            // 当前槽位宝箱解锁倒计时未结束 设置并启动倒计时
                            _mainBoxTouchTimeUis[i].SetActive(true);
                            _ = RefreshTime(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刷新倒计时
        /// </summary>
        async UniTask RefreshTime(int index)
        {
            _cancellationToken[index]?.Cancel();
            _cancellationToken[index]?.Dispose();
            _cancellationToken[index] = new CancellationTokenSource();
            while (_timeNum[index] > 0)
            {
                int minute = _timeNum[index] / 60 % 60;
                int second = _timeNum[index] % 60;
                _mainBoxTouchTimeTexts[index].text = $"{minute:D2}:{second:D2}";
                _timeNum[index] -= 1;
                
                await UniTask.Delay(1000, true, cancellationToken: _cancellationToken[index].Token);
            }

            _timeNum[index] = 0;
            _mainBoxTouchTimeTexts[index].text = "";
            MainManager._instance.RefreshBox();
        }

        /// <summary>
        /// 刷新提示红点
        /// </summary>
        /// <param name="index">提示红点编号</param>
        private void RefreshRedPoint(int index)
        {
            _btnSignRedPoint.SetActive(MainManager._instance._redPointManager.GetRedPoint_Sign());
            _btnRaffleRedPoint.SetActive(MainManager._instance._redPointManager.GetRedPoint_Raffle());
            _btnTaskRedPoint.SetActive(MainManager._instance._redPointManager.GetRedPoint_Task());
            _btnClockInRedPoint.SetActive(MainManager._instance._redPointManager.GetRedPoint_LandMark());
        }

        /// <summary>
        /// 引导结束
        /// </summary>
        /// <param name="type">引导结束触发操作类型</param>
        private void GuideComplete(int type)
        {
            switch (type)
            {
                case 0:
                    OnBtnStart();
                    break;
            }
        }

        /// <summary>
        /// 刷新城市选择
        /// </summary>
        private void RefreshMapSelect()
        {
            string mapName = new StringBuilder("MapImage" + DataHelper.CurLevelNum).ToString();
            GameGlobalManager._instance.SetImage(_mapImage, mapName);
            _btnMapOffL.SetActive(DataHelper.CurLevelNum == 1);
            _btnMapOnL.SetActive(DataHelper.CurLevelNum != 1);
            _btnMapOffR.SetActive(DataHelper.CurLevelNum == 10 || DataHelper.CurLevelNum >= DataHelper.CurUserInfoData.curLevelNum);
            _btnMapOnR.SetActive(DataHelper.CurLevelNum != 10 && DataHelper.CurLevelNum < DataHelper.CurUserInfoData.curLevelNum);
        }

        // ---------------------------------------------- 排行榜 ----------------------------------------------
        /// <summary>
        /// 刷新地区排行榜
        /// </summary>
        private void RefreshRankCity()
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

                List<string> rankProvinceNames = new List<string>(DataHelper.ProvinceRanks.Count);
                for (int index = 0; index < provinceNames.Count; index++)
                {
                    rankProvinceNames.Add(provinceNames[index]);
                }

                if (DataHelper.CurUserInfoData.userProvince != "")
                {
                    int rankNum = rankProvinceNames.IndexOf(DataHelper.CurUserInfoData.userProvince) + 1;
                    _myCityRankText.text = new StringBuilder("My regional rankings：Rank " + rankNum).ToString();
                }
                else
                {
                    _myCityRankText.text = "Regional information is not yet available";
                }
            });
        }

        /// <summary>
        /// 刷新个人排行榜
        /// </summary>
        private void RefreshRankPersonal()
        {
            GameSdkManager._instance._serverScript.GetRankAll(2, () =>
            {
                string myRankKey = GameSdkManager._instance._serverScript.GetRankAllJudgeKey();
                string rankData = DataHelper.RankAllSort(2);
                List<RankDisUserData> disUserDatas = JsonConvert.DeserializeObject<List<RankDisUserData>>(rankData);
                
                int rankMeNum = -1;
                for (int i = 0; i < disUserDatas.Count; i++)
                {
                    if (disUserDatas[i].openId == myRankKey)
                    {
                        rankMeNum = (i + 1);
                    }
                }

                _myRankText.text = rankMeNum == -1
                    ? "My Global Ranking: Not on the list"
                    : new StringBuilder("My Global Ranking: Rank " + rankMeNum).ToString();
            });
        }

        // ----------------------------------------------- 按钮 -----------------------------------------------
        /// <summary>
        /// 按钮 收藏
        /// </summary>
        private void OnBtnFollow()
        {
            MainManager._instance.OnOpenPop_Follow(true);
        }
        
        /// <summary>
        /// 按钮 邀请
        /// </summary>
        private void OnBtnCall()
        {
            MainManager._instance.OnOpenPop_Call(true);
        }

        /// <summary>
        /// 按钮 开始
        /// </summary>
        private void OnBtnStart()
        {
            if(isLoading) return;
            isLoading = true;
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            if (GameGlobalManager._instance)
                GameGlobalManager._instance.LoadScene("BattleScene");
        }

        /// <summary>
        /// 按钮 点击宝箱
        /// </summary>
        /// <param name="index"></param>
        private void OnBtnBoxTouch(int index)
        {
            if (!_boxTouchs[index]) return;
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            
            if (_timeNum[index] > 0)
            {
                DataHelper.CurOpenBoxInfo = new[] { index, _boxIds[index] };
                MainManager._instance.OnOpenPop_OpenBox(true);
            }
            else
            {
                // 消耗宝箱
                DataHelper.CurUserInfoData.boxList[index] = null;
                // 打开宝箱
                GameGlobalManager._instance.OpenBox(_boxIds[index]);
                // 完成日常任务 打开X个部件宝箱 TaskID:4
                DataHelper.CompleteDailyTask(4, 1, 0);
                // 完成成就任务 累计打开X个部件宝箱 TaskID:5
                DataHelper.CompleteGloalTask(5, 1);
                
                // 保存数据
                DataHelper.ModifyLocalData(new List<string>(3) { "boxsList", "taskInfo1", "taskInfo2" }, () => { });
                
                // 刷新宝箱
                MainManager._instance.RefreshBox();
                
                // 上报自定义分析数据 事件: 正常打开宝箱
                GameSdkManager._instance._sdkScript.ReportAnalytics("OpenBox_Common", "boxId", _boxIds[index]);
            }
        }

        /// <summary>
        /// 按钮 签到
        /// </summary>
        private void OnBtnSign()
        {
            MainManager._instance.OnOpenPop_Sign(true);
        }

        /// <summary>
        /// 按钮 转盘
        /// </summary>
        private void OnBtnRaffle()
        {
            MainManager._instance.OnOpenPop_Raffle(true);
        }

        /// <summary>
        /// 按钮 任务
        /// </summary>
        private void OnBtnTask()
        {
            MainManager._instance.OnOpenPop_Task(true);
        }

        /// <summary>
        /// 按钮 城市选择 左
        /// </summary>
        private void OnBtnMapLeft()
        {
            if (DataHelper.CurLevelNum == 1)
            {
                GameGlobalManager._instance.ShowTips("Is First City");
                return;
            }

            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            DataHelper.CurLevelNum -= 1;
            RefreshMapSelect();
            _mapImageAni.Play("ChangeMapL");
        }

        /// <summary>
        /// 按钮 城市选择 右
        /// </summary>
        private void OnBtnMapRight()
        {
            if (DataHelper.CurLevelNum == 10)
            {
                GameGlobalManager._instance.ShowTips("Comming Soon");
                return;
            }
            
            if (DataHelper.CurLevelNum >= DataHelper.CurUserInfoData.curLevelNum)
            {
                GameGlobalManager._instance.ShowTips("Need Pass Current City");
                return;
            }

            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            DataHelper.CurLevelNum += 1;
            RefreshMapSelect();
            _mapImageAni.Play("ChangeMapR");
        }

        /// <summary>
        /// 按钮 地标打卡
        /// </summary>
        private void OnBtnClockIn()
        {
            MainManager._instance.OnOpenPop_ClockIn(true);
        }

        /// <summary>
        /// 按钮 订阅
        /// </summary>
        private void OnBtnMassage()
        {
            MainManager._instance.OnOpenPop_Massage(true);
        }
    }
}