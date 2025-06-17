using System.Collections.Generic;
using System.Text;
using Common.Event;
using Common.Event.CustomEnum;
using Common.Tool;
using Data;
using Data.ConfigData;
using GamePlay.Globa;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.Round.Task
{
    public class ItemTaskUi2 : MonoBehaviour
    {
        /** 成就任务页面 */
        internal OpenTaskUi2 _openTaskUi2;
        
        /** 任务名称 */
        private TextMeshProUGUI _nameText;
        /** 任务进度条 */
        private Image _progressBar;
        /** 任务进度值 */
        private TextMeshProUGUI _progressNumText;
        /** 奖励数量 */
        private TextMeshProUGUI _rewardNumText;
        /** 任务描述 */
        private TextMeshProUGUI _descText;
        /** 领取奖励按钮 */
        private GameObject _btnGet;
        
        /** 任务ID */
        private int _taskId;
        /** 奖励数量 */
        private int _rewardNum;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _nameText = transform.Find("Tittle/Name").GetComponent<TextMeshProUGUI>();
            _progressBar = transform.Find("Progress/Bar").GetComponent<Image>();
            _progressNumText = transform.Find("Progress/Num").GetComponent<TextMeshProUGUI>();
            _rewardNumText = transform.Find("Item/Num").GetComponent<TextMeshProUGUI>();
            _descText = transform.Find("TaskDesc").GetComponent<TextMeshProUGUI>();
            _btnGet = transform.Find("BtnGet").gameObject;
            _btnGet.GetComponent<Button>().onClick.AddListener(OnBtnGet);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="taskData">任务数据</param>
        internal void SetData(int[] taskData)
        {
            TaskConfig2 config2 = ConfigManager.Instance.TaskConfigDict2[taskData[0]];
            Dictionary<int, int[]> taskInfo = JsonConvert.DeserializeObject<Dictionary<int, int[]>>(DataHelper.CurUserInfoData.taskInfo2);
            int[] taskInfoData = taskInfo.GetValueOrDefault(taskData[0], new[] { 0, 0 });

            _nameText.text = config2.Name;

            List<int> nums = ToolFunManager.GetNumFromStrNew(config2.Num);
            List<int> rewardNums = ToolFunManager.GetNumFromStrNew(config2.Re);
            
            StringBuilder desc = new StringBuilder(config2.Doc);
            _rewardNum = 0;

            float showProgress;
            float maxProgress;
            if (taskInfoData[0] >= nums.Count)
            {
                // 已经完成全部阶段且已领取全部阶段奖励
                showProgress = nums[^1];
                maxProgress = nums[^1];
                _btnGet.SetActive(false);
                desc.Replace("X", nums[^1].ToString());
                _rewardNum = rewardNums[^1];
            }
            else
            {
                if (taskInfoData[1] >= nums[taskInfoData[0]])
                {
                    // 还有阶段已完成未领取
                    showProgress = nums[taskInfoData[0]];
                    maxProgress = nums[taskInfoData[0]];
                    _btnGet.SetActive(true);
                }
                else
                {
                    // 没有阶段已完成未领取
                    showProgress = taskInfoData[1];
                    maxProgress = nums[taskInfoData[0]];
                    _btnGet.SetActive(false);
                }
                
                desc.Replace("X", nums[taskInfoData[0]].ToString());
                _rewardNum = rewardNums[taskInfoData[0]];
            }
            
            _descText.text = desc.ToString();
            _rewardNumText.text = _rewardNum.ToString();

            _progressBar.fillAmount = showProgress / maxProgress;
            _progressNumText.text = new StringBuilder(showProgress + "/" + maxProgress).ToString();

            _taskId = taskData[0];
        }

        /// <summary>
        /// 按钮 领取奖励
        /// </summary>
        private void OnBtnGet()
        {
            // 领取钻石
            DataHelper.CurUserInfoData.diamond += _rewardNum;
            DataHelper.CurGetItem = new[] { 1, 200, _rewardNum };
            GameGlobalManager._instance.OpenGetItem(true);
            EventManager.Send(CustomEventType.RefreshMoney);
            
            // 完成成就任务
            DataHelper.CompleteGloalTask(_taskId, -1);
            // 刷新成就任务列表
            _openTaskUi2.RefreshTaskList();
            // 刷新提示红点
            EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);

            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(2) { "diamond", "taskInfo2" }, () => { });
        }
    }
}