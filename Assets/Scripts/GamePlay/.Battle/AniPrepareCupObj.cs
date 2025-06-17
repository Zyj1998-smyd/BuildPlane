using Data;
using UnityEngine;

namespace GamePlay.Battle
{
    public class AniPrepareCupObj : MonoBehaviour
    {
        /// <summary>
        /// 动画事件 初始化备料杯 ==> 确保所有的原备料杯移出屏幕外初始化备料杯
        /// </summary>
        public void AniEventClearPrepareCup()
        {
            BattleManager._instance.InitPrepareCups();
        }

        /// <summary>
        /// 动画事件 备料杯移动动画播放完全结束
        /// </summary>
        public void AniEventOutComplete()
        {
            if (BattleManager._instance.prepareCupClearType == 0)
            {
                // 使用道具清空备料杯
                ConfigManager.Instance.ConsoleLog(0, "退出清空备料道具使用状态...");
                BattleManager._instance.itemUsing = false;
            }
            else if (BattleManager._instance.prepareCupClearType == 1)
            {
                // 复活清空备料杯
                ConfigManager.Instance.ConsoleLog(0, "复活 清空备料杯...");
                BattleManager._instance.OnRevive();
            }
        }
    }
}