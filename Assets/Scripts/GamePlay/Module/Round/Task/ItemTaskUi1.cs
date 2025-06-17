using System.Collections.Generic;
using System.Text;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Data;
using Data.ConfigData;
using GamePlay.Globa;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.Round.Task
{
    public class ItemTaskUi1 : MonoBehaviour
    {
        /** 日常任务页面 */
        internal OpenTaskUi1 _openTaskUi1;
        
        /** 任务描述 */
        private TextMeshProUGUI _descText;
        /** 任务进度值 */
        private TextMeshProUGUI _progressBarNumText;
        /** 任务进度条 */
        private Image _progressBar;
        /** 领取奖励按钮 */
        private GameObject _btnGet;
        /** 已完成 */
        private GameObject _complete;

        /** 任务ID */
        private int _taskId;
        /** 活跃点 */
        private int _activePoint;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _descText = transform.Find("TaskDesc").GetComponent<TextMeshProUGUI>();
            _progressBar = transform.Find("Progress/Bar").GetComponent<Image>();
            _progressBarNumText = transform.Find("Progress/Num").GetComponent<TextMeshProUGUI>();
            _btnGet = transform.Find("BtnGet").gameObject;
            _complete = transform.Find("Complete").gameObject;
            _btnGet.GetComponent<Button>().onClick.AddListener(OnBtnGet);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="taskData">任务数据</param>
        internal void SetData(int[] taskData)
        {
            TaskConfig1 config = ConfigManager.Instance.TaskConfigDict1[taskData[0]];
            
            StringBuilder desc = new StringBuilder(config.Doc);
            desc.Replace("X", config.Num.ToString());
            _descText.text = desc.ToString();

            switch (taskData[2])
            {
                case -1: // 已完成且已领取
                    _btnGet.SetActive(false);
                    _complete.SetActive(true);
                    _progressBar.fillAmount = 1f;
                    _progressBarNumText.text = new StringBuilder(config.Num + "/" + config.Num).ToString();
                    break;
                case 0:  // 未完成
                    _btnGet.SetActive(false);
                    _complete.SetActive(false);
                    _progressBar.fillAmount = (float)taskData[1] / config.Num;
                    _progressBarNumText.text = new StringBuilder(taskData[1] + "/" + config.Num).ToString();
                    break;
                case 1:  // 已完成且未领取
                    _btnGet.SetActive(true);
                    _complete.SetActive(false);
                    _progressBar.fillAmount = 1f;
                    _progressBarNumText.text = new StringBuilder(config.Num + "/" + config.Num).ToString();
                    break;
            }

            _taskId = taskData[0];
            _activePoint = 10;
        }

        /// <summary>
        /// 按钮 领取奖励
        /// </summary>
        private void OnBtnGet()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            // 完成任务并领取活跃度
            DataHelper.CompleteDailyTask(_taskId, -1, _activePoint);
            // 刷新日常任务列表
            _openTaskUi1.RefreshTaskList();
            // 刷新提示红点
            EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);
            
            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(1) { "taskInfo1" }, () => { });
        }
    }
}