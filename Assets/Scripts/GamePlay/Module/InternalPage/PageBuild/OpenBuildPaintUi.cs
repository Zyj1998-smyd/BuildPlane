using System;
using System.Collections.Generic;
using System.Threading;
using Common.GameRoot.AudioHandler;
using Cysharp.Threading.Tasks;
using Data;
using GamePlay.Globa;
using GamePlay.Main;
using GamePlay.Module.InternalPage.ItemPrefabs;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage.PageBuild
{
    public class OpenBuildPaintUi : MonoBehaviour
    {
        /** 选中颜色操作节点预制 */
        public ItemBuildColorSelectUi BuildColorSelectUiPre;

        /** 页面动画组件 */
        private Animation _animation;
        
        /** 滑动列表 */
        private ScrollRect _scrollRect;
        /** 滑动列表挂载容器 */
        private Transform _content;

        /** 底漆颜色列表 */
        private readonly ItemBuildColorUi[] _diQiUis = new ItemBuildColorUi[14];
        /** 装饰颜色列表 */
        private readonly ItemBuildColorUi[] _zhuangShiUis = new ItemBuildColorUi[14];
        /** LED灯组列表 */
        private readonly ItemBuildColorUi[] _ledLightUis = new ItemBuildColorUi[7];
        /** 底漆贴纸列表 */
        private readonly ItemBuildColorUi[] _diQiTieZhiUis = new ItemBuildColorUi[7];
        
        /** 当前选中的部件涂装 */
        private readonly ItemBuildColorUi[] _curSelectColorItem = new ItemBuildColorUi[4];
        /** 选中部件涂装操作节点 */
        private readonly ItemBuildColorSelectUi[] _itemBuildColorSelect = new ItemBuildColorSelectUi[4];

        /** 解锁条件文本 */
        private readonly GameObject[] _tittleTextUis_1 = new GameObject[4];
        /** 解锁条件星级 */
        private readonly GameObject[] _tittleTextUis_2 = new GameObject[4];
        /** 锁定框 */
        private readonly GameObject[][] _colorListLockUis = new GameObject[4][];

        /** 记录当前选择的部件类型用于还原动画状态 */
        private int _curEquipmentType;
        /** 当前选择的部件ID */
        internal int _curEquipmentId;
        /** 记录当前装备的涂装列表 */
        internal List<int> _oldEquipEquipmentColors;
        
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;

        /** 组装主页面 */
        internal OpenBuildMainUi _openBuildMainUi;

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            transform.Find("BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);

            _animation = gameObject.GetComponent<Animation>();
            
            _scrollRect = transform.Find("List").GetComponent<ScrollRect>();
            _content = _scrollRect.transform.Find("Viewport/Content");

            for (int i = 0; i < 4; i++)
            {
                _tittleTextUis_1[i] = _content.Find("Title" + (i + 1) + "/Text/TextTip").gameObject;
                _tittleTextUis_2[i] = _content.Find("Title" + (i + 1) + "/Text/StartTip").gameObject;
            }

            _colorListLockUis[0] = new[] { _content.Find("BuildColorList1_1/Lock").gameObject, _content.Find("BuildColorList1_2/Lock").gameObject };
            _colorListLockUis[1] = new[] { _content.Find("BuildColorList2_1/Lock").gameObject, _content.Find("BuildColorList2_2/Lock").gameObject };
            _colorListLockUis[2] = new[] { _content.Find("BuildColorList3_1/Lock").gameObject };
            _colorListLockUis[3] = new[] { _content.Find("BuildColorList4_1/Lock").gameObject };

            for (int i = 0; i < 7; i++)
            {
                ItemBuildColorUi diQiUi = _content.Find("BuildColorList1_1/BuildColor" + i).GetComponent<ItemBuildColorUi>();
                diQiUi.Initial();
                diQiUi._openBuildPaintUi = this;
                diQiUi.SetData(i, 1, GlobalValueManager.BuildPaintIds[0][i]);
                _diQiUis[i] = diQiUi;
                
                ItemBuildColorUi zhuangShiUi = _content.Find("BuildColorList2_1/BuildColor" + i).GetComponent<ItemBuildColorUi>();
                zhuangShiUi.Initial();
                zhuangShiUi._openBuildPaintUi = this;
                zhuangShiUi.SetData(i, 2, GlobalValueManager.BuildPaintIds[1][i]);
                _zhuangShiUis[i] = zhuangShiUi;
                
                ItemBuildColorUi ledLightUi = _content.Find("BuildColorList3_1/BuildColor" + i).GetComponent<ItemBuildColorUi>();
                ledLightUi.Initial();
                ledLightUi._openBuildPaintUi = this;
                ledLightUi.SetData(i, 3, GlobalValueManager.BuildPaintIds[2][i]);
                _ledLightUis[i] = ledLightUi;
                
                ItemBuildColorUi diQiTieZhiUi = _content.Find("BuildColorList4_1/BuildColor" + i).GetComponent<ItemBuildColorUi>();
                diQiTieZhiUi.Initial();
                diQiTieZhiUi._openBuildPaintUi = this;
                diQiTieZhiUi.SetData(i, 4, GlobalValueManager.BuildPaintIds[3][i]);
                _diQiTieZhiUis[i] = diQiTieZhiUi;
            }

            for (int i = 7; i < 14; i++)
            {
                ItemBuildColorUi diQiUi = _content.Find("BuildColorList1_2/BuildColor" + i).GetComponent<ItemBuildColorUi>();
                diQiUi.Initial();
                diQiUi._openBuildPaintUi = this;
                diQiUi.SetData(i, 1, GlobalValueManager.BuildPaintIds[0][i]);
                _diQiUis[i] = diQiUi;

                ItemBuildColorUi zhuangShiUi = _content.Find("BuildColorList2_2/BuildColor" + i).GetComponent<ItemBuildColorUi>();
                zhuangShiUi.Initial();
                zhuangShiUi._openBuildPaintUi = this;
                zhuangShiUi.SetData(i, 2, GlobalValueManager.BuildPaintIds[1][i]);
                _zhuangShiUis[i] = zhuangShiUi;
            }

            for (int i = 0; i < 4; i++)
            {
                _itemBuildColorSelect[i] = Instantiate(BuildColorSelectUiPre, _content.parent);
                _itemBuildColorSelect[i].Initial();
                _itemBuildColorSelect[i]._openBuildPaintUi = this;
                _itemBuildColorSelect[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 打开涂装页面
        /// <param name="curEquipmentType">当前选择的部件类型</param>
        /// <param name="curEquipmentId">当前选择的部件ID</param>
        /// </summary>
        internal void OpenBuildPaint(int curEquipmentType, int curEquipmentId)
        {
            _scrollRect.verticalNormalizedPosition = 1f;
            _curEquipmentType = curEquipmentType;
            _curEquipmentId = curEquipmentId;
            _oldEquipEquipmentColors = new List<int>(DataHelper.CurUserInfoData.equipmentPaints.GetValueOrDefault(_curEquipmentId, new List<int> { 0, 0, 0, 0 }));
            
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);
            
            // MainManager._instance.SetMainCamAniTrigger(6);

            SetColorListLock();

            _ = DelayRunFun(() =>
            {
                _scrollRect.verticalNormalizedPosition = 1f;
                SetColorSelect(1);
                SetColorSelect(2);
                SetColorSelect(3);
                SetColorSelect(4);
            });
        }

        /// <summary>
        /// 延迟执行方法
        /// <param name="cb">回调</param>
        /// </summary>
        async UniTask DelayRunFun(Action cb)
        {
            _cancellationToken = new CancellationTokenSource();
            float timeTmp = (30 / 60f * 1000);
            await UniTask.Delay((int)timeTmp, cancellationToken: _cancellationToken.Token);
            cb();
        }

        /// <summary>
        /// 设置涂装解锁状态
        /// </summary>
        private void SetColorListLock()
        {
            int curLevel = DataHelper.CurUserInfoData.equipments.GetValueOrDefault(_curEquipmentId, GlobalValueManager.InitEquipmentLv);
            // 底漆颜色
            _tittleTextUis_1[0].SetActive(curLevel < 2);
            _tittleTextUis_2[0].SetActive(curLevel < 2);
            _colorListLockUis[0][0].SetActive(curLevel < 2);
            _colorListLockUis[0][1].SetActive(curLevel < 2);
            // 装饰颜色
            _tittleTextUis_1[1].SetActive(curLevel < 2);
            _tittleTextUis_2[1].SetActive(curLevel < 2);
            _colorListLockUis[1][0].SetActive(curLevel < 2);
            _colorListLockUis[1][1].SetActive(curLevel < 2);
            // LED灯组
            _tittleTextUis_1[2].SetActive(curLevel < 3);
            _tittleTextUis_2[2].SetActive(curLevel < 3);
            _colorListLockUis[2][0].SetActive(curLevel < 3);
            // 底漆贴纸
            _tittleTextUis_1[3].SetActive(curLevel < 4);
            _tittleTextUis_2[3].SetActive(curLevel < 4);
            _colorListLockUis[3][0].SetActive(curLevel < 4);
        }

        /// <summary>
        /// 设置装备中的涂装
        /// <param name="type">类型</param>
        /// </summary>
        internal void SetColorSelect(int type)
        {
            List<int> equipmentPaints = DataHelper.CurUserInfoData.equipmentPaints.GetValueOrDefault(_curEquipmentId, new List<int> { 0, 0, 0, 0 });
            switch (type)
            {
                case 1: // 底漆
                    for (int i = 0; i < _diQiUis.Length; i++)
                    {
                        if (_diQiUis[i]._id == equipmentPaints[0])
                        {
                            OnSelectColorItem(_diQiUis[i]);
                            break;
                        }
                    }
                    break;
                case 2: // 装饰
                    for (int i = 0; i < _zhuangShiUis.Length; i++)
                    {
                        if (_zhuangShiUis[i]._id == equipmentPaints[1])
                        {
                            OnSelectColorItem(_zhuangShiUis[i]);
                            break;
                        }
                    }
                    break;
                case 3: // 灯组
                    for (int i = 0; i < _ledLightUis.Length; i++)
                    {
                        if (_ledLightUis[i]._id == equipmentPaints[2])
                        {
                            OnSelectColorItem(_ledLightUis[i]);
                            break;
                        }
                    }
                    break;
                case 4: // 贴纸
                    for (int i = 0; i < _diQiTieZhiUis.Length; i++)
                    {
                        if (_diQiTieZhiUis[i]._id == equipmentPaints[3])
                        {
                            OnSelectColorItem(_diQiTieZhiUis[i]);
                            break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 关闭涂装页面
        /// </summary>
        async UniTask OnCloseBuildPaint()
        {
            float timeTmp = (10 / 60f * 1000);
            await UniTask.Delay((int)timeTmp, cancellationToken: _cancellationToken.Token);

            for (int i = 0; i < _curSelectColorItem.Length; i++)
            {
                _curSelectColorItem[i] = null;
                _itemBuildColorSelect[i].gameObject.SetActive(false);
            }

            Debug.Log(JsonConvert.SerializeObject(_oldEquipEquipmentColors));
            
            if (!DataHelper.CurUserInfoData.equipmentPaints.ContainsKey(_curEquipmentId))
            {
                DataHelper.CurUserInfoData.equipmentPaints.Add(_curEquipmentId, _oldEquipEquipmentColors);
            }
            else
            {
                DataHelper.CurUserInfoData.equipmentPaints[_curEquipmentId] = _oldEquipEquipmentColors;
            }

            DataHelper.ModifyLocalData(new List<string>(2) { "equipmentPaints", "taskInfo1" }, () => { });
            
            // MainManager._instance.SetMainCamAniTrigger(_curEquipmentType);
            MainManager._instance.RefreshPlaneColor();

            _openBuildMainUi.OnClosePainting();
        }

        // --------------------------------------------- 按钮 ---------------------------------------------
        /// <summary>
        /// 按钮 关闭
        /// </summary>
        private void OnBtnClose()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            _animation.Play("UiPaintingClose");

            _cancellationToken = new CancellationTokenSource();
            _ = OnCloseBuildPaint();
        }

        /// <summary>
        /// 选择部件涂装
        /// </summary>
        /// <param name="itemBuildColorUi">涂装</param>
        /// <param name="isClick">玩家点击</param>
        internal void OnSelectColorItem(ItemBuildColorUi itemBuildColorUi, bool isClick = false)
        {
            if (_curSelectColorItem[itemBuildColorUi._type - 1] == itemBuildColorUi)
            {
                // Debug.Log("当前选择即是当前选中");
                return;
            }

            if (isClick) AudioHandler._instance.PlayAudio(MainManager._instance.audioPaint);
            
            _curSelectColorItem[itemBuildColorUi._type - 1] = itemBuildColorUi;

            _itemBuildColorSelect[itemBuildColorUi._type - 1].SetSelect(itemBuildColorUi);
            _itemBuildColorSelect[itemBuildColorUi._type - 1].gameObject.SetActive(true);

            if (isClick)
            {
                // 玩家点击才需要执行 打开页面调用不需要执行
                // 设置涂装 本地保存数据 待涂装页面关闭保存数据
                if (!DataHelper.CurUserInfoData.equipmentPaints.ContainsKey(_curEquipmentId))
                {
                    // 已拥有的配件涂装列表中没有当前配件的涂装列表 开辟一份列表并存入当前涂装数据
                    List<int> listTmp = new List<int>(4);
                    for (int i = 0; i < 4; i++)
                    {
                        listTmp.Add(i == itemBuildColorUi._type - 1 ? itemBuildColorUi._id : 0);
                    }

                    DataHelper.CurUserInfoData.equipmentPaints.Add(_curEquipmentId, listTmp);
                }
                else
                {
                    // 已拥有的配件涂装列表中已有当前配件的涂装列表 直接存入当前涂装数据
                    DataHelper.CurUserInfoData.equipmentPaints[_curEquipmentId][itemBuildColorUi._type - 1] = itemBuildColorUi._id;
                }

                // 刷新飞机涂装
                MainManager._instance.RefreshPlaneColor();
            
                // 刷新真实数据(打开页面时保存的数据)
                bool isNeedBuy = _itemBuildColorSelect[itemBuildColorUi._type - 1]._isNeedBuy;
                bool isBuyComplete = _itemBuildColorSelect[itemBuildColorUi._type - 1]._isBuyComplete;
                if ((!isNeedBuy && isBuyComplete) || (isNeedBuy && isBuyComplete))
                {
                    // 无需购买的 || 需要购买且购买完成的
                    Debug.Log("保存涂装");
                    _oldEquipEquipmentColors[itemBuildColorUi._type - 1] = itemBuildColorUi._id;
                    // 完成日常任务 完成X次涂装更换 TaskID:7
                    DataHelper.CompleteDailyTask(7, 1, 0);
                }
            }
        }
    }
}