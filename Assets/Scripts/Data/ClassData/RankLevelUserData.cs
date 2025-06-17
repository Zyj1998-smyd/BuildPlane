using UnityEngine.Scripting;

namespace Data.ClassData
{
    /// <summary>
    /// 排行榜用户数据 通关榜
    /// </summary>
    public class RankLevelUserData
    {
        public string openId;     // 唯一ID
        public string nickName;   // 昵称
        public string userAvatar; // 头像
        public int levelNum;      // 通关关卡数

        [Preserve]
        public RankLevelUserData()
        {
            
        }
    }
}