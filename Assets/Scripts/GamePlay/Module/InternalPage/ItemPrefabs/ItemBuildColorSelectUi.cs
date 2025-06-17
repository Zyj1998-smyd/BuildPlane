using System.Collections.Generic;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Data;
using Data.ConfigData;
using GamePlay.Globa;
using GamePlay.Main;
using GamePlay.Module.InternalPage.PageBuild;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage.ItemPrefabs
{
    public class ItemBuildColorSelectUi : MonoBehaviour
    {
        private RectTransform _rect;
        private RectTransform _parentRect;

        /** 动画组件 */
        private Animation _animation;
        /** 购买按钮 */
        private GameObject _btnBuy;
        /** 购买价格 */
        private TextMeshProUGUI _buyPriceNumText;

        /** 当前选中的部件涂装 */
        private ItemBuildColorUi _itemBuildColorUi;

        /** 购买价格 */
        private int _priceNum;

        /** 当前涂装需要购买 */
        internal bool _isNeedBuy;
        /** 当前涂装购买完成 */
        internal bool _isBuyComplete;

        /** 涂装页面 */
        internal OpenBuildPaintUi _openBuildPaintUi;

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _rect = gameObject.GetComponent<RectTransform>();
            _parentRect = transform.parent.GetComponent<RectTransform>();

            _animation = gameObject.GetComponent<Animation>();
            _btnBuy = transform.Find("Frame/BtnBuy").gameObject;
            
            _buyPriceNumText = _btnBuy.transform.Find("Gem").GetComponent<TextMeshProUGUI>();
            
            _btnBuy.GetComponent<Button>().onClick.AddListener(OnBtnBuy);
        }

        private void Update()
        {
            if (!gameObject.activeSelf) return;
            if (!_itemBuildColorUi) return;
            
            var screenPoint = RectTransformUtility.WorldToScreenPoint(MainManager._instance.uiCamera, _itemBuildColorUi._parentRect.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, screenPoint, MainManager._instance.uiCamera, out var localPoint);
            
            transform.localPosition = new Vector3(localPoint.x, localPoint.y, 0);
        }

        /// <summary>
        /// 设置选中
        /// </summary>
        /// <param name="itemBuildColorUi">目标父节点</param>
        internal void SetSelect(ItemBuildColorUi itemBuildColorUi)
        {
            _itemBuildColorUi = itemBuildColorUi;

            // _itemBuildColorUi.openBuildMainUi.SetItemBuildColorSelect(itemBuildColorUi._id);

            _rect.sizeDelta = itemBuildColorUi._parentRect.sizeDelta;
            
            var screenPoint = RectTransformUtility.WorldToScreenPoint(MainManager._instance.uiCamera, itemBuildColorUi._parentRect.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, screenPoint, MainManager._instance.uiCamera, out var localPoint);
            
            transform.localPosition = new Vector3(localPoint.x, localPoint.y, 0);

            int status = SetButton();
            _animation.Play(status == 0 ? "SelectColorFrameUse" : "SelectColorFrameBuy");
        }

        /// <summary>
        /// 设置按钮
        /// </summary>
        private int SetButton()
        {
            int status;
            bool isNeedBuy = ConfigManager.Instance.ShopConfigDict.ContainsKey(_itemBuildColorUi._id);
            if (!isNeedBuy)
            {
                // 无需购买 默认解锁的
                _btnBuy.SetActive(false);
                _isNeedBuy = false;
                _isBuyComplete = true;
                status = 0;
            }
            else
            {
                // 需要购买
                List<int> buyList = DataHelper.CurUserInfoData.buyEquipmentPaints.GetValueOrDefault(_openBuildPaintUi._curEquipmentId, new List<int>());
                bool isBuyDone = buyList.Contains(_itemBuildColorUi._id);
                if (!isBuyDone)
                {
                    // 未购买
                    _btnBuy.SetActive(true);
                    ShopConfig shopConfig = ConfigManager.Instance.ShopConfigDict[_itemBuildColorUi._id];
                    _priceNum = ToolFunManager.GetNumFromStrNew(shopConfig.BuyNum)[0];
                    _buyPriceNumText.text = _priceNum.ToString();
                    _isNeedBuy = true;
                    _isBuyComplete = false;
                    status = 1;
                }
                else
                {
                    // 已购买
                    _btnBuy.SetActive(false);
                    _isNeedBuy = true;
                    _isBuyComplete = true;
                    status = 0;
                }
            }

            return status;
        }

        /// <summary>
        /// 按钮 购买
        /// </summary>
        private void OnBtnBuy()
        {
            if (DataHelper.CurUserInfoData.diamond < _priceNum)
            {
                GameGlobalManager._instance.OpenNoMoney(true, 2);
                return;
            }

            List<string> modifyKeys = new List<string>();
            // 消耗钻石
            DataHelper.CurUserInfoData.diamond -= _priceNum;
            modifyKeys.Add("diamond");
            // 获得新涂装
            if (!DataHelper.CurUserInfoData.buyEquipmentPaints.ContainsKey(_openBuildPaintUi._curEquipmentId))
            {
                // 已购买的配件涂装列表中没有配件的购买列表 开辟一份列表并存入新获得的涂装ID
                DataHelper.CurUserInfoData.buyEquipmentPaints.Add(_openBuildPaintUi._curEquipmentId, new List<int> { _itemBuildColorUi._id });
            }
            else
            {
                // 已购买的配件涂装列表中已有配件的购买列表 直接存入新获得的涂装ID
                DataHelper.CurUserInfoData.buyEquipmentPaints[_openBuildPaintUi._curEquipmentId].Add(_itemBuildColorUi._id);
            }
            modifyKeys.Add("buyEquipmentPaints");
            // 刷新按钮
            SetButton();
            GameGlobalManager._instance.ShowTips("涂装方案已获得!");
            // 刷新货币
            EventManager.Send(CustomEventType.RefreshMoney);
            // 真实数据 跟随设置
            _itemBuildColorUi._openBuildPaintUi._oldEquipEquipmentColors[_itemBuildColorUi._type - 1] = _itemBuildColorUi._id;
            
            // 完成日常任务 完成X次涂装更换 TaskID:7
            DataHelper.CompleteDailyTask(7, 1, 0);
            modifyKeys.Add("taskInfo1");
            
            // 保存数据
            DataHelper.ModifyLocalData(modifyKeys, () => { });

            // 改播放使用动画
            _animation.Play("SelectColorFrameUse");
        }
    }
}