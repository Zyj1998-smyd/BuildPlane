using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Common.GameRoot;
using Common.Tool;
using Data;
using Data.ClassData;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Platform.ServerHandler
{
    /// <summary>
    /// 编辑器/自发模式
    /// </summary>
    public class Server_Editor : ServerHandler
    {
        private void Start()
        {
            Debug.Log("当前运行在编辑器模式");
        }
        
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="logStr"></param>
        private void DeBugLog(string logStr)
        {
            if (!ConfigManager.Instance.isPrintLog) return;
            Debug.Log(logStr);
        }

        #region 获取网络IP地址信息数据

        /// <summary>
        /// 获取网络IP地址信息数据
        /// <param name="cb">回调</param>
        /// </summary>
        public override void GetNetIpInfo(Action cb)
        {
            cb();
        }

        #endregion

        #region 服务器初始化

        /// <summary>
        /// 服务器初始化
        /// <param name="code"></param>
        /// <param name="loginCb"></param>
        /// <param name="cb"></param>
        /// </summary>
        public override void Init(string code, Action loginCb, Action<string> cb)
        {
            cb(code);
        }
        
        #endregion
        
        #region 用户数据初始化

        /** 游戏数据 */
        private Dictionary<string, string> _gameData;
        
        /// <summary>
        /// 用户数据初始化
        /// </summary>
        public override void InitUserData(Action<string> cb)
        {
            string gameData = PlayerPrefs.GetString("GameData", "");
            if (gameData == "")
            {
                DeBugLog("当前没有游戏数据");
                _gameData = new Dictionary<string, string>();
            }
            else
            {
                DeBugLog("当前已有游戏数据");
                _gameData = JsonConvert.DeserializeObject<Dictionary<string, string>>(gameData);
            }
            // 设置数据
            SetData();
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            List<string> modifyKeys = new List<string>();
            
            // 用户ID
            string[] userId = GetLocalData(GetUserDataKey("userId"), DataHelper.CurOpenId);
            if (userId[0] != "") modifyKeys.Add(userId[0]);
            
            // 用户昵称
            string[] userName = GetLocalData(GetUserDataKey("userName"), "");
            if (userName[0] != "") modifyKeys.Add(userName[0]);
            
            // 用户头像
            string[] userAvatar = GetLocalData(GetUserDataKey("userAvatar"), "");
            if (userAvatar[0] != "") modifyKeys.Add(userAvatar[0]);
            
            // 用户所在省份
            string[] userProvince = GetLocalData(GetUserDataKey("userProvince"), "");
            if (userProvince[0] != "") modifyKeys.Add(userProvince[0]);
            
            // 金币
            string[] gold = GetLocalData(GetUserDataKey("gold"), "0");
            if (gold[0] != "") modifyKeys.Add(gold[0]);
            
            // 钻石
            string[] diamond = GetLocalData(GetUserDataKey("diamond"), "0");
            if (diamond[0] != "") modifyKeys.Add(diamond[0]);
            
            // 关卡
            string[] curLevelNum = GetLocalData(GetUserDataKey("curLevelNum"), "1");
            if (curLevelNum[0] != "") modifyKeys.Add(curLevelNum[0]);
            
            // 加到我的小程序奖励领取完成
            string[] addedToMyMiniProgramGet = GetLocalData(GetUserDataKey("addedToMyMiniProgramGet"), "0");
            if (addedToMyMiniProgramGet[0] != "") modifyKeys.Add(addedToMyMiniProgramGet[0]);
            
            // 邀请奖励领取记录
            string[] callRewardGet = GetLocalData(GetUserDataKey("callRewardGet"), "0");
            if (callRewardGet[0] != "") modifyKeys.Add(callRewardGet[0]);
            
            // 直玩订阅奖励领取记录
            string[] feedSubGet = GetLocalData(GetUserDataKey("feedSubGet"), "0");
            if (feedSubGet[0] != "") modifyKeys.Add(feedSubGet[0]);
            
            // 装备中的配件列表
            string equipEquipmentsTmp = JsonConvert.SerializeObject(new List<int>(GlobalValueManager.InitEquipments));
            string[] equipEquipments = GetLocalData(GetUserDataKey("equipEquipments"), equipEquipmentsTmp);
            if (equipEquipments[0] != "") modifyKeys.Add(equipEquipments[0]);
            
            // 已拥有的配件列表
            Dictionary<int, int> equipmentsTmp = new Dictionary<int, int>();
            for (int i = 0; i < GlobalValueManager.InitEquipments.Count; i++)
            {
                equipmentsTmp.Add(GlobalValueManager.InitEquipments[i], 1);
            }
            string[] equipments = GetLocalData(GetUserDataKey("equipments"), JsonConvert.SerializeObject(equipmentsTmp));
            if (equipments[0] != "") modifyKeys.Add(equipments[0]);
            
            // 已拥有的配件碎片列表
            string equipmentChipsTmp = JsonConvert.SerializeObject(new Dictionary<int, int>());
            string[] equipmentChips = GetLocalData(GetUserDataKey("equipmentChips"), equipmentChipsTmp);
            if (equipmentChips[0] != "") modifyKeys.Add(equipmentChips[0]);
            
            // 已拥有的配件涂装列表
            string equipmentPaintsTmp = JsonConvert.SerializeObject(new Dictionary<int, List<int>>());
            string[] equipmentPaints = GetLocalData(GetUserDataKey("equipmentPaints"), equipmentPaintsTmp);
            if (equipmentPaints[0] != "") modifyKeys.Add(equipmentPaints[0]);
            
            // 已购买的配件涂装列表
            string buyEquipmentPaintsTmp = JsonConvert.SerializeObject(new Dictionary<int, List<int>>());
            string[] buyEquipmentPaints = GetLocalData(GetUserDataKey("buyEquipmentPaints"), buyEquipmentPaintsTmp);
            if (buyEquipmentPaints[0] != "") modifyKeys.Add(buyEquipmentPaints[0]);
            
            // 商店出售装备配件刷新幸运值
            string[] shopLuckNum = GetLocalData(GetUserDataKey("shopLuckNum"), "1");
            if (shopLuckNum[0] != "") modifyKeys.Add(shopLuckNum[0]);
            
            // 商店出售装备配件列表
            int curLevelNumTmp = int.Parse(curLevelNum[1]);
            int curShopLuckNumTmp = int.Parse(shopLuckNum[1]);
            string shopSaleIdsTmp = JsonConvert.SerializeObject(DataHelper.GetShopSaleList(curLevelNumTmp, curShopLuckNumTmp));
            string[] shopSaleIds = GetLocalData(GetUserDataKey("shopSaleIds"), shopSaleIdsTmp);
            if (shopSaleIds[0] != "") modifyKeys.Add(shopSaleIds[0]);
            
            // 商店出售装备配件刷新时间
            string[] shopRefreshTime = GetLocalData(GetUserDataKey("shopRefreshTime"), "0");
            long shopRefreshTimeTmp = long.Parse(shopRefreshTime[1]);
            long nextTime = shopRefreshTimeTmp + GlobalValueManager.ShopRefreshTime * 60;
            long subTime = nextTime - ToolFunManager.GetCurrTime();
            if (subTime <= 0)
            {
                modifyKeys.Add(GetUserDataKey("shopSaleIds"));
                modifyKeys.Add(GetUserDataKey("shopLuckNum"));
                modifyKeys.Add(GetUserDataKey("shopRefreshTime"));
                shopSaleIds[1] = JsonConvert.SerializeObject(DataHelper.GetShopSaleList(int.Parse(curLevelNum[1]), 1));
                shopLuckNum[1] = "1";
                shopRefreshTime[1] = JsonConvert.SerializeObject(ToolFunManager.GetCurrTime());
            }
            
            // 商店限购商品购买记录
            string shopLimitsTmp = JsonConvert.SerializeObject(new Dictionary<int, int>());
            string[] shopLimits = GetLocalData(GetUserDataKey("shopLimits"), shopLimitsTmp);
            if (shopLimits[0] != "") modifyKeys.Add(shopLimits[0]);
            
            // 商店限购商品购买记录刷新时间
            string shopLimitTimeTmp = JsonConvert.SerializeObject(ToolFunManager.GetCurrTime());
            string[] shopLimitTime = GetLocalData(GetUserDataKey("shopLimitTime"), shopLimitTimeTmp);
            if (shopLimitTime[0] != "") modifyKeys.Add(shopLimitTime[0]);
            else
            {
                if (ToolFunManager.JudgeDayStampOutTime(ToolFunManager.GetDateTime(long.Parse(shopLimitTime[1]))))
                {
                    ConfigManager.Instance.ConsoleLog(0, "新的一天，每日限购商品已刷新...");
                    shopLimits[1] = JsonConvert.SerializeObject(new Dictionary<int, int>());
                    shopLimitTime[1] = JsonConvert.SerializeObject(ToolFunManager.GetCurrTime());
                    modifyKeys.Add(GetUserDataKey("shopLimits"));
                    modifyKeys.Add(GetUserDataKey("shopLimitTime"));
                }
            }

            // 最远距离
            string[] scoreDistanceMax = GetLocalData(GetUserDataKey("scoreDistanceMax"), "0");
            if (scoreDistanceMax[0] != "") modifyKeys.Add(scoreDistanceMax[0]);
            
            // 最远距离刷新时间
            string disMaxRefreshTimeTmp = JsonConvert.SerializeObject(ToolFunManager.GetCurrTime());
            string[] disMaxRefreshTime = GetLocalData(GetUserDataKey("disMaxRefreshTime"), disMaxRefreshTimeTmp);
            if (disMaxRefreshTime[0] != "")
            {
                modifyKeys.Add(disMaxRefreshTime[0]);
                modifyKeys.Add(GetUserDataKey("scoreDistanceMax"));
                scoreDistanceMax[1] = "0";
            }
            else
            {
                if (ToolFunManager.JudgeDayStampOutTime(ToolFunManager.GetDateTime(long.Parse(disMaxRefreshTime[1]))))
                {
                    ConfigManager.Instance.ConsoleLog(0, "新的一天，最远距离已刷新...");
                    modifyKeys.Add(GetUserDataKey("scoreDistanceMax"));
                    modifyKeys.Add(GetUserDataKey("disMaxRefreshTime"));
                    scoreDistanceMax[1] = "0";
                    disMaxRefreshTime[1] = disMaxRefreshTimeTmp;
                }
            }

            // 加成列表
            Dictionary<int, int> additionsNew = new Dictionary<int, int> { { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 } };
            string[] additions = GetLocalData(GetUserDataKey("additions"), JsonConvert.SerializeObject(additionsNew));
            if (additions[0] != "") modifyKeys.Add(additions[0]);
            
            // 起飞次数
            string[] flyNum = GetLocalData(GetUserDataKey("flyNum"), "0");
            if (flyNum[0] != "") modifyKeys.Add(flyNum[0]);
            
            // 宝箱列表
            Dictionary<int, string[]> boxsListTmp = new Dictionary<int, string[]>();
            for (int i = 0; i < 5; i++)
            {
                boxsListTmp.Add(i, null);
            }
            string[] boxList = GetLocalData(GetUserDataKey("boxsList"), JsonConvert.SerializeObject(boxsListTmp));
            if (boxList[0] != "") modifyKeys.Add(boxList[0]);
            
            // 宝箱获取次数
            string[] boxGetNum = GetLocalData(GetUserDataKey("boxGetNum"), "0");
            if (boxGetNum[0] != "") modifyKeys.Add(boxGetNum[0]);
            
            // 宝箱获取累计飞行距离
            string[] boxGetFlyDis = GetLocalData(GetUserDataKey("boxGetFlyDis"), "0");
            if (boxGetFlyDis[0] != "") modifyKeys.Add(boxGetFlyDis[0]);
            
            // 宝箱获取所属关卡ID
            string[] boxGetLevel = GetLocalData(GetUserDataKey("boxGetLevel"), int.Parse(curLevelNum[1]).ToString());
            if (boxGetLevel[0] != "") modifyKeys.Add(boxGetLevel[0]);
            
            // 签到信息
            long signTime = ToolFunManager.GetCurrTime();
            string signInfoTmp = JsonConvert.SerializeObject(new SignInfoData { dayStamp = signTime, day = 1, isSign = 0 });
            string[] signInfo = GetLocalData(GetUserDataKey("signInfo"), signInfoTmp);
            if (signInfo[0] != "") modifyKeys.Add(signInfo[0]);
            else
            {
                SignInfoData signInfoData = JsonConvert.DeserializeObject<SignInfoData>(signInfo[1]);
                if (ToolFunManager.JudgeDayStampOutTime(ToolFunManager.GetDateTime(signInfoData.dayStamp)))
                {
                    ConfigManager.Instance.ConsoleLog(0, "新的一天，签到数据已刷新...");
                    modifyKeys.Add(GetUserDataKey("signInfo"));
                    int newDay = signInfoData.isSign == 0 ? signInfoData.day : signInfoData.day + 1;
                    signInfo[1] = JsonConvert.SerializeObject(new SignInfoData { dayStamp = signTime, day = newDay, isSign = 0 });
                }
            }
            
            // 转盘信息
            long raffleTime = ToolFunManager.GetCurrTime();
            string raffleInfoTmp = JsonConvert.SerializeObject(new RaffleInfoData { dayStamp = raffleTime, lastFreeTime = 0, luckNum = 0, luckStartTime = 0 });
            string[] raffleInfo = GetLocalData(GetUserDataKey("raffleInfo"), raffleInfoTmp);
            if (raffleInfo[0] != "") modifyKeys.Add(raffleInfo[0]);
            else
            {
                RaffleInfoData raffleInfoData = JsonConvert.DeserializeObject<RaffleInfoData>(raffleInfo[1]);
                if (ToolFunManager.JudgeDayStampOutTime(ToolFunManager.GetDateTime(raffleInfoData.dayStamp)))
                {
                    ConfigManager.Instance.ConsoleLog(0, "新的一天，转盘数据已刷新...");
                    modifyKeys.Add(GetUserDataKey("raffleInfo"));
                    raffleInfo[1] = JsonConvert.SerializeObject(new RaffleInfoData { dayStamp = raffleTime, lastFreeTime = 0, luckNum = 0, luckStartTime = 0 });
                }
            }
            
            // 每日任务信息
            TaskDailyInfoData taskDailyInfoTmp = new TaskDailyInfoData
            {
                dayStamp = ToolFunManager.GetCurrTime(),
                activePoint = 0,
                rewardGet = new List<int>(5) { 0, 0, 0, 0, 0 },
                taskState = new Dictionary<int, int>(10) { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 }, { 10, 0 } }
            };
            string[] taskInfo1 = GetLocalData(GetUserDataKey("taskInfo1"), JsonConvert.SerializeObject(taskDailyInfoTmp));
            if (taskInfo1[0] != "") modifyKeys.Add(taskInfo1[0]);
            else
            {
                TaskDailyInfoData taskDailyInfoData = JsonConvert.DeserializeObject<TaskDailyInfoData>(taskInfo1[1]);
                if (ToolFunManager.JudgeDayStampOutTime(ToolFunManager.GetDateTime(taskDailyInfoData.dayStamp)))
                {
                    ConfigManager.Instance.ConsoleLog(0, "新的一天，每日任务数据已刷新...");
                    modifyKeys.Add(GetUserDataKey("taskInfo1"));
                    taskInfo1[1] = JsonConvert.SerializeObject(taskDailyInfoTmp);
                }
            }
            
            // 成就任务信息
            string[] taskInfo2 = GetLocalData(GetUserDataKey("taskInfo2"), JsonConvert.SerializeObject(new Dictionary<int, int[]>()));
            if (taskInfo2[0] != "") modifyKeys.Add(taskInfo2[0]);
            
            // 新用户
            string[] isNewUser = GetLocalData(GetUserDataKey("isNewUser"), "0");
            if (isNewUser[0] != "") modifyKeys.Add(isNewUser[0]);
            
            // 每日登录
            string[] isLogin = GetLocalData(GetUserDataKey("isLogin"), "0");
            if (isLogin[0] != "") modifyKeys.Add(isLogin[0]);
            
            // 每日登录刷新时间
            string[] isLoginTime = GetLocalData(GetUserDataKey("isLoginTime"), JsonConvert.SerializeObject(ToolFunManager.GetCurrTime()));
            if (isLoginTime[0] != "") modifyKeys.Add(isLoginTime[0]);
            else
            {
                if (ToolFunManager.JudgeDayStampOutTime(ToolFunManager.GetDateTime(long.Parse(isLoginTime[1]))))
                {
                    ConfigManager.Instance.ConsoleLog(0, "新的一天，每日登录已刷新...");
                    modifyKeys.Add(GetUserDataKey("isLogin"));
                    modifyKeys.Add(GetUserDataKey("isLoginTime"));
                    isLogin[1] = "0";
                    isLoginTime[1] = JsonConvert.SerializeObject(ToolFunManager.GetCurrTime());
                }
            }
            
            // 地标打卡列表
            string[] landMarkInfo = GetLocalData(GetUserDataKey("landMarkInfo"), JsonConvert.SerializeObject(new Dictionary<int, int>()));
            if (landMarkInfo[0] != "") modifyKeys.Add(landMarkInfo[0]);
            
            // 新获得的配件涂装状态(涂装按钮提示红点)
            string equipmentPaintNewsTmp = JsonConvert.SerializeObject(new Dictionary<int, int>());
            string[] equipmentPaintNews = GetLocalData(GetUserDataKey("equipmentPaintNews"), equipmentPaintNewsTmp);
            if (equipmentPaintNews[0] != "") modifyKeys.Add(equipmentPaintNews[0]);
            
            // 设置
            string settingsTmp = JsonConvert.SerializeObject(new List<int>(4) { 1, 1, 1, 0 });
            string[] settings = GetLocalData(GetUserDataKey("settings"), settingsTmp);
            if (settings[0] != "") modifyKeys.Add(settings[0]);
            
            // 飞行距离最高记录
            string[] distanceRecord = GetLocalData(GetUserDataKey("distanceRecord"), "0");
            if (distanceRecord[0] != "") modifyKeys.Add(distanceRecord[0]);
            
            // 构建游戏数据
            DataHelper.CurUserInfoData = new UserInfoData
            {
                userId = userId[1],
                userName = userName[1],
                userAvatar = userAvatar[1],
                userProvince = userProvince[1],
                gold = int.Parse(gold[1]),
                diamond = int.Parse(diamond[1]),
                curLevelNum = int.Parse(curLevelNum[1]),
                addedToMyMiniProgramGet = int.Parse(addedToMyMiniProgramGet[1]),
                callRewardGet = int.Parse(callRewardGet[1]),
                feedSubGet = int.Parse(feedSubGet[1]),
                equipEquipments = JsonConvert.DeserializeObject<List<int>>(equipEquipments[1]),
                equipments = JsonConvert.DeserializeObject<Dictionary<int, int>>(equipments[1]),
                equipmentChips = JsonConvert.DeserializeObject<Dictionary<int, int>>(equipmentChips[1]),
                equipmentPaints = JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(equipmentPaints[1]),
                buyEquipmentPaints = JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(buyEquipmentPaints[1]),
                shopRefreshTime = long.Parse(shopRefreshTime[1]),
                shopSaleIds = JsonConvert.DeserializeObject<Dictionary<int, int>>(shopSaleIds[1]),
                shopLuckNum = int.Parse(shopLuckNum[1]),
                shopLimitTime = long.Parse(shopLimitTime[1]),
                shopLimits = JsonConvert.DeserializeObject<Dictionary<int, int>>(shopLimits[1]),
                scoreDistanceMax = float.Parse(scoreDistanceMax[1]),
                disMaxRefreshTime = long.Parse(disMaxRefreshTime[1]),
                additions = JsonConvert.DeserializeObject<Dictionary<int, int>>(additions[1]),
                flyNum = int.Parse(flyNum[1]),
                boxList = JsonConvert.DeserializeObject<Dictionary<int, string[]>>(boxList[1]),
                boxGetNum = int.Parse(boxGetNum[1]),
                boxGetFlyDis = int.Parse(boxGetFlyDis[1]),
                boxGetLevel = int.Parse(boxGetLevel[1]),
                signInfo = signInfo[1],
                raffleInfo = raffleInfo[1],
                taskInfo1 = taskInfo1[1],
                taskInfo2 = taskInfo2[1],
                isNewUser = int.Parse(isNewUser[1]),
                isLogin = int.Parse(isLogin[1]),
                isLoginTime = long.Parse(isLoginTime[1]),
                landMarkInfo = JsonConvert.DeserializeObject<Dictionary<int, int>>(landMarkInfo[1]),
                equipmentPaintNews = JsonConvert.DeserializeObject<Dictionary<int, int>>(equipmentPaintNews[1]),
                settings = JsonConvert.DeserializeObject<List<int>>(settings[1]),
                distanceRecord = float.Parse(distanceRecord[1])
            };
            
            // 完成成就任务 累计登录X天游戏 TaskID:1
            if (DataHelper.CurUserInfoData.isLogin == 0)
            {
                DataHelper.CurUserInfoData.isLogin = 1;
                string isLoginKey = GetUserDataKey("isLogin");
                if (!modifyKeys.Contains(isLoginKey)) modifyKeys.Add(isLoginKey);
                DataHelper.CurUserInfoData.isLoginTime = ToolFunManager.GetCurrTime();
                string isLoginTimeKey = GetUserDataKey("isLoginTime");
                if (!modifyKeys.Contains(isLoginTimeKey)) modifyKeys.Add(isLoginTimeKey);
                DataHelper.CompleteGloalTask(1, 1);
                string taskInfo2Key = GetUserDataKey("taskInfo2");
                if (!modifyKeys.Contains(taskInfo2Key)) modifyKeys.Add(taskInfo2Key);
            }
            
            // 上传数据
            if (modifyKeys.Count > 0)
            {
                Debug.Log("UnityLog.当前有需要更新的数据..." + JsonConvert.SerializeObject(modifyKeys));
                DataHelper.CurModifyKeys.Clear();
                for (int i = 0; i < modifyKeys.Count; i++)
                {
                    DataHelper.CurModifyKeys.Add(modifyKeys[i].Split('_')[0]);
                }

                UpdateServerData(() => { });
            }
            else
                Debug.Log("UnityLog.当前没有需要更新的数据...");

            GetNetIpInfo(() =>
            {
                Debug.Log("当前省份 = " + DataHelper.CurUserInfoData.userProvince);
                        
                DataHelper.InitGameData();

                DataHelper.nextSceneName = DataHelper.CurUserInfoData.isNewUser == 0 ? "BattleScene" : "MainScene";
                GameRootLoad.Instance.StartLoad(DataHelper.nextSceneName);
            });
        }

        /// <summary>
        /// 获取用户数据存储Key
        /// </summary>
        /// <param name="keyTmp">数据Key</param>
        private string GetUserDataKey(string keyTmp)
        {
            // return new StringBuilder(keyTmp + "_" + DataHelper.CurOpenId).ToString();
            return keyTmp;
        }

        /// <summary>
        /// 获取本地存储数据
        /// </summary>
        /// <param name="keyTmp">数据Key</param>
        /// <param name="defalutValue">本地没有数据时的默认值</param>
        /// <returns></returns>
        private string[] GetLocalData(string keyTmp, string defalutValue)
        {
            string[] data = new string[2];
            if (_gameData.ContainsKey(keyTmp))
            {
                // Debug.Log("已存在" + keyTmp);
                data[0] = "";
                data[1] = _gameData[keyTmp];
            }
            else
            {
                // Debug.Log("不存在" + keyTmp);
                data[0] = keyTmp;
                data[1] = defalutValue;
            }

            return data;
        }

        #endregion

        #region 用户上传数据

        /// <summary>
        /// 用户上传数据
        /// </summary>
        /// <param name="cb">回调</param>
        public override void UpdateServerData(Action cb)
        {
            DeBugLog("用户保存数据成功");
            UploadGameData();
            PlayerPrefs.SetString("GameData", JsonConvert.SerializeObject(_gameData));
        }
        
        /// <summary>
        /// 更新数据
        /// </summary>
        private void UploadGameData()
        {
            // Debug.Log("开始调用更新数据...");
            for (int i = 0; i < DataHelper.CurModifyKeys.Count; i++)
            {
                switch (DataHelper.CurModifyKeys[i])
                {
                    case "userId":
                        string key_userId = GetUserDataKey("userId");
                        _gameData[key_userId] = DataHelper.CurUserInfoData.userId;
                        break;
                    case "userName":
                        string key_userName = GetUserDataKey("userName");
                        _gameData[key_userName] = DataHelper.CurUserInfoData.userName;
                        break;
                    case "userAvatar":
                        string key_userAvatar = GetUserDataKey("userAvatar");
                        _gameData[key_userAvatar] = DataHelper.CurUserInfoData.userAvatar;
                        break;
                    case "userProvince":
                        string key_userProvince = GetUserDataKey("userProvince");
                        _gameData[key_userProvince] = DataHelper.CurUserInfoData.userProvince;
                        break;
                    case "gold":
                        string key_gold = GetUserDataKey("gold");
                        _gameData[key_gold] = DataHelper.CurUserInfoData.gold.ToString();
                        break;
                    case "diamond":
                        string key_diamond = GetUserDataKey("diamond");
                        _gameData[key_diamond] = DataHelper.CurUserInfoData.diamond.ToString();
                        break;
                    case "curLevelNum":
                        string key_curLevelNum = GetUserDataKey("curLevelNum");
                        _gameData[key_curLevelNum] = DataHelper.CurUserInfoData.curLevelNum.ToString();
                        break;
                    case "addedToMyMiniProgramGet":
                        string key_addedToMyMiniProgramGet = GetUserDataKey("addedToMyMiniProgramGet");
                        _gameData[key_addedToMyMiniProgramGet] = DataHelper.CurUserInfoData.addedToMyMiniProgramGet.ToString();
                        break;
                    case "callRewardGet":
                        string key_callRewardGet = GetUserDataKey("callRewardGet");
                        _gameData[key_callRewardGet] = DataHelper.CurUserInfoData.callRewardGet.ToString();
                        break;
                    case "feedSubGet":
                        string key_feedSubGet = GetUserDataKey("feedSubGet");
                        _gameData[key_feedSubGet] = DataHelper.CurUserInfoData.feedSubGet.ToString();
                        break;
                    case "equipEquipments":
                        string key_equipEquipments = GetUserDataKey("equipEquipments");
                        _gameData[key_equipEquipments] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.equipEquipments);
                        break;
                    case "equipments":
                        string key_equipments = GetUserDataKey("equipments");
                        _gameData[key_equipments] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.equipments);
                        break;
                    case "equipmentChips":
                        string key_equipmentChips = GetUserDataKey("equipmentChips");
                        _gameData[key_equipmentChips] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.equipmentChips);
                        break;
                    case "equipmentPaints":
                        string key_equipmentPaints = GetUserDataKey("equipmentPaints");
                        _gameData[key_equipmentPaints] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.equipmentPaints);
                        break;
                    case "buyEquipmentPaints":
                        string key_buyEquipmentPaints = GetUserDataKey("buyEquipmentPaints");
                        _gameData[key_buyEquipmentPaints] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.buyEquipmentPaints);
                        break;
                    case "shopRefreshTime":
                        string key_shopRefreshTime = GetUserDataKey("shopRefreshTime");
                        _gameData[key_shopRefreshTime] = DataHelper.CurUserInfoData.shopRefreshTime.ToString();
                        break;
                    case "shopSaleIds":
                        string key_shopSaleIds = GetUserDataKey("shopSaleIds");
                        _gameData[key_shopSaleIds] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.shopSaleIds);
                        break;
                    case "shopLuckNum":
                        string key_shopLuckNum = GetUserDataKey("shopLuckNum");
                        _gameData[key_shopLuckNum] = DataHelper.CurUserInfoData.shopLuckNum.ToString();
                        break;
                    case "shopLimitTime":
                        string key_shopLimitTime = GetUserDataKey("shopLimitTime");
                        _gameData[key_shopLimitTime] = DataHelper.CurUserInfoData.shopLimitTime.ToString();
                        break;
                    case "shopLimits":
                        string key_shopLimits = GetUserDataKey("shopLimits");
                        _gameData[key_shopLimits] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.shopLimits);
                        break;
                    case "scoreDistanceMax":
                        string key_scoreDistanceMax = GetUserDataKey("scoreDistanceMax");
                        _gameData[key_scoreDistanceMax] = DataHelper.CurUserInfoData.scoreDistanceMax.ToString(CultureInfo.InvariantCulture);
                        break;
                    case "disMaxRefreshTime":
                        string key_disMaxRefreshTime = GetUserDataKey("disMaxRefreshTime");
                        _gameData[key_disMaxRefreshTime] = DataHelper.CurUserInfoData.disMaxRefreshTime.ToString();
                        break;
                    case "additions":
                        string key_additions = GetUserDataKey("additions");
                        _gameData[key_additions] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.additions);
                        break;
                    case "flyNum":
                        string key_flyNum = GetUserDataKey("flyNum");
                        _gameData[key_flyNum] = DataHelper.CurUserInfoData.flyNum.ToString();
                        break;
                    case "boxsList":
                        string key_boxsList = GetUserDataKey("boxsList");
                        _gameData[key_boxsList] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.boxList);
                        break;
                    case "boxGetNum":
                        string key_boxGetNum = GetUserDataKey("boxGetNum");
                        _gameData[key_boxGetNum] = DataHelper.CurUserInfoData.boxGetNum.ToString();
                        break;
                    case "boxGetFlyDis":
                        string key_boxGetFlyDis = GetUserDataKey("boxGetFlyDis");
                        _gameData[key_boxGetFlyDis] = DataHelper.CurUserInfoData.boxGetFlyDis.ToString();
                        break;
                    case "boxGetLevel":
                        string key_boxGetLevel = GetUserDataKey("boxGetLevel");
                        _gameData[key_boxGetLevel] = DataHelper.CurUserInfoData.boxGetLevel.ToString();
                        break;
                    case "signInfo":
                        string key_signInfo = GetUserDataKey("signInfo");
                        _gameData[key_signInfo] = DataHelper.CurUserInfoData.signInfo;
                        break;
                    case "raffleInfo":
                        string key_raffleInfo = GetUserDataKey("raffleInfo");
                        _gameData[key_raffleInfo] = DataHelper.CurUserInfoData.raffleInfo;
                        break;
                    case "taskInfo1":
                        string key_taskInfo1 = GetUserDataKey("taskInfo1");
                        _gameData[key_taskInfo1] = DataHelper.CurUserInfoData.taskInfo1;
                        break;
                    case "taskInfo2":
                        string key_taskInfo2 = GetUserDataKey("taskInfo2");
                        _gameData[key_taskInfo2] = DataHelper.CurUserInfoData.taskInfo2;
                        break;
                    case "isNewUser":
                        string key_isNewUser = GetUserDataKey("isNewUser");
                        _gameData[key_isNewUser] = DataHelper.CurUserInfoData.isNewUser.ToString();
                        break;
                    case "isLogin":
                        string key_isLogin = GetUserDataKey("isLogin");
                        _gameData[key_isLogin] = DataHelper.CurUserInfoData.isLogin.ToString();
                        break;
                    case "isLoginTime":
                        string key_isLoginTime = GetUserDataKey("isLoginTime");
                        _gameData[key_isLoginTime] = DataHelper.CurUserInfoData.isLoginTime.ToString();
                        break;
                    case "landMarkInfo":
                        string key_landMarkInfo = GetUserDataKey("landMarkInfo");
                        _gameData[key_landMarkInfo] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.landMarkInfo);
                        break;
                    case "equipmentPaintNews":
                        string key_equipmentPaintNews = GetUserDataKey("equipmentPaintNews");
                        _gameData[key_equipmentPaintNews] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.equipmentPaintNews);
                        break;
                    case "settings":
                        string key_settings = GetUserDataKey("settings");
                        _gameData[key_settings] = JsonConvert.SerializeObject(DataHelper.CurUserInfoData.settings);
                        break;
                    case "distanceRecord":
                        string key_distanceRecord = GetUserDataKey("distanceRecord");
                        _gameData[key_distanceRecord] = DataHelper.CurUserInfoData.distanceRecord.ToString(CultureInfo.InvariantCulture);
                        break;
                }
            }
        }
        
        #endregion

        #region 获取省份排名列表

        /// <summary>
        /// 获取排名列表
        /// </summary>
        /// <param name="cb"></param>
        public override void GetGeneralRank(Action cb)
        {
            cb();
        }

        #endregion

        #region 上传省份排名值

        /// <summary>
        /// 上传排名值
        /// </summary>
        public override void UpdateGeneralRank()
        {
            if (string.IsNullOrEmpty(DataHelper.CurUserInfoData.userProvince)) return; // 未获取省份
            // todo 更新省份排行榜
        }
        
        #endregion

        #region 获取全国排行榜

        /// <summary>
        /// 获取全国排行榜
        /// </summary>
        /// <param name="type">排行榜类型 0: 通关榜 1: 积分榜 2: 飞行距离榜</param>
        /// <param name="cb">回调</param>
        public override void GetRankAll(int type, Action cb)
        {
            if (type == 2)
            {
                // --------------------------------------- 排行榜数据 --------------------------------------- //
                // 服务器排行榜列表(自己接入自己的)
                // --------------------------------------- 排行榜数据 --------------------------------------- //
                
                // 如果排行榜人数少于30个 补充机器人
                if (DataHelper.RankDisUserDatas.Count < 30)
                {
                    int index = 0;
                    while (DataHelper.RankDisUserDatas.Count < 30)
                    {
                        List<int> colorList = new List<int>();
                        for (int i = 0; i < 24; i++)
                        {
                            colorList.Add(0);
                        }

                        DataHelper.RankDisUserDatas.Add(new RankDisUserData
                        {
                            openId = new StringBuilder("Robot" + index).ToString(),
                            nickName = GlobalValueManager.NickNames[index],
                            userAvatar = new StringBuilder(GlobalValueManager.HeadImageUrl + "Image" + (index + 1) + ".png").ToString(),
                            distance = Random.Range(100, 500),
                            userProvince = "江苏",
                            planeEquipments = new List<int>(6) { 1000, 2000, 3000, 4000, 5000, 6000 },
                            planeEquipmentColor = colorList,
                            planeEquipmentLvs = new List<int>(6) { 1, 1, 1, 1, 1, 1 }
                        });

                        index += 1;
                    }
                }
                // 执行回调
                cb();
            }
        }

        #endregion

        #region 更新全国排行榜

        /// <summary>
        /// 更新全国排行榜
        /// </summary>
        /// <param name="type">排行榜类型 0:通关榜 1:积分榜 2:飞行距离榜</param>
        public override void UpdateRankAll(int type)
        {
            if (string.IsNullOrEmpty(DataHelper.CurUserInfoData.userName)) return;   // 没昵称
            if (string.IsNullOrEmpty(DataHelper.CurUserInfoData.userAvatar)) return; // 没头像
            ConfigManager.Instance.ConsoleLog(0, "上报全国" + new[] { "通关榜", "积分榜", "距离榜" }[type]);
            // todo 更新全国排行榜
        }

        #endregion

        #region 上传用户名头像

        /// <summary>
        /// 上传用户名头像
        /// <param name="cb">回调</param>
        /// </summary>
        public override void UpdateUserInfo(Action cb)
        {
            cb();
        }

        #endregion

        #region 获取全国排行榜判断Key

        /// <summary>
        /// 获取全国排行榜判断Key
        /// </summary>
        /// <returns>Key</returns>
        public override string GetRankAllJudgeKey()
        {
            return DataHelper.CurOpenId;
        }

        #endregion

        #region 广告上报

        /// <summary>
        /// 广告上报
        /// </summary>
        public override void ReportAd(bool complete)
        {
            // 编辑器 不支持广告上报
        }

        #endregion

        #region 获取用户信息判断Key

        /// <summary>
        /// 获取用户信息判断Key
        /// </summary>
        /// <returns>Key</returns>
        public override string GetUserInfoJudgeKey()
        {
            return "";
        }

        #endregion
        
        #region 清除用户数据

        /// <summary>
        /// 清除用户数据
        /// </summary>
        public override void ClearUserData()
        {
            DataHelper.ClearUserInfoData();
        }

        #endregion
        
    }
}