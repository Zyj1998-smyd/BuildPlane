using UnityEngine.Scripting;

namespace Data.ConfigData
{
    /// <summary>
    /// 省份配置表
    /// </summary>
    public class ProvinceConfig
    {
        public int ID;              // 列表ID
        public string name;         // 名称

        [Preserve]
        public ProvinceConfig()
        {
            
        }
    }
}