using System.Collections.Generic;

namespace Data
{
    /// <summary>
    /// 全局变量管理类
    /// </summary>
    public static class GlobalValueManager
    {
        /** Json配置分类 */
        public static readonly Dictionary<string, List<string[]>> JsonConfig = new Dictionary<string, List<string[]>>
        {
            {
                "MainConfig", new List<string[]>
                {
                    // ---------------------------- 游戏配置表 ----------------------------
                    new[] { "GetEquipmentConfig", "0" }, // 部件配置表
                    new[] { "GetShopConfig", "1" }, // 商店配置表
                    new[] { "GetTrainConfig", "2" }, // 强化配置表
                    new[] { "GetRewardBox", "3" }, // 宝箱配置表
                    new[] { "GetProvinceConfig", "4" } // 省份配置表
                    // ---------------------------- 活动配置表 ----------------------------
                }
            }
        };

        /** Json数据表名称列表 */
        public static readonly Dictionary<string, string> JsonNameList = new Dictionary<string, string>
        {
            // ---------------------------- 游戏配置表 ----------------------------
            { "GetEquipmentConfig", "ComponentConfig" }, // 读取配置 部件配置表
            { "GetShopConfig", "ShopConfig" }, // 读取配置 商店配置表
            { "GetTrainConfig", "TrainConfig" }, // 读取配置 强化配置表
            { "GetRewardBox", "RewardBox" }, // 读取配置 宝箱配置表
            { "GetProvinceConfig", "ProvinceConfigList" } // 读取配置 省份配置表
            // ---------------------------- 活动配置表 ----------------------------
        };

#if UNITY_EDITOR || PF_WEB || UNITY_ANDROID
        // 本地测试CDN
        public static readonly string RemoteUrl = "https://cjkbx.nwxgt.cn/";
        /** 远程配置Url地址 公共配置 */
        public static readonly string RemoteConfigUrl_Public = RemoteUrl + "XX/Plane/Public/";
        /** 远程配置Url地址 游戏配置 */
        public static readonly string RemoteConfigUrl = RemoteUrl + "XX/Plane/Public/Config/V20250328_1/";
        /** 机器人头像 */
        public static readonly string HeadImageUrl = RemoteUrl + "XX/Public/NPCImage/";
        /** 旧版本微信或无法获得系统字体文件时的备选字体CDN URL */
        public static readonly string FallbackFontUrl = RemoteUrl + "XX/Public/FontCommon.ttf";
        /** 游戏圈按钮CDN URL */
        public static readonly string BtnGameClubUrl = RemoteUrl + "XX/Plane/Public/BtnGameClub.png";
#elif PF_WX

        #if SP_XX // ------------------------------------------ 相信 ------------------------------------------
            // 本地测试CDN
            public static readonly string RemoteUrl = "https://cjkbx.nwxgt.cn/";
            /** 远程配置Url地址 公共配置 */
            public static readonly string RemoteConfigUrl_Public = RemoteUrl + "XX/Plane/Public/";
            /** 远程配置Url地址 游戏配置 */
            public static readonly string RemoteConfigUrl = RemoteUrl + "XX/Plane/Public/Config/V20250328_1/";
            /** 机器人头像 */
            public static readonly string HeadImageUrl = RemoteUrl + "XX/Public/NPCImage/";
            /** 旧版本微信或无法获得系统字体文件时的备选字体CDN URL */
            public static readonly string FallbackFontUrl = RemoteUrl + "XX/Public/FontCommon.ttf";
            /** 游戏圈按钮CDN URL */
            public static readonly string BtnGameClubUrl = RemoteUrl + "XX/Plane/Public/BtnGameClub.png";
        #endif
        
        #if SP_ZS // ------------------------------------------ 指色 ------------------------------------------
            
        #endif
        
        #if SP_ZJ // ------------------------------------------ 正经 ------------------------------------------
            // 本地测试CDN
            public static readonly string RemoteUrl = "https://y.rattletrap.cn/";
            /** 远程配置Url地址 公共配置 */
            public static readonly string RemoteConfigUrl_Public = RemoteUrl + "BuildPlane/NoName/Config/";
            /** 远程配置Url地址 游戏配置 */
            public static readonly string RemoteConfigUrl = RemoteUrl + "BuildPlane/NoName/Config/V20250410_1/";
            /** 机器人头像 */
            public static readonly string HeadImageUrl = RemoteUrl + "BuildPlane/Public/NPCImage/";
            /** 旧版本微信或无法获得系统字体文件时的备选字体CDN URL */
            public static readonly string FallbackFontUrl = RemoteUrl + "BuildPlane/Public/FontCommon.ttf";
            /** 游戏圈按钮CDN URL */
            public static readonly string BtnGameClubUrl = RemoteUrl + "BuildPlane/Public/BtnGameClub.png";
        #endif
        
        #if SP_WG // ------------------------------------------ 挽弓 ------------------------------------------
            // 本地测试CDN
            public static readonly string RemoteUrl = "https://wrsds-res.chuxinhd.com/BuildPlane/Wx_WG/";
            /** 远程配置Url地址 公共配置 */
            public static readonly string RemoteConfigUrl_Public = RemoteUrl + "Config/";
            /** 远程配置Url地址 游戏配置 */
            public static readonly string RemoteConfigUrl = RemoteUrl + "Config/V20250510_1/";
            /** 机器人头像 */
            public static readonly string HeadImageUrl = RemoteUrl + "Public/NPCImage/";
            /** 旧版本微信或无法获得系统字体文件时的备选字体CDN URL */
            public static readonly string FallbackFontUrl = RemoteUrl + "Public/FontCommon.ttf";
            /** 游戏圈按钮CDN URL */
            public static readonly string BtnGameClubUrl = RemoteUrl + "Public/BtnGameClub.png";
        #endif
        
#elif PF_TT

        #if SP_XX // ------------------------------------------ 相信 ------------------------------------------
            // 本地测试CDN
            public static readonly string RemoteUrl = "https://cdn.ooxx.games/";
            /** 远程配置Url地址 公共配置 */
            public static readonly string RemoteConfigUrl_Public = RemoteUrl + "gao/BuildPlane/Config/";
            /** 远程配置Url地址 游戏配置 */
            public static readonly string RemoteConfigUrl = RemoteUrl + "gao/BuildPlane/Config/V20250416_1/";
            /** 机器人头像 */
            public static readonly string HeadImageUrl = RemoteUrl + "gao/Public/NPCImage/";
            /** 旧版本微信或无法获得系统字体文件时的备选字体CDN URL */
            public static readonly string FallbackFontUrl = RemoteUrl + "gao/Public/FontCommon.ttf";
            /** 游戏圈按钮CDN URL */
            public static readonly string BtnGameClubUrl = RemoteUrl + "gao/BuildPlane/BtnGameClub.png";
        #endif
        
        #if SP_ZS // ------------------------------------------ 指色 ------------------------------------------
            
        #endif
        
        #if SP_KD // ------------------------------------------ 快点 ------------------------------------------
            // 本地测试CDN
            public static readonly string RemoteUrl = "https://file-pgame.kidikidi.net/plane/";
            /** 远程配置Url地址 公共配置 */
            public static readonly string RemoteConfigUrl_Public = RemoteUrl + "PlaneBuild/Public/";
            /** 远程配置Url地址 游戏配置 */
            public static readonly string RemoteConfigUrl = RemoteUrl + "PlaneBuild/Public/Config/V20250402_1/";
            /** 机器人头像 */
            public static readonly string HeadImageUrl = RemoteUrl + "PlaneBuild/Public/NPCImage/";
            /** 旧版本微信或无法获得系统字体文件时的备选字体CDN URL */
            public static readonly string FallbackFontUrl = RemoteUrl + "PlaneBuild/Public/FontCommon.ttf";
            /** 游戏圈按钮CDN URL */
            public static readonly string BtnGameClubUrl = RemoteUrl + "PlaneBuild/Public/BtnGameClub.png";
        #endif
#endif
        
        // public static readonly string RemoteConfigUrl_Public = "https://res.wqop2018.com/partner/njxx/qsccc/BuildPlane/Public/";
        // public static readonly string RemoteConfigUrl = "https://res.wqop2018.com/partner/njxx/qsccc/BuildPlane/Public/Config/V20250310_1/";
        // public static readonly string HeadImageUrl = "https://res.wqop2018.com/partner/njxx/qsccc/BuildPlane/Public/NPCImage/";

        /** 机器人昵称 */
        public static readonly List<string> NickNames = new List<string>(30)
        {
            "醉酒的蝴蝶", "怎么没把你淹死", "小兮Cindy在路上", "哑巴说_爱", "粉红少女啤", "上学威龙", "智妍", "佩琪的夏天", 
            "静香爱洗澡", "不知道叫什么名字", "宕机少女", "瑶瑶", "阳阳", "Jonathan", "尹子", "做不完的作业",
            "淡茶一杯寄相思", "Hi!Chocolate", "Mangooo", "爱画画的小姐姐", "明眸水色", "千里共婵娟", "傲气一世", "城南大宝贝", 
            "哆啦有A梦", "妖精!_也行吧", "笑口常开", "烟斗男", "AAA莆田硬货", "魔人海星欧"
        };

        /** 飞机部件发光颜色列表 跟涂装一一对应 */
        public static readonly List<string> PlaneEquipLightColors = new List<string>(6)
        {
            "66CAFF", "A4FFA2", "F7FF79", "A486FF", "FF6DFB", "FF584C"
        };

        /** 使用道具增加时间 2分钟 */
        public static readonly int UseItemAddTime = 2;
        /** 复活增加时间 5分钟 */
        public static readonly int ReviveAddTime = 3;
        /** 游戏时间不足增加时间 2分钟 */
        public static readonly int LessTimeAddTime = 2;

        /** 每日免费挑战次数 3次 */
        public static readonly int FreeChallengeNum = 3;

        /** 地狱挑战通关人数 Key */
        public static readonly string DiYuChallengeName = "地狱模式通关人数";

        /** 初始装备中的配件 */
        public static readonly List<int> InitEquipments = new List<int>(6) { 1000, 2000, 3000, 4000, 5000, 6000 };
        /** 部件初始等级 */
        public static readonly int InitEquipmentLv = 1;

        /** 装备升星 消耗碎片基数 */
        public static readonly int EquipmentUpGradeChipNum = 10;
        /** 装备升星 消耗碎片提升数量 */
        public static readonly float EquipmentUpGradeChipUpGradeNum = 2;
        /** 装备升星 属性提高值  */
        public static readonly float EquipmentUpGradeNum = 1.4f;
        
        /** 升级基础消耗数量 弹弓强化/喷射推进板/推进环/金币增幅 */
        public static readonly int[] TrainBaseNums = { 100, 90, 100, 0 };
        /** 强化数值类型 弹弓强化/喷射推进板/推进环/金币增幅 */
        public static readonly int[] TrainNumTypes = { 1, 1, 1, 2 };

        /** 商店商品刷新时间 180分钟 */
        public static readonly int ShopRefreshTime = 180;

        /** 组装部件涂装ID列表 [类型，ID] */
        public static readonly int[][] BuildPaintIds =
        {
            new[] { 0, 4101, 4102, 4103, 4104, 4105, 4106, 4107, 4108, 4109, 4110, 4111, 4112, 4113 }, // 底漆颜色
            new[] { 0, 4201, 4202, 4203, 4204, 4205, 4206, 4207, 4208, 4209, 4210, 4211, 4212, 4113 }, // 装饰颜色
            new[] { 0, 4301, 4302, 4303, 4304, 4305, 4306 }, // LED灯组
            new[] { 0, 4401, 4402, 4403, 4404, 4405, 4406 } // 底漆贴纸
        };

        /** 邀请好友奖励数量 */
        public static readonly int CallRewardNum = 200;
        
        /** 收藏游戏(侧边栏)奖励数量 */
        public static readonly int FollowRewardNum = 200;

        /** 每日活跃度挡位ID列表 */
        public static readonly List<int> TaskDayActiveIds = new List<int>(5) { 101, 102, 103, 104, 105 };
        /** 每日活跃度宝箱领取需求活跃度 */
        public static readonly List<int> TaskDayActivePoints = new List<int>(5) { 10, 20, 40, 70, 100 };
        /** 每日活跃度奖励获得目标活跃度间隔列表 */
        public static readonly List<float> TaskDayActiveSubArr = new List<float>(5) { 10, 10, 20, 30, 30 };
        
        /** 免费抽奖冷却时间 */
        public static readonly int RaffleFreeTime = 30;

        /** 宝箱获取品质列表 */
        public static readonly int[][] RewardBoxGetQualitys =
        {
            new[] { 1, 1, 1, 1, 2 },
            new[] { 1, 1, 2 },
            new[] { 1, 1, 2, 1, 1, 2, 1, 1, 3 },
            new[] { 1, 1, 2, 1, 2, 3 },
            new[] { 1, 2, 1, 2, 3 }
        };

        /** 地标ID列表 */
        public static readonly List<List<int>> _landMarkIds = new List<List<int>>
        {
            new List<int> { 0, 1, 2, 3, 4 },      // (城市ID: 1) 南京
            new List<int> { 5, 6, 7, 8, 9 },      // (城市ID: 2) 成都
            new List<int> { 10, 11, 12, 13, 14 }, // (城市ID: 3) 哈尔滨
            new List<int> { 15, 16, 17, 18, 19 }, // (城市ID: 4) 北京
            new List<int> { 20, 21, 22, 23, 24 }, // (城市ID: 5) 广州
            new List<int> { 25, 26, 27, 28, 29 }, // (城市ID: 6) 西安
            new List<int> { 30, 31, 32, 33, 34 }, // (城市ID: 7) 上海
            new List<int> { 35, 36, 37, 38, 39 }, // (城市ID: 8) ？？？
            new List<int> { 40, 41, 42, 43, 44 }, // (城市ID: 9) ？？？
            new List<int> { 45, 46, 47, 48, 49 }, // (城市ID: 10) ？？？
            new List<int> { 50, 51, 52, 53, 54 }, // (城市ID: 11) ？？？
            new List<int> { 55, 56, 57, 58, 59 }, // (城市ID: 12) ？？？
            new List<int> { 60, 61, 62, 63, 64 }, // (城市ID: 13) ？？？
            new List<int> { 65, 66, 67, 68, 69 }, // (城市ID: 14) ？？？
            new List<int> { 70, 71, 72, 73, 74 }  // (城市ID: 15) ？？？
        };

        /** 抖音直玩ContentId Scene: 1 离线收益场景 转盘 */
        public static readonly string _ttFeedContentId_1 = "CONTENT629610754";
        /** 抖音直玩ContentId Scene: 2 体力恢复场景 签到 */
        public static readonly string _ttFeedContentId_2 = "CONTENT609642242";
        /** 抖音直玩ContentId Scene: 3 重要事件场景 开箱 */
        public static readonly string _ttFeedContentId_3 = "CONTENT629300738";
        
    }
}