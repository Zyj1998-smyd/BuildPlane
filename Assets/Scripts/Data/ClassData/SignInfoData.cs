using UnityEngine.Scripting;

namespace Data.ClassData
{
    /// <summary>
    /// 签到信息数据
    /// </summary>
    public class SignInfoData
    {
        public long dayStamp;     // 签到时间
        public int day;           // 签到第几天
        public int isSign;        // 是否签到 0: 未签到 1: 已签到(单倍领取) 2: 已签到(双倍领取)

        [Preserve]
        public SignInfoData()
        {
            
        }
    }
}