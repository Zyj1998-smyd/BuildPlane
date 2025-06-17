using UnityEngine.Scripting;

namespace Data.ConfigData
{
    /// <summary>
    /// 商店配置表
    /// </summary>
    public class ShopItemConfig
    {
        public int LevelID;  // 当前关卡
        public int Quality1; // 绿色品质概率
        public int Quality2; // 蓝色品质概率
        public int Quality3; // 黄色品质概率
        public int Quality4; // 紫色品质概率
        public int Quality5; // 红色品质概率

        [Preserve]
        public ShopItemConfig()
        {
            
        }
    }
}