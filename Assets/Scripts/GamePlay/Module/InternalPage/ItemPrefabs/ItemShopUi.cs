using System.Collections.Generic;
using System.Text;
using Common.Event;
using Common.Event.CustomEnum;
using Common.Tool;
using Data;
using Data.ConfigData;
using GamePlay.Globa;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage.ItemPrefabs
{
    public class ItemShopUi : MonoBehaviour
    {
        /** 图标 */
        private Image _iconImage;
        /** 名称 */
        private TextMeshProUGUI _nameText;
        /** 购买方式 金币 */
        private TextMeshProUGUI _buy_GoldNumText;
        /** 购买方式 钻石 */
        private TextMeshProUGUI _buy_DiamondNumText;
        /** 购买方式 视频 */
        private GameObject _buy_Video;
        /** 提示红点 */
        private GameObject _redPoint;
        /** 售罄 */
        private GameObject _sellOut;
        /** 品质框 */
        private Image _qualityFrame;
        /** 包含数量 */
        private TextMeshProUGUI _numText;
        /** 装备碎片 */
        private GameObject _equipmentChip;
        /** 新装备 */
        private GameObject _equipmentNew;
        /** 装备可升级 */
        private GameObject _equipmentLvUp;

        /** 商店页面 */
        internal OpenShopPageUi _openShopPageUi;

        /** 类型 */
        private int _type;
        /** ID */
        private int _id;

        /** 购买方式 */
        private int _buyType;
        /** 购买价格 */
        private int _buyNum;

        /** 限购次数 */
        private int _limitNum;

        /** 购买商品 "类型,数量" */
        private string _buyInfo;

        /** 装备品质 非装备为-1 */
        private int _equipmentQuality;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _iconImage = transform.Find("Image").GetComponent<Image>();
            _nameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            _buy_GoldNumText = transform.Find("Frame/Gold").GetComponent<TextMeshProUGUI>();
            _buy_DiamondNumText = transform.Find("Frame/Gem").GetComponent<TextMeshProUGUI>();
            _buy_Video = transform.Find("Frame/Free").gameObject;
            _redPoint = transform.Find("RedPoint").gameObject;
            _sellOut = transform.Find("SellOut").gameObject;
            _qualityFrame = gameObject.GetComponent<Image>();
            _numText = transform.Find("Num").GetComponent<TextMeshProUGUI>();
            _numText.gameObject.SetActive(true);
            _equipmentChip = transform.Find("Debris").gameObject;
            _equipmentNew = transform.Find("New").gameObject;
            _equipmentLvUp = transform.Find("LevelUp").gameObject;

            Button btnItem = gameObject.AddComponent<Button>();
            btnItem.transition = Selectable.Transition.None;
            btnItem.onClick.AddListener(OnBtnBuy);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="type">商品类型</param>
        /// <param name="id">商品ID</param>
        public void SetData(int type, int id)
        {
            _type = type;
            _id = id;

            int buyType = 0;
            int buyNum = 0;
            int canBuyNum = -1;
            _equipmentQuality = -1;
            
            _equipmentChip.SetActive(false);
            _equipmentNew.SetActive(false);
            _equipmentLvUp.SetActive(false);
            
            switch (type)
            {
                case 1: // 宝箱
                {
                    ShopConfig config = ConfigManager.Instance.ShopConfigDict[id];
                    List<int> buyNumsTmp = ToolFunManager.GetNumFromStrNew(config.BuyNum);
                    int buyNumIndex = DataHelper.CurUserInfoData.curLevelNum >= 5
                        ? 5
                        : DataHelper.CurUserInfoData.curLevelNum;
                    buyType = config.BuyType;
                    buyNum = buyNumsTmp[buyNumIndex - 1];
                    int buyOkNum = DataHelper.CurUserInfoData.shopLimits.GetValueOrDefault(id, 0);
                    canBuyNum = config.Limit == 0 ? -1 : config.Limit - buyOkNum;
                    _nameText.text = config.Name;
                    GameGlobalManager._instance.SetImage(_iconImage, new StringBuilder("IconImage" + id).ToString());
                    _buyInfo = config.Num;
                    _numText.text = "";
                    break;
                }
                case 2: // 货币
                {
                    ShopConfig config = ConfigManager.Instance.ShopConfigDict[id];
                    buyType = config.BuyType;
                    buyNum = ToolFunManager.GetNumFromStrNew(config.BuyNum)[0];
                    int buyOkNum = DataHelper.CurUserInfoData.shopLimits.GetValueOrDefault(id, 0);
                    canBuyNum = config.Limit == 0 ? -1 : config.Limit - buyOkNum;
                    _nameText.text = config.Name;
                    GameGlobalManager._instance.SetImage(_iconImage, new StringBuilder("IconImage" + id).ToString());
                    _buyInfo = config.Num;
                    List<int> buyInfoTmp = ToolFunManager.GetNumFromStrNew(_buyInfo);
                    _numText.text = buyInfoTmp[1].ToString();
                    break;
                }
                case 3: // 装备
                {
                    ComponentConfig config = ConfigManager.Instance.ComponentConfigDict[id];
                    buyType = 1;
                    buyNum = config.Price;
                    canBuyNum = DataHelper.CurUserInfoData.shopSaleIds[id];
                    _qualityFrame.sprite = _openShopPageUi.qualityFrames[config.Quality];
                    _nameText.text = new StringBuilder(config.Name).ToString();
                    GameGlobalManager._instance.SetImage(_iconImage, new StringBuilder("IconImage" + id).ToString());
                    _buyInfo = new StringBuilder(id + ",1").ToString();
                    _numText.text = "";
                    _equipmentQuality = config.Quality;
                    if (DataHelper.CurUserInfoData.equipments.ContainsKey(_id))
                    {
                        // 已有的装备
                        _equipmentChip.SetActive(true);
                        int lv = DataHelper.CurUserInfoData.equipments.GetValueOrDefault(_id, 1);
                        int chipNum = DataHelper.CurUserInfoData.equipmentChips.GetValueOrDefault(_id, 0);
                        float targetChipNumTmp = GlobalValueManager.EquipmentUpGradeChipNum;
                        for (int j = 0; j < lv - 1; j++)
                        {
                            targetChipNumTmp *= GlobalValueManager.EquipmentUpGradeChipUpGradeNum;
                        }
                        
                        int targetChipNum = Mathf.CeilToInt(targetChipNumTmp);
                        _equipmentLvUp.SetActive(targetChipNum - chipNum <= 10);
                    }
                    else
                    {
                        // 新装备
                        _equipmentNew.SetActive(true);
                    }
                    break;
                }
            }

            _buy_Video.gameObject.SetActive(false);
            _buy_GoldNumText.gameObject.SetActive(false);
            _buy_DiamondNumText.gameObject.SetActive(false);
            _redPoint.SetActive(false);
            
            switch (buyType)
            {
                case 0:
                    _buy_Video.gameObject.SetActive(canBuyNum != 0);
                    _redPoint.SetActive(canBuyNum > 0);
                    break;
                case 1:
                    _buy_GoldNumText.gameObject.SetActive(canBuyNum != 0);
                    _buy_GoldNumText.text = buyNum.ToString();
                    break;
                case 2:
                    _buy_DiamondNumText.gameObject.SetActive(canBuyNum != 0);
                    _buy_DiamondNumText.text = buyNum.ToString();
                    break;
            }

            _sellOut.SetActive(canBuyNum == 0);

            _buyType = buyType;
            _buyNum = buyNum;
            _limitNum = canBuyNum;
        }

        /// <summary>
        /// 按钮 购买
        /// </summary>
        private void OnBtnBuy()
        {
            if (_limitNum == 0) return;
            switch (_buyType)
            {
                case 0:
                    DataHelper.CurReportDf_adScene = "ShopBuy";
                    GameSdkManager._instance._sdkScript.VideoControl("商店购买物品", () =>
                    {
                        if (_limitNum != -1)
                        {
                            if (!DataHelper.CurUserInfoData.shopLimits.ContainsKey(_id))
                            {
                                DataHelper.CurUserInfoData.shopLimits.Add(_id, 1);
                            }
                            else
                            {
                                DataHelper.CurUserInfoData.shopLimits[_id] += 1;
                            }

                            _modifyKeys.Add("shopLimits");
                        }

                        // 完成日常任务 观看X次广告 TaskID:2
                        DataHelper.CompleteDailyTask(2, 1, 0);
                        _modifyKeys.Add("taskInfo1");
                        // 完成成就任务 累计观看X次广告 TaskID:3
                        DataHelper.CompleteGloalTask(3, 1);
                        _modifyKeys.Add("taskInfo2");

                        BuyComplete();
                    }, () => { });
                    break;
                case 1:
                    if (DataHelper.CurUserInfoData.gold < _buyNum)
                    {
                        // GameGlobalManager._instance.ShowTips("金币不足!");
                        GameGlobalManager._instance.OpenNoMoney(true, 1);
                        return;
                    }

                    DataHelper.CurUserInfoData.gold -= _buyNum;
                    _modifyKeys.Add("gold");
                    
                    BuyComplete();
                    break;
                case 2:
                    if (DataHelper.CurUserInfoData.diamond < _buyNum)
                    {
                        // GameGlobalManager._instance.ShowTips("钻石不足!");
                        GameGlobalManager._instance.OpenNoMoney(true, 2);
                        return;
                    }
                    
                    DataHelper.CurUserInfoData.diamond -= _buyNum;
                    _modifyKeys.Add("diamond");
                    
                    BuyComplete();
                    break;
            }
        }

        /** 保存数据Key列表 */
        private readonly List<string> _modifyKeys = new List<string>();
        
        /** 购买完成 */
        private void BuyComplete()
        {
            List<int> buyInfo = ToolFunManager.GetNumFromStrNew(_buyInfo);
            switch (_type)
            {
                case 1: // 宝箱
                    int curLevelNum = DataHelper.CurUserInfoData.curLevelNum;
                    if (curLevelNum >= 5) curLevelNum = 5;
                    int boxIdTmp = ((_id % 300) + 1) * 100 + (curLevelNum - 1);
                    GameGlobalManager._instance.OpenBox(boxIdTmp);
                    
                    // 完成日常任务 打开X个部件宝箱 TaskID:4
                    DataHelper.CompleteDailyTask(4, 1, 0);
                    if (!_modifyKeys.Contains("taskInfo1")) _modifyKeys.Add("taskInfo1");
                    // 完成成就任务 累计打开X个部件宝箱 TaskID:5
                    DataHelper.CompleteGloalTask(5, 1);
                    if (!_modifyKeys.Contains("taskInfo2")) _modifyKeys.Add("taskInfo2");
                    
                    // 上报自定义分析数据 事件: 商店购买宝箱A/B/C
                    string eventName = new StringBuilder("BuyBox" + new List<string> { "A", "B", "C" }[_id % 300]).ToString();
                    GameSdkManager._instance._sdkScript.ReportAnalytics(eventName, "", "");
                    break;
                case 2: // 货币
                    switch (buyInfo[0])
                    {
                        case 1:
                            DataHelper.CurUserInfoData.gold += buyInfo[1];
                            if (!_modifyKeys.Contains("gold")) _modifyKeys.Add("gold");
                            DataHelper.CurGetItem = new[] { 1, _id, buyInfo[1] };
                            GameGlobalManager._instance.OpenGetItem(true);

                            // 上报自定义分析数据 
                            if (_buyType == 2)      // 事件: 商店钻石购买金币
                                GameSdkManager._instance._sdkScript.ReportAnalytics("BuyCoins_Common", "", "");
                            else if (_buyType == 0) // 事件: 商店广告购买金币
                                GameSdkManager._instance._sdkScript.ReportAnalytics("BuyCoins_AD", "", "");
                            break;
                        case 2:
                            DataHelper.CurUserInfoData.diamond += buyInfo[1];
                            if (!_modifyKeys.Contains("diamond")) _modifyKeys.Add("diamond");
                            DataHelper.CurGetItem = new[] { 1, _id, buyInfo[1] };
                            GameGlobalManager._instance.OpenGetItem(true);
                            break;
                    }
                    break;
                case 3: // 部件
                    bool isNew;
                    if (DataHelper.CurUserInfoData.equipments.ContainsKey(_id))
                    {
                        // 老部件 转化为碎片
                        if (DataHelper.CurUserInfoData.equipmentChips.ContainsKey(_id))
                        {
                            DataHelper.CurUserInfoData.equipmentChips[_id] += 10;
                        }
                        else
                        {
                            DataHelper.CurUserInfoData.equipmentChips.Add(_id, 10);
                        }

                        _modifyKeys.Add("equipmentChips");
                        isNew = false;
                    }
                    else
                    {
                        // 新部件
                        DataHelper.CurUserInfoData.equipments.Add(_id, 1);
                        _modifyKeys.Add("equipments");
                        isNew = true;
                    }
                    
                    DataHelper.CurUserInfoData.shopSaleIds[_id] -= 1;
                    _modifyKeys.Add("shopSaleIds");

                    DataHelper.CurGetItem = new[] { 2, _id, isNew ? 1 : 10 };
                    GameGlobalManager._instance.OpenGetItem(true);
                    
                    // 完成日常任务 商店购买X次部件 TaskID:5
                    DataHelper.CompleteDailyTask(5, 1, 0);
                    if (!_modifyKeys.Contains("taskInfo1")) _modifyKeys.Add("taskInfo1");
                    
                    // 完成成就任务 商店累计购买X次部件 TaskID:6
                    DataHelper.CompleteGloalTask(6, 1);
                    // 完成成就任务 累计获得X个部件 TaskID:17
                    DataHelper.CompleteGloalTask(17, 1);
                    // 完成成就任务 累计获得X个红色部件 TaskID:18
                    if (_equipmentQuality >= 4) DataHelper.CompleteGloalTask(18, 1);
                    if (!_modifyKeys.Contains("taskInfo2")) _modifyKeys.Add("taskInfo2");
                    
                    // 上报自定义分析数据 事件: 商店购买装备
                    GameSdkManager._instance._sdkScript.ReportAnalytics("ShopBuy_Equipment", "", "");
                    // 上报自定义分析数据 事件: 获得新部件
                    GameSdkManager._instance._sdkScript.ReportAnalytics("GetNewEquipment", "equipmentNum", DataHelper.CurUserInfoData.equipments.Count);
                    
                    break;
            }

            if (_modifyKeys.Count > 0)
            {
                DataHelper.ModifyLocalData(_modifyKeys, () => { });
            }

            SetData(_type, _id);
            // 刷新货币
            EventManager.Send(CustomEventType.RefreshMoney);
            // 刷新提示红点 菜单栏商店页签
            EventManager<int>.Send(CustomEventType.RefreshRedPoint, 1);
            // 刷新提示红点 菜单栏组装页签
            EventManager<int>.Send(CustomEventType.RefreshRedPoint, 3);
        }
    }
}