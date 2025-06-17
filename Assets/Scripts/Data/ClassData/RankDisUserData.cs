using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Data.ClassData
{
    /// <summary>
    /// 排行榜用户数据 飞行距离榜
    /// </summary>
    public class RankDisUserData
    {
        public string openId;     // 唯一ID
        public string nickName;   // 昵称
        public string userAvatar; // 头像
        public float distance;    // 飞行距离
        public string userProvince;           // 所在省份
        public List<int> planeEquipments;     // 飞机部件
        public List<int> planeEquipmentColor; // 飞机涂装
        public List<int> planeEquipmentLvs;   // 飞机部件等级

        [Preserve]
        public RankDisUserData()
        {
            
        }
    }
}