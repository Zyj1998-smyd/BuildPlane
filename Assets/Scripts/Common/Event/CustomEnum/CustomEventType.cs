namespace Common.Event.CustomEnum
{
    public enum CustomEventType
    {
        /** 全局事件 资源加载完成 */
        ResLoadDone,
        /** 全局事件 刷新货币 */
        RefreshMoney,
        /** 全局事件 刷新提示红点 */
        RefreshRedPoint,
        /** 全局事件 宝箱打开完成 */
        BoxOpenDone,
        /** 全局事件 刷新收藏游戏(侧边栏)按钮 */
        RefreshBtnFollow,
        /** 全局事件 刷新邀请好友按钮 */
        RefreshBtnCall,
        /** 全局事件 恭喜获得弹窗关闭 */
        GetItemClose,
        /** 全局事件 引导点击组装部件 */
        GuideClickBuild,
        /** 全局事件 引导完成 */
        GuideComplete,
        /** 全局事件 刷新直玩订阅按钮 */
        RefreshBtnFeedSub,
        /** 全局事件 刷新主页面地图 */
        RefreshMainPageMap
    }
}