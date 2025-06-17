using System;
using System.Collections.Generic;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Data;
using GamePlay.Globa;
using GamePlay.Main;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage.ItemPrefabs
{
    public class ItemBuildSelectUi : MonoBehaviour
    {
        private RectTransform _rect;
        private RectTransform _parentRect;

        /** 动画组件 */
        private Animation _animation;

        /** 涂装提示红点 */
        private GameObject _redPoint_Paint;

        /** 当前选中的部件 */
        private ItemBuildUi _itemBuildUi;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _rect = gameObject.GetComponent<RectTransform>();
            _parentRect = transform.parent.GetComponent<RectTransform>();

            _animation = gameObject.GetComponent<Animation>();

            _redPoint_Paint = transform.Find("BtnPainting/RedPoint").gameObject;

            transform.Find("BtnPainting").GetComponent<Button>().onClick.AddListener(OnBtnPaint);
        }

        private void Update()
        {
            if (!gameObject.activeSelf) return;
            if (!_itemBuildUi) return;
            
            var screenPoint = RectTransformUtility.WorldToScreenPoint(MainManager._instance.uiCamera, _itemBuildUi._parentRect.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, screenPoint, MainManager._instance.uiCamera, out var localPoint);
            
            transform.localPosition = new Vector3(localPoint.x, localPoint.y, 0);
        }

        private void OnEnable()
        {
            EventManager<int>.Add(CustomEventType.RefreshRedPoint, RefrshRedPoint);
        }

        private void OnDisable()
        {
            EventManager<int>.Remove(CustomEventType.RefreshRedPoint, RefrshRedPoint);
        }

        /// <summary>
        /// 设置选中
        /// </summary>
        /// <param name="itemBuildUi">目标父节点</param>
        internal void SetSelect(ItemBuildUi itemBuildUi)
        {
            _itemBuildUi = itemBuildUi;

            _itemBuildUi.openBuildMainUi.SetItemBuildSelect(itemBuildUi._id);
            
            _rect.sizeDelta = itemBuildUi._parentRect.sizeDelta;
            
            var screenPoint = RectTransformUtility.WorldToScreenPoint(MainManager._instance.uiCamera, itemBuildUi._parentRect.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, screenPoint, MainManager._instance.uiCamera, out var localPoint);
            
            transform.localPosition = new Vector3(localPoint.x, localPoint.y, 0);

            _animation.Play("SelectItemFrame");

            SetButton();

            RefrshRedPoint(-1);
        }

        /// <summary>
        /// 刷新提示红点
        /// </summary>
        internal void RefrshRedPoint(int index)
        {
            int equipmentId;
            if (DataHelper.CurUpGradeEquipment == -1)
            {
                equipmentId = _itemBuildUi._id;
            }
            else
            {
                equipmentId = DataHelper.CurUpGradeEquipment;
                DataHelper.CurUpGradeEquipment = -1;
            }

            int isShowRedPoint = DataHelper.CurUserInfoData.equipmentPaintNews.GetValueOrDefault(equipmentId, 1);
            _redPoint_Paint.SetActive(isShowRedPoint == 0);
        }

        /// <summary>
        /// 设置按钮
        /// </summary>
        internal void SetButton()
        {
        }

        /// <summary>
        /// 按钮 装备
        /// </summary>
        private void OnBtnEquip()
        {
            if (_itemBuildUi._type == 5)
            {
                // 当前选择的配件是推进器 需要判断是否安装了机翼
                if (DataHelper.CurUserInfoData.equipEquipments[2] == -1 && DataHelper.CurUserInfoData.equipEquipments[3] == -1)
                {
                    GameGlobalManager._instance.ShowTips("请先装备机翼!");
                    return;
                }
            }

            AudioHandler._instance.PlayAudio(MainManager._instance.audioEquip);
            DataHelper.CurUserInfoData.equipEquipments[_itemBuildUi._type] = _itemBuildUi._id;
            DataHelper.ModifyLocalData(new List<string>(1) { "equipEquipments" }, () => { });
            SetButton();
            // 更换飞机部件
            MainManager._instance.ChangePlaneEquipment(_itemBuildUi._type, _itemBuildUi._id);
            // 刷新属性
            _itemBuildUi.openBuildMainUi.RefreshPropety(false);
            // 刷新选中框
            _itemBuildUi.openBuildMainUi.SetItemBuildSelect(_itemBuildUi._id);
        }

        /// <summary>
        /// 按钮 拆卸
        /// </summary>
        private void OnBtnUnEquip()
        {
            DataHelper.CurUserInfoData.equipEquipments[_itemBuildUi._type] = -1;
            DataHelper.ModifyLocalData(new List<string>(1) { "equipEquipments" }, () => { });
            SetButton();
            // 更换飞机部件
            MainManager._instance.ChangePlaneEquipment(_itemBuildUi._type, -1);
            // 刷新属性
            _itemBuildUi.openBuildMainUi.RefreshPropety(false);
            // 刷新选中框
            _itemBuildUi.openBuildMainUi.SetItemBuildSelect(_itemBuildUi._id);
        }

        /// <summary>
        /// 按钮 不可拆卸
        /// </summary>
        private void OnBtnUnEquipNoUse()
        {
            GameGlobalManager._instance.ShowTips("当前部件无法拆卸!");
        }

        /// <summary>
        /// 按钮 涂装
        /// </summary>
        private void OnBtnPaint()
        {
            _itemBuildUi.openBuildMainUi.OnBtnPainting(_itemBuildUi._id);

            if (DataHelper.CurUserInfoData.equipmentPaintNews.ContainsKey(_itemBuildUi._id))
            {
                DataHelper.CurUserInfoData.equipmentPaintNews.Remove(_itemBuildUi._id);
                DataHelper.ModifyLocalData(new List<string> { "equipmentPaintNews" }, () => { });
                // 刷新提示红点
                EventManager<int>.Send(CustomEventType.RefreshRedPoint, 3);
                _itemBuildUi.SetData(_itemBuildUi._index, _itemBuildUi._id);
            }
        }
    }
}