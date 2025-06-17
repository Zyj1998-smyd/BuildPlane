using System;
using System.Collections.Generic;
using System.Text;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Data.ClassData;
using Data.ConfigData;
using GamePlay.Main;
using Newtonsoft.Json;
using Platform;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    /// <summary>
    /// 全局数据管理类
    /// </summary>
    public static class DataHelper
    {
        /** 当前用户OpenId */
        public static string CurOpenId = "";
        /** 当前用户登录渠道 */
        public static string CurUserChannel = "";

        /** 当前用户角色ID */
        public static string CurRoleId = "";

        /** 省份排行榜 [省份名称，通关次数] */
        public static Dictionary<string, int> ProvinceRanks = new Dictionary<string, int>();

        /** 服务器存储的省份通关数据 [省份名称，通关次数] */
        public static List<KeyValuePair<string, int>> GeneralRanks = new List<KeyValuePair<string, int>>();

        /** 游戏冷启动 */
        public static bool isRootLoad;

        /** 当前用户数据 */
        public static UserInfoData CurUserInfoData;
        
        /** 加载的场景名称 */
        public static string nextSceneName;

        /** 当前游戏分享使用状态 */
        public static bool CurGameShareUsed;

        /** 通用(假)分享判断时间 */
        public static long ShareTime;
        
        /** 当前关卡模式 0: 关卡模式 1: 挑战模式 2: 地狱模式 */
        public static int CurLevelModel;

        /** 当前所在省份排名 */
        public static int CurProvinceRankNum;

        /** 当前地狱挑战通关人数 */
        public static int CurDiYuChallengNum;

        /** 当前待更新数据的Key列表 */
        public static readonly List<string> CurModifyKeys = new List<string>();

        /** 当前上报省份排行榜数值 */
        public static int CurUpdataProvinceNum;
        
        /** 全国排行榜 通关榜 */
        public static List<RankLevelUserData> RankLevelUserDatas = new List<RankLevelUserData>();

        /** 全国排行榜 积分榜 */
        public static List<RankScoreUserData> RankScoreUserDatas = new List<RankScoreUserData>();

        /** 全国排行榜 飞行距离榜 */
        public static List<RankDisUserData> RankDisUserDatas = new List<RankDisUserData>();
        
        /** 获取用户信息时点击状态(仅字节生效) */
        public static bool GetUserInfoClick;

        /** 当前运行场景 */
        public static string CurRunScene;

        /** 当前获得的物品 [类型,ID,数量] */
        public static int[] CurGetItem;

        /** 当前打开宝箱 [宝箱槽位,宝箱ID] */
        public static int[] CurOpenBoxInfo;

        /** 不是新手引导首次进入主场景主动弹出签到 */
        public static bool SignAutoPop;

        /** 当前升级的装备ID */
        public static int CurUpGradeEquipment;

        /** 当前选择的关卡 */
        public static int CurLevelNum;
        
        /** 上一局选择的关卡 */
        public static int LastLevelNum;

        /** GM开关 每次飞行都获得箱子 */
        public static bool GmSwitch_GetBox;
        /** GM开关 解锁全部地图 */
        public static bool GmSwitch_UnlockAllMap;
        /** GM开关 直接跳过广告 */
        public static bool GmSwitch_FreeVideo;

        /** 真实当前最高关卡(GM解锁地图开关还原时用) */
        public static int RealCurLevelNum;

        /** 当前游戏启动场景 */
        public static int CurLaunchSceneId;

        /** 数据回传 字节启动参数1 */
        public static string CurLaunchQueryClickid;
        /** 数据回传 字节启动参数2 */
        public static string CurLaunchQueryAadvid;

        /** 当前上报的DataFinder属性 广告激励 */
        public static string CurReportDf_adScene;
        /** 当前上报的DataFinder属性 异常[异常场景，异常描述] */
        public static string[] CurReportDf_error;

        /** 当前运行的GM配置 */
        public static GmConfig CurRunGmConfig;

        /** 初始化游戏数据 */
        public static void InitGameData()
        {
            // 初始化游戏分享使用状态
            CurGameShareUsed = true;
            // 初始化签到主动弹出状态
            SignAutoPop = false;
            // 初始化当前升级的装备ID
            CurUpGradeEquipment = -1;
            // 初始化当前选择的关卡
            CurLevelNum = CurUserInfoData.curLevelNum;
            // 初始化上一局选择的关卡
            LastLevelNum = -1;
            // 初始化开关
            AudioHandler._instance.InitAudioSet(0); // 背景音乐
            AudioHandler._instance.InitAudioSet(1); // 音效
            AudioHandler._instance.InitAudioSet(2); // 震动
            AudioHandler._instance.InitAudioSet(3); // 低能耗模式
            // 初始化GM开关
            GmSwitch_GetBox = false;
            GmSwitch_UnlockAllMap = false;
            GmSwitch_FreeVideo = false;
            // 初始化真实当前最高关卡
            RealCurLevelNum = CurUserInfoData.curLevelNum;
        }

        #region 游戏数据
        
        /// <summary>
        /// 更新本地存储数据(用户云存储数据)
        /// </summary>
        /// <param name="modifyKeys">待更新数据的Key列表</param>
        /// <param name="cb">回调</param>
        public static void ModifyLocalData(List<string> modifyKeys, Action cb)
        {
            CurModifyKeys.Clear();
            for (int i = 0; i < modifyKeys.Count; i++)
            {
                CurModifyKeys.Add(modifyKeys[i]);
            }

            GameSdkManager._instance._serverScript.UpdateServerData(cb);
        }

        /// <summary>
        /// 更新管理员云存储数据
        /// <param name="cb">回调</param>
        /// </summary>
        public static void ModifyAdminCloudData(Action cb)
        {
        }

        /// <summary>
        /// 清空用户数据
        /// </summary>
        public static void ClearUserInfoData()
        {
            List<string> modifyKeys = new List<string>();
            CurUserInfoData.userName = "";
            modifyKeys.Add("userName");
            CurUserInfoData.userAvatar = "";
            modifyKeys.Add("userAvatar");
            CurUserInfoData.gold = 0;
            modifyKeys.Add("gold");
            CurUserInfoData.diamond = 0;
            modifyKeys.Add("diamond");
            CurUserInfoData.curLevelNum = 1;
            modifyKeys.Add("curLevelNum");
            CurUserInfoData.scoreNum = 0;
            modifyKeys.Add("scoreNum");
            CurUserInfoData.isLogin = 0;
            modifyKeys.Add("isLogin");
            CurUserInfoData.isLoginTime = ToolFunManager.GetCurrTime();
            modifyKeys.Add("isLoginTime");
            CurUserInfoData.addedToMyMiniProgramGet = 0;
            modifyKeys.Add("addedToMyMiniProgramGet");
            CurUserInfoData.callRewardGet = 0;
            modifyKeys.Add("callRewardGet");
            CurUserInfoData.equipEquipments = new List<int>(GlobalValueManager.InitEquipments);
            modifyKeys.Add("equipEquipments");
            CurUserInfoData.equipments = new Dictionary<int, int>();
            for (int i = 0; i < GlobalValueManager.InitEquipments.Count; i++)
            {
                CurUserInfoData.equipments.Add(GlobalValueManager.InitEquipments[i], 1);
            }
            modifyKeys.Add("equipments");
            CurUserInfoData.equipmentChips = new Dictionary<int, int>();
            modifyKeys.Add("equipmentChips");
            CurUserInfoData.equipmentPaintNews = new Dictionary<int, int>();
            modifyKeys.Add("equipmentPaintNews");
            CurUserInfoData.equipmentPaints = new Dictionary<int, List<int>>();
            modifyKeys.Add("equipmentPaints");
            CurUserInfoData.buyEquipmentPaints = new Dictionary<int, List<int>>();
            modifyKeys.Add("buyEquipmentPaints");
            CurUserInfoData.shopRefreshTime = ToolFunManager.GetCurrTime();
            modifyKeys.Add("shopRefreshTime");
            CurUserInfoData.shopSaleIds = GetShopSaleList(1, 1);
            modifyKeys.Add("shopSaleIds");
            CurUserInfoData.shopLuckNum = 1;
            modifyKeys.Add("shopLuckNum");
            CurUserInfoData.scoreDistanceMax = 0;
            modifyKeys.Add("scoreDistanceMax");
            CurUserInfoData.disMaxRefreshTime = ToolFunManager.GetCurrTime();
            modifyKeys.Add("disMaxRefreshTime");
            CurUserInfoData.additions = new Dictionary<int, int> { { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 } };
            modifyKeys.Add("additions");
            CurUserInfoData.flyNum = 0;
            modifyKeys.Add("flyNum");
            CurUserInfoData.boxList = new Dictionary<int, string[]>();
            for (int i = 0; i < 5; i++)
            {
                CurUserInfoData.boxList.Add(i, null);
            }
            modifyKeys.Add("boxsList");
            CurUserInfoData.boxGetNum = 0;
            modifyKeys.Add("boxGetNum");
            CurUserInfoData.boxGetFlyDis = 0;
            modifyKeys.Add("boxGetFlyDis");
            CurUserInfoData.boxGetLevel = 1;
            modifyKeys.Add("boxGetLevel");
            CurUserInfoData.shopLimits = new Dictionary<int, int>();
            modifyKeys.Add("shopLimits");
            CurUserInfoData.shopLimitTime = ToolFunManager.GetCurrTime();
            modifyKeys.Add("shopLimitTime");
            CurUserInfoData.signInfo = JsonConvert.SerializeObject(new SignInfoData { dayStamp = ToolFunManager.GetCurrTime(), day = 1, isSign = 0 });
            modifyKeys.Add("signInfo");
            CurUserInfoData.raffleInfo = JsonConvert.SerializeObject(new RaffleInfoData { dayStamp = ToolFunManager.GetCurrTime(), lastFreeTime = 0, luckNum = 0 });
            modifyKeys.Add("raffleInfo");
            CurUserInfoData.taskInfo1 = JsonConvert.SerializeObject(new TaskDailyInfoData
            {
                dayStamp = ToolFunManager.GetCurrTime(),
                activePoint = 0,
                rewardGet = new List<int>(5) { 0, 0, 0, 0, 0 },
                taskState = new Dictionary<int, int>(10) { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 }, { 10, 0 } }
            });
            modifyKeys.Add("taskInfo1");
            CurUserInfoData.taskInfo2 = JsonConvert.SerializeObject(new Dictionary<int, int[]>());
            modifyKeys.Add("taskInfo2");
            CurUserInfoData.isNewUser = 0;
            modifyKeys.Add("isNewUser");
            CurUserInfoData.landMarkInfo = new Dictionary<int, int>();
            modifyKeys.Add("landMarkInfo");
            CurUserInfoData.settings = new List<int>(4) { 1, 1, 1, 0 };
            modifyKeys.Add("settings");
            CurUserInfoData.feedSubGet = 0;
            modifyKeys.Add("feedSubGet");
            CurUserInfoData.distanceRecord = 0;
            modifyKeys.Add("distanceRecord");
            
            ModifyLocalData(modifyKeys, () => { });
        }
        
        #endregion

        #region 省份

        /// <summary>
        /// 省份排序
        /// </summary>
        public static List<string> GetProvinceSort()
        {
            List<string> provinceNames = new List<string>(ProvinceRanks.Count);
            List<int> provinceNums = new List<int>(ProvinceRanks.Count);
            foreach (var provinceRank in ProvinceRanks)
            {
                provinceNames.Add(provinceRank.Key);
                provinceNums.Add(provinceRank.Value);
            }
            
            string provinceName;
            int provinceNum;
            for (int i = 0; i < provinceNums.Count; i++)
            {
                for (int j = 0; j < provinceNums.Count - i - 1; j++)
                {
                    if (provinceNums[j] < provinceNums[j + 1])
                    {
                        provinceNum = provinceNums[j];
                        provinceNums[j] = provinceNums[j + 1];
                        provinceNums[j + 1] = provinceNum;
                        provinceName = provinceNames[j];
                        provinceNames[j] = provinceNames[j + 1];
                        provinceNames[j + 1] = provinceName;
                    }
                }
            }

            return new List<string>(2)
                { JsonConvert.SerializeObject(provinceNames), JsonConvert.SerializeObject(provinceNums) };
        }

        #endregion

        #region 全国排行榜

        /// <summary>
        /// 全国排行榜排序
        /// </summary>
        /// <param name="type">榜单类型 0:通关榜 1:积分榜 2:飞行距离榜</param>
        public static string RankAllSort(int type)
        {
            switch (type)
            {
                case 0: // 通关榜
                {
                    List<RankLevelUserData> levelUserDatas = new List<RankLevelUserData>();
                    for (int i = 0; i < RankLevelUserDatas.Count; i++)
                    {
                        levelUserDatas.Add(RankLevelUserDatas[i]);
                    }

                    RankLevelUserData levelUserData;
                    for (int i = 0; i < levelUserDatas.Count; i++)
                    {
                        for (int j = 0; j < levelUserDatas.Count - i - 1; j++)
                        {
                            if (levelUserDatas[j].levelNum < levelUserDatas[j + 1].levelNum)
                            {
                                levelUserData = levelUserDatas[j];
                                levelUserDatas[j] = levelUserDatas[j + 1];
                                levelUserDatas[j + 1] = levelUserData;
                            }
                        }
                    }

                    return JsonConvert.SerializeObject(levelUserDatas);
                }
                case 1: // 积分榜
                {
                    List<RankScoreUserData> levelUserDatas = new List<RankScoreUserData>();
                    for (int i = 0; i < DataHelper.RankScoreUserDatas.Count; i++)
                    {
                        levelUserDatas.Add(DataHelper.RankScoreUserDatas[i]);
                    }

                    RankScoreUserData levelUserData;
                    for (int i = 0; i < levelUserDatas.Count; i++)
                    {
                        for (int j = 0; j < levelUserDatas.Count - i - 1; j++)
                        {
                            if (levelUserDatas[j].scoreNum < levelUserDatas[j + 1].scoreNum)
                            {
                                levelUserData = levelUserDatas[j];
                                levelUserDatas[j] = levelUserDatas[j + 1];
                                levelUserDatas[j + 1] = levelUserData;
                            }
                        }
                    }

                    return JsonConvert.SerializeObject(levelUserDatas);
                }
                case 2: // 飞行距离榜
                {
                    List<RankDisUserData> list = new List<RankDisUserData>();
                    for (int i = 0; i < RankDisUserDatas.Count; i++)
                    {
                        list.Add(RankDisUserDatas[i]);
                    }

                    RankDisUserData disUserData;
                    for (int i = 0; i < list.Count; i++)
                    {
                        for (int j = 0; j < list.Count - i - 1; j++)
                        {
                            if (list[j].distance < list[j + 1].distance)
                            {
                                disUserData = list[j];
                                list[j] = list[j + 1];
                                list[j + 1] = disUserData;
                            }
                        }
                    }

                    return JsonConvert.SerializeObject(list);
                }
            }

            return "";
        }
        
        /// <summary>
        /// 更新全国排行榜
        /// </summary>
        /// <param name="type">榜单类型 0:通关榜 1:积分榜 2:飞行距离榜</param>
        public static void UpdateRankAll(int type)
        {
            switch (type)
            {
                case 0: // 通关榜
                {
                    bool isMeInRank = false;
                    for (int i = 0; i < RankLevelUserDatas.Count; i++)
                    {
                        if (RankLevelUserDatas[i].openId == CurOpenId)
                        {
                            isMeInRank = true;
                            // 当前已上榜 更新榜单数值
                            RankLevelUserDatas[i].levelNum = CurUserInfoData.curLevelNum;
                            break;
                        }
                    }

                    // 当前未上榜 判断能否上榜
                    if (!isMeInRank)
                    {
                        string rankData = RankAllSort(0);
                        List<RankLevelUserData> list = JsonConvert.DeserializeObject<List<RankLevelUserData>>(rankData);
                        bool isCanInRank = true;
                        if (list.Count > 30)
                        {
                            // 排行榜长度大于30 移除最后一名
                            RankLevelUserData rank = list[^1];
                            if (CurUserInfoData.curLevelNum > rank.levelNum)
                            {
                                for (int i = 0; i < RankLevelUserDatas.Count; i++)
                                {
                                    if (RankLevelUserDatas[i].openId == rank.openId)
                                    {
                                        RankLevelUserDatas.Remove(RankLevelUserDatas[i]);
                                        break;
                                    }
                                }
                            }
                            else isCanInRank = false;
                        }
                        // 上榜
                        if (isCanInRank)
                        {
                            RankLevelUserDatas.Add(new RankLevelUserData
                            {
                                openId = CurOpenId,
                                nickName = CurUserInfoData.userName,
                                userAvatar = CurUserInfoData.userAvatar,
                                levelNum = CurUserInfoData.curLevelNum
                            });
                        }
                    }

                    break;
                }
                case 1: // 积分榜
                {
                    bool isMeInRank = false;
                    for (int i = 0; i < RankScoreUserDatas.Count; i++)
                    {
                        if (RankScoreUserDatas[i].openId == CurOpenId)
                        {
                            isMeInRank = true;
                            // 当前已上榜 更新榜单数值
                            RankScoreUserDatas[i].scoreNum = CurUserInfoData.scoreNum;
                            break;
                        }
                    }

                    // 当前未上榜 判断能否上榜
                    if (!isMeInRank)
                    {
                        string rankData = RankAllSort(1);
                        List<RankScoreUserData> list = JsonConvert.DeserializeObject<List<RankScoreUserData>>(rankData);
                        bool isCanInRank = true;
                        if (list.Count > 30)
                        {
                            // 排行榜长度大于30 移除最后一名
                            RankScoreUserData rank = list[^1];
                            if (CurUserInfoData.scoreNum > rank.scoreNum)
                            {
                                for (int i = 0; i < RankScoreUserDatas.Count; i++)
                                {
                                    if (RankScoreUserDatas[i].openId == rank.openId)
                                    {
                                        RankScoreUserDatas.Remove(RankScoreUserDatas[i]);
                                        break;
                                    }
                                }
                            }
                            else isCanInRank = false;
                        }
                        // 上榜
                        if (isCanInRank)
                        {
                            RankScoreUserDatas.Add(new RankScoreUserData
                            {
                                openId = CurOpenId,
                                nickName = CurUserInfoData.userName,
                                userAvatar = CurUserInfoData.userAvatar,
                                scoreNum = CurUserInfoData.scoreNum
                            });
                        }
                    }

                    break;
                }
                case 2: // 飞行距离榜
                {
                    bool isMeInRank = false;
                    for (int i = 0; i < RankDisUserDatas.Count; i++)
                    {
                        if (RankDisUserDatas[i].openId == CurOpenId)
                        {
                            isMeInRank = true;
                            // 当前已上榜 更新榜单数值
                            RankDisUserDatas[i].distance = CurUserInfoData.scoreDistanceMax;
                            break;
                        }
                    }
                    
                    // 当前未上榜 判断能否上榜
                    if (!isMeInRank)
                    {
                        string rankData = RankAllSort(2);
                        List<RankDisUserData> list = JsonConvert.DeserializeObject<List<RankDisUserData>>(rankData);
                        bool isCanInRank = true;
                        if (list.Count > 30)
                        {
                            // 排行榜长度大于30 移除最后一名
                            RankDisUserData rank = list[^1];
                            if (CurUserInfoData.scoreDistanceMax > rank.distance)
                            {
                                for (int i = 0; i < RankDisUserDatas.Count; i++)
                                {
                                    if (RankDisUserDatas[i].openId == rank.openId)
                                    {
                                        RankDisUserDatas.Remove(RankDisUserDatas[i]);
                                        break;
                                    }
                                }
                            }
                            else isCanInRank = false;
                        }
                        // 上榜
                        if (isCanInRank)
                        {
                            RankDisUserDatas.Add(new RankDisUserData
                            {
                                openId = CurOpenId,
                                nickName = CurUserInfoData.userName,
                                userAvatar = CurUserInfoData.userAvatar,
                                distance = CurUserInfoData.scoreDistanceMax,
                                planeEquipments = CurUserInfoData.equipEquipments,
                                userProvince = CurUserInfoData.userProvince,
                                planeEquipmentColor = GetEquipEquipmentColor(),
                                planeEquipmentLvs = new List<int>(6)
                                {
                                    CurUserInfoData.equipments.GetValueOrDefault(CurUserInfoData.equipEquipments[0], 1),
                                    CurUserInfoData.equipments.GetValueOrDefault(CurUserInfoData.equipEquipments[1], 1),
                                    CurUserInfoData.equipments.GetValueOrDefault(CurUserInfoData.equipEquipments[2], 1),
                                    CurUserInfoData.equipments.GetValueOrDefault(CurUserInfoData.equipEquipments[3], 1),
                                    CurUserInfoData.equipments.GetValueOrDefault(CurUserInfoData.equipEquipments[4], 1),
                                    CurUserInfoData.equipments.GetValueOrDefault(CurUserInfoData.equipEquipments[5], 1)
                                }
                            });
                        }
                    }

                    break;
                }
            }
        }

        #endregion
        
        #region 获取变量

        /// <summary>
        /// 获取当前运行的场景播放的背景音乐名称
        /// </summary>
        public static string GetCurSceneBgmName()
        {
            string bgmName = "";
            switch (CurRunScene)
            {
                case "Main":
                    bgmName = "BgmMain";
                    break;
                case "Battle":
                    bgmName = "BgmBattle";
                    break;
            }

            return bgmName;
        }

        /// <summary>
        /// 获取商店出售商品列表
        /// </summary>
        public static Dictionary<int, int> GetShopSaleList(int levelNumTmp = -1, int luckNumTmp = -1)
        {
            int curLevelNum = levelNumTmp == -1 ? CurUserInfoData.curLevelNum : levelNumTmp;
            if (curLevelNum >= 5) curLevelNum = 5;
            
            int luckNum = luckNumTmp == -1 ? CurUserInfoData.shopLuckNum : luckNumTmp;
            if (luckNum >= 20) luckNum = 20;
            
            ShopItemConfig shopItemConfig = ConfigManager.Instance.ShopItemConfigDict[curLevelNum];
            ShopItemConfig shopItemConfig_Change = ConfigManager.Instance.ShopItemConfigDict[(curLevelNum + 100)];

            int[] qualityNums = new int[5];
            qualityNums[0] = shopItemConfig.Quality1 + (shopItemConfig_Change.Quality1 * luckNum);
            qualityNums[1] = shopItemConfig.Quality2 + (shopItemConfig_Change.Quality2 * luckNum);
            qualityNums[2] = shopItemConfig.Quality3 + (shopItemConfig_Change.Quality3 * luckNum);
            qualityNums[3] = shopItemConfig.Quality4 + (shopItemConfig_Change.Quality4 * luckNum);
            qualityNums[4] = shopItemConfig.Quality5 + (shopItemConfig_Change.Quality5 * luckNum);

            int totalWeight = 0;
            for (int i = 0; i < qualityNums.Length; i++)
            {
                totalWeight += qualityNums[i];
            }

            List<int> newWeightArr = new List<int>();
            int n = 0;
            while (newWeightArr.Count != qualityNums.Length)
            {
                int newWeight = 0;
                for (int i = 0; i < qualityNums.Length; i++)
                {
                    if (i <= n)
                    {
                        newWeight += qualityNums[i];
                    }
                }

                newWeightArr.Add(newWeight);
                n += 1;
            }

            ConfigManager.Instance.ConsoleLog(0, new StringBuilder("配置表权重列表 = " + JsonConvert.SerializeObject(qualityNums)).ToString());
            ConfigManager.Instance.ConsoleLog(0, new StringBuilder("计算后权重列表 = " + JsonConvert.SerializeObject(newWeightArr)).ToString());

            List<int> selectQualityNums = new List<int>();
            while (selectQualityNums.Count < 9)
            {
                int ranWeight = Random.Range(0, totalWeight);
                int index = 0;
                while (index != newWeightArr.Count)
                {
                    if (ranWeight < newWeightArr[index]) break;
                    index += 1;
                }

                selectQualityNums.Add(index);
            }

            ConfigManager.Instance.ConsoleLog(0, new StringBuilder("选出的品质列表 = " + JsonConvert.SerializeObject(selectQualityNums)).ToString());

            Dictionary<int, int> ids = new Dictionary<int, int>();
            while (ids.Count < selectQualityNums.Count)
            {
                int qualityTmp = selectQualityNums[ids.Count];
                int index = Random.Range(0, ConfigManager.Instance.ComponentQualityConfigDict[qualityTmp].Count);
                ComponentConfig config = ConfigManager.Instance.ComponentQualityConfigDict[qualityTmp][index];
                int id = config.ID;
                if (!ids.ContainsKey(id))
                {
                    ids.Add(id, 1);
                }
            }
            
            // 保底机制 完整的四叶草的下一次刷新 幸运值为 5, 9, 13, 17, 21 时判断触发
            List<int> luckNumsTmp = new List<int>(5) { 5, 9, 13, 17, 21 };
            int realLuckNum = luckNumTmp == -1 ? CurUserInfoData.shopLuckNum : luckNum;
            if (luckNumsTmp.Contains(realLuckNum))
            {
                ConfigManager.Instance.ConsoleLog(0, "触发保底判断");
                int qualityMax = 0;
                if (qualityNums[4] != 0) qualityMax = 4;
                else if (qualityNums[3] != 0) qualityMax = 3;
                else if (qualityNums[2] != 0) qualityMax = 2;
                else if (qualityNums[1] != 0) qualityMax = 1;
                else if (qualityNums[0] != 0) qualityMax = 0;
                ConfigManager.Instance.ConsoleLog(0, new StringBuilder("本次触发保底 可获取的最高品质 = " + qualityMax).ToString());

                bool isGetMaxQulity = selectQualityNums.Contains(qualityMax);
                if (!isGetMaxQulity)
                {
                    ConfigManager.Instance.ConsoleLog(0, "保底触发生效");
                    int index = Random.Range(0, ConfigManager.Instance.ComponentQualityConfigDict[qualityMax].Count);
                    ComponentConfig config = ConfigManager.Instance.ComponentQualityConfigDict[qualityMax][index];
                    List<int> keys = new List<int>(ids.Keys);
                    int removeKey = keys[Random.Range(0, keys.Count)];
                    ids.Remove(removeKey);
                    ids.Add(config.ID, 1);
                }
            }

            ConfigManager.Instance.ConsoleLog(0, new StringBuilder("最终返回列表 = " + JsonConvert.SerializeObject(ids)).ToString());

            return ids;
        }

        /// <summary>
        /// 获取结算金币超级加倍倍率
        /// </summary>
        /// <param name="posX">箭头停止位置</param>
        /// <returns>倍率</returns>
        public static int GetAccountMultRatio(float posX)
        {
            if (posX >= -75 && posX <= 75) return 5;
            if ((posX >= -300 && posX < -75) || (posX > 75 && posX <= 300)) return 3;
            if ((posX >= -550 && posX < -300) || (posX > 300 && posX <= 550)) return 2;
            return 1;
        }

        /// <summary>
        /// 获取可用宝箱槽位
        /// </summary>
        public static int GetEmptyBoxSlot()
        {
            int slot = -1;
            foreach (var data in CurUserInfoData.boxList)
            {
                if (data.Value == null)
                {
                    slot = data.Key;
                    break;
                }
            }

            return slot;
        }

        /// <summary>
        /// 获取机翼平衡值
        /// </summary>
        /// <param name="wingL">左机翼ID</param>
        /// <param name="wingR">右机翼ID</param>
        /// <returns>机翼平衡值</returns>
        public static bool GetPlaneWingBalance(int wingL = -1, int wingR = -1)
        {
            int wingId_L = wingL == -1 ? DataHelper.CurUserInfoData.equipEquipments[2] : wingL;
            int wingId_R = wingR == -1 ? DataHelper.CurUserInfoData.equipEquipments[3] : wingR;

            // Debug.Log("wingId_L = " + wingId_L);
            // Debug.Log("wingId_R = " + wingId_R);
            
            if (wingId_L == -1 && wingId_R == -1) return true;
            if (wingId_L == -1 && wingId_R != -1) return false;
            if (wingId_L != -1 && wingId_R == -1) return false;
            
            // float wingWeightL = ConfigManager.Instance.ComponentConfigDict[wingId_L].ZhongLiang;
            // float wingWeightR = ConfigManager.Instance.ComponentConfigDict[wingId_R].ZhongLiang;
            
            // Debug.Log("wingWeightL = " + wingWeightL);
            // Debug.Log("wingWeightR = " + wingWeightR);
            
            // if (wingWeightL == 0 && wingWeightR == 0) return 0; // 左右机翼都没有装备 平衡
            // if (Mathf.Approximately(wingWeightL, wingWeightR)) return 0; // 左右机翼重量相等 平衡

            // 左右机翼重量总和
            // float total = wingWeightL + wingWeightR;
            // 如果b大于a，则结果为负；如果a大于b，则结果为正
            // float proportion = (wingWeightL - wingWeightR) / total;

            float proportion = (wingId_L % 1000) == (wingId_R % 1000) ? 0 : 1;

            return !(proportion > 0 || proportion < 0);
        }

        /// <summary>
        /// 获取当前装备中的飞机部件涂装
        /// </summary>
        /// <param name="idTmp">部件ID</param>
        /// <returns>[LightColor, MainTex1, MainTex2, MatcapTex1, MatcapTex2]</returns>
        public static List<string> GetPlaneEquipmentColor(int idTmp)
        {
            List<string> result = new List<string>(5);
            List<int> equipmentPaints = CurUserInfoData.equipmentPaints.GetValueOrDefault(idTmp, new List<int> { 0, 0, 0, 0 });

            // Debug.Log(JsonConvert.SerializeObject(equipmentPaints));
            
            int level = CurUserInfoData.equipments.GetValueOrDefault(idTmp, 1);
            if (level >= 3)
            {
                if (equipmentPaints[2] == 0)
                {
                    result.Add("000000");
                }
                else
                {
                    int colorId = equipmentPaints[2] % 100 - 1;
                    result.Add(GlobalValueManager.PlaneEquipLightColors[colorId]);
                }
            }
            else
            {
                result.Add("000000");
            }

            switch (level)
            {
                case 1:  // 木质 固定组合
                    result.AddRange(new List<string>(4) { "MainTex0_0", "MainTex0_0", "MatcapTex0_0", "MatcapTex0_1" });
                    break;
                case 2:  // 底漆/装饰各自读取
                case 3:  // 底漆/装饰各自读取
                    List<string> listTmp1 = new List<string>(4) { "", "", "", "" };
                    // 底漆
                    if (equipmentPaints[0] == 0)
                    {
                        // 透明底漆 显示默认木质
                        listTmp1[0] = "MainTex0_0";
                        listTmp1[2] = "MatcapTex0_0";
                    }
                    else
                    {
                        // 非透明底漆 显示底漆颜色
                        int colorId_1 = equipmentPaints[0] % 100 - 1;
                        string matcapTexName_1 = new StringBuilder("MatcapTex1_" + colorId_1).ToString();
                        listTmp1[0] = "MainTex1_0";
                        listTmp1[2] = matcapTexName_1;
                    }
                    // 装饰
                    if (equipmentPaints[1] == 0)
                    {
                        // 透明装饰 显示默认木质
                        listTmp1[1] = "MainTex0_0";
                        listTmp1[3] = "MatcapTex0_1";
                    }
                    else
                    {
                        // 非透明装饰 显示装饰颜色
                        int colorId_2 = equipmentPaints[1] % 100 - 1;
                        string matcapTexName_2 = new StringBuilder("MatcapTex1_" + colorId_2).ToString();
                        listTmp1[1] = "MainTex1_0";
                        listTmp1[3] = matcapTexName_2;
                    }

                    result.AddRange(listTmp1);
                    break;
                default: // 底漆贴纸
                    if (equipmentPaints[3] == 0)
                    {
                        // 当前装备中的是透明贴纸
                        List<string> listTmp2 = new List<string>(4) { "", "", "", "" };
                        // 底漆
                        if (equipmentPaints[0] == 0)
                        {
                            // 透明底漆 显示默认木质
                            listTmp2[0] = "MainTex0_0";
                            listTmp2[2] = "MatcapTex0_0";
                        }
                        else
                        {
                            // 非透明底漆 显示底漆颜色
                            int colorId_1 = equipmentPaints[0] % 100 - 1;
                            string matcapTexName_1 = new StringBuilder("MatcapTex1_" + colorId_1).ToString();
                            listTmp2[0] = "MainTex1_0";
                            listTmp2[2] = matcapTexName_1;
                        }
                        // 装饰
                        if (equipmentPaints[1] == 0)
                        {
                            // 透明装饰 显示默认木质
                            listTmp2[1] = "MainTex0_0";
                            listTmp2[3] = "MatcapTex0_1";
                        }
                        else
                        {
                            // 非透明装饰 显示装饰颜色
                            int colorId_2 = equipmentPaints[1] % 100 - 1;
                            string matcapTexName_2 = new StringBuilder("MatcapTex1_" + colorId_2).ToString();
                            listTmp2[1] = "MainTex1_0";
                            listTmp2[3] = matcapTexName_2;
                        }
                        result.AddRange(listTmp2);
                    }
                    else
                    {
                        // 当前装备中的不是透明贴纸 显示底漆贴纸
                        List<string> listTmp2 = new List<string>(4) { "", "", "", "" };
                        // 贴纸
                        int colorId = equipmentPaints[3] % 100 - 1;
                        string mainTexName = new StringBuilder("MainTex2_" + colorId).ToString();
                        listTmp2[0] = mainTexName;
                        // 底漆
                        if (equipmentPaints[0] == 0)
                        {
                            // 透明底漆 显示默认木质
                            listTmp2[2] = "MatcapTex0_0";
                        }
                        else
                        {
                            // 非透明底漆 显示底漆颜色
                            int colorId_1 = equipmentPaints[0] % 100 - 1;
                            string matcapTexName_1 = new StringBuilder("MatcapTex1_" + colorId_1).ToString();
                            listTmp2[2] = matcapTexName_1;
                        }
                        // 装饰
                        if (equipmentPaints[1] == 0)
                        {
                            // 透明装饰 显示默认木质
                            listTmp2[1] = "MainTex0_0";
                            listTmp2[3] = "MatcapTex0_1";
                        }
                        else
                        {
                            // 非透明装饰 显示装饰颜色
                            int colorId_2 = equipmentPaints[1] % 100 - 1;
                            string matcapTexName_2 = new StringBuilder("MatcapTex1_" + colorId_2).ToString();
                            listTmp2[1] = "MainTex1_0";
                            listTmp2[3] = matcapTexName_2;
                        }
                        result.AddRange(listTmp2);
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// 获取初始化装备中的涂装列表
        /// </summary>
        /// <returns>装备中的涂装列表</returns>
        public static List<int> GetEquipEquipmentColor()
        {
            List<int> result = new List<int>(24);
            for (int i = 0; i < CurUserInfoData.equipEquipments.Count; i++)
            {
                int id = CurUserInfoData.equipEquipments[i];
                List<int> equipmentPaints = CurUserInfoData.equipmentPaints.GetValueOrDefault(id, new List<int> { 0, 0, 0, 0 });
                result.AddRange(equipmentPaints);
            }

            return result;
        }

        /// <summary>
        /// 装备列表排序
        /// </summary>
        /// <param name="list">待排序的列表</param>
        public static List<ComponentConfig> EquipmentSort(List<ComponentConfig> list)
        {
            List<int> sortInfo = new List<int>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                List<int> propetyNums = new List<int>(5) { list[i].FuKong, list[i].SuDu, list[i].KangZu, list[i].TuiJin, list[i].NengLiang };
                int propetyTmp = 0;
                for (int j = 0; j < propetyNums.Count; j++)
                {
                    if (propetyNums[j] > 0)
                    {
                        int curLevel = CurUserInfoData.equipments.GetValueOrDefault(list[i].ID, GlobalValueManager.InitEquipmentLv);
                        float curNumTmp = propetyNums[j];
                        for (int k = 0; k < (curLevel - 1); k++)
                        {
                            curNumTmp *= GlobalValueManager.EquipmentUpGradeNum;
                        }

                        int curNum = Mathf.FloorToInt(curNumTmp);
                        propetyTmp += curNum;
                    }
                }
                
                sortInfo.Add(propetyTmp);
            }

            int numTmp;
            ComponentConfig equipmentTmp;
            for (int i = 0; i < sortInfo.Count; i++)
            {
                for (int j = 0; j < sortInfo.Count - i - 1; j++)
                {
                    if (sortInfo[j] < sortInfo[j + 1])
                    {
                        numTmp = sortInfo[j];
                        sortInfo[j] = sortInfo[j + 1];
                        sortInfo[j + 1] = numTmp;
                        equipmentTmp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = equipmentTmp;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取指定关卡ID的地标打卡列表
        /// <param name="curLevelNum">指定关卡ID</param>
        /// </summary>
        public static List<bool> GetLandMarks(int curLevelNum = -1)
        {
            curLevelNum = curLevelNum == -1 ? CurLevelNum : curLevelNum;
            List<bool> result = new List<bool>(5) { false, false, false, false, false };
            List<int> landMarkIds = GlobalValueManager._landMarkIds[curLevelNum - 1];
            foreach (KeyValuePair<int, int> data in DataHelper.CurUserInfoData.landMarkInfo)
            {
                int index = landMarkIds.IndexOf(data.Key);
                if (index != -1)
                {
                    result[index] = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取倒计时已启动且倒计时最短的宝箱
        /// </summary>
        /// <returns>宝箱列表索引，倒计时，开箱结束时间</returns>
        public static string[] GetFristUnlockBox()
        {
            long[] boxTimes = new long[5];
            int[] countDownTimes = new int[5];
            long[] boxNextTime = new long[5];
            int emptySlotNum = 0;
            foreach (KeyValuePair<int, string[]> boxData in DataHelper.CurUserInfoData.boxList)
            {
                if (boxData.Value == null)
                {
                    boxTimes[boxData.Key] = -1;
                    countDownTimes[boxData.Key] = -1;
                    boxNextTime[boxData.Key] = -1;
                    emptySlotNum += 1;
                }
                else
                {
                    int boxId = int.Parse(boxData.Value[0]);
                    long boxTime = long.Parse(boxData.Value[1]);
                    boxTimes[boxData.Key] = boxTime;
                    long boxTimeTmp = boxTime == 0 ? ToolFunManager.GetCurrTime() : boxTime;
                    long nextTime = boxTimeTmp + ConfigManager.Instance.RewardBoxConfigDict[boxId].OpenTime * 60;
                    int subTime = (int)(nextTime - ToolFunManager.GetCurrTime());
                    countDownTimes[boxData.Key] = subTime <= 0 ? 0 : subTime;
                    boxNextTime[boxData.Key] = nextTime;
                }
            }
            
            // 没有宝箱
            if (emptySlotNum >= 5) return new[] { "-1", "-1", "-1" };
            // 有宝箱
            // 找到倒计时结束的
            int clickOpenIndex = -1;
            for (int i = 0; i < countDownTimes.Length; i++)
            {
                if (countDownTimes[i] == 0)
                {
                    clickOpenIndex = i;
                    break;
                }
            }
            // 剩余倒计时时间最短且已启动倒计时
            int clickAdvanceIndex = -1;
            int clickAdvanceTimeTmp = 0;
            long clickAdvanceNextTimeTmp = 0;
            for (int i = 0; i < boxTimes.Length; i++)
            {
                if (boxTimes[i] != 0)
                {
                    if (clickOpenIndex == i) continue;
                    if (clickAdvanceTimeTmp == 0)
                    {
                        clickAdvanceTimeTmp = countDownTimes[i];
                        clickAdvanceIndex = i;
                        clickAdvanceNextTimeTmp = boxNextTime[i];
                    }
                    else
                    {
                        if (countDownTimes[i] < clickAdvanceTimeTmp)
                        {
                            clickAdvanceTimeTmp = countDownTimes[i];
                            clickAdvanceIndex = i;
                            clickAdvanceNextTimeTmp = boxNextTime[i];
                        }
                    }
                }
            }

            return new[] { clickAdvanceIndex.ToString(), clickAdvanceTimeTmp.ToString(), clickAdvanceNextTimeTmp.ToString() };
        }

        /// <summary>
        /// 获取指定强化属性升级消耗货币数量
        /// </summary>
        /// <param name="idTmp">指定强化的ID</param>
        /// <param name="lvTmp">指定强化属性的等级</param>
        public static int GetTrainUpGradeNum(int idTmp, int lvTmp)
        {
            if (idTmp == 4)
            {
                // 金币增幅强化
                float num = 1000 * Mathf.Pow((lvTmp / 10f), 3.2f) + 1000;
                return Mathf.CeilToInt(num);
            }
            else
            {
                // 属性强化
                float num = 320 * Mathf.Pow((lvTmp / 100f), 3) + 300;
                return Mathf.CeilToInt(num / 10f) * 10;
            }
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取全部属性
        /// </summary>
        public static Dictionary<string, float> GetAllPropety(int typeTmp = -1, int idTmp = -1, bool isCalTrain = true)
        {
            List<int> curEquipments = new List<int>(CurUserInfoData.equipEquipments);
            if (typeTmp != -1 && idTmp != -1)
            {
                curEquipments[typeTmp] = idTmp;
            }

            List<string> propetyNames = new List<string>(7)
                { "propetyZhongLiang", "propetyFuKong", "propetySuDu", "propetyKangZu", "steering", "propetyTuiJin", "propetyNengLiang" };
            Dictionary<string, float> result = new Dictionary<string, float>();
            for (int i = 0; i < propetyNames.Count; i++)
            {
                result.Add(propetyNames[i], 0);
            }
            
            for (int i = 0; i < curEquipments.Count; i++)
            {
                int id = curEquipments[i];
                if (id == -1) continue;
                int level = !CurUserInfoData.equipments.ContainsKey(id) ? 1 : CurUserInfoData.equipments[id];
                Dictionary<string, float> propetyNum = GetEquipmentPropety(id, level);
                foreach (var data in propetyNum)
                {
                    result[data.Key] += data.Value;
                }
            }

            if (isCalTrain)
            {
                List<string> trainPropetyNames = new List<string>(3) { "propetyTanLi", "propetyThrusterLauncher", "propetyThrusterRing" };
                for (int i = 0; i < trainPropetyNames.Count; i++)
                {
                    int id = (i + 1);
                    int additionLv = DataHelper.CurUserInfoData.additions[id];
                    int baseNum = GlobalValueManager.TrainBaseNums[i];
                    int curAddNum = baseNum + (additionLv - 1);
                    result.Add(trainPropetyNames[i], curAddNum);
                }
            }
            
            return result;
        }

        /// <summary>
        /// 获取指定部件的属性
        /// </summary>
        /// <param name="id">部件ID</param>
        /// <param name="level">部件等级</param>
        public static Dictionary<string, float> GetEquipmentPropety(int id, int level)
        {
            ComponentConfig config = ConfigManager.Instance.ComponentConfigDict[id];
            List<string> propetyNames = new List<string>(6)
                { "propetyZhongLiang", "propetyFuKong", "propetySuDu", "propetyKangZu", "propetyTuiJin", "propetyNengLiang" };
            List<int> propetyNums = new List<int>(6)
            {
                config.ZhongLiang, config.FuKong, config.SuDu, config.KangZu, config.TuiJin, config.NengLiang
            };

            Dictionary<string, float> result = new Dictionary<string, float>();
            for (int i = 0; i < propetyNames.Count; i++)
            {
                // 重量属性 不用阶乘计算
                if (propetyNames[i] == "propetyZhongLiang")
                {
                    result.Add(propetyNames[i], propetyNums[i]);
                    continue;
                }
                // 非重量属性
                if (propetyNums[i] != 0)
                {
                    // float curNum = propetyNums[i] * (1 + (level - 1) * trainConfig.UpGradeNum); // 旧计算逻辑
                    float curNumTmp = propetyNums[i];
                    for (int j = 0; j < (level - 1); j++)
                    {
                        curNumTmp *= GlobalValueManager.EquipmentUpGradeNum;
                    }

                    int curNum = Mathf.FloorToInt(curNumTmp);
                    
                    result.Add(propetyNames[i], curNum);
                }
            }

            return result;
        }

        #endregion

        #region 开箱

        /// <summary>
        /// 获取开箱获得的物品
        /// </summary>
        /// <param name="boxId">宝箱ID</param>
        /// <param name="cb">回调</param>
        public static void GetBoxOpenItems(int boxId, Action<List<int>, List<string>, int> cb)
        {
            // 新手引导 固定获取
            if (MainManager._instance._guideMain1 != null)
            {
                List<int> boxItemListTmp = new List<int>(6) { 1001, 2001, 3001, 4001, 5001, 6001 };
                for (int i = 0; i < 6; i++)
                {
                    List<ComponentConfig> typeEquipments = ConfigManager.Instance.ComponentTypeConfigDict[i];
                    List<ComponentConfig> qualityEquipments = new List<ComponentConfig>();
                    for (int j = 0; j < typeEquipments.Count; j++)
                    {
                        if (typeEquipments[j].Quality == 0 && !GlobalValueManager.InitEquipments.Contains(typeEquipments[j].ID))
                        {
                            qualityEquipments.Add(typeEquipments[j]);
                        }
                    }

                    boxItemListTmp[i] = qualityEquipments[Random.Range(0, qualityEquipments.Count)].ID;
                }
                
                // 特殊处理 保证机翼为同一套
                int wingId_L = boxItemListTmp[2] % 1000;
                boxItemListTmp[3] = 4000 + wingId_L;
                
                int extraIdTmp = 1002;

                List<List<string>> resultTmp = ModifyEquipments(boxItemListTmp);
                if (resultTmp[0].Count > 0) ModifyLocalData(resultTmp[0], () => { });

                // 执行回调
                cb(boxItemListTmp, resultTmp[1], extraIdTmp);
                return;
            }
            
            // 正常游戏
            RewardBoxConfig config = ConfigManager.Instance.RewardBoxConfigDict[boxId];
            
            List<int> boxItemList = new List<int>();

            int[] subNums = { 0, 0, 0, 0, 0 };
            // 红色品质 Quality == 4
            if (config.QualityNum5 != "0-0")
            {
                List<int> subNum = ToolFunManager.GetNumListFromStrBySubStr(config.QualityNum5);
                subNums[4] = Random.Range(subNum[0], subNum[1] + 1);
            }

            // 紫色品质 Quality == 3
            if (config.QualityNum4 != "0-0")
            {
                List<int> subNum = ToolFunManager.GetNumListFromStrBySubStr(config.QualityNum4);
                subNums[3] = Random.Range(subNum[0], subNum[1] + 1);
            }

            // 黄色品质 Quality == 2
            if (config.QualityNum3 != "0-0")
            {
                List<int> subNum = ToolFunManager.GetNumListFromStrBySubStr(config.QualityNum3);
                subNums[2] = Random.Range(subNum[0], subNum[1] + 1);
            }

            // 蓝色品质 Quality == 1
            if (config.QualityNum2 != "0-0")
            {
                List<int> subNum = ToolFunManager.GetNumListFromStrBySubStr(config.QualityNum2);
                subNums[1] = Random.Range(subNum[0], subNum[1] + 1);
            }
            
            // 绿色品质 Quality == 0
            if (config.QualityNum1 != "0-0")
            {
                List<int> subNum = ToolFunManager.GetNumListFromStrBySubStr(config.QualityNum1);
                subNums[0] = Random.Range(subNum[0], subNum[1] + 1);
            }

            // Debug.Log(JsonConvert.SerializeObject(subNums));

            for (int i = 0; i < subNums.Length; i++)
            {
                if (subNums[i] != 0)
                {
                    int qualityTmp = i;
                    List<ComponentConfig> configsTmp = ConfigManager.Instance.ComponentQualityConfigDict[qualityTmp];
                    for (int j = 0; j < subNums[i]; j++)
                    {
                        boxItemList.Add(configsTmp[Random.Range(0, configsTmp.Count)].ID);
                    }
                }
            }

            // 开箱获得的部件 多余9个 随机舍去多余部件
            if (boxItemList.Count > 9)
            {
                while (boxItemList.Count > 9)
                {
                    int removeIndex = Random.Range(0, boxItemList.Count);
                    boxItemList.RemoveAt(removeIndex);
                }
            }

            // Debug.Log("开箱获得的部件 = " + JsonConvert.SerializeObject(boxItemList));

            int extraId = -1;

            if (CurUserInfoData.equipEquipments[2] == -1 && CurUserInfoData.equipEquipments[3] == -1)
            {
                // 没有装备机翼(现在应该已经不会执行了)
                // 匹配品质
                List<int> extraQualityNums = ToolFunManager.GetNumFromStrNew(config.ExtraQualityNum);
                int extraQuality = extraQualityNums[Random.Range(0, extraQualityNums.Count)];
                // 从匹配的品质中匹配装备
                List<ComponentConfig> configsTmp = ConfigManager.Instance.ComponentQualityConfigDict[extraQuality];
                extraId = configsTmp[Random.Range(0, configsTmp.Count)].ID;
            }
            else
            {
                // 已经装备机翼
                int quality1 = CurUserInfoData.equipEquipments[2] == -1
                    ? -1 // 不可能执行到 翅膀不能卸载
                    : ConfigManager.Instance.ComponentConfigDict[CurUserInfoData.equipEquipments[2]].Quality;
                int quality2 = CurUserInfoData.equipEquipments[3] == -1
                    ? -1 // 不可能执行到 翅膀不能卸载
                    : ConfigManager.Instance.ComponentConfigDict[CurUserInfoData.equipEquipments[3]].Quality;

                // 当前宝箱额外装备的最高品质
                List<int> extraQualityNums = ToolFunManager.GetNumFromStrNew(config.ExtraQualityNum);
                int qualityMax = extraQualityNums[0];
                for (int i = 1; i < extraQualityNums.Count; i++)
                {
                    if (extraQualityNums[i] > qualityMax)
                    {
                        qualityMax = extraQualityNums[i];
                    }
                }
                
                if (quality1 > quality2)
                {
                    // 左机翼品质 高于 右机翼品质
                    if (quality1 <= qualityMax)
                    {
                        // 品质高的左机翼 低于或者等于 当前宝箱产出的最高品质 补齐对应的右机翼
                        int id = ConfigManager.Instance.ComponentConfigDict[CurUserInfoData.equipEquipments[2]].ID;
                        int findId = 4000 + (id % 1000);
                        if (!CurUserInfoData.equipments.ContainsKey(findId))
                        {
                            extraId = findId;
                        }
                    }
                }
                else if (quality1 < quality2)
                {
                    // 左机翼品质 低于 右机翼品质
                    if (quality2 <= qualityMax)
                    {
                        // 品质高的右机翼 低于或者等于 当前宝箱产出的最高品质 补齐对应的左机翼
                        int id = ConfigManager.Instance.ComponentConfigDict[CurUserInfoData.equipEquipments[3]].ID;
                        int findId = 3000 + (id % 1000);
                        if (!CurUserInfoData.equipments.ContainsKey(findId))
                        {
                            extraId = findId;
                        }
                    }
                }

                // 未执行机翼保底逻辑 随机任意一个类型的装备
                if (extraId == -1)
                {
                    // 匹配品质
                    int extraQuality = extraQualityNums[Random.Range(0, extraQualityNums.Count)];
                    // 从匹配的品质中匹配装备
                    List<ComponentConfig> configsTmp = ConfigManager.Instance.ComponentQualityConfigDict[extraQuality];
                    extraId = configsTmp[Random.Range(0, configsTmp.Count)].ID;
                }
            }

            // Debug.Log("额外奖励 = " + extraId);

            List<List<string>> result = ModifyEquipments(boxItemList);
            if (result[0].Count > 0) ModifyLocalData(result[0], () => { });

            // 执行回调
            cb(boxItemList, result[1], extraId);
        }

        /// <summary>
        /// 获得装备
        /// </summary>
        /// <param name="list">装备ID列表</param>
        /// <returns>保存的数据key</returns>
        public static List<List<string>> ModifyEquipments(List<int> list)
        {
            List<string> modifyKeys = new List<string>();
            List<string> isNewEquipments = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                if (CurUserInfoData.equipments.ContainsKey(list[i]))
                {
                    // 老部件 转化为碎片
                    isNewEquipments.Add("0");
                    if (!modifyKeys.Contains("equipmentChips")) modifyKeys.Add("equipmentChips");
                    if (CurUserInfoData.equipmentChips.ContainsKey(list[i]))
                    {
                        CurUserInfoData.equipmentChips[list[i]] += 10;
                    }
                    else
                    {
                        CurUserInfoData.equipmentChips.Add(list[i], 10);
                    }
                }
                else
                {
                    // 新部件
                    isNewEquipments.Add("1");
                    if (!modifyKeys.Contains("equipments")) modifyKeys.Add("equipments");
                    CurUserInfoData.equipments.Add(list[i], 1);
                }

                // 完成成就任务 累计获得X个红色部件 TaskID:18
                if (ConfigManager.Instance.ComponentConfigDict[list[i]].Quality >= 4) DataHelper.CompleteGloalTask(18, 1);
            }
            
            // 完成成就任务 累计获得X个部件 TaskID:17
            CompleteGloalTask(17, 1);
            if (!modifyKeys.Contains("taskInfo2")) modifyKeys.Add("taskInfo2");

            return new List<List<string>>(2) { modifyKeys, isNewEquipments };
        }

        #endregion

        #region 任务

        /// <summary>
        /// 领取任务活跃度宝箱奖励
        /// <param name="index">奖励列表索引</param>
        /// </summary>
        public static void GetTaskActiveReward(int index)
        {
            TaskDailyInfoData taskDailyInfo = JsonConvert.DeserializeObject<TaskDailyInfoData>(CurUserInfoData.taskInfo1);
            taskDailyInfo.rewardGet[index] = 1;
            CurUserInfoData.taskInfo1 = JsonConvert.SerializeObject(taskDailyInfo);
        }

        /// <summary>
        /// 完成日常任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="taskNum">任务数值: -1: 完成并领取活跃点 1: 累计任务完成进度</param>
        /// <param name="activeNum">活跃点数</param>
        public static void CompleteDailyTask(int taskId, int taskNum, int activeNum)
        {
            bool isModifyData = false;
            TaskDailyInfoData taskDailyInfo = JsonConvert.DeserializeObject<TaskDailyInfoData>(CurUserInfoData.taskInfo1);
            if (taskNum == -1)
            {
                taskDailyInfo.taskState[taskId] = -1;
                taskDailyInfo.activePoint += activeNum;
                isModifyData = true;
            }
            else
            {
                if (taskDailyInfo.taskState[taskId] != -1)
                {
                    taskDailyInfo.taskState[taskId] += taskNum;
                    isModifyData = true;
                }
            }

            if (isModifyData)
            {
                CurUserInfoData.taskInfo1 = JsonConvert.SerializeObject(taskDailyInfo);
                EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);
            }
        }

        /// <summary>
        /// 完成成就任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="taskNum">任务数值: -1: 完成 >=1: 累计任务完成进度</param>
        public static void CompleteGloalTask(int taskId, int taskNum)
        {
            Dictionary<int, int[]> taskGloalInfo = JsonConvert.DeserializeObject<Dictionary<int, int[]>>(CurUserInfoData.taskInfo2);
            if (taskNum == -1)
            {
                // 完成阶段领取奖励
                taskGloalInfo[taskId][0] += 1;
            }
            else
            {
                List<int> ids = new List<int> { 12, 13, 14, 15, 16 };
                if (!ids.Contains(taskId))
                {
                    // 任务类型 累计进度
                    if (taskGloalInfo.ContainsKey(taskId))
                    {
                        // 当前成就有记录
                        taskGloalInfo[taskId][1] += taskNum;
                    }
                    else
                    {
                        // 当前成就没有记录
                        taskGloalInfo.Add(taskId, new[] { 0, taskNum });
                    }
                }
                else
                {
                    // 任务类型 替换最高纪录
                    if (taskGloalInfo.ContainsKey(taskId))
                    {
                        // 当前成就有记录
                        int taskGloalMax = taskGloalInfo[taskId][1];
                        if (taskNum > taskGloalMax)
                        {
                            taskGloalInfo[taskId][1] = taskNum;
                        }
                    }
                    else
                    {
                        // 当前成就没有记录
                        taskGloalInfo.Add(taskId, new[] { 0, taskNum });
                    }
                }
            }

            CurUserInfoData.taskInfo2 = JsonConvert.SerializeObject(taskGloalInfo);
            EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);
        }

        #endregion
    }
}