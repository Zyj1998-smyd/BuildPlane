using Common.GameRoot.AudioHandler;
using GamePlay.Globa;
using GamePlay.Main;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.Round.Task
{
    public class OpenTaskPageUi : MonoBehaviour
    {
        /** 每日任务 */
        private OpenTaskUi1 _openTaskUi1;
        /** 成就任务 */
        private OpenTaskUi2 _openTaskUi2;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            transform.Find("Mask").gameObject.AddComponent<Button>().onClick.AddListener(ClosePop);

            _openTaskUi1 = transform.Find("Task/Task1").GetComponent<OpenTaskUi1>();
            _openTaskUi1.Initial();
            _openTaskUi1._openTaskPageUi = this;

            _openTaskUi2 = transform.Find("Task/Task2").GetComponent<OpenTaskUi2>();
            _openTaskUi2.Initial();
            _openTaskUi2._openTaskPageUi = this;
        }

        /// <summary>
        /// 打开弹窗 任务
        /// </summary>
        internal void OpenPop()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);
            ChangeToDailyTask();
        }

        /// <summary>
        /// 关闭弹窗
        /// </summary>
        internal void ClosePop()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            _openTaskUi1.gameObject.SetActive(false);
            _openTaskUi2.gameObject.SetActive(false);
            MainManager._instance.OnOpenPop_Task(false);
        }

        /// <summary>
        /// 切换到日常任务
        /// </summary>
        internal void ChangeToDailyTask()
        {
            _openTaskUi1.gameObject.SetActive(true);
            _openTaskUi2.gameObject.SetActive(false);
            _openTaskUi1.OpenPop();
        }

        /// <summary>
        /// 切换到成就任务
        /// </summary>
        internal void ChangeToTask()
        {
            _openTaskUi1.gameObject.SetActive(false);
            _openTaskUi2.gameObject.SetActive(true);
            _openTaskUi2.OpenPop();
        }
    }
}