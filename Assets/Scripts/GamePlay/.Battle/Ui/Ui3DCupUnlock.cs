using System;
using Data;
using Platform;
using UnityEngine;

namespace GamePlay.Battle.Ui
{
    public class Ui3DCupUnlock : MonoBehaviour
    {
        /** 视频提示标 */
        private GameObject _videoTip;
        /** 分享提示标 */
        private GameObject _shareTip;

        /** 激励类型 0: 视频 1: 分享 */
        private int _type;
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        public void AwakeOnUi()
        {
            _videoTip = transform.Find("Null1").gameObject;
            _shareTip = transform.Find("Null2").gameObject;
        }

        /// <summary>
        /// 设置解锁方式
        /// </summary>
        /// <param name="typeTmp">0: 视频 1: 分享</param>
        public void SetUnLockType(int typeTmp)
        {
            _type = typeTmp;
            _videoTip.SetActive(typeTmp == 0);
            _shareTip.SetActive(typeTmp == 1);
        }

        /// <summary>
        /// 解锁杯子
        /// </summary>
        /// <param name="typeTmp">解锁杯子类型 0: 订单杯 1: 备料杯</param>
        /// <param name="idTmp">解锁杯子列表索引</param>
        public void UnLockCup(int typeTmp, int idTmp)
        {
            if (_type == 0)
            {
                // 播放激励视频
                GameSdkManager.Instance._sdkScript.VideoControl(() =>
                {
                    // 解锁杯子
                    DataHelper.ModifyCupUnlockStatus(typeTmp, idTmp);
                    // 通知刷新
                    BattleManager._instance.RefreshUnlockCups(typeTmp, idTmp);
                }, () => { });
            }
            else
            {
                // 分享
                GameSdkManager.Instance._sdkScript.ShareControl(() =>
                {
                    // 记录分享使用状态
                    if (!BattleManager._instance.unlockShareUsed) BattleManager._instance.unlockShareUsed = true;
                    // 解锁杯子
                    DataHelper.ModifyCupUnlockStatus(typeTmp, idTmp);
                    // 通知刷新
                    BattleManager._instance.RefreshUnlockCups(typeTmp, idTmp);
                });
            }
        }
    }
}