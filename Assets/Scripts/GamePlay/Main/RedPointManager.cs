using System.Collections.Generic;
using Common.Tool;
using Data;
using Data.ClassData;
using Data.ConfigData;
using Newtonsoft.Json;
using Platform;
using UnityEngine;

namespace GamePlay.Main
{
    public class RedPointManager : MonoBehaviour
    {
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            ConfigManager.Instance.ConsoleLog(0, "提示红点管理类初始化完成");
        }

        /// <summary>
        /// 获取菜单栏提示红点
        /// </summary>
        internal bool[] GetMenuRedPoint()
        {
            bool[] result = { false, false, false, false, false };
            result[1] = GetRedPoint_Shop();
            result[2] = GetRedPoint_Main();
            result[3] = GetRedPoint_Build();
            return result;
        }

        /// <summary>
        /// 获取商店提示红点
        /// </summary>
        internal bool GetRedPoint_Shop()
        {
            for (int i = 0; i < ConfigManager.Instance.ShopConfigs.Count; i++)
            {
                ShopConfig shopConfig = ConfigManager.Instance.ShopConfigs[i];
                if (shopConfig.Limit > 0)
                {
                    if (shopConfig.BuyType == 0)
                    {
                        int buyOkNum = DataHelper.CurUserInfoData.shopLimits.GetValueOrDefault(shopConfig.ID, 0);
                        int canBuyNum = shopConfig.Limit - buyOkNum;
                        if (canBuyNum > 0) return true;
                    }
                }
            }
            
            return false;
        }

        /// <summary>
        /// 获取组装页面提示红点
        /// </summary>
        internal bool GetRedPoint_Build()
        {
            List<int> ids = new List<int>(DataHelper.CurUserInfoData.equipments.Keys);
            List<int> lvs = new List<int>(DataHelper.CurUserInfoData.equipments.Values);

            // 可以升级
            for (int i = 0; i < ids.Count; i++)
            {
                int curLevel = lvs[i];
                int chipNum = DataHelper.CurUserInfoData.equipmentChips.GetValueOrDefault(ids[i], 0);
                float targetChipNumTmp = GlobalValueManager.EquipmentUpGradeChipNum;
                for (int j = 0; j < curLevel - 1; j++)
                {
                    targetChipNumTmp *= GlobalValueManager.EquipmentUpGradeChipUpGradeNum;
                }

                int targetChipNum = Mathf.CeilToInt(targetChipNumTmp);
                
                if (chipNum >= targetChipNum) return true;
            }
            
            // 可以查看涂装
            if (DataHelper.CurUserInfoData.equipmentPaintNews.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取主页面提示红点
        /// </summary>
        internal bool GetRedPoint_Main()
        {
            if (GetRedPoint_Sign()) return true;
            if (GetRedPoint_Raffle()) return true;
            if (GetRedPoint_Task()) return true;
            if (GetRedPoint_LandMark()) return true;
            return false;
        }

        /// <summary>
        /// 获取签到提示红点
        /// </summary>
        internal bool GetRedPoint_Sign()
        {
            SignInfoData signInfo = JsonConvert.DeserializeObject<SignInfoData>(DataHelper.CurUserInfoData.signInfo);
            if (signInfo.day > 7) return false;
            return signInfo.isSign != 2;
        }

        /// <summary>
        /// 获取转盘提示红点
        /// </summary>
        internal bool GetRedPoint_Raffle()
        {
            RaffleInfoData raffleInfo = JsonConvert.DeserializeObject<RaffleInfoData>(DataHelper.CurUserInfoData.raffleInfo);
            if (raffleInfo.lastFreeTime == 0) return true;
            long timeTmp = ToolFunManager.GetCurrTime() - raffleInfo.lastFreeTime;
            if (timeTmp >= GlobalValueManager.RaffleFreeTime * 60) return true;
            return false;
        }

        /// <summary>
        /// 获取任务提示红点
        /// </summary>
        internal bool GetRedPoint_Task()
        {
            if (GetRedPoint_Task_Daily()) return true;
            if (GetRedPoint_Task_Goal()) return true;
            return false;
        }

        /// <summary>
        /// 获取日常任务提示红点
        /// </summary>
        internal bool GetRedPoint_Task_Daily()
        {
            TaskDailyInfoData taskDailyInfo = JsonConvert.DeserializeObject<TaskDailyInfoData>(DataHelper.CurUserInfoData.taskInfo1);
            for (int i = 0; i < taskDailyInfo.rewardGet.Count; i++)
            {
                if (taskDailyInfo.rewardGet[i] == 0)
                {
                    if (taskDailyInfo.activePoint >= GlobalValueManager.TaskDayActivePoints[i]) return true;
                }
            }

            foreach (KeyValuePair<int, int> taskState in taskDailyInfo.taskState)
            {
                if (taskState.Value != -1)
                {
                    TaskConfig1 taskConfig = ConfigManager.Instance.TaskConfigDict1[taskState.Key];
                    if (taskState.Value >= taskConfig.Num) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取成就任务提示红点
        /// </summary>
        internal bool GetRedPoint_Task_Goal()
        {
            Dictionary<int, int[]> taskInfo = JsonConvert.DeserializeObject<Dictionary<int, int[]>>(DataHelper.CurUserInfoData.taskInfo2);
            foreach (KeyValuePair<int, int[]> taskInfoData in taskInfo)
            {
                int taskId = taskInfoData.Key;
                int[] taskData = taskInfoData.Value;
                TaskConfig2 taskConfig = ConfigManager.Instance.TaskConfigDict2[taskId];
                List<int> targets = ToolFunManager.GetNumFromStrNew(taskConfig.Num);
                if (taskData[0] < targets.Count)
                {
                    int n = 0;
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (taskData[1] >= targets[i]) n += 1;
                    }

                    if (n != 0)
                    {
                        if (taskData[0] < n) return true;
                    }
                }
            }
            
            return false;
        }

        /// <summary>
        /// 获取地标打卡提示红点
        /// </summary>
        internal bool GetRedPoint_LandMark()
        {
            foreach (KeyValuePair<int, int> data in DataHelper.CurUserInfoData.landMarkInfo)
            {
                if (data.Value == 0) return true;
            }
            
            return false;
        }
    }
}