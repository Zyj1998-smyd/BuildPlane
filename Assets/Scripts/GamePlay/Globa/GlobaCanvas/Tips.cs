using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Globa.GlobaCanvas
{
    public class Tips : MonoBehaviour
    {
        /** 提示文本列表 */
        public Sprite[] textSprites;

        /** 提示字符串列表 */
        private readonly List<string> _tipStrList = new List<string>(18)
        {
            "暂未开放", "金币不足", "钻石不足", "体力不足", "今日已签到，明天再来吧！", "抽奖正在进行中...", "能力未解锁", "挑战关卡未解锁",
            "没有前置关卡", "请先完成当前关卡", "奖励已领取", "未达到领取条件", "签到时间未到！", "签到奖励已领取！", "超过当日购买限制", "科技碎片不足",
            "请先完成前置关卡", "广告还未准备好！", "已装备触手", "未装备触手"
        };

        /// <summary>
        /// 获取提示文本
        /// </summary>
        /// <param name="tip">提示字符串</param>
        /// <returns></returns>
        public Sprite GetTipText(string tip)
        {
            return textSprites[_tipStrList.IndexOf(tip)];
        }
    }
}