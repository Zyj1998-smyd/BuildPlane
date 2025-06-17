using UnityEngine.Scripting;

namespace Data.ConfigData
{
    /// <summary>
    /// 日常任务配置表
    /// </summary>
    public class TaskConfig1
    {
        public int ID;     // 任务ID/活跃奖励ID
        public string Doc; // 任务说明/活跃奖励说明
        public int Type;   // 任务类型/活跃奖励类型
        public int Num;    // 任务目标/活跃奖励数量

        [Preserve]
        public TaskConfig1()
        {
            
        }
    }
}