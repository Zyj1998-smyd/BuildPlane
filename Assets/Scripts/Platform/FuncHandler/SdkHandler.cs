using System;
using UnityEngine;
using UnityEngine.UI;

namespace Platform.FuncHandler
{
    public abstract class SdkHandler : MonoBehaviour
    {
        /// <summary>
        /// 平台登录
        /// </summary>
        /// <param name="cb">平台登录完成回调</param>
        public abstract void PlatformLogin(Action<string> cb);

        /// <summary>
        /// 播放激励视频广告
        /// </summary>
        /// <param name="pointFrom">广告点位</param>
        /// <param name="successCb">激励视频播放完成回调</param>
        /// <param name="failCb">激励视频播放未完成回调</param>
        /// <param name="type">0:创建 1:展示</param>
        /// <param name="index">激励视频广告ID列表索引</param>
        public abstract void VideoControl(string pointFrom, Action successCb, Action failCb, int type = 1, int index = 0);

        /// <summary>
        /// 分享(通用分享)
        /// </summary>
        /// <param name="successCb">分享成功回调</param>
        public abstract void ShareControl(Action successCb);

        /// <summary>
        /// 获取用户位置信息
        /// </summary>
        /// <param name="successCb">获取用户位置信息成功回调</param>
        /// <param name="failCb">获取用户位置信息失败回调</param>
        public abstract void LocationControl(Action successCb, Action failCb);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="rect">授权UI按钮</param>
        /// <param name="successCb">获取用户信息成功回调</param>
        /// <param name="failCb">获取用户信息失败回调</param>
        public abstract void UserInfoControl(RectTransform rect, Action successCb, Action failCb);

        /// <summary>
        /// 游戏圈
        /// </summary>
        /// <param name="type">0:创建游戏圈按钮 1:展示游戏圈按钮 2:隐藏游戏圈按钮 3:销毁游戏圈按钮</param>
        /// <param name="rect">游戏圈UI按钮</param>
        /// <param name="cb">回调</param>
        public abstract void GameClubControl(int type, RectTransform rect, Action cb);

        /// <summary>
        /// 排行榜
        /// </summary>
        /// <param name="isShow">展示/隐藏排行榜</param>
        /// <param name="type">上报的排行榜类型</param>
        /// <param name="rawImage">排行榜渲染节点</param>
        /// <param name="canvasScaler">Canvas缩放组件</param>
        /// <param name="cb">回调</param>
        public abstract void RankControl(bool isShow, int type, RawImage rawImage, CanvasScaler canvasScaler, Action cb);

        /// <summary>
        /// 上报排行数据
        /// <param name="type">上报的排行榜类型 0: 关卡 1: 积分</param>
        /// </summary>
        public abstract void ReportRankData(int type);

        /// <summary>
        /// 检测是否加到我的小程序
        /// </summary>
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        /// <param name="noCanUseCb">API不可用回调</param>
        public abstract void CheckIsAddedToMyMiniProgram(Action<bool> successCb, Action failCb, Action noCanUseCb);

        /// <summary>
        /// 上报打点数据
        /// </summary>
        public abstract void ReportEvent(string reportId, string data = null);
        
        /// <summary>
        /// 获取用户信息权限
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        /// <param name="isClick">是否用户主动触发</param>
        /// </summary>
        public abstract void GetUserInfoPermission(Action successCb, Action failCb, bool isClick);

        /// <summary>
        /// 获取系统字体
        /// </summary>
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        public abstract void GetSystemFont(Action<Font> successCb, Action failCb);

        /// <summary>
        /// 主动展示弹窗请求用户同意
        /// </summary>
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        public abstract void RequirePrivacyAuthorize(Action successCb, Action failCb);

        /// <summary>
        /// 上报自定义分析数据(打点)
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="paramKey">上报数据Key</param>
        /// <param name="paramValue">上报数据Value</param>
        public abstract void ReportAnalytics<T>(string eventName, string paramKey, T paramValue);

        /// <summary>
        /// 短震动
        /// </summary>
        public abstract void ShortVibrateControl();

        /// <summary>
        /// 长震动
        /// </summary>
        public abstract void LongVibrateControl();

        /// <summary>
        /// Banner横幅广告
        /// <param name="type">0:创建 1:展示 2:隐藏 3:销毁</param>
        /// <param name="isBottom">底部/顶部</param>
        /// </summary>
        public abstract void BannerControl(int type, bool isBottom);

        /// <summary>
        /// 插屏广告
        /// <param name="type">0:创建 1:展示 2:销毁</param>
        /// </summary>
        public abstract void InsertControl(int type);

        /// <summary>
        /// 原生广告
        /// <param name="type">0:创建 1:加载 2:展示 3:销毁</param>
        /// <param name="index">广告ID列表索引</param>
        /// </summary>
        public abstract void NativeControl(int type, int index);

        // ------------------------------------------------- 商业化方案 -------------------------------------------------
        /// <summary>
        /// 读取远程配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="cb">回调</param>
        public abstract void GetRemoteConfig(string configName, Action cb);
        
        // ------------------------------------------------- 字节特有 -------------------------------------------------
        /// <summary>
        /// 侧边栏进入
        /// </summary>
        [HideInInspector] public bool _isSideBarStart;

        /// <summary>
        /// 需要录屏
        /// </summary>
        [HideInInspector] public bool _isNeedRecord;
        
        /// <summary>
        /// 判断当前宿主APP是否支持从侧边栏进入游戏
        /// </summary>
        /// <param name="btnFollow">收藏游戏按钮</param>
        public abstract void CheckSideBarStartGame(GameObject btnFollow);

        /// <summary>
        /// 跳转到侧边栏入口
        /// </summary>
        public abstract void NavigateToSideBar();

        /// <summary>
        /// 开始录屏
        /// </summary>
        public abstract void OnStartRecord();

        /// <summary>
        /// 结束录屏
        /// </summary>
        public abstract void OnStopRecord(bool isComplete);

        /// <summary>
        /// 分享录屏
        /// </summary>
        /// <param name="cb">回调</param>
        public abstract void OnShareVideo(Action cb);

        /// <summary>
        /// 直出授权订阅检测
        /// </summary>
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        public abstract void CheckFeedSubscribeStatus(Action successCb, Action failCb);

        /// <summary>
        /// 直出订阅弹窗触发
        /// </summary>
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        public abstract void RequestFeedSubscribe(Action successCb, Action failCb);

        /// <summary>
        /// 直出上报可交互事件
        /// </summary>
        public abstract void ReportSceneFeed();

    }
}