using UnityEngine.Scripting;

namespace Data.ConfigData
{
    /// <summary>
    /// 部件配置表
    /// </summary>
    public class ComponentConfig
    {
        public int    ID;         // ID
        public string Name;       // 名称
        public int    Type;       // 类型
        public int    Quality;    // 品质
        public int    Price;      // 价格
        public int    ZhongLiang; // 重量
        public int    FuKong;     // 浮空
        public int    SuDu;       // 速度
        public int    KangZu;     // 抗阻
        public int    TuiJin;     // 推进
        public int    NengLiang;  // 能量

        [Preserve]
        public ComponentConfig()
        {
            
        }
    }
}