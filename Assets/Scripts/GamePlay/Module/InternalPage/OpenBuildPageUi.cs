using System.Collections.Generic;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Data;
using GamePlay.Main;
using GamePlay.Module.InternalPage.PageBuild;
using TMPro;
using UnityEngine;

namespace GamePlay.Module.InternalPage
{
    public class OpenBuildPageUi : InternalPageScript
    {
        /** 品质框 */
        public Sprite[] qualityFrames;
        
        /** 组装主页面 */
        private OpenBuildMainUi _buildMainUi;
        /** 属性说明页面 */
        private OpenBuildDetailsUi _buildDetailsUi;

        /** 属性雷达图 */
        private UIPolygon _uiPolygon;
        
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;
        
        /** 金币 */
        private TextMeshProUGUI _goldNumText;
        /** 钻石 */
        private TextMeshProUGUI _diamondNumText;

        /** 组装页面打开 */
        private bool _buildPageOpen;
        
        private void OnEnable()
        {
            EventManager.Add(CustomEventType.RefreshMoney, RefreshMoney);
        }

        private void OnDisable()
        {
            EventManager.Remove(CustomEventType.RefreshMoney, RefreshMoney);
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }

        public override void Initial()
        {
            base.Initial();

            _buildMainUi = transform.Find("BuildList").GetComponent<OpenBuildMainUi>();
            _buildMainUi.Initial();
            _buildMainUi._openBuildPageUi = this;
            _buildMainUi.gameObject.SetActive(false);

            _buildDetailsUi = transform.Find("Details").GetComponent<OpenBuildDetailsUi>();
            _buildDetailsUi.Initial();
            _buildDetailsUi._openBuildPageUi = this;
            _buildDetailsUi.gameObject.SetActive(false);
            
            _goldNumText = transform.Find("Frame/Money/Gold/Num").GetComponent<TextMeshProUGUI>();
            _diamondNumText = transform.Find("Frame/Money/Gem/Num").GetComponent<TextMeshProUGUI>();

            // _uiPolygon = transform.Find("Propety/Radar").GetComponent<UIPolygon>();
        }

        public override void OpenInternalPage()
        {
            base.OpenInternalPage();

            AudioHandler._instance.PlayAudio(MainManager._instance.audioPageOpen);
            MainManager._instance.SetMainPlaneAni("PlaneBuild");

            _buildPageOpen = true;

            _buildMainUi.gameObject.SetActive(true);
            _buildMainUi.OpenBuildMain();
            
            RefreshMoney();
        }
        
        public override void CloseInternalPage()
        {
            if (_buildPageOpen)
            {
                // 保存装备部件数据
                DataHelper.ModifyLocalData(new List<string>(1) { "equipEquipments" }, () => { });
                _buildPageOpen = false;
            }
            
            base.CloseInternalPage();
        }
        
        /// <summary>
        /// 刷新货币
        /// </summary>
        private void RefreshMoney()
        {
            _goldNumText.text = ToolFunManager.GetText(DataHelper.CurUserInfoData.gold, false);
            _diamondNumText.text = ToolFunManager.GetText(DataHelper.CurUserInfoData.diamond, false);
        }

        /// <summary>
        /// 打开属性说明
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OpenBuildDetails(bool isOpen)
        {
            _buildDetailsUi.gameObject.SetActive(isOpen);
            if (isOpen)
            {
                _buildDetailsUi.OpenDetails();
            }
        }
    }
}