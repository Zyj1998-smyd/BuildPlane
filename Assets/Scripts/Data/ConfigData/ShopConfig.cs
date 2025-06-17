using UnityEngine.Scripting;

namespace Data.ConfigData
{
    /// <summary>
    /// 商店配置表
    /// </summary>
    public class ShopConfig
    {
        public int ID;        // ID
        public int Type;      // 类型
        public string Name;   // 名称
        public string Num;    // 包含数量
        public int BuyType;   // 购买方式
        public string BuyNum; // 购买价格
        public int Limit;     // 限购

        [Preserve]
        public ShopConfig()
        {
            
        }
    }
}