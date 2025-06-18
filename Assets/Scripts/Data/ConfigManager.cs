using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Data.ConfigData;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// 配置表管理类
    /// </summary>
    public class ConfigManager : MonoBehaviour
    {
        /** 配置表管理类 */
        public static ConfigManager Instance;
        
        // /** 读取Json配置数量 */
        // private int _loadConfigNum;
        
        /** 开启测试模式 */
        public bool isDebug;
        /** 开启全解锁模式 测试用 */
        public bool isUnlockAll;
        /** 开启输出日志 */
        public bool isPrintLog;

        /** 省份配置表 */
        internal List<ProvinceConfig> provinceConfigs;

        /** 配件配置表 */
        internal List<ComponentConfig> ComponentConfigs;
        /** 配件配置表 数据字典 Key == ID */
        internal readonly Dictionary<int, ComponentConfig> ComponentConfigDict = new Dictionary<int, ComponentConfig>();
        /** 配件配置表 数据字典 Key == Type */
        internal readonly Dictionary<int, List<ComponentConfig>> ComponentTypeConfigDict = new Dictionary<int, List<ComponentConfig>>();
        /** 配件配置表 数据字典 Key == Quality */
        internal readonly Dictionary<int, List<ComponentConfig>> ComponentQualityConfigDict = new Dictionary<int, List<ComponentConfig>>();

        /** 商店配置表 */
        internal List<ShopConfig> ShopConfigs;
        /** 商店配置表 数据字典 Key == ID */
        internal readonly Dictionary<int, ShopConfig> ShopConfigDict = new Dictionary<int, ShopConfig>();
        /** 商店配置表 数据字典 Key == Type */
        internal readonly Dictionary<int, List<ShopConfig>> ShopTypeConfigDict = new Dictionary<int, List<ShopConfig>>();

        /** 宝箱配置表 */
        internal List<RewardBoxConfig> RewardBoxConfigs;
        /** 宝箱配置表 数据字典 */
        internal readonly Dictionary<int, RewardBoxConfig> RewardBoxConfigDict = new Dictionary<int, RewardBoxConfig>();

        /** 签到配置表 */
        internal List<SignConfig> SignConfigs;
        
        /** 日常任务配置表 */
        internal List<TaskConfig1> TaskConfig1s;
        /** 日常任务配置表 数据字典 */
        internal readonly Dictionary<int, TaskConfig1> TaskConfigDict1 = new Dictionary<int, TaskConfig1>();
        
        /** 成就任务配置表 */
        internal List<TaskConfig2> TaskConfig2s;
        /** 成就任务配置表 数据字典 */
        internal readonly Dictionary<int, TaskConfig2> TaskConfigDict2 = new Dictionary<int, TaskConfig2>();

        /** 限时商店商品配置表 */
        internal List<ShopItemConfig> ShopItemConfigs;
        /** 限时商店商品配置表 数据字典 */
        internal readonly Dictionary<int, ShopItemConfig> ShopItemConfigDict = new Dictionary<int, ShopItemConfig>();

        /** GM配置表 */
        internal List<GmConfig> GmConfigs;
        /** GM配置表 数据字典 */
        internal readonly Dictionary<string, GmConfig> GmConfigDict = new Dictionary<string, GmConfig>();
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="logLevel">日志输出等级</param>
        /// <param name="logStr">日志内容</param>
        public void ConsoleLog(int logLevel, string logStr)
        {
            if (!isPrintLog) return;
            switch (logLevel)
            {
                case 0:
                    Debug.Log(logStr);
                    break;
                case 1:
                    Debug.LogWarning(logStr);
                    break;
                case 2:
                    Debug.LogError(logStr);
                    break;
            }
        }
        
        /** 读取Json配置 */
        public void GetJsonConfig(string configName, Action callBack)
        {
            // 全部读取本地配置
            // _loadConfigNum = 0;
            // GetNextJsonConfig(configName, callBack);
            
            // 先读取远程配置 再读取本地配置
            LoadConfig_Equipment(callBack);
        }

        /// <summary>
        /// 读取装备配置表
        /// </summary>
        /// <param name="cb">读取完成回调</param>
        private void LoadConfig_Equipment(Action cb)
        {
            if (isDebug)
            {
                LoadLocalConfig(0, "ComponentConfig", () => { LoadConfig_Shop(cb); });
                return;
            }

            StartCoroutine(ServerGetData.LoadRemoteConfig("ComponentConfig", text =>
            {
                if (text == null)
                {
                    // 读取远程配置失败 读取本地配置保底容错 继续读取下一配置
                    LoadLocalConfig(0, "ComponentConfig", () => { LoadConfig_Shop(cb); });
                }
                else
                {
                    // 读取远程配置成功 继续读取下一配置
                    JsonConfigToDict(0, text);
                    LoadConfig_Shop(cb);
                }
            }));
        }

        /// <summary>
        /// 读取商店配置表
        /// </summary>
        /// <param name="cb">读取完成回调</param>
        private void LoadConfig_Shop(Action cb)
        {
            if (isDebug)
            {
                LoadLocalConfig(1, "ShopConfig", () => { LoadConfig_RewardBox(cb); });
                return;
            }

            StartCoroutine(ServerGetData.LoadRemoteConfig("ShopConfig", text =>
            {
                if (text == null)
                {
                    // 读取远程配置失败 读取本地配置保底容错 继续读取下一配置
                    LoadLocalConfig(1, "ShopConfig", () => { LoadConfig_RewardBox(cb); });
                }
                else
                {
                    // 读取远程配置成功 继续读取下一配置
                    JsonConfigToDict(1, text);
                    LoadConfig_RewardBox(cb);
                }
            }));
        }

        /// <summary>
        /// 读取宝箱配置表
        /// </summary>
        /// <param name="cb">读取完成回调</param>
        private void LoadConfig_RewardBox(Action cb)
        {
            if (isDebug)
            {
                LoadLocalConfig(3, "RewardBox", () => { LoadConfig_Gm(cb); });
                return;
            }

            StartCoroutine(ServerGetData.LoadRemoteConfig("RewardBox", text =>
            {
                if (text == null)
                {
                    // 读取远程配置失败 读取本地配置保底容错 继续读取下一配置
                    LoadLocalConfig(3, "RewardBox", () => { LoadConfig_Gm(cb); });
                }
                else
                {
                    // 读取远程配置成功 继续读取下一配置
                    JsonConfigToDict(3, text);
                    LoadConfig_Gm(cb);
                }
            }));
        }

        /// <summary>
        /// 读取GM配置表
        /// </summary>
        /// <param name="cb">读取完成回调</param>
        private void LoadConfig_Gm(Action cb)
        {
            if (isDebug)
            {
                LoadLocalConfig(2, "GmConfig", () => { LoadConfig_Sign(cb); });
                return;
            }

            StartCoroutine(ServerGetData.LoadRemoteConfigPublic("GmConfig", text =>
            {
                if (text == null)
                {
                    // 读取远程配置失败 读取本地配置保底容错 继续读取下一配置
                    LoadLocalConfig(2, "GmConfig", () => { LoadConfig_Sign(cb); });
                }
                else
                {
                    // 读取远程配置成功 继续读取下一配置
                    JsonConfigToDict(2, text);
                    LoadConfig_Sign(cb);
                }
            }));
        }

        /// <summary>
        /// 读取签到配置表
        /// </summary>
        /// <param name="cb">读取完成回调</param>
        private void LoadConfig_Sign(Action cb)
        {
            if (isDebug)
            {
                LoadLocalConfig(4, "SignConfig", () => { LoadConfig_Task_1(cb); });
                return;
            }

            StartCoroutine(ServerGetData.LoadRemoteConfig("SignConfig", text =>
            {
                if (text == null)
                {
                    // 读取远程配置失败 读取本地配置保底容错 继续读取下一配置
                    LoadLocalConfig(4, "SignConfig", () => { LoadConfig_Task_1(cb); });
                }
                else
                {
                    // 读取远程配置成功 继续读取下一配置
                    JsonConfigToDict(4, text);
                    LoadConfig_Task_1(cb);
                }
            }));
        }

        private void LoadConfig_Task_1(Action cb)
        {
            if (isDebug)
            {
                LoadLocalConfig(5, "Task1Config", () => { LoadConfig_Task_2(cb); });
                return;
            }

            StartCoroutine(ServerGetData.LoadRemoteConfig("Task1Config", text =>
            {
                if (text == null)
                {
                    // 读取远程配置失败 读取本地配置保底容错 继续读取下一配置
                    LoadLocalConfig(5, "Task1Config", () => { LoadConfig_Task_2(cb); });
                }
                else
                {
                    // 读取远程配置成功 继续读取下一配置
                    JsonConfigToDict(5, text);
                    LoadConfig_Task_2(cb);
                }
            }));
        }

        private void LoadConfig_Task_2(Action cb)
        {
            if (isDebug)
            {
                LoadLocalConfig(6, "Task2Config", () => { LoadConfig_ShopItems(cb); });
                return;
            }

            StartCoroutine(ServerGetData.LoadRemoteConfig("Task2Config", text =>
            {
                if (text == null)
                {
                    // 读取远程配置失败 读取本地配置保底容错 继续读取下一配置
                    LoadLocalConfig(6, "Task2Config", () => { LoadConfig_ShopItems(cb); });
                }
                else
                {
                    // 读取远程配置成功 继续读取下一配置
                    JsonConfigToDict(6, text);
                    LoadConfig_ShopItems(cb);
                }
            }));
        }

        private void LoadConfig_ShopItems(Action cb)
        {
            if (isDebug)
            {
                LoadLocalConfig(7, "ShopItem", () => { LoadLocalConfig(8, "ProvinceConfigList", cb); });
                return;
            }

            StartCoroutine(ServerGetData.LoadRemoteConfig("ShopItem", text =>
            {
                if (text == null)
                {
                    // 读取远程配置失败 读取本地配置保底容错 继续读取下一配置
                    LoadLocalConfig(7, "ShopItem", () => { LoadLocalConfig(8, "ProvinceConfigList", cb); });
                }
                else
                {
                    // 读取远程配置成功 继续读取下一配置
                    JsonConfigToDict(7, text);
                    LoadLocalConfig(8, "ProvinceConfigList", cb);
                }
            }));
        }

        /// <summary>
        /// 读取本地数据
        /// </summary>
        /// <param name="configIndex">配置索引</param>
        /// <param name="configName">配置名称</param>
        /// <param name="cb">回调</param>
        private void LoadLocalConfig(int configIndex, string configName, Action cb)
        {
            ConsoleLog(0, new StringBuilder("本地配置[" + configName + "]读取成功").ToString());
            TextAsset text = Resources.Load<TextAsset>("Json/" + configName);
            string jsonData = "" + text.text;
            JsonConfigToDict(configIndex, jsonData);
            cb();
        }

        // /** 当前配置读取完成 读取下一个配置 */
        // private void GetNextJsonConfig(string configName, Action callBack)
        // {
        //     if (_loadConfigNum == GlobalValueManager.JsonConfig[configName].Count)
        //     {
        //         // 配置全部读取完成 执行回调
        //         ConsoleLog(0, "全部配置读取完成，执行回调");
        //         callBack();
        //         return;
        //     }
        //     // 配置没有全部读取完成 继续读取配置
        //     var remoteUrlKey = GlobalValueManager.JsonConfig[configName][_loadConfigNum][0];
        //     var remoteUrlIdx = int.Parse(GlobalValueManager.JsonConfig[configName][_loadConfigNum][1]);
        //     var text = Resources.Load<TextAsset>("Json/" + GlobalValueManager.JsonNameList[remoteUrlKey]);
        //     var jsonData = "" + text.text;
        //     JsonConfigToDict(remoteUrlIdx, jsonData);
        //     // 读取下一个配置
        //     _loadConfigNum += 1;
        //     GetNextJsonConfig(configName, callBack);
        // }

        /** Json配置表解析为数据字典 */
        private void JsonConfigToDict(int configInd, string configData)
        {
            switch (configInd)
            {
                case 0:  // 配件配置表
                    ComponentConfigs = JsonConvert.DeserializeObject<List<ComponentConfig>>(configData);
                    foreach (var config in ComponentConfigs)
                    {
                        ComponentConfigDict.Add(config.ID, config);
                        if (!ComponentTypeConfigDict.ContainsKey(config.Type))
                        {
                            ComponentTypeConfigDict.Add(config.Type, new List<ComponentConfig>());
                        }

                        ComponentTypeConfigDict[config.Type].Add(config);

                        if (!ComponentQualityConfigDict.ContainsKey(config.Quality))
                        {
                            ComponentQualityConfigDict.Add(config.Quality, new List<ComponentConfig>());
                        }

                        ComponentQualityConfigDict[config.Quality].Add(config);
                    }
                    break;
                case 1:  // 商店配置表
                    ShopConfigs = JsonConvert.DeserializeObject<List<ShopConfig>>(configData);
                    foreach (var shopConfig in ShopConfigs)
                    {
                        ShopConfigDict.Add(shopConfig.ID, shopConfig);
                        if (!ShopTypeConfigDict.ContainsKey(shopConfig.Type))
                        {
                            ShopTypeConfigDict.Add(shopConfig.Type, new List<ShopConfig>());
                        }

                        ShopTypeConfigDict[shopConfig.Type].Add(shopConfig);
                    }
                    break;
                case 2:  // GM配置表
                    GmConfigs = JsonConvert.DeserializeObject<List<GmConfig>>(configData);
                    foreach (var gmConfig in GmConfigs)
                    {
                        GmConfigDict.Add(gmConfig.appId, gmConfig);
                    }
                    break;
                case 3:  // 宝箱配置表
                    RewardBoxConfigs = JsonConvert.DeserializeObject<List<RewardBoxConfig>>(configData);
                    foreach (var rewardBoxConfig in RewardBoxConfigs)
                    {
                        RewardBoxConfigDict.Add(rewardBoxConfig.ID, rewardBoxConfig);
                    }
                    break;
                case 4:  // 签到配置表
                    SignConfigs = JsonConvert.DeserializeObject<List<SignConfig>>(configData);
                    break;
                case 5:  // 日常任务配置表
                    TaskConfig1s = JsonConvert.DeserializeObject<List<TaskConfig1>>(configData);
                    foreach (var config in TaskConfig1s)
                    {
                        TaskConfigDict1.Add(config.ID, config);
                    }
                    break;
                case 6:  // 成就任务配置表
                    TaskConfig2s = JsonConvert.DeserializeObject<List<TaskConfig2>>(configData);
                    foreach (var config in TaskConfig2s)
                    {
                        TaskConfigDict2.Add(config.ID, config);
                    }
                    break;
                case 7:  // 限时商店商品配置表
                    ShopItemConfigs = JsonConvert.DeserializeObject<List<ShopItemConfig>>(configData);
                    foreach (var config in ShopItemConfigs)
                    {
                        ShopItemConfigDict.Add(config.LevelID, config);
                    }
                    break;
                case 8:  // 省份配置表
                    provinceConfigs = JsonConvert.DeserializeObject<List<ProvinceConfig>>(configData);
                    break;
            }
        }
    }
}