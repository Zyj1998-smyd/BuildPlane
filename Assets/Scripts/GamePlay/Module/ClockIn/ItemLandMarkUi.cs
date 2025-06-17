using System.Collections.Generic;
using System.Text;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Data;
using GamePlay.Globa;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.ClockIn
{
    public class ItemLandMarkUi : MonoBehaviour
    {
        /** 地标打卡页面 */
        internal OpenClockInPageUi _openClockInPageUi;

        /** 动画组件 */
        private Animation _animation;

        /** 地标 尚未飞达 */
        private GameObject _frameOff;
        /** 地标 */
        private GameObject _frameOn;
        /** 地标 名称 */
        private Image _landMarkName;
        /** 地标 提示红点 */
        private GameObject _landMarkRedPoint;

        /** 详情图片 */
        private Image _infoImage;
        /** 详情奖励 */
        private GameObject _infoReward;

        /** 地标ID */
        internal int _landMarkId;
        /** 是否打卡 */
        private bool _isMark;

        /** 奖励数量 */
        private readonly int _rewardNum = 50;

        /** 当前正在播放的动画名称 */
        private string _curPlayAniName;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _animation = gameObject.GetComponent<Animation>();
            _frameOff = transform.Find("FrameOff").gameObject;
            _frameOn = transform.Find("FrameOn").gameObject;
            _landMarkName = _frameOn.transform.Find("LandmarkName").GetComponent<Image>();
            _landMarkRedPoint = _frameOn.transform.Find("RedPoint").gameObject;
            _infoImage = transform.Find("Info/Image").GetComponent<Image>();
            _infoReward = transform.Find("Info/Reward").gameObject;

            _frameOn.GetComponent<Button>().onClick.AddListener(OnBtnFrameOn);
            _infoReward.GetComponent<Button>().onClick.AddListener(OnBtnInfoReward);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="landMarkId">地标ID</param>
        /// <param name="isMark">是否打卡</param>
        internal void SetData(int landMarkId, bool isMark)
        {
            _landMarkId = landMarkId;
            _isMark = isMark;

            if (isMark)
            {
                // 已打卡
                _frameOff.SetActive(false);
                _frameOn.SetActive(true);
                
                string landMarkName = new StringBuilder("ClockIn" + _landMarkId).ToString();
                GameGlobalManager._instance.SetImage(_landMarkName, landMarkName);

                string infoImageName = new StringBuilder("ClockInImage" + _landMarkId).ToString();
                GameGlobalManager._instance.SetImage(_infoImage, infoImageName);

                RefreshRedPoint();
            }
            else
            {
                // 未打卡
                _frameOff.SetActive(true);
                _frameOn.SetActive(false);
                _landMarkRedPoint.SetActive(false);
            }
        }

        /// <summary>
        /// 刷新提示红点
        /// </summary>
        private void RefreshRedPoint()
        {
            int rewardGetNum = DataHelper.CurUserInfoData.landMarkInfo.GetValueOrDefault(_landMarkId, 0);
            if (rewardGetNum == 0)
            {
                // 未领取打卡奖励
                _infoReward.SetActive(true);
                _landMarkRedPoint.SetActive(true);
            }
            else
            {
                // 已领取打卡奖励
                _infoReward.SetActive(false);
                _landMarkRedPoint.SetActive(false);
            }
        }

        /// <summary>
        /// 设置播放动画
        /// </summary>
        /// <param name="animationId">动画ID</param>
        internal void SetAnimation(int animationId)
        {
            switch (animationId)
            {
                case 0: // 待机
                    _curPlayAniName = "ItemLandmarkNull";
                    _animation.Play(_curPlayAniName);
                    break;
                case 1: // 打开
                    _curPlayAniName = "ItemLandmarkOpen";
                    _animation.Play(_curPlayAniName);
                    break;
                case 2: // 关闭
                    _curPlayAniName = "ItemLandmarkClose";
                    _animation.Play(_curPlayAniName);
                    break;
            }
        }

        /// <summary>
        /// 按钮 地标
        /// </summary>
        private void OnBtnFrameOn()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            _openClockInPageUi.OnSelectLandMark(_landMarkId);
        }

        /// <summary>
        /// 按钮 详情奖励
        /// </summary>
        private void OnBtnInfoReward()
        {
            // 未打卡 不能领取
            if (!_isMark) return;
            if (_curPlayAniName != "ItemLandmarkOpen") return;
            // 已打卡
            List<string> modifyKeys = new List<string>();
            // 领取钻石
            DataHelper.CurUserInfoData.diamond += _rewardNum;
            modifyKeys.Add("diamond");
            // 领取打卡奖励
            if (DataHelper.CurUserInfoData.landMarkInfo.ContainsKey(_landMarkId))
            {
                DataHelper.CurUserInfoData.landMarkInfo[_landMarkId] = 1;
                modifyKeys.Add("landMarkInfo");
            }

            // 保存数据
            if (modifyKeys.Count > 0) DataHelper.ModifyLocalData(modifyKeys, () => { });
            
            // 恭喜获得
            DataHelper.CurGetItem = new[] { 1, 200, _rewardNum };
            GameGlobalManager._instance.OpenGetItem(true);
            // 刷新货币
            EventManager.Send(CustomEventType.RefreshMoney);
            // 刷新提示红点
            RefreshRedPoint();
            _openClockInPageUi.RefrehCityListRedpoint();
            EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);
        }
    }
}