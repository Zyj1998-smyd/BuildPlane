using UnityEngine.Scripting;

namespace Data.ConfigData
{
    /// <summary>
    /// 成就任务配置表
    /// </summary>
    public class TaskConfig2
    {
        public int ID;      // 成就任务ID
        public string Name; // 成就任务名称
        public string Doc;  // 成就任务说明
        public int Type;    // 成就任务类型
        public string Num;  // 成就任务目标
        public string Re;   // 成就任务奖励数量

        [Preserve]
        public TaskConfig2()
        {
            
        }
    }
}