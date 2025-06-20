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
            "Coming soon"
            ,"Not enough coins" 
            ,"Not enough gems" 
            ,"Not enough energy" 
            ,"Already checked in today. Come back tomorrow!"
            ,"Spinning in progress..." 
            ,"Not unlocked yet"
            ,"Challenge level locked"
            ,"No prerequisite level"
            ,"Please complete the current level first"
            ,"Reward already claimed"
            ,"Conditions not met" 
            ,"Check-in time not reached yet!"
            ,"Daily login reward claimed!"
            ,"Daily purchase limit exceeded"
            ,"Not enough tech fragments"
            ,"Please complete the prerequisite level first"
            ,"Ads not ready yet!"
            ,"Tentacle equipped"
            ,"Tentacle not equipped"
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