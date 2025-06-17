using System.Collections.Generic;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using GamePlay.Globa;
using GamePlay.Main;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.Round.Task
{
    public class OpenTaskUi2 : MonoBehaviour
    {
        /** 成就任务预制 */
        public ItemTaskUi2 ItemTaskUi2Pre;
        
        /** 任务总弹窗 */
        internal OpenTaskPageUi _openTaskPageUi;
        
        /** 成就任务列表 */
        private ScrollRect _scrollRect;
        /** 成就任务列表项挂载容器 */
        private Transform _listSvContent;
        
        /** 日常任务页签标题提示红点 */
        private GameObject _tittleRedPoint;

        /** 成就任务列表 */
        private readonly List<ItemTaskUi2> _itemTaskUis = new List<ItemTaskUi2>();
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            transform.Find("Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            transform.Find("Label/Label2").GetComponent<Button>().onClick.AddListener(OnBtnChangeToDailyTask);
            
            _tittleRedPoint = transform.Find("Label/Label2/RedPoint").gameObject;

            _scrollRect = transform.Find("TaskList").GetComponent<ScrollRect>();
            _listSvContent = _scrollRect.transform.Find("Viewport/Content");
            
            // 初始化任务列表
            InitTaskList();
        }
        
        /// <summary>
        /// 打开成就任务
        /// </summary>
        internal void OpenPop()
        {
            RefreshTaskList();
            _scrollRect.verticalNormalizedPosition = 1f;
            // 刷新日常任务页签提示红点
            _tittleRedPoint.SetActive(MainManager._instance._redPointManager.GetRedPoint_Task_Daily());
        }

        /// <summary>
        /// 初始化任务列表
        /// </summary>
        private void InitTaskList()
        {
            var listTmp = RefreshTaskData();
            if (listTmp == null) return;
            
            // 初始化任务列表
            var num = listTmp.Count;
            for (var i = 0; i < num; i++)
            {
                var iTmp = i;
                DelayTime.DelaySeconds(() =>
                {
                    // if (!gameObject.activeSelf) return;
                    var taskItem = Instantiate(ItemTaskUi2Pre, _listSvContent, false);
                    taskItem._openTaskUi2 = this;
                    taskItem.Initial();
                    taskItem.SetData(listTmp[iTmp]);
                    _itemTaskUis.Add(taskItem);
                }, i * 0.016f, this.GetCancellationTokenOnDestroy());
            }
        }
        
        /// <summary>
        /// 刷新任务列表
        /// </summary>
        private List<int[]> RefreshTaskData()
        {
            // if (!gameObject.activeSelf) return null;

            var taskList_1 = new List<int>(); // 已完成且已领取
            var taskList_2 = new List<int>(); // 已完成且未领取
            var taskList_3 = new List<int>(); // 未完成

            var gloalTaskInfo = JsonConvert.DeserializeObject<Dictionary<int, int[]>>(DataHelper.CurUserInfoData.taskInfo2);
            for (int i = 0; i < ConfigManager.Instance.TaskConfig2s.Count; i++)
            {
                var taskConfig = ConfigManager.Instance.TaskConfig2s[i];
                if (gloalTaskInfo.ContainsKey(taskConfig.ID))
                {
                    // 当前成就任务有记录
                    List<int> targets = ToolFunManager.GetNumFromStrNew(taskConfig.Num);
                    int[] taskInfo = gloalTaskInfo[taskConfig.ID];
                    if (taskInfo[0] >= targets.Count)
                    {
                        // 已完成且已领取奖励
                        taskList_1.Add(taskConfig.ID);
                    }
                    else
                    {
                        int n = 0;
                        for (int j = 0; j < targets.Count; j++)
                        {
                            if (taskInfo[1] >= targets[j]) n += 1;
                        }

                        if (n == 0)
                        {
                            // 没有完成的任务 ==> 未完成
                            taskList_3.Add(taskConfig.ID);
                        }
                        else
                        {
                            // 有完成的任务
                            if (taskInfo[0] < n)
                            {
                                // 还有未领取奖励的任务
                                taskList_2.Add(taskConfig.ID);
                            }
                            else
                            {
                                // 没有未领取奖励的任务
                                taskList_3.Add(taskConfig.ID);
                            }
                        }
                    }
                }
                else
                {
                    // 当前成就任务没有记录 ==> 未完成
                    taskList_3.Add(taskConfig.ID);
                }
            }

            var taskList = new List<int[]>(taskList_1.Count + taskList_2.Count + taskList_3.Count);
            for (var i = 0; i < taskList_2.Count; i++)
            {
                taskList.Add(new[] { taskList_2[i], 1 });
            }

            for (var i = 0; i < taskList_3.Count; i++)
            {
                taskList.Add(new[] { taskList_3[i], 0 });
            }

            for (var i = 0; i < taskList_1.Count; i++)
            {
                taskList.Add(new[] { taskList_1[i], -1 });
            }

            return taskList;
        }

        /// <summary>
        /// 刷新任务列表
        /// </summary>
        internal void RefreshTaskList()
        {
            var listTmp = RefreshTaskData();
            if (listTmp == null) return;
            for (int i = 0; i < _itemTaskUis.Count; i++)
            {
                _itemTaskUis[i].SetData(listTmp[i]);
            }
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
        /// 按钮 切换到日常任务
        /// </summary>
        private void OnBtnChangeToDailyTask()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            _openTaskPageUi.ChangeToDailyTask();
        }
    }
}