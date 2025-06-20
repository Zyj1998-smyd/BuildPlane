using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common.Tool
{
    public static class ToolFunManager
    {
        /** 获取当前秒级时间戳 */
        public static long GetCurrTime()
        {
            return new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        }
        
        /** UTC时间戳Timestamp转换为北京时间 */
        public static DateTime GetDateTime(long timestamp)
        {
            DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long lTime = long.Parse(timestamp + "0000000");
            TimeSpan timeSpan = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(timeSpan).AddHours(8);
            return targetDt;
        }
        
        /** 时间转字符串 */
        public static string TimeToString(long result)
        {
            var hour = result / 3600;
            var minute = (result - hour * 3600) / 60;
            var second = result - hour * 3600 - minute * 60;
            var data = $"{hour:D2}:{minute:D2}:{second:D2}";
            return data;
        }

        /// <summary>
        /// 格式化数字
        /// </summary>
        /// <param name="num">数字</param>
        /// <param name="isDis">是否是距离</param>
        /// <returns>格式化的字符串</returns>
        public static string GetText(float num, bool isDis)
        {
            StringBuilder numString = new StringBuilder();
            if (num >= 1000000000)
            {
                // 大于一B
                float numTmp = num / 1000000000f;
                numString.Append(numTmp.ToString("F2").TrimEnd('0').TrimEnd('.'));
                numString.Append("B");
            }
            else if (num >= 1000000)
            {
                // 大于一M
                float numTmp = num / 1000000f;
                numString.Append(numTmp.ToString("F2").TrimEnd('0').TrimEnd('.'));
                numString.Append(" M");
            }
            else if (num >= 1000)
            {
                // 大于一K
                float numTmp = num / 1000f;
                numString.Append(numTmp.ToString("F2").TrimEnd('0').TrimEnd('.'));
                numString.Append("K");
            }
            else
            {
                // 小于一千 不进行折算
                numString.Append(num.ToString("F2").TrimEnd('0').TrimEnd('.'));
            }

            return numString.ToString();
        }
        
        /** 获取字符串中的数值列表(新) "num" or "num,num" */
        public static List<int> GetNumFromStrNew(string str)
        {
            // Debug.Log(str);
            if (str == "") return new List<int>();
            // 字符串 "[num]" or "[num,num]
            if (str.IndexOf("[", StringComparison.Ordinal) != -1)
            {
                return GetNumListFromStr(str);
            }
            // 普通逗号分隔的字符串 num,num
            var strSplitArr = str.IndexOf(",", StringComparison.Ordinal) != -1 ? str.Split(',') : str.Split('|');
            var numList = new List<int>(strSplitArr.Length);
            for (var i = 0; i < strSplitArr.Length; i++)
            {
                numList.Add(int.Parse(strSplitArr[i]));
            }

            return numList;
        }
        
        /** 获取字符串中的数值列表 "[num]" or "[num,num]" */
        public static List<int> GetNumListFromStr(string str)
        {
            if (str == "[]") return new List<int>();
            var strTemp1 = str.Remove(0, 1);
            var strTemp2 = strTemp1.Remove(strTemp1.Length - 1, 1);
            var strTempArr = strTemp2.Split(',');
            var numList = new List<int>(strTempArr.Length);
            foreach (var numStr in strTempArr)
            {
                numList.Add(int.Parse(numStr));
            }

            return numList;
        }

        /// <summary>
        /// 获取字符串中的数字(减号分隔)
        /// </summary>
        public static List<int> GetNumListFromStrBySubStr(string str)
        {
            if (str == "") return new List<int>();
            string[] strings = str.Split('-');
            List<int> nums = new List<int>(strings.Length);
            for (int i = 0; i < strings.Length; i++)
            {
                nums.Add(int.Parse(strings[i]));
            }

            return nums;
        }

        /** 获取字符串中的数值列表(多个列表) "num,num|num,num" */
        public static List<List<int>> GetMoreNumListFromStr(string str)
        {
            var numList = new List<List<int>>();
            var strArr = str.Split('|');
            for (int i = 0; i < strArr.Length; i++)
            {
                var numListTmp = new List<int>();
                if (strArr[i].Contains(","))
                {
                    var strSubArr = strArr[i].Split(',');
                    for (int j = 0; j < strSubArr.Length; j++)
                    {
                        numListTmp.Add(int.Parse(strSubArr[j]));
                    }
                }
                else
                {
                    numListTmp.Add(int.Parse(strArr[i]));
                }

                numList.Add(numListTmp);
            }

            return numList;
        }

        /// <summary>
        /// 矩形伤害范围
        /// </summary>
        /// <param name="rectLength">攻击矩形长度</param>
        /// <param name="rectWidth">攻击矩形宽度</param>
        /// <param name="attacker">攻击者</param>
        /// <param name="target">攻击目标</param>
        /// <returns></returns>
        public static bool IsInRange(float rectLength, float rectWidth, Transform attacker, Transform target)
        {
            // 攻击者位置指向目标位置的向量
            Vector3 direction = target.position - attacker.position;
            // 点乘结果 如果大于0表示目标在攻击者前方
            float dot = Vector3.Dot(attacker.forward, direction);
            // 小于0表示在攻击者后方 不在矩形攻击区域 返回false
            if (dot < 0) return false;
            // direction在attacker正前方上的投影
            float forwardProject = Vector3.Project(direction, attacker.forward).magnitude;
            // 大于矩形长度表示不在矩形攻击区域 返回false
            if (forwardProject > rectLength) return false;
            // direction在attacker右方的投影
            float rightProject = Vector3.Project(direction, attacker.right).magnitude;
            // 取绝对值与矩形宽度的一半进行比较
            return Mathf.Abs(rightProject) <= rectWidth * 0.5f;
        }
        
        /** 判断今天是否结束 */
        public static bool JudgeDayStampOutTime(DateTime dayStamp)
        {
            var next = new DateTime(dayStamp.Year, dayStamp.Month, dayStamp.Day, 23, 59, 59);
            var totalSeconds = (next - DateTime.Now).TotalSeconds;
            if (totalSeconds > 0)
            {
                // 今天还没结束
                ConfigManager.Instance.ConsoleLog(0, "判断 未到新的一天...");
                return false;
            }
            // 今天已经结束了
            ConfigManager.Instance.ConsoleLog(0, "判断 已到新的一天...");
            return true;
        }

        /** 判断本周是否结束 */
        public static bool JudgeDayStampOutWeek(DateTime dayStamp)
        {
            var curWeekEndTime = dayStamp.AddDays((7 - (int)dayStamp.DayOfWeek) % 7);
            var time = new DateTime(curWeekEndTime.Year, curWeekEndTime.Month, curWeekEndTime.Day, 23, 59, 59);
            var totalSeconds = (time - DateTime.Now).TotalSeconds;
            if (totalSeconds > 0)
            {
                // 本周还没结束
                return false;
            }
            // 本周已经结束了
            return true;
        }

        /// <summary>
        /// 获取关卡间隔刷新敌人数量 形如 "num|num,num"
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>关卡间隔刷新敌人数量数组 0: 敌人类型 1: 敌人数量</returns>
        public static List<int> GetLevelGapEnemyNum(string str)
        {
            var result = new List<int>();
            if (str == "" || str == "0") return result;
            var stringArr = str.Split('|');
            result.Add(int.Parse(stringArr[0]));
            var numStrArr = stringArr[1].Split(',');
            var minNum = int.Parse(numStrArr[0]);
            var maxNum = int.Parse(numStrArr[1]);
            result.Add(Random.Range(minNum, maxNum));
            return result;
        }
        
        /** 计算两点的距离 */
        public static float Distance2DPos(Vector2 pos0, Vector2 pos1)
        {
            var x = (pos0.x - pos1.x) * (pos0.x - pos1.x);
            var y = (pos0.y - pos1.y) * (pos0.y - pos1.y);
            var dis = Mathf.Sqrt((x + y));
            return dis;
        }

        /// <summary>
        /// 十六进制颜色转GRBA颜色
        /// </summary>
        /// <param name="hex">十六进制颜色</param>
        /// <returns>RGBA颜色</returns>
        public static Color HexToColor(string hex)
        {
            // 分离红、绿、蓝色值
            byte r = (byte)Convert.ToUInt32(hex.Substring(0, 2), 16);
            byte g = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16);
            byte b = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16);
            return new Color32(r, g, b, 255);
        }

        /// <summary>
        /// 获取毫秒为单位的时间戳
        /// </summary>
        public static long GetCurrTimeMs()
        {
            return new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        }
        
        /// <summary>
        /// 计算MD5签名
        /// </summary>
        /// <param name="input">签名算法字符串</param>
        public static string CalculateMd5(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
        
        /// <summary>
        /// 长字符串截断处理
        /// </summary>
        /// <param name="str">长字符串</param>
        /// <param name="len">截取长度</param>
        /// <param name="endStr">截取后结尾字符串</param>
        public static string LongStrDeal(string str, int len, string endStr)
        {
            var temp = str.Substring(0, (str.Length < len + 1) ? str.Length : len + 1);
            var encodedBytes = System.Text.Encoding.ASCII.GetBytes(temp);
            var outPutStr = "";
            var count = 0;
            for (var i = 0; i < temp.Length; i++)
            {
                count += ((encodedBytes[i] == 63) ? 2 : 1);
                if (count <= len - endStr.Length)
                {
                    outPutStr += temp.Substring(i, 1);
                }
                else if (count > len)
                {
                    break;
                }
            }
            if (count <= len)
            {
                outPutStr = temp;
                endStr = "";
            }

            outPutStr += endStr;
            return outPutStr;
        }

        /// <summary>
        /// 次日0点时间戳（毫秒）
        /// </summary>
        public static long GetTomorrowMidnightTime()
        {
            // 获取次日0点（本地时间）
            DateTime tomorrowMidnightLocal = DateTime.Today.AddDays(1);
            // 转换为UTC时间
            DateTime tomorrowMidnightUtc = tomorrowMidnightLocal.ToUniversalTime();
            // 计算与Unix纪元的时间差
            TimeSpan span = tomorrowMidnightUtc - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timestamp = (long)span.TotalMilliseconds;
            return timestamp;
        }
    }
}