using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Data.ClassData
{
    /// <summary>
    /// 用户数据
    /// </summary>
    public class UserInfoData
    {
        public string userId;       // 用户ID
        public string userName;     // 用户昵称
        public string userAvatar;   // 用户头像
        public string userProvince; // 用户所在省份
        
        public int gold;          // 金币
        public int diamond;       // 钻石
        
        public int curLevelNum; // 关卡
        public int scoreNum;    // 积分

        public int addedToMyMiniProgramGet; // 加到我的小程序奖励领取完成
        public int callRewardGet; // 邀请奖励领取记录
        public int feedSubGet;    // 直玩订阅奖励领取记录

        public List<int> equipEquipments;            // 装备中的配件列表
        public Dictionary<int, int> equipments;      // 已拥有的配件列表 key: 配件ID value: 配件等级
        public Dictionary<int, int> equipmentChips;  // 已拥有的配件碎片列表 key: 配件ID value: 配件碎片数量

        public Dictionary<int, int> equipmentPaintNews; // 新获得的配件涂装状态(涂装按钮提示红点) key: 配件ID value: 涂装状态 0: 未查看 1: 已查看

        public Dictionary<int, List<int>> equipmentPaints;    // 已拥有的配件涂装列表 key: 配件ID value: 涂装列表 [底漆，装饰，灯组，贴纸]
        public Dictionary<int, List<int>> buyEquipmentPaints; // 已购买的配件涂装列表 key: 配件ID value: 涂装列表

        public long shopRefreshTime;             // 商店出售装备配件刷新时间
        public Dictionary<int, int> shopSaleIds; // 商店出售装备配件列表
        public int shopLuckNum;                  // 商店出售装备配件刷新幸运值

        public long shopLimitTime;              // 商店限购商品购买记录刷新时间
        public Dictionary<int, int> shopLimits; // 商店限购商品购买记录 key: 商品ID value: 购买次数

        public float scoreDistanceMax; // 最远距离
        public long disMaxRefreshTime; // 最远距离刷新时间

        public float distanceRecord;  // 飞行距离最高纪录

        public Dictionary<int, int> additions; // 加成列表 key: 加成ID value: 加成等级

        public int flyNum; // 起飞次数

        public Dictionary<int, string[]> boxList; // 宝箱列表 key: 宝箱槽位 value: [宝箱ID,开箱时间]
        public int boxGetNum; // 宝箱获取次数
        public int boxGetFlyDis; // 宝箱获取累计飞行距离
        public int boxGetLevel; // 宝箱获取所属关卡ID

        public string signInfo;   // 签到信息
        public string raffleInfo; // 转盘信息
        public string taskInfo1;  // 每日任务信息
        public string taskInfo2;  // 成就任务信息
        
        public int isNewUser; // 新用户

        public int isLogin;      // 每日登录
        public long isLoginTime; // 每日登录刷新时间

        public Dictionary<int, int> landMarkInfo;  // 地标打卡列表 key: 地标ID value: 打卡奖励领取状态

        public List<int> settings; // 设置 [音乐，音效，震动，低能耗模式]

        public int isFirstLogin; // 首次登录

        [Preserve]
        public UserInfoData()
        {
            
        }
    }
}