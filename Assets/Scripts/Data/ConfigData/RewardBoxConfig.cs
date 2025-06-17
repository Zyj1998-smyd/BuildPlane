using UnityEngine.Scripting;

namespace Data.ConfigData
{
    /// <summary>
    /// 宝箱配置表
    /// </summary>
    public class RewardBoxConfig
    {
        public int ID;                 // 宝箱ID
        public string QualityNum1;     // 绿色品质占比
        public string QualityNum2;     // 蓝色品质占比
        public string QualityNum3;     // 黄色品质占比
        public string QualityNum4;     // 紫色品质占比
        public string QualityNum5;     // 红色品质占比
        public string ExtraQualityNum; // 额外奖励品质
        public int OpenTime;           // 打开消耗时间
        public int OpenGem;            // 打开消耗钻石

        [Preserve]
        public RewardBoxConfig()
        {
            
        }
    }
}