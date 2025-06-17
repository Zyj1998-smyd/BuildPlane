using System;
using System.Text;
using Common.LoadRes;
using Data;
using GamePlay.Globa;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Platform.FuncHandler
{
    public class Sdk_Editor : SdkHandler
    {
        /** 激励视频播放完成回调 */
        private Action _successCallBack;

        /** 系统字体 */
        private Font _systemFont;

        private void Awake()
        {
            _isSideBarStart = false;
            _isNeedRecord = false;
        }

        /// <summary>
        /// 平台登录
        /// </summary>
        /// <param name="cb">平台登录完成回调</param>
        public override void PlatformLogin(Action<string> cb)
        {
            DataHelper.CurLaunchSceneId = -1;
            
            string openId = PlayerPrefs.GetString("OpenId", "");
            if (openId == "")
            {
                openId = new StringBuilder("XXGame" + Random.Range(1000001, 10000001)).ToString();
                PlayerPrefs.SetString("OpenId", openId);
            }

            GameSdkManager._instance._serverScript.Init(openId, null, cb);
        }
        
        /// <summary>
        /// 播放激励视频广告
        /// </summary>
        /// <param name="pointFrom">广告点位</param>
        /// <param name="successCb">激励视频播放完成回调</param>
        /// <param name="failCb">激励视频播放未完成回调</param>
        /// <param name="type">0:创建 1:展示</param>
        /// <param name="index">激励视频广告ID列表索引</param>
        public override void VideoControl(string pointFrom, Action successCb, Action failCb, int type = 1, int index = 0)
        {
            _successCallBack = successCb;
            ConfigManager.Instance.ConsoleLog(0, "当前运行环境是编辑器，直接执行播放完成回调");
            _successCallBack();
        }

        /// <summary>
        /// 分享(通用分享)
        /// </summary>
        /// <param name="successCb">分享成功回调</param>
        public override void ShareControl(Action successCb)
        {
            successCb();
        }

        /// <summary>
        /// 获取用户位置信息
        /// </summary>
        /// <param name="successCb">获取用户位置信息成功回调</param>
        /// <param name="failCb">获取用户位置信息失败回调</param>
        public override void LocationControl(Action successCb, Action failCb)
        {
            // 编辑器 不支持获取位置信息
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="rect">授权UI按钮</param>
        /// <param name="successCb">获取用户信息成功回调</param>
        /// <param name="failCb">获取用户信息失败回调</param>
        public override void UserInfoControl(RectTransform rect, Action successCb, Action failCb)
        {
            // 编辑器 不支持获取用户信息
            GameGlobalManager._instance.ShowTips("暂时无法获取用户信息");
        }

        /// <summary>
        /// 游戏圈
        /// </summary>
        /// <param name="type">0:创建游戏圈按钮 1:展示游戏圈按钮 2:隐藏游戏圈按钮 3:销毁游戏圈按钮</param>
        /// <param name="rect">游戏圈UI按钮</param>
        /// <param name="cb">回调</param>
        public override void GameClubControl(int type, RectTransform rect, Action cb)
        {
            // 编辑器 不支持游戏圈
            if (rect)
                rect.gameObject.SetActive(false);
        }

        /// <summary>
        /// 排行榜
        /// </summary>
        /// <param name="isShow">展示/隐藏排行榜</param>
        /// <param name="type">上报的排行榜类型</param>
        /// <param name="rawImage">排行榜渲染节点</param>
        /// <param name="canvasScaler">Canvas缩放组件</param>
        /// <param name="cb">回调</param>
        public override void RankControl(bool isShow, int type, RawImage rawImage, CanvasScaler canvasScaler, Action cb)
        {
            // 编辑器 不支持排行榜
        }

        /// <summary>
        /// 上报排行数据
        /// <param name="type">上报的排行榜类型 0: 关卡 1: 积分</param>
        /// </summary>
        public override void ReportRankData(int type)
        {
            // 编辑器 不支持上报排行数据
        }

        /// <summary>
        /// 检测是否加到我的小程序
        /// </summary>
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        /// <param name="noCanUseCb">API不可用回调</param>
        public override void CheckIsAddedToMyMiniProgram(Action<bool> successCb, Action failCb, Action noCanUseCb)
        {
            // 编辑器 不支持检测是否加到我的小程序
            noCanUseCb();
        }
        
        /// <summary>
        /// 数据上报
        /// </summary>
        public override void ReportEvent(string reportId, string data = null)
        {
            // 编辑器 不支持上报数据
        }

        /// <summary>
        /// 获取用户信息权限
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        /// </summary>
        public override void GetUserInfoPermission(Action successCb, Action failCb, bool isClick)
        {
            // 编辑器 不支持获取权限
            successCb();
        }

        /// <summary>
        /// 获取系统字体
        /// </summary>
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        public override void GetSystemFont(Action<Font> successCb, Action failCb)
        {
            if (_systemFont == null)
            {
                string resName = "Font_Common";
                LoadResources.XXResourcesLoad(resName, handleTmp =>
                {
                    _systemFont = handleTmp;
                    successCb(_systemFont);
                });
            }
            else
            {
                successCb(_systemFont);
            }
        }

        /// <summary>
        /// 主动展示弹窗请求用户同意
        /// </summary>
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        public override void RequirePrivacyAuthorize(Action successCb, Action failCb)
        {
            successCb();
        }

        /// <summary>
        /// 上报自定义分析数据(打点)
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="paramKey">上报数据Key</param>
        /// <param name="paramValue">上报数据Value</param>
        public override void ReportAnalytics<T>(string eventName, string paramKey, T paramValue)
        {
            // 编辑器 不支持上报自定义分析数据
        }

        /// <summary>
        /// 短震动
        /// </summary>
        public override void ShortVibrateControl()
        {
            // 编辑器 不支持震动
        }

        /// <summary>
        /// 长震动
        /// </summary>
        public override void LongVibrateControl()
        {
            // 编辑器 不支持震动
        }

        public override void BannerControl(int type, bool isBottom)
        {
            // 编辑器 不支持横幅广告
        }

        public override void InsertControl(int type)
        {
            // 编辑器 不支持插屏广告
        }

        /// <summary>
        /// 原生广告
        /// <param name="type">0:创建 1:加载 2:展示 3:销毁</param>
        /// <param name="index">广告ID列表索引</param>
        /// </summary>
        public override void NativeControl(int type, int index)
        {
            // 编辑器 不支持原生广告
        }

        // ------------------------------------------------- 商业化方案 -------------------------------------------------
        /// <summary>
        /// 读取远程配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="cb">回调</param>
        public override void GetRemoteConfig(string configName, Action cb)
        {
            // 编辑器 不需要远程配置 直接执行完成回调
            cb();
        }

        public override void CheckSideBarStartGame(GameObject btnFollow)
        {
            // 编辑器 不需要检测侧边栏
            btnFollow.SetActive(true);
        }

        public override void NavigateToSideBar()
        {
            // 编辑器 不需要侧边栏跳转
        }

        public override void OnStartRecord()
        {
            // 编辑器 不需要录屏
        }

        public override void OnStopRecord(bool isComplete)
        {
            // 编辑器 不需要录屏
        }

        public override void OnShareVideo(Action cb)
        {
            // 编辑器 不需要分享录屏
        }

        /// <summary>
        /// 直出授权订阅检测
        /// </summary>
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        public override void CheckFeedSubscribeStatus(Action successCb, Action failCb)
        {
            // 编辑器 不需要直出授权订阅检测
            if (DataHelper.CurUserInfoData.feedSubGet == 0)
            {
                failCb();
            }
            else
            {
                successCb();
            }
        }

        /// <summary>
        /// 直出订阅弹窗触发
        /// </summary>
        /// <param name="successCb">成功回调</param>
        /// <param name="failCb">失败回调</param>
        public override void RequestFeedSubscribe(Action successCb, Action failCb)
        {
            // 编辑器 不需要直出订阅弹窗触发
            successCb();
        }

        /// <summary>
        /// 直出上报可交互事件
        /// </summary>
        public override void ReportSceneFeed()
        {
            // 编辑器 不需要直出上报可交互事件
        }
    }
}