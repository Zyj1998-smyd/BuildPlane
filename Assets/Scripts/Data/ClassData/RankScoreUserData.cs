using UnityEngine.Scripting;

namespace Data.ClassData
{
    /// <summary>
    /// 排行榜用户数据 积分榜
    /// </summary>
    public class RankScoreUserData
    {
        public string openId;     // 唯一ID
        public string nickName;   // 昵称
        public string userAvatar; // 头像
        public int scoreNum;      // 积分

        [Preserve]
        public RankScoreUserData()
        {
            
        }
    }
}