using UnityEngine.Scripting;

namespace Data.ConfigData
{
    public class GmConfig
    {
        public string appId;    // appid
        public string appName;  // appName
        public string videoId;  // 激励视频广告ID
        public string appToken; // appToken
        public string gmUser;   // GM用户
        public string gameId;   // 游戏ID 后台配置

        [Preserve]
        public GmConfig()
        {
            
        }
    }
}