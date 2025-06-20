using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using GamePlay.Globa;
using GamePlay.Main;
using GamePlay.Module.InternalPage.ItemPrefabs;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage
{
    public class OpenShopPageUi : InternalPageScript
    {
        /** 品质框 */
        public Sprite[] qualityFrames;
        /** 商品预制 */
        public ItemShopUi ItemShopUiPre;
        /** 幸运值图标 */
        public Sprite[] luckIcons;
        
        /** 滑动列表 */
        private ScrollRect _scrollRect;
        /** 滑动列表容器 */
        private Transform _content;
        
        /** 金币 */
        private TextMeshProUGUI _goldNumText;
        /** 钻石 */
        private TextMeshProUGUI _diamondNumText;
        
        /** 刷新时间 */
        private TextMeshProUGUI _refreshTimeText;
        /** 刷新按钮视频标 */
        private GameObject _btnRefreshVideo;
        
        /** 幸运值图标列表 */
        private readonly Image[] _btnRefreshLuckIcons = new Image[5];

        /** 商品挂载节点 */
        private readonly Transform[][] _itemUiPoints = new Transform[3][];

        /** 宝箱列表 */
        private readonly ItemShopUi[] _itemShopBoxUis = new ItemShopUi[3];
        /** 货币列表 */
        private readonly ItemShopUi[] _itemShopMoneyUis = new ItemShopUi[6];
        /** 装备列表 */
        private readonly ItemShopUi[] _itemShopEquipmentUis = new ItemShopUi[9];
        
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;

        /** 刷新倒计时 */
        private int _timeNum;

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

            _scrollRect = transform.Find("ScrollView").GetComponent<ScrollRect>();
            _content = _scrollRect.transform.Find("Viewport/Content");
            
            _goldNumText = _scrollRect.transform.Find("Frame/Money/Gold/Num").GetComponent<TextMeshProUGUI>();
            _diamondNumText = _scrollRect.transform.Find("Frame/Money/Gem/Num").GetComponent<TextMeshProUGUI>();

            Transform shopBox = _content.Find("Box");
            Transform shopMoneys = _content.Find("Moneys");
            Transform shopEquipments = _content.Find("Equipments");

            _refreshTimeText = shopEquipments.Find("Tittle/Time").GetComponent<TextMeshProUGUI>();

            Transform btnRefresh = shopEquipments.Find("Refresh/BtnRefresh");
            _btnRefreshVideo = btnRefresh.Find("Text/Video").gameObject;
            btnRefresh.gameObject.GetComponent<Button>().onClick.AddListener(OnBtnRefresh);

            for (int i = 0; i < 5; i++)
            {
                _btnRefreshLuckIcons[i] = btnRefresh.Find("LuckFrame/Luck/LuckIcon" + (i + 1)).GetComponent<Image>();
            }

            _itemUiPoints[0] = new Transform[3];
            for (int i = 0; i < 3; i++)
            {
                _itemUiPoints[0][i] = shopBox.Find("ItemA/Item" + (i + 1));
            }

            _itemUiPoints[1] = new Transform[6];
            for (int i = 0; i < 3; i++)
            {
                _itemUiPoints[1][i] = shopMoneys.Find("ItemA/Item" + (i + 1));
                _itemUiPoints[1][i + 3] = shopMoneys.Find("ItemB/Item" + (i + 1));
            }

            _itemUiPoints[2] = new Transform[9];
            for (int i = 0; i < 3; i++)
            {
                _itemUiPoints[2][i] = shopEquipments.Find("ItemA/Item" + (i + 1));
                _itemUiPoints[2][i + 3] = shopEquipments.Find("ItemB/Item" + (i + 1));
                _itemUiPoints[2][i + 6] = shopEquipments.Find("ItemC/Item" + (i + 1));
            }
        }

        public override void OpenInternalPage()
        {
            base.OpenInternalPage();

            AudioHandler._instance.PlayAudio(MainManager._instance.audioPageOpen);

            _scrollRect.verticalNormalizedPosition = 1f;
            
            RefreshMoney();
            RefreshShopMoney();
            RefreshShopBox();
            RefreshShopEquipment();
            RefreshShopEquipmentLuckNum();
            StartRefreshTime();

            // DataHelper.CurUserInfoData.boxList[0] = new[] { "300", "0" };
            // DataHelper.CurUserInfoData.boxList[1] = new[] { "301", "0" };
            // DataHelper.CurUserInfoData.boxList[2] = new[] { "302", "0" };
            // DataHelper.ModifyLocalData(new List<string>(1) { "boxsList" }, () => { });
        }

        public override void CloseInternalPage()
        {
            ClearShopBox();
            ClearShopMoney();
            ClearShopEquipment();
            
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
        /// 刷新时间
        /// </summary>
        private void StartRefreshTime()
        {
            _timeNum = 0;
            long nextTime = DataHelper.CurUserInfoData.shopRefreshTime + GlobalValueManager.ShopRefreshTime * 60;
            long subTime = nextTime - ToolFunManager.GetCurrTime();
            if (subTime <= 0)
            {
                // 自动刷新时间到了 执行自动刷新
                _refreshTimeText.text = "00:00:00 Auto Freshed";
                RunRefreshShopEquipments();
            }
            else
            {
                // 自动属性时间未到 开启倒计时
                int hour = (int)(subTime / 60 / 60);
                int minute = (int)(subTime / 60 % 60);
                int second = (int)(subTime % 60);
                _refreshTimeText.text = new StringBuilder($"{hour:D2}:{minute:D2}:{second:D2}" + " Auto Freshed").ToString();

                _timeNum = (int)subTime;
                _ = RefreshTime();
            }
        }

        /// <summary>
        /// 刷新倒计时
        /// </summary>
        async UniTask RefreshTime()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            while (_timeNum > 0)
            {
                int hour = _timeNum / 60 / 60;
                int minute = _timeNum / 60 % 60;
                int second = _timeNum % 60;
                _refreshTimeText.text = new StringBuilder($"{hour:D2}:{minute:D2}:{second:D2}" + " Auto Freshed").ToString();
                _timeNum -= 1;

                await UniTask.Delay(1000, true, cancellationToken: _cancellationToken.Token);
            }

            // 倒计时结束 执行自动刷新
            _timeNum = 0;
            _refreshTimeText.text = "00:00:00 Auto Freshed";
            RunRefreshShopEquipments();
        }

        /// <summary>
        /// 刷新商店出售飞机部件
        /// </summary>
        private void RefreshShopEquipment()
        {
            ClearShopEquipment();
            List<int> shopSaleIds = new List<int>(DataHelper.CurUserInfoData.shopSaleIds.Keys);
            for (int i = 0; i < _itemShopEquipmentUis.Length; i++)
            {
                ItemShopUi itemShopUi = Instantiate(ItemShopUiPre, _itemUiPoints[2][i]);
                itemShopUi._openShopPageUi = this;
                itemShopUi.Initial();
                _itemShopEquipmentUis[i] = itemShopUi;
                _itemShopEquipmentUis[i].SetData(3, shopSaleIds[i]);
            }
        }

        /// <summary>
        /// 清空商店出售飞机部件
        /// </summary>
        private void ClearShopEquipment()
        {
            for (int i = 0; i < _itemShopEquipmentUis.Length; i++)
            {
                if (_itemShopEquipmentUis[i] == null) continue;
                Destroy(_itemShopEquipmentUis[i].gameObject);
                _itemShopEquipmentUis[i] = null;
            }
        }

        /// <summary>
        /// 刷新商店出售货币
        /// </summary>
        private void RefreshShopMoney()
        {
            ClearShopMoney();
            List<int> shopSaleIds = new List<int>(ConfigManager.Instance.ShopTypeConfigDict[1].Count);
            for (int i = 0; i < ConfigManager.Instance.ShopTypeConfigDict[1].Count; i++)
            {
                shopSaleIds.Add(ConfigManager.Instance.ShopTypeConfigDict[1][i].ID);
            }
            for (int i = 0; i < _itemShopMoneyUis.Length; i++)
            {
                if (i >= shopSaleIds.Count) continue;
                ItemShopUi itemShopUi = Instantiate(ItemShopUiPre, _itemUiPoints[1][i]);
                itemShopUi._openShopPageUi = this;
                itemShopUi.Initial();
                _itemShopMoneyUis[i] = itemShopUi;
                _itemShopMoneyUis[i].SetData(2, shopSaleIds[i]);
            }
        }

        /// <summary>
        /// 清空商店出售货币
        /// </summary>
        private void ClearShopMoney()
        {
            for (int i = 0; i < _itemShopMoneyUis.Length; i++)
            {
                if (_itemShopMoneyUis[i] == null) continue;
                Destroy(_itemShopMoneyUis[i].gameObject);
                _itemShopMoneyUis[i] = null;
            }
        }

        /// <summary>
        /// 刷新商店出售宝箱
        /// </summary>
        private void RefreshShopBox()
        {
            ClearShopBox();
            List<int> shopSaleIds = new List<int>(ConfigManager.Instance.ShopTypeConfigDict[2].Count);
            for (int i = 0; i < ConfigManager.Instance.ShopTypeConfigDict[2].Count; i++)
            {
                shopSaleIds.Add(ConfigManager.Instance.ShopTypeConfigDict[2][i].ID);
            }
            for (int i = 0; i < _itemShopBoxUis.Length; i++)
            {
                ItemShopUi itemShopUi = Instantiate(ItemShopUiPre, _itemUiPoints[0][i]);
                itemShopUi._openShopPageUi = this;
                itemShopUi.Initial();
                _itemShopBoxUis[i] = itemShopUi;
                _itemShopBoxUis[i].SetData(1, shopSaleIds[i]);
            }
        }

        /// <summary>
        /// 清空商店出售宝箱
        /// </summary>
        private void ClearShopBox()
        {
            for (int i = 0; i < _itemShopBoxUis.Length; i++)
            {
                if (_itemShopBoxUis[i] == null) continue;
                Destroy(_itemShopBoxUis[i].gameObject);
                _itemShopBoxUis[i] = null;
            }
        }

        /// <summary>
        /// 执行自动刷新
        /// </summary>
        private void RunRefreshShopEquipments()
        {
            List<string> modifyKeys = new List<string>();
            // 刷新 商店出售装备配件列表
            DataHelper.CurUserInfoData.shopSaleIds = DataHelper.GetShopSaleList();
            modifyKeys.Add("shopSaleIds");
            // 刷新 商店出售装备配件刷新时间
            DataHelper.CurUserInfoData.shopRefreshTime = ToolFunManager.GetCurrTime();
            modifyKeys.Add("shopRefreshTime");
            // 刷新 商店出售装备配件刷新幸运值
            DataHelper.CurUserInfoData.shopLuckNum = 1;
            modifyKeys.Add("shopLuckNum");
            // 保存数据
            DataHelper.ModifyLocalData(modifyKeys, () => { });
            
            // 刷新商店出售的飞机部件
            RefreshShopEquipment();
            // 重启倒计时
            StartRefreshTime();
            // 刷新商店出售装备配件刷新幸运值
            RefreshShopEquipmentLuckNum();
        }

        /// <summary>
        /// 刷新商店出售装备配件刷新幸运值
        /// </summary>
        private void RefreshShopEquipmentLuckNum()
        {
            int luckNum = DataHelper.CurUserInfoData.shopLuckNum;
            if (luckNum >= 20) luckNum = 20;
            int[][] shows =
            {
                new[] { 1, 0, 0, 0, 0 }, new[] { 2, 0, 0, 0, 0 }, new[] { 3, 0, 0, 0, 0 }, new[] { 4, 0, 0, 0, 0 },
                new[] { 4, 1, 0, 0, 0 }, new[] { 4, 2, 0, 0, 0 }, new[] { 4, 3, 0, 0, 0 }, new[] { 4, 4, 0, 0, 0 },
                new[] { 4, 4, 1, 0, 0 }, new[] { 4, 4, 2, 0, 0 }, new[] { 4, 4, 3, 0, 0 }, new[] { 4, 4, 4, 0, 0 },
                new[] { 4, 4, 4, 1, 0 }, new[] { 4, 4, 4, 2, 0 }, new[] { 4, 4, 4, 3, 0 }, new[] { 4, 4, 4, 4, 0 },
                new[] { 4, 4, 4, 4, 1 }, new[] { 4, 4, 4, 4, 2 }, new[] { 4, 4, 4, 4, 3 }, new[] { 4, 4, 4, 4, 4 }
            };
            
            for (int i = 0; i < _btnRefreshLuckIcons.Length; i++)
            {
                if (shows[luckNum - 1][i] != 0)
                {
                    _btnRefreshLuckIcons[i].gameObject.SetActive(true);
                    _btnRefreshLuckIcons[i].sprite = luckIcons[shows[luckNum - 1][i] - 1];
                }
                else
                {
                    _btnRefreshLuckIcons[i].gameObject.SetActive(false);
                }
            }
        }

        // ----------------------------------------------- 按钮 -----------------------------------------------
        /// <summary>
        /// 按钮 刷新
        /// </summary>
        private void OnBtnRefresh()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            DataHelper.CurReportDf_adScene = "ShopLuckRefresh";
            GameSdkManager._instance._sdkScript.VideoControl("Part Shop Flushed", () =>
            {
                List<string> modifyKeys = new List<string>();
                // 刷新商店出售装备配件刷新幸运值
                DataHelper.CurUserInfoData.shopLuckNum += 1;
                modifyKeys.Add("shopLuckNum");
                // 刷新商店出售商品(装备配件)列表
                DataHelper.CurUserInfoData.shopSaleIds = DataHelper.GetShopSaleList();
                modifyKeys.Add("shopSaleIds");
                // 完成日常任务 观看X次广告 TaskID:2
                DataHelper.CompleteDailyTask(2, 1, 0);
                modifyKeys.Add("taskInfo1");
                // 完成成就任务 累计观看X次广告 TaskID:3
                DataHelper.CompleteGloalTask(3, 1);
                modifyKeys.Add("taskInfo2");
                // 保存数据
                DataHelper.ModifyLocalData(modifyKeys, () => { });

                // 刷新商店出售的飞机部件
                RefreshShopEquipment();
                // 刷新商店出售装备配件刷新幸运值
                RefreshShopEquipmentLuckNum();

                // 上报自定义分析数据 事件: 刷新每日商品
                GameSdkManager._instance._sdkScript.ReportAnalytics("RefreshLimitShop", "", "");
            }, () => { });
        }
    }
}