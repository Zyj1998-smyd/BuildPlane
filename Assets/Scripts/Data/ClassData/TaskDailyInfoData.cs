using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Data.ClassData
{
    public class TaskDailyInfoData
    {
        public long dayStamp;       // 时间戳
        public int activePoint;     // 活跃点
        public List<int> rewardGet; // 奖励领取状态 0: 未领取 1: 已领取
        public Dictionary<int, int> taskState; // [Key: 任务ID, Value: 任务完成状态 -1: 已完成且已领取 >-1: 任务完成进度]

        [Preserve]
        public TaskDailyInfoData()
        {
            
        }
    }
}