using Common.GameRoot.AudioHandler;
using Data;
using Platform;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Ui
{
    public class UiRevive : MonoBehaviour
    {
        /** 视频/分享 提示标 */
        private GameObject _videoImage, _shareImage;
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        public void AwakeOnUi()
        {
            transform.Find("UseItemFrame/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            transform.Find("UseItemFrame/BtnUse").GetComponent<Button>().onClick.AddListener(OnBtnUse);

            _videoImage = transform.Find("UseItemFrame/BtnUse/Voide").gameObject;
            _shareImage = transform.Find("UseItemFrame/BtnUse/Share").gameObject;
        }

        /// <summary>
        /// 打开复活页面
        /// </summary>
        public void OpenReviveUi()
        {
            _videoImage.SetActive(true);
            _shareImage.SetActive(false);
        }

        /// <summary>
        /// 复活回调
        /// </summary>
        private void ReviveCallBack()
        {
            // 复活
            
            // 清空备料杯
            BattleManager._instance.prepareCupClearType = 1; // 清空备料杯类型 复活
            DataHelper.UseClearPrepareCups();
            BattleManager._instance.OnClearPrepareCups();
            
            // 刷新复活次数
            BattleManager._instance.reviveNum -= 1;
            // 关闭复活页
            UiBattle._instance.OnBtnOpenRevive(false);
        }

        // ---------------------------------------------- 按钮 ----------------------------------------------
        /** 按钮 关闭 */
        private void OnBtnClose()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            UiBattle._instance.OnBtnOpenRevive(false);
            UiBattle._instance.OnOpenAccount(false);
        }

        /// <summary>
        /// 按钮 复活
        /// </summary>
        private void OnBtnUse()
        {
            AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
            OnBtnReviveVideo();
        }

        /// <summary>
        /// 按钮 观看视频复活
        /// </summary>
        private void OnBtnReviveVideo()
        {
            // 播放激励视频
            GameSdkManager.Instance._sdkScript.VideoControl(ReviveCallBack, () => { });
        }

        /// <summary>
        /// 按钮 分享复活
        /// </summary>
        private void OnBtnReviveShare()
        {
        }
    }
}