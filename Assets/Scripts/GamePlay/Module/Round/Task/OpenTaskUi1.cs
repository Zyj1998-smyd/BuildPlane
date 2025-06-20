using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Cysharp.Threading.Tasks;
using Data;
using Data.ClassData;
using Data.ConfigData;
using GamePlay.Globa;
using GamePlay.Main;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.Round.Task
{
    public class OpenTaskUi1 : MonoBehaviour
    {
        /** 日常任务预制 */
        public ItemTaskUi1 ItemTaskUi1Pre;
        
        /** 任务总弹窗 */
        internal OpenTaskPageUi _openTaskPageUi;

        /** 日常任务列表 */
        private ScrollRect _scrollRect;
        /** 日常任务列表项挂载容器 */
        private Transform _content;

        /** 日常任务刷新倒计时 */
        private TextMeshProUGUI _refreshTimeText;
        /** 每日活跃点 */
        private TextMeshProUGUI _activePointNumText;
        /** 每日活跃进度条 */
        private RectTransform _activeBar;
        /** 每日活跃 宝箱领取目标活跃点 */
        private readonly TextMeshProUGUI[] _activeItemNumTexts = new TextMeshProUGUI[5];
        /** 每日活跃 宝箱 */
        private readonly Animation[] _activeItemRewards = new Animation[5];
        /** 每日活跃 宝箱领取完成 */
        private readonly GameObject[] _activeItemGetOks = new GameObject[5];
        /** 每日活跃 宝箱可领取提示红点 */
        private readonly GameObject[] _activeItemRedPoints = new GameObject[5];
        /** 每日活跃 宝箱详情 */
        private readonly GameObject[] _activeItemRewardItems = new GameObject[5];
        /** 每日活跃 宝箱详情 奖励图标 */
        private readonly Image[] _activeItemRewardItemImages = new Image[5];
        /** 每日活跃 宝箱详情 奖励数量 */
        private readonly TextMeshProUGUI[] _activeItemRewardItemNumTexts = new TextMeshProUGUI[5];
        /** 触摸层(关闭奖励详情) */
        private GameObject _touchLayer;

        /** 成就任务页签标题提示红点 */
        private GameObject _tittleRedPoint;
        
        /** 每日活跃奖励可领取状态列表 -1: 已领取 0: 不可领取 1: 可领取 */
        private List<int> _dayRewardCanGetList;
        /** 当前点击的奖励索引 */
        private int _curClickIndex;

        /** 每日任务列表 */
        private readonly List<ItemTaskUi1> _itemTaskUis = new List<ItemTaskUi1>();
        
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;
        
        /** 刷新倒计时 */
        private int _timeNum;

        /** 活跃度宝箱奖励ID */
        private readonly int[] _rewardIds = new int[5];
        /** 活跃度宝箱奖励数量 */
        private readonly int[] _rewardNums = new int[5];
        
        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            transform.Find("Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            transform.Find("Label/Label1").GetComponent<Button>().onClick.AddListener(OnBtnChangeToTask);

            _tittleRedPoint = transform.Find("Label/Label1/RedPoint").gameObject;

            _scrollRect = transform.Find("TaskList").GetComponent<ScrollRect>();
            _content = _scrollRect.transform.Find("Viewport/Content");

            _refreshTimeText = transform.Find("Active/Time").GetComponent<TextMeshProUGUI>();
            _activePointNumText = transform.Find("Active/ActiveIcon/Num").GetComponent<TextMeshProUGUI>();
            _activeBar = transform.Find("Active/ActiveBar/Bar").GetComponent<RectTransform>();

            for (int i = 0; i < 5; i++)
            {
                Transform item = transform.Find("Active/ActiveBar/Item_" + (i + 1));
                _activeItemNumTexts[i] = item.Find("Num").GetComponent<TextMeshProUGUI>();
                _activeItemRewards[i] = item.Find("Reward").GetComponent<Animation>();
                _activeItemGetOks[i] = item.Find("GetOk").gameObject;
                _activeItemRedPoints[i] = item.Find("RedPoint").gameObject;
                _activeItemRewardItems[i] = item.Find("RewardItem").gameObject;
                _activeItemRewardItems[i].SetActive(false);
                _activeItemRewardItemImages[i] = item.Find("RewardItem/Image").GetComponent<Image>();
                _activeItemRewardItemNumTexts[i] = item.Find("RewardItem/Image/Num").GetComponent<TextMeshProUGUI>();

                int index = i;
                item.GetComponent<Button>().onClick.AddListener(() => { OnBtnReward(index); });
            }

            _touchLayer = transform.Find("Touch").gameObject;
            _touchLayer.GetComponent<Button>().onClick.AddListener(OnBtnTouchLayer);

            _itemTaskUis.Clear();
            for (int i = 0; i < 10; i++)
            {
                ItemTaskUi1 itemTaskUi1 = Instantiate(ItemTaskUi1Pre, _content);
                itemTaskUi1._openTaskUi1 = this;
                itemTaskUi1.Initial();
                _itemTaskUis.Add(itemTaskUi1);
            }
        }

        /// <summary>
        /// 打开每日任务
        /// </summary>
        internal void OpenPop()
        {
            // 刷新活跃度奖励
            RefreshActiveReward();
            // 刷新任务列表
            RefreshTaskList();
            _scrollRect.verticalNormalizedPosition = 1f;
            // 每日任务刷新倒计时
            StartRefreshTime();
            // 刷新成就页签提示红点
            _tittleRedPoint.SetActive(MainManager._instance._redPointManager.GetRedPoint_Task_Goal());
        }

        /// <summary>
        /// 刷新活跃度奖励
        /// </summary>
        private void RefreshActiveReward()
        {
            for (int i = 0; i < _activeItemRewardItems.Length; i++)
            {
                TaskConfig1 config = ConfigManager.Instance.TaskConfigDict1[GlobalValueManager.TaskDayActiveIds[i]];
                _activeItemRewardItemNumTexts[i].text = config.Num == 1 ? "" : config.Num.ToString();
                GameGlobalManager._instance.SetImage(_activeItemRewardItemImages[i], new StringBuilder("IconImage" + config.Type).ToString());
                _activeItemNumTexts[i].text = GlobalValueManager.TaskDayActivePoints[i].ToString();

                _rewardIds[i] = config.Type;
                _rewardNums[i] = config.Num;
            }

            _touchLayer.SetActive(false);
        }

        /// <summary>
        /// 刷新任务列表
        /// </summary>
        internal void RefreshTaskList()
        {
            TaskDailyInfoData taskDailyInfoData = JsonConvert.DeserializeObject<TaskDailyInfoData>(DataHelper.CurUserInfoData.taskInfo1);
            int dayActiveNum = taskDailyInfoData.activePoint;

            List<int> dayActiveNumArr = new List<int> { 0 };
            List<int> dayReqPoints = new List<int>();
            for (int i = 0; i < GlobalValueManager.TaskDayActivePoints.Count; i++)
            {
                dayReqPoints.Add(GlobalValueManager.TaskDayActivePoints[i]);
                if (i < GlobalValueManager.TaskDayActivePoints.Count - 1) dayActiveNumArr.Add(GlobalValueManager.TaskDayActivePoints[i]);
            }
            
            int nTmp2 = GetActiveIndex(dayActiveNum, dayReqPoints);
            float baseWidthTmp2 = nTmp2 * 0.2f;
            float widthTmp2 = baseWidthTmp2 + (dayActiveNum - dayActiveNumArr[nTmp2]) / GlobalValueManager.TaskDayActiveSubArr[nTmp2] * 0.2f;
            Vector2 sizeTmp = _activeBar.sizeDelta;
            float widthTmp = 940 * (widthTmp2 >= 1f ? 1f : widthTmp2 <= 0 ? 0f : widthTmp2);
            _activeBar.sizeDelta = new Vector2(widthTmp, sizeTmp.y);
            
            _activePointNumText.text = dayActiveNum.ToString();

            _dayRewardCanGetList = new List<int>(taskDailyInfoData.rewardGet.Count);
            for (int i = 0; i < taskDailyInfoData.rewardGet.Count; i++)
            {
                switch (taskDailyInfoData.rewardGet[i])
                {
                    case 0: // 未领取
                        _activeItemRewards[i].gameObject.SetActive(true);
                        _activeItemGetOks[i].SetActive(false);
                        if (dayActiveNum >= GlobalValueManager.TaskDayActivePoints[i])
                        {
                            _activeItemRewards[i].Play("RewardOn");
                            _activeItemRedPoints[i].SetActive(true);
                            _dayRewardCanGetList.Add(1);
                        }
                        else
                        {
                            _activeItemRewards[i].Play("RewardOff");
                            _activeItemRedPoints[i].SetActive(false);
                            _dayRewardCanGetList.Add(0);
                        }
                        break;
                    case 1: // 已领取
                        _activeItemRewards[i].gameObject.SetActive(false);
                        _activeItemGetOks[i].SetActive(true);
                        _activeItemRedPoints[i].SetActive(false);
                        _dayRewardCanGetList.Add(-1);
                        break;
                }
            }

            // 每日任务
            var task_1 = new List<int[]>(); // 已完成且已领取
            var task_2 = new List<int[]>(); // 已完成且未领取
            var task_3 = new List<int[]>(); // 未完成
            foreach (var taskState in taskDailyInfoData.taskState)
            {
                if (taskState.Value == -1)
                {
                    // 已完成且已领取
                    task_1.Add(new[] {taskState.Key, taskState.Value, -1});
                }
                else
                {
                    // 未领取
                    var taskConfig = ConfigManager.Instance.TaskConfigDict1[taskState.Key];
                    if (taskState.Value >= taskConfig.Num)
                    {
                        // 已完成
                        task_2.Add(new[] {taskState.Key, taskState.Value, 1});
                    }
                    else
                    {
                        // 未完成
                        task_3.Add(new[] {taskState.Key, taskState.Value, 0});
                    }
                }
            }
            
            var taskList = new List<int[]>(task_1.Count + task_2.Count + task_3.Count);
            for (var i = 0; i < task_2.Count; i++)
            {
                taskList.Add(task_2[i]);
            }

            for (var i = 0; i < task_3.Count; i++)
            {
                taskList.Add(task_3[i]);
            }

            for (var i = 0; i < task_1.Count; i++)
            {
                taskList.Add(task_1[i]);
            }
            
            for (var i = 0; i < taskList.Count; i++)
            {
                _itemTaskUis[i].SetData(taskList[i]);
            }
        }
      
        /// <summary>
        /// 获取活跃度奖励进度条索引
        /// </summary>
        /// <param name="activeNum">活跃度</param>
        /// <param name="reqPoints">活跃度挡位需求</param>
        private int GetActiveIndex(int activeNum, List<int> reqPoints)
        {
            var n = 0;
            if (activeNum < reqPoints[0])
            {
                n = 0;
            }else if (activeNum < reqPoints[1])
            {
                n = 1;
            }else if (activeNum < reqPoints[2])
            {
                n = 2;
            }else if (activeNum < reqPoints[3])
            {
                n = 3;
            }else if (activeNum < reqPoints[4])
            {
                n = 4;
            }

            return n;
        }
        
        /// <summary>
        /// 开始倒计时
        /// </summary>
        private void StartRefreshTime()
        {
            DateTime nextTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            double subTime = (nextTime - DateTime.Now).TotalSeconds;
            int hour = (int)(subTime / 60 / 60);
            int minute = (int)(subTime / 60 % 60);
            int second = (int)(subTime % 60);
            _refreshTimeText.text = new StringBuilder($"After" + "{hour:D2}:{minute:D2}:{second:D2}" + "Refresh Task").ToString();
            
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
                int hour = _timeNum / 60 / 60;
                int minute = _timeNum / 60 % 60;
                int second = _timeNum % 60;
                _refreshTimeText.text = new StringBuilder($"After {hour:D2}:{minute:D2}:{second:D2}" + " Refresh Tasks").ToString();
                _timeNum -= 1;
                
                await UniTask.Delay(1000, true, cancellationToken: _cancellationToken.Token);
            }
            
            _timeNum = 0;
            
            
            _refreshTimeText.text = "After 00:00:00 Refresh Tasks";
        }

        // ----------------------------------------------- 按钮 -----------------------------------------------
        /// <summary>
        /// 按钮 关闭
        /// </summary>
        private void OnBtnClose()
        {
            _openTaskPageUi.ClosePop();
        }

        /// <summary>
        /// 按钮 切换到成就任务
        /// </summary>
        private void OnBtnChangeToTask()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            _openTaskPageUi.ChangeToTask();
        }

        /// <summary>
        /// 按钮 触摸层(关闭奖励详情)
        /// </summary>
        private void OnBtnTouchLayer()
        {
            _activeItemRewardItems[_curClickIndex].SetActive(false);
            _touchLayer.SetActive(false);
        }

        /// <summary>
        /// 按钮 领取奖励
        /// </summary>
        /// <param name="index">奖励列表索引</param>
        private void OnBtnReward(int index)
        {
            List<string> modifyKeys = new List<string>();
            switch (_dayRewardCanGetList[index])
            {
                case -1: // 奖励已领取
                case 0:  // 未达到领取条件
                    AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                    _curClickIndex = index;
                    _activeItemRewardItems[index].SetActive(true);
                    _touchLayer.SetActive(true);
                    break;
                case 1:  // 领取奖励
                    switch (_rewardIds[index] / 100)
                    {
                        case 1: // 金币
                            DataHelper.CurUserInfoData.gold += _rewardNums[index];
                            modifyKeys.Add("gold");
                            DataHelper.CurGetItem = new[] { 1, _rewardIds[index], _rewardNums[index] };
                            GameGlobalManager._instance.OpenGetItem(true);
                            EventManager.Send(CustomEventType.RefreshMoney);
                            break;
                        case 2: // 钻石
                            DataHelper.CurUserInfoData.diamond += _rewardNums[index];
                            modifyKeys.Add("diamond");
                            DataHelper.CurGetItem = new[] { 1, _rewardIds[index], _rewardNums[index] };
                            GameGlobalManager._instance.OpenGetItem(true);
                            EventManager.Send(CustomEventType.RefreshMoney);
                            break;
                        case 3: // 宝箱
                            int boxIdTmp = _rewardIds[index];
                            int curLevelNum = DataHelper.CurUserInfoData.curLevelNum;
                            if (curLevelNum >= 5) curLevelNum = 5;
                            int boxId = ((boxIdTmp % 300) + 1) * 100 + (curLevelNum - 1);
                            GameGlobalManager._instance.OpenBox(boxId);
                            // 完成日常任务 打开X个部件宝箱 TaskID:4
                            DataHelper.CompleteDailyTask(4, 1, 0);
                            // 完成成就任务 累计打开X个部件宝箱 TaskID:5
                            DataHelper.CompleteGloalTask(5, 1);
                            break;
                    }

                    DataHelper.GetTaskActiveReward(index);
                    modifyKeys.Add("taskInfo1");
                    RefreshTaskList();

                    if (modifyKeys.Count > 0)
                    {
                        DataHelper.ModifyLocalData(modifyKeys, () => { });
                        EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);
                    }
                    
                    break;
            }
        }
    }
}