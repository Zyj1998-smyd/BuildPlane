using System;
using System.Collections.Generic;
using UnityEngine;

namespace Platform.ServerHandler
{
    public abstract class ServerHandler : MonoBehaviour
    {
        /// <summary>
        /// 获取网络IP地址信息数据
        /// <param name="cb">回调</param>
        /// </summary>
        public abstract void GetNetIpInfo(Action cb);

        /// <summary>
        /// 初始化
        /// <param name="code"></param>
        /// <param name="loginCb">登录方法</param>
        /// <param name="cb">初始化完成回调</param>
        /// </summary>
        public abstract void Init(string code, Action loginCb, Action<string> cb);

        /// <summary>
        /// 初始化用户数据
        /// </summary>
        public abstract void InitUserData(Action<string> cb);

        /// <summary>
        /// 用户上传数据
        /// </summary>
        /// <param name="cb">回调</param>
        public abstract void UpdateServerData(Action cb);

        /// <summary>
        /// 获取排名列表
        /// </summary>
        /// <param name="cb">回调</param>
        public abstract void GetGeneralRank(Action cb);

        /// <summary>
        /// 上传排名值
        /// </summary>
        public abstract void UpdateGeneralRank();

        /// <summary>
        /// 获取全国排行榜
        /// </summary>
        /// <param name="type">排行榜类型 0:通关榜 1:积分榜 2:飞行距离榜</param>
        /// <param name="cb">回调</param>
        public abstract void GetRankAll(int type, Action cb);

        /// <summary>
        /// 更新全国排行榜
        /// </summary>
        /// <param name="type">排行榜类型 0:通关榜 1:积分榜 2:飞行距离榜</param>
        public abstract void UpdateRankAll(int type);

        /// <summary>
        /// 上传用户名头像
        /// <param name="cb">回调</param>
        /// </summary>
        public abstract void UpdateUserInfo(Action cb);

        /// <summary>
        /// 获取全国排行榜判断Key
        /// </summary>
        /// <returns>Key</returns>
        public abstract string GetRankAllJudgeKey();

        /// <summary>
        /// 广告上报
        /// </summary>
        public abstract void ReportAd(bool complete);

        /// <summary>
        /// 获取用户信息判断Key
        /// </summary>
        /// <returns>Key</returns>
        public abstract string GetUserInfoJudgeKey();
        
        /// <summary>
        /// 清空用户数据
        /// </summary>
        public abstract void ClearUserData();
    }
}