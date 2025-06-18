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
using GamePlay.Globa;
using GamePlay.Main;
using Newtonsoft.Json;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GamePlay.Module.Round.Raffle
{
    public class OpenRafflePageUi : MonoBehaviour
    {
        /** 转盘动画组件 */
        private Animator _raffleAni;
        /** 抽奖按钮 抽奖 */
        private GameObject _btnRaffleText_1;
        /** 抽奖按钮 提前抽奖 */
        private GameObject _btnRaffleText_2;
        /** 抽奖按钮 提示红点 */
        private GameObject _btnRaffleRedPoint;
        /** 抽奖按钮 免费抽奖倒计时 */
        private TextMeshProUGUI _btnRaffleTimeText;

        /** 抽奖按钮 */
        private GameObject _btnStartRaffle;
        /** 关闭抽奖弹窗按钮 */
        private GameObject _btnClose;
        /** 关闭抽奖弹窗按钮(弹窗底层遮罩) */
        private Button _btnClose_Mask;

        /** 每日活动 */
        private GameObject _infoObj;
        /** 每日活动说明 */
        private TextMeshProUGUI _infoText;
        /** 每日活动倒计时 */
        private TextMeshProUGUI _infoTimeText;

        /** 奖励权重(正常流程下) */
        private readonly int[] _raffleData_Common = { 1, 500, 100, 300, 2, 200, 3, 50 };
        /** 奖励权重(每日活动下) */
        private readonly int[] _raffleData_Info = { 0, 500, 100, 300, 0, 200, 0, 50 };
        /** 奖励ID */
        private readonly int[] _rewardIds = { 302, 102, 103, 101, 301, 101, 300, 200 };
        /** 奖励数量 */
        private readonly int[] _rewardNums = { 1, 1000, 5000, 2000, 1, 3000, 1, 100 };
        /** 每日活动限时 300分钟 */
        private readonly int _infoLimitTime = 300;

        /** 奖励权重 */
        private readonly int[] _raffleData = new int[8];
        
        /** 是否开始抽奖转动 */
        private bool _isStartRotate;
        /** 本次中奖索引 */
        private int _rewardIndex;

        /** 动画索引 */
        private static readonly int _RewardId = Animator.StringToHash("RewardId");
        
        /** 动画播放完成 */
        private bool _aniPlayComplete;
        
        /** 本次抽奖是否免费 */
        private bool _isFree;

        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;
        /** UniTask异步信标 每日活动 */
        private CancellationTokenSource _cancellationToken_Info;
        
        /** 刷新倒计时 */
        private int _timeNum;
        /** 刷新倒计时 每日活动 */
        private int _timeNum_Info;

        private void Update()
        {
            if (!_aniPlayComplete)
            {
                var aniInfo = _raffleAni.GetCurrentAnimatorStateInfo(0);
                var aniPlayTime = aniInfo.normalizedTime;
                if (aniPlayTime >= 1f && aniInfo.IsName(new StringBuilder("ArrowEnd" + (_rewardIndex + 1)).ToString()))
                {
                    _aniPlayComplete = true;
                    ConfigManager.Instance.ConsoleLog(0, new StringBuilder("抽奖结束 索引: " + _rewardIndex).ToString());
                    _isStartRotate = false;
                    GetReward();
                }
            }
        }

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
            _cancellationToken_Info?.Cancel();
            _cancellationToken_Info?.Dispose();
            _cancellationToken_Info = null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _btnClose_Mask = transform.Find("Mask").GetComponent<Button>();
            _btnClose_Mask.onClick.AddListener(OnBtnClose);
            _btnClose = transform.Find("Frame/Close").gameObject;
            _btnClose.GetComponent<Button>().onClick.AddListener(OnBtnClose);

            _raffleAni = transform.Find("Raffle/Arrow").GetComponent<Animator>();
            _btnStartRaffle = transform.Find("Btn").gameObject;
            _btnRaffleText_1 = _btnStartRaffle.transform.Find("Text1").gameObject;
            _btnRaffleText_2 = _btnStartRaffle.transform.Find("Text2").gameObject;
            _btnRaffleTimeText = _btnRaffleText_2.transform.Find("Time/Num").GetComponent<TextMeshProUGUI>();
            _btnRaffleRedPoint = _btnStartRaffle.transform.Find("RedPoint").gameObject;

            _infoObj = transform.Find("InfoDoc").gameObject;
            _infoText = _infoObj.transform.Find("Doc").GetComponent<TextMeshProUGUI>();
            _infoTimeText = _infoObj.transform.Find("Tittle/Time").GetComponent<TextMeshProUGUI>();

            _btnStartRaffle.GetComponent<Button>().onClick.AddListener(OnBtnRaffle);
        }

        /// <summary>
        /// 打开弹窗 转盘
        /// </summary>
        internal void OpenPop()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);

            RefreshFree();
        }

        /// <summary>
        /// 关闭弹窗
        /// </summary>
        private void ClosePop()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            MainManager._instance.OnOpenPop_Raffle(false);
        }

        /// <summary>
        /// 获取本次抽奖的结果索引
        /// </summary>
        private int calculateResult()
        {
            // 设置本次抽奖的权重列表
            RaffleInfoData data = JsonConvert.DeserializeObject<RaffleInfoData>(DataHelper.CurUserInfoData.raffleInfo);
            // 达成每日活动三阶段 再转动10次必得宝箱 豪华部件宝箱
            if (data.luckNum == 19) return 0;
            // 达成每日活动二阶段 再转动5次必得宝箱 精品部件宝箱
            if (data.luckNum == 9) return 4;
            // 达成每日活动一阶段 再转动5次必得宝箱 基础部件宝箱
            if (data.luckNum == 4) return 6;

            // 未达成每日活动或者每日活动已结束
            if (data.luckNum > 20)
            {
                // 每日活动已结束 恢复正常权重
                for (int i = 0; i < _raffleData_Common.Length; i++)
                {
                    _raffleData[i] = _raffleData_Common[i];
                }
            }
            else
            {
                // 每日活动累计中
                for (int i = 0; i < _raffleData_Info.Length; i++)
                {
                    _raffleData[i] = _raffleData_Info[i];
                }
            }

            // 计算总权重
            var totalWeight = 0;
            for (var i = 0; i < _raffleData.Length; i++)
            {
                totalWeight += _raffleData[i];
            }
            
            // 计算最终权重
            var newWeightArr = new List<int>();
            var n = 0;
            while (newWeightArr.Count != _raffleData.Length)
            {
                var newWeight = 0;
                for (var i = 0; i < _raffleData.Length; i++)
                {
                    if (i <= n)
                    {
                        newWeight += _raffleData[i];
                    }
                }

                newWeightArr.Add(newWeight);
                n += 1;
            }
            
            // Debug.Log("配置表权重列表 = " + JsonConvert.SerializeObject(_raffleData));
            // Debug.Log("计算后权重列表 = " + JsonConvert.SerializeObject(newWeightArr));
            
            var ranWeight = Random.Range(0, totalWeight);
            var index = 0;
            while (index != newWeightArr.Count)
            {
                if (ranWeight < newWeightArr[index])
                {
                    break;
                }

                index += 1;
            }

            // Debug.Log("本次抽奖的结果 = " + index);

            return index;
        }

        /// <summary>
        /// 刷新免费
        /// </summary>
        private void RefreshFree()
        {
            RaffleInfoData data = JsonConvert.DeserializeObject<RaffleInfoData>(DataHelper.CurUserInfoData.raffleInfo);
            _isFree = true;
            if (data.lastFreeTime == 0)
            {
                // 上次免费时间为0
                _btnRaffleText_1.SetActive(true);
                _btnRaffleText_2.SetActive(false);
                _btnRaffleRedPoint.SetActive(true);
                _btnRaffleTimeText.text = "00:00";
            }
            else
            {
                // 上次免费时间不为0
                long timeTmp = ToolFunManager.GetCurrTime() - data.lastFreeTime;
                if (timeTmp >= GlobalValueManager.RaffleFreeTime * 60)
                {
                    // 免费时间已到
                    _btnRaffleText_1.SetActive(true);
                    _btnRaffleText_2.SetActive(false);
                    _btnRaffleRedPoint.SetActive(true);
                    _btnRaffleTimeText.text = "00:00";
                }
                else
                {
                    // 免费时间未到
                    _btnRaffleText_1.SetActive(false);
                    _btnRaffleText_2.SetActive(true);
                    _btnRaffleRedPoint.SetActive(false);
                    _isFree = false;
                    // 开始倒计时
                    StartRefreshTime(data.lastFreeTime);
                }
            }

            bool luckTimeEnd;
            if (data.luckStartTime == 0)
            {
                // Debug.Log("限时保底开始时间为0");
                // 限时保底开始时间为0 记录开始时间 开始倒计时
                luckTimeEnd = false;
                long infoStartTime = ToolFunManager.GetCurrTime();
                StartRefreshTimeInfo(infoStartTime);

                data.luckStartTime = infoStartTime;
                DataHelper.CurUserInfoData.raffleInfo = JsonConvert.SerializeObject(data);
                DataHelper.ModifyLocalData(new List<string>(1) { "raffleInfo" }, () => { });
            }
            else
            {
                // 限时保底开始时间不为0
                long timeTmp = ToolFunManager.GetCurrTime() - data.luckStartTime;
                if (timeTmp >= _infoLimitTime * 60)
                {
                    // 限时保底已结束
                    // Debug.Log("限时保底已结束");
                    luckTimeEnd = true;
                    _infoTimeText.text = "00:00:00";
                }
                else
                {
                    // 限时保底未结束 开始倒计时
                    // Debug.Log("限时保底未结束");
                    luckTimeEnd = false;
                    StartRefreshTimeInfo(data.luckStartTime);
                }
            }

            if (!luckTimeEnd)
            {
                if (data.luckNum >= 20)
                {
                    _infoObj.SetActive(false);
                }
                else
                {
                    _infoObj.SetActive(true);
                    if (data.luckNum >= 10)
                    {
                        int num = 20 - data.luckNum;
                        _infoText.text = new StringBuilder("再转动<color=#F6D200>" + num + "次转盘</color>\n至少获得豪华品质以上部件宝箱奖励！").ToString();
                    }
                    else if (data.luckNum >= 5)
                    {
                        int num = 10 - data.luckNum;
                        _infoText.text = new StringBuilder("再转动<color=#F6D200>" + num + "次转盘</color>\n至少获得精品品质以上部件宝箱奖励！").ToString();
                    }
                    else
                    {
                        int num = 5 - data.luckNum;
                        _infoText.text = new StringBuilder("再转动<color=#F6D200>" + num + "次转盘</color>\n必定获得宝箱奖励！").ToString();
                    }
                }
            }
            else
            {
                _infoObj.SetActive(false);
            }
        }

        /// <summary>
        /// 领取奖励
        /// </summary>
        private void GetReward()
        {
            List<string> modifyKeys = new List<string>();
            RaffleInfoData dataTmp = JsonConvert.DeserializeObject<RaffleInfoData>(DataHelper.CurUserInfoData.raffleInfo);
            if (_isFree)
            {
                // 本次是免费 刷新免费
                dataTmp.lastFreeTime = ToolFunManager.GetCurrTime();
                dataTmp.luckNum += (dataTmp.luckNum >= 20 ? 0 : 1);
                DataHelper.CurUserInfoData.raffleInfo = JsonConvert.SerializeObject(dataTmp);
                modifyKeys.Add("raffleInfo");
                
                // 完成日常任务 领取X次在线奖励 TaskID:6
                DataHelper.CompleteDailyTask(6, 1, 0);
                modifyKeys.Add("taskInfo1");
                // 完成成就任务 累计领取X次在线奖励 TaskID:7
                DataHelper.CompleteGloalTask(7, 1);
                modifyKeys.Add("taskInfo2");
                
                // 上报自定义分析数据 事件: 领取转盘奖励
                GameSdkManager._instance._sdkScript.ReportAnalytics("GetRaffle", "", "");
            }
            else
            {
                // 本次不是免费 是观看视频
                // 刷新每日活动累计次数
                dataTmp.luckNum += (dataTmp.luckNum >= 20 ? 0 : 1);
                DataHelper.CurUserInfoData.raffleInfo = JsonConvert.SerializeObject(dataTmp);
                modifyKeys.Add("raffleInfo");
                
                // 完成日常任务 观看X次广告 TaskID:2
                DataHelper.CompleteDailyTask(2, 1, 0);
                // 完成日常任务 领取X次在线奖励 TaskID:6
                DataHelper.CompleteDailyTask(6, 1, 0);
                modifyKeys.Add("taskInfo1");
                // 完成成就任务 累计观看X次广告 TaskID:3
                DataHelper.CompleteGloalTask(3, 1);
                // 完成成就任务 累计领取X次在线奖励 TaskID:7
                DataHelper.CompleteGloalTask(7, 1);
                modifyKeys.Add("taskInfo2");
                
                // 上报自定义分析数据 事件: 提前领取转盘奖励
                GameSdkManager._instance._sdkScript.ReportAnalytics("GetRaffle_Advance", "", "");
            }

            // 领取奖励
            switch (_rewardIds[_rewardIndex] / 100)
            {
                case 1: // 金币
                    DataHelper.CurUserInfoData.gold += _rewardNums[_rewardIndex];
                    modifyKeys.Add("gold");
                    DataHelper.CurGetItem = new[] { 1, _rewardIds[_rewardIndex], _rewardNums[_rewardIndex] };
                    GameGlobalManager._instance.OpenGetItem(true);
                    EventManager.Send(CustomEventType.RefreshMoney);
                    break;
                case 2: // 钻石
                    DataHelper.CurUserInfoData.diamond += _rewardNums[_rewardIndex];
                    modifyKeys.Add("diamond");
                    DataHelper.CurGetItem = new[] { 1, _rewardIds[_rewardIndex], _rewardNums[_rewardIndex] };
                    GameGlobalManager._instance.OpenGetItem(true);
                    EventManager.Send(CustomEventType.RefreshMoney);
                    break;
                case 3: // 宝箱
                    int boxIdTmp = _rewardIds[_rewardIndex];
                    int curLevelNum = DataHelper.CurUserInfoData.curLevelNum;
                    if (curLevelNum >= 5) curLevelNum = 5;
                    int boxId = ((boxIdTmp % 300) + 1) * 100 + (curLevelNum - 1);
                    GameGlobalManager._instance.OpenBox(boxId);
                    // 完成日常任务 打开X个部件宝箱 TaskID:4
                    DataHelper.CompleteDailyTask(4, 1, 0);
                    if (!modifyKeys.Contains("taskInfo1")) modifyKeys.Add("taskInfo1");
                    // 完成成就任务 累计打开X个部件宝箱 TaskID:5
                    DataHelper.CompleteGloalTask(5, 1);
                    if (!modifyKeys.Contains("taskInfo2")) modifyKeys.Add("taskInfo2");
                    break;
            }

            if (modifyKeys.Count > 0) DataHelper.ModifyLocalData(modifyKeys, () => { });
            
            // 刷新提示红点
            EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);

            SetStartRaffle(false); // 结束抽奖状态
            RefreshFree();         // 刷新免费
        }

        /// <summary>
        /// 开始倒计时
        /// <param name="lastFreeTime">上次免费抽奖的时间</param>
        /// </summary>
        private void StartRefreshTime(long lastFreeTime)
        {
            long nextTime = lastFreeTime + GlobalValueManager.RaffleFreeTime * 60;
            long subTime = nextTime - ToolFunManager.GetCurrTime();
            int minute = (int)(subTime / 60 % 60);
            int second = (int)(subTime % 60);
            _btnRaffleTimeText.text = $"{minute:D2}:{second:D2}";
            
            _timeNum = (int)subTime;
            _ = RefreshTime();
        }

        /// <summary>
        /// 刷新倒计时
        /// </summary>
        async UniTask RefreshTime()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            while (_timeNum > 0)
            {
                int minute = _timeNum / 60 % 60;
                int second = _timeNum % 60;
                _btnRaffleTimeText.text = $"{minute:D2}:{second:D2}";
                _timeNum -= 1;
                
                await UniTask.Delay(1000, true, cancellationToken: _cancellationToken.Token);
            }
            
            _timeNum = 0;
            _btnRaffleTimeText.text = "00:00";
            RefreshFree();
        }

        /// <summary>
        /// 设置抽奖状态
        /// </summary>
        /// <param name="isStart">进入/结束</param>
        private void SetStartRaffle(bool isStart)
        {
            _btnStartRaffle.SetActive(!isStart);
            _btnClose.SetActive(!isStart);
            _btnClose_Mask.interactable = !isStart;
        }

        /// <summary>
        /// 开始限时保底倒计时
        /// </summary>
        /// <param name="timeTmp">限时保底开始的时间</param>
        private void StartRefreshTimeInfo(long timeTmp)
        {
            long nextTime = timeTmp + _infoLimitTime * 60;
            long subTime = nextTime - ToolFunManager.GetCurrTime();
            int hour = (int)subTime / 60 / 60;
            int minute = (int)subTime / 60 % 60;
            int second = (int)subTime % 60;
            _infoTimeText.text = $"{hour:D2}:{minute:D2}:{second:D2}";

            _timeNum_Info = (int)subTime;
            _ = RefreshTimeInfo();
        }

        /// <summary>
        /// 刷新限时保底倒计时
        /// </summary>
        async UniTask RefreshTimeInfo()
        {
            _cancellationToken_Info?.Cancel();
            _cancellationToken_Info?.Dispose();
            _cancellationToken_Info = new CancellationTokenSource();
            while (_timeNum_Info > 0)
            {
                int hour = _timeNum_Info / 60 / 60;
                int minute = _timeNum_Info / 60 % 60;
                int second = _timeNum_Info % 60;
                _infoTimeText.text = $"{hour:D2}:{minute:D2}:{second:D2}";
                _timeNum_Info -= 1;

                await UniTask.Delay(1000, true, cancellationToken: _cancellationToken_Info.Token);
            }

            _timeNum_Info = 0;
            _infoTimeText.text = "00:00:00";
            RefreshFree();
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
        /// 按钮 抽奖
        /// </summary>
        private void OnBtnRaffle()
        {
            if (_isStartRotate)
            {
                GameGlobalManager._instance.ShowTips("抽奖正在进行中...");
                return;
            }
            
            _isStartRotate = true;

            if (_isFree)
            {
                // 本次是免费
                OnStartLottery();
                _btnRaffleRedPoint.SetActive(false);
            }
            else
            {
                // 本次是广告
                DataHelper.CurReportDf_adScene = "Raffle";
                GameSdkManager._instance._sdkScript.VideoControl("幸运转盘", OnStartLottery, () => { _isStartRotate = false; });
            }
        }

        /// <summary>
        /// 转盘抽奖开始
        /// </summary>
        private void OnStartLottery()
        {
            SetStartRaffle(true);
            _rewardIndex = calculateResult();
            _raffleAni.SetInteger(_RewardId, (_rewardIndex + 1));
            _raffleAni.Play("ArrowStart", 0, 0);
            AudioHandler._instance.PlayAudio(MainManager._instance.audioRaffle);
            _ = DelayStartCheckAni();
        }

        /// <summary>
        /// 延迟开始检测动画
        /// </summary>
        async UniTask DelayStartCheckAni()
        {
            _cancellationToken = new CancellationTokenSource();
            await UniTask.Delay(1000, true, cancellationToken: _cancellationToken.Token);
            _aniPlayComplete = false;
        }
    }
}