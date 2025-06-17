using System;
using System.Text;
using Common.GameRoot.AudioHandler;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Ui
{
    public class UiBattle : MonoBehaviour
    {
        public static UiBattle _instance;

        /** 关卡 */
        private TextMeshProUGUI _levelText;
        /** 关卡进度 */
        private TextMeshProUGUI _orderCompleteNumText;
        /** 关卡进度条 */
        private Image _orderCompleteBar;

        /** 暂停页 */
        private UiPause _uiPause;
        
        /** 道具使用页 */
        private UiOperation _uiOperation;

        /** 结算页 */
        private UiAccount _uiAccount;

        /** 复活页 */
        private UiRevive _uiRevive;
        
        /** 新手引导UI */
        public UiGuide _uiGuide;

        /** 主动点击打开使用道具 */
        public bool _isClickOpenUseItem;
        
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            Transform mainUiTransform = transform.Find("MainUi");

            _levelText = mainUiTransform.Find("Info/Level").GetComponent<TextMeshProUGUI>();
            _orderCompleteNumText = mainUiTransform.Find("Info/OrderFrame/Order").GetComponent<TextMeshProUGUI>();
            _orderCompleteBar = mainUiTransform.Find("Info/OrderFrame/Value").GetComponent<Image>();

            mainUiTransform.Find("BtnPause").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
                OnBtnOpenPause(true);
            });
            
            mainUiTransform.Find("BottomFrame/BtnOperation_1").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
                _isClickOpenUseItem = true;
                OnBtnOpenOperation(true, 0);
            });
            mainUiTransform.Find("BottomFrame/BtnOperation_2").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
                _isClickOpenUseItem = true;
                OnBtnOpenOperation(true, 1);
            });
            mainUiTransform.Find("BottomFrame/BtnOperation_3").GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioHandler._instance.PlayAudio(BattleManager._instance.BtnClickAudio);
                _isClickOpenUseItem = true;
                OnBtnOpenOperation(true, 2);
            });
            
            // 暂停页
            _uiPause = transform.Find("Pause").GetComponent<UiPause>();
            _uiPause.AwakeOnUi();
            _uiPause.gameObject.SetActive(false);

            // 道具使用页
            _uiOperation = transform.Find("UseItem").GetComponent<UiOperation>();
            _uiOperation.AwakeOnUi();
            _uiOperation.gameObject.SetActive(false);
            
            // 结算页
            _uiAccount = transform.Find("Account").GetComponent<UiAccount>();
            _uiAccount.AwakeOnUi();
            _uiAccount.gameObject.SetActive(false);
            
            // 复活页
            _uiRevive = transform.Find("FuHuo").GetComponent<UiRevive>();
            _uiRevive.AwakeOnUi();
            _uiRevive.gameObject.SetActive(false);
            
            // 新手引导
            _uiGuide = transform.Find("Guide").GetComponent<UiGuide>();
            _uiGuide.Initial();
            _uiGuide.gameObject.SetActive(false);
        }

        private void Start()
        {
            RefreshLevel();
            OrderComplete();
        }

        /// <summary>
        /// 订单完成刷新UI
        /// </summary>
        public void OrderComplete()
        {
            // var str = new StringBuilder();
            // str.Append(BattleManager._instance.orderCompleteNum);
            // str.Append("/");
            // str.Append(BattleManager._instance.orderCompleteNumMax);
            // _orderCompleteNumText.text = str.ToString();

            var progressTmp = (float)BattleManager._instance.orderCompleteNum / BattleManager._instance.orderCompleteNumMax;
            _orderCompleteBar.fillAmount = progressTmp <= 0 ? 0 : progressTmp >= 1 ? 1 : progressTmp;
            _orderCompleteNumText.text = new StringBuilder("关卡进度：" + Mathf.FloorToInt(progressTmp * 100) + "%").ToString();
        }

        /// <summary>
        /// 刷新关卡
        /// </summary>
        public void RefreshLevel()
        {
            var str = new StringBuilder("-第" + DataHelper.CurUserInfoData.levelNum + "关-");
            _levelText.text = str.ToString();
        }

        /// <summary>
        /// 打开暂停页
        /// </summary>
        /// <param name="isOpen">选项 true: 打开 false: 关闭</param>
        public void OnBtnOpenPause(bool isOpen)
        {
            _uiPause.gameObject.SetActive(isOpen);
            BattleManager._instance.gamePause = isOpen;
            if (isOpen)
            {
                _uiPause.OpenPauseUi();
            }
        }

        /// <summary>
        /// 打开道具使用页
        /// </summary>
        /// <param name="isOpen">选项 true: 打开 false: 关闭</param>
        /// <param name="index">道具索引 0: 清空备料杯 1: 刷新订单杯 2: 刷新原料瓶</param>
        public void OnBtnOpenOperation(bool isOpen, int index)
        {
            _uiOperation.gameObject.SetActive(isOpen);
            BattleManager._instance.gamePause = isOpen;
            if (isOpen)
            {
                _uiOperation.OpenOperationUi(index);
            }
        }

        /// <summary>
        /// 打开结算页
        /// </summary>
        /// <param name="isWin">结算结果</param>
        public void OnOpenAccount(bool isWin)
        {
            if (BattleManager._instance.gameEnd) return;
            BattleManager._instance.gameEnd = true;
            _uiAccount.gameObject.SetActive(true);
            BattleManager._instance.gamePause = true;
            _uiAccount.OpenAccountUi(isWin);
        }

        /// <summary>
        /// 打开复活页
        /// </summary>
        /// <param name="isOpen">选项 true: 打开 false: 关闭</param>
        public void OnBtnOpenRevive(bool isOpen)
        {
            _uiRevive.gameObject.SetActive(isOpen);
            if (isOpen)
            {
                _uiRevive.OpenReviveUi();
            }
        }

        /// <summary>
        /// 执行新手引导步骤
        /// </summary>
        /// <param name="isComplete">是否完成</param>
        public void RunGuideStep(bool isComplete)
        {
            _uiGuide.gameObject.SetActive(!isComplete);
        }
    }
}