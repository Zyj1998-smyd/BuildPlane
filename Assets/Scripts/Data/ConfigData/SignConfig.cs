using UnityEngine.Scripting;

namespace Data.ConfigData
{
    /// <summary>
    /// 签到配置表
    /// </summary>
    public class SignConfig
    {
        public int Day;   // 签到天数
        public string ID; // 奖励ID
        public int Type;  // 奖励类型
        public int Num;   // 奖励数量

        [Preserve]
        public SignConfig()
        {
            
        }
    }
}