using UnityEngine.Scripting;

namespace Data.ClassData
{
    /// <summary>
    /// 转盘信息数据
    /// </summary>
    public class RaffleInfoData
    {
        public long dayStamp;      // 时间戳
        public long lastFreeTime;  // 上次免费抽奖时间
        public int luckNum;        // 幸运值(累计抽奖次数)
        public long luckStartTime; // 限时保底开始时间

        [Preserve]
        public RaffleInfoData()
        {
            
        }
    }
}