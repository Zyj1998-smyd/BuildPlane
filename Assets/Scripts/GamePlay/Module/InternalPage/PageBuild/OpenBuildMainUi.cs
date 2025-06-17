using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.LoadRes;
using Cysharp.Threading.Tasks;
using Data;
using Data.ConfigData;
using GamePlay.Globa;
using GamePlay.Main;
using GamePlay.Module.InternalPage.ItemPrefabs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage.PageBuild
{
    public class OpenBuildMainUi : MonoBehaviour
    {
        /** 飞机部件行列表预制 */
        public ItemBuildListUi ItemBuildListUiPre;
        /** 飞机部件预制 */
        public ItemBuildUi ItemBuildUiPre;
        /** 选中部件操作节点预制 */
        public ItemBuildSelectUi ItemSelectPre;
        
        /** 组装总页面 */
        internal OpenBuildPageUi _openBuildPageUi;
        
        /** 滑动列表 */
        private ScrollRect _scrollRect;
        /** 滑动列表项挂载容器 */
        private RectTransform _contentRect;
        
        /** 列表 部件行列表 */
        private readonly List<ItemBuildListUi> _itemBuildListUis = new List<ItemBuildListUi>();
        /** 列表 部件 */
        private readonly List<ItemBuildUi> _itemBuildUis = new List<ItemBuildUi>();

        /** 当前选中的标题列表索引 */
        private int _curSelectTittle;
        /** 部件类型 */
        private int _type;
        
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;
        
        /** 当前选择的部件 */
        private ItemBuildUi _curSelectItem;
        
        /** 标题栏列表 */
        private readonly GameObject[] _tittleOnUis = new GameObject[6];
        /** 标题栏A */
        private GameObject _tittleLabelA;
        /** 标题栏B */
        private GameObject _tittleLabelB;
        /** 标题栏提示红点 */
        private readonly GameObject[] _tittleRedPoints = new GameObject[6];

        /** 选中部件操作节点 */
        private ItemBuildSelectUi _itemBuildSelect;

        /** 属性节点 */
        private GameObject _propetyObj;
        /** 属性值 */
        private readonly TextMeshProUGUI[] _propetyNumTexts = new TextMeshProUGUI[6];
        /** 属性条 */
        private readonly Image[] _propetyBars = new Image[6];
        /** 属性值 增加值 */
        private readonly TextMeshProUGUI[] _propetyNumTextsAdd = new TextMeshProUGUI[6];
        /** 属性值 减少值 */
        private readonly TextMeshProUGUI[] _propetyNumTextsSub = new TextMeshProUGUI[6];

        /** 平衡说明 */
        private GameObject _balanceTip;

        /** 暂存部件 [类型,ID] */
        private int[] _oldEquipment;

        /** 涂装页面 */
        private OpenBuildPaintUi _openBuildPaintUi;

        /** 提示红点显示状态 */
        private readonly bool[] _showRedPoints = new bool[6];

        private void OnEnable()
        {
            EventManager<int>.Add(CustomEventType.RefreshRedPoint, RefreshRedPoint);
            EventManager<int, bool>.Add(CustomEventType.GuideClickBuild, OnBtnTittle);
            EventManager<ItemBuildUi, bool>.Add(CustomEventType.GuideClickBuild, OnSelectItem);
        }

        private void OnDisable()
        {
            EventManager<int>.Remove(CustomEventType.RefreshRedPoint, RefreshRedPoint);
            EventManager<int, bool>.Remove(CustomEventType.GuideClickBuild, OnBtnTittle);
            EventManager<ItemBuildUi, bool>.Remove(CustomEventType.GuideClickBuild, OnSelectItem);
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
            
            for (int i = 0; i < _cancellationTokenScrollNum.Length; i++)
            {
                _cancellationTokenScrollNum[i]?.Cancel();
                _cancellationTokenScrollNum[i]?.Dispose();
                _cancellationTokenScrollNum[i] = null;
            }

            for (int i = 0; i < _cancellationTokenScrollBar.Length; i++)
            {
                _cancellationTokenScrollBar[i]?.Cancel();
                _cancellationTokenScrollBar[i]?.Dispose();
                _cancellationTokenScrollBar[i] = null;
            }
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _tittleLabelA = transform.Find("Label/LabelA").gameObject;
            _tittleLabelB = transform.Find("Label/LabelB").gameObject;
            List<int> tittlesA = new List<int>(3) { 3, 1, 4 };
            List<int> tittlesB = new List<int>(4) { 2, 5, 6 };
            for (int i = 0; i < tittlesA.Count; i++)
            {
                int index = tittlesA[i] - 1;
                Transform tittleItem = _tittleLabelA.transform.Find("Label" + tittlesA[i]);
                Transform tittleOff = tittleItem.Find("Off");
                Transform tittleOn = tittleItem.Find("On");
                _tittleOnUis[index] = tittleOn.gameObject;
                _tittleRedPoints[index] = tittleOff.Find("RedPoint").gameObject;
                _tittleRedPoints[index].SetActive(false);

                Button btnTittle = tittleOff.GetComponent<Button>();
                btnTittle.onClick.AddListener(() => { OnBtnTittle(index, true); });
            }
            
            for (int i = 0; i < tittlesB.Count; i++)
            {
                int index = tittlesB[i] - 1;
                Transform tittleItem = _tittleLabelB.transform.Find("Label" + tittlesB[i]);
                Transform tittleOff = tittleItem.Find("Off");
                Transform tittleOn = tittleItem.Find("On");
                _tittleOnUis[index] = tittleOn.gameObject;
                _tittleRedPoints[index] = tittleOff.Find("RedPoint").gameObject;
                _tittleRedPoints[index].SetActive(false);

                Button btnTittle = tittleOff.GetComponent<Button>();
                btnTittle.onClick.AddListener(() => { OnBtnTittle(index, true); });
            }
            
            _scrollRect = transform.Find("ListFrame/List").GetComponent<ScrollRect>();
            _contentRect = _scrollRect.transform.Find("Viewport/Content").GetComponent<RectTransform>();

            // 选中部件操作节点
            _itemBuildSelect = Instantiate(ItemSelectPre, _contentRect.parent);
            _itemBuildSelect.Initial();
            _itemBuildSelect.gameObject.SetActive(false);

            _propetyObj = transform.Find("Propety").gameObject;
            Transform propetyA = transform.Find("Propety/PropetyA");
            Transform propetyB = transform.Find("Propety/PropetyB");
            List<int> propetyIdsA = new List<int>(3) { 1, 3, 5 };
            List<int> propetyIdsB = new List<int>(3) { 2, 4, 6 };
            for (int i = 0; i < propetyIdsA.Count; i++)
            {
                Transform propetyItem = propetyA.Find("Propety_" + propetyIdsA[i]);
                if (propetyItem.Find("Value"))
                    _propetyBars[propetyIdsA[i] - 1] = propetyItem.Find("Value").GetComponent<Image>();
                if (propetyItem.Find("Num/Num"))
                    _propetyNumTexts[propetyIdsA[i] - 1] = propetyItem.Find("Num/Num").GetComponent<TextMeshProUGUI>();
                if (propetyItem.Find("Num/NumAdd"))
                {
                    _propetyNumTextsAdd[propetyIdsA[i] - 1] = propetyItem.Find("Num/NumAdd").GetComponent<TextMeshProUGUI>();
                    _propetyNumTextsAdd[propetyIdsA[i] - 1].gameObject.SetActive(false);
                }
                if (propetyItem.Find("Num/NumSub"))
                {
                    _propetyNumTextsSub[propetyIdsA[i] - 1] = propetyItem.Find("Num/NumSub").GetComponent<TextMeshProUGUI>();
                    _propetyNumTextsSub[propetyIdsA[i] - 1].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < propetyIdsB.Count; i++)
            {
                Transform propetyItem = propetyB.Find("Propety_" + propetyIdsB[i]);
                if (propetyItem.Find("Value"))
                    _propetyBars[propetyIdsB[i] - 1] = propetyItem.Find("Value").GetComponent<Image>();
                if (propetyItem.Find("Num/Num"))
                    _propetyNumTexts[propetyIdsB[i] - 1] = propetyItem.Find("Num/Num").GetComponent<TextMeshProUGUI>();
                if (propetyItem.Find("Num/NumAdd"))
                {
                    _propetyNumTextsAdd[propetyIdsB[i] - 1] = propetyItem.Find("Num/NumAdd").GetComponent<TextMeshProUGUI>();
                    _propetyNumTextsAdd[propetyIdsB[i] - 1].gameObject.SetActive(false);
                }
                if (propetyItem.Find("Num/NumSub"))
                {
                    _propetyNumTextsSub[propetyIdsB[i] - 1] = propetyItem.Find("Num/NumSub").GetComponent<TextMeshProUGUI>();
                    _propetyNumTextsSub[propetyIdsB[i] - 1].gameObject.SetActive(false);
                }
            }

            _balanceTip = _propetyObj.transform.Find("Balance").gameObject;

            transform.Find("Propety/Details").GetComponent<Button>().onClick.AddListener(OnBtnDetails);
        }

        /// <summary>
        /// 打开组装主页面
        /// </summary>
        internal void OpenBuildMain()
        {
            // 初始化暂存部件
            _oldEquipment = new[] { -1, -1 };
            
            // 默认选择
            _curSelectTittle = -1;
            OnBtnTittle(0, false);

            // 刷新属性
            RefreshPropety(true);

            // 刷新提示红点
            RefreshRedPoint(3);

            // 涂装页面
            if (_openBuildPaintUi) OnClosePainting();
            else LoadBuildPaint(() => { });

            // 刷新平衡
            RefreshBalance(-1, -1);
        }
        
        /// <summary>
        /// 清除列表
        /// </summary>
        private void ClearList()
        {
            _itemBuildSelect.gameObject.SetActive(false);
            
            for (int i = 0; i < _itemBuildUis.Count; i++)
            {
                Destroy(_itemBuildUis[i].gameObject);
            }

            for (int i = 0; i < _itemBuildListUis.Count; i++)
            {
                Destroy(_itemBuildListUis[i].gameObject);
            }
            
            _itemBuildUis.Clear();
            _itemBuildListUis.Clear();
        }

        /// <summary>
        /// 获取排序后的装备数据
        /// </summary>
        private List<ComponentConfig> GetEquipmentsData()
        {
            List<ComponentConfig> listTmp = new List<ComponentConfig>(ConfigManager.Instance.ComponentTypeConfigDict[_type]);
            List<ComponentConfig> list = new List<ComponentConfig>();
            if (ConfigManager.Instance.isUnlockAll)
            {
                // 测试模式 全部解锁
                for (int i = 0; i < listTmp.Count; i++)
                {
                    list.Add(listTmp[i]);
                }
            }
            else
            {
                // 正常模式 仅展示已获得
                for (int i = 0; i < listTmp.Count; i++)
                {
                    if (DataHelper.CurUserInfoData.equipments.ContainsKey(listTmp[i].ID))
                    {
                        list.Add(listTmp[i]);
                    }
                }
            }
            
            // 列表排序 新手引导期间不执行
            if (MainManager._instance._guideMain1 == null)
                list = new List<ComponentConfig>(DataHelper.EquipmentSort(list));

            return list;
        }

        /// <summary>
        /// 刷新飞机部件列表
        /// </summary>
        async UniTask RefreshList()
        {
            _cancellationToken = new CancellationTokenSource();
            
            // 清除上次刷新的列表
            ClearList();
            // 刷新本次列表
            List<ComponentConfig> list = GetEquipmentsData();
            
            int buildListNumTmp = list.Count / 4;
            int buildListNum = list.Count % 4 == 0 ? buildListNumTmp : buildListNumTmp + 1;
            
            // Debug.Log("行数 = " + buildListNum);

            for (int i = 0; i < buildListNum; i++)
            {
                ItemBuildListUi itemBuildListUi = Instantiate(ItemBuildListUiPre, _contentRect);
                itemBuildListUi.gameObject.name = new StringBuilder("ItemBuildList" + (i + 1)).ToString();
                itemBuildListUi.Initial();
                _itemBuildListUis.Add(itemBuildListUi);
            }

            float timeTmp = (5 / 60f * 1000);
            await UniTask.Delay((int)timeTmp, cancellationToken: _cancellationToken.Token);

            int hangNum = 0;
            int loopNum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                ItemBuildUi itemBuildUi = Instantiate(ItemBuildUiPre, _itemBuildListUis[hangNum].itemPoints[loopNum]);
                itemBuildUi.gameObject.name = new StringBuilder("ItemBuild" + (loopNum + 1)).ToString();
                itemBuildUi.Initial();
                itemBuildUi.openBuildMainUi = this;
                itemBuildUi.SetData(i, list[i].ID);
                _itemBuildUis.Add(itemBuildUi);
                loopNum += 1;
                if (loopNum >= 4)
                {
                    hangNum += 1;
                    loopNum = 0;
                }
            }

            // 归位滑动列表
            _scrollRect.verticalNormalizedPosition = 1f;
            // 设置选中框
            SetItemBuildSelect(-1);
            
            // 当前默认选中
            for (int i = 0; i < _itemBuildUis.Count; i++)
            {
                if (DataHelper.CurUserInfoData.equipEquipments.Contains(_itemBuildUis[i]._id))
                {
                    OnSelectItem(_itemBuildUis[i], false);
                    break;
                }
            }
        }

        /// <summary>
        /// 刷新装备列表
        /// </summary>
        private void RefreshEquipments()
        {
            List<ComponentConfig> list = GetEquipmentsData();
            for (int i = 0; i < _itemBuildUis.Count; i++)
            {
                _itemBuildUis[i].SetData(_itemBuildUis[i]._index, list[i].ID);
            }
            
            // 当前默认选中
            for (int i = 0; i < _itemBuildUis.Count; i++)
            {
                if (DataHelper.CurUserInfoData.equipEquipments.Contains(_itemBuildUis[i]._id))
                {
                    OnSelectItem(_itemBuildUis[i], false);
                    break;
                }
            }
        }

        /// <summary>
        /// 设置选中框
        /// </summary>
        /// <param name="id">指定要显示的部件ID</param>
        internal void SetItemBuildSelect(int id)
        {
            for (int i = 0; i < _itemBuildUis.Count; i++)
            {
                _itemBuildUis[i].SetSelect(DataHelper.CurUserInfoData.equipEquipments.Contains(_itemBuildUis[i]._id));
            }
        }

        /// <summary>
        /// 刷新属性
        /// </summary>
        internal void RefreshPropety(bool isInit)
        {
            Dictionary<string, float> allPropetyNum = DataHelper.GetAllPropety(-1, -1, false);
            List<float> propetyNums = new List<float>(6)
            {
                allPropetyNum["propetyZhongLiang"],
                allPropetyNum["propetyFuKong"],
                allPropetyNum["propetySuDu"],
                allPropetyNum["propetyKangZu"],
                allPropetyNum["propetyTuiJin"],
                allPropetyNum["propetyNengLiang"]
            };
            List<float> propetyValues = new List<float>(6);
            for (int i = 0; i < 6; i++)
            {
                float valueTmp = Mathf.Pow((propetyNums[i] / 5000f), 0.5f);
                float value = Mathf.Clamp01(valueTmp);
                propetyValues.Add(value);
            }
            
            // Debug.Log(JsonConvert.SerializeObject(allPropetyNum));

            if (isInit)
            {
                _oldPropetyNums = new List<float>(propetyNums);
                _oldPropetyValues = new List<float>(propetyValues);
            }

            for (int i = 0; i < _propetyNumTexts.Length; i++)
            {
                if (isInit)
                {
                    // 打开页面刷新数值
                    _propetyNumTexts[i].text = propetyNums[i].ToString("F1").TrimEnd('0').TrimEnd('.');
                    if (_propetyBars[i] != null) _propetyBars[i].fillAmount = propetyValues[i];
                }
                else
                {
                    // 更换部件刷新数值
                    _ = ScrollNumberToTargetAsync(i, _oldPropetyNums[i], propetyNums[i], 0.3f);
                    if (_propetyBars[i] != null)
                        _ = ScrollProgressToAsync(i, _oldPropetyValues[i], propetyValues[i], 0.3f);
                }
            }
        }

        /// <summary>
        /// 刷新换装属性
        /// </summary>
        /// <param name="typeTmp">换装部件类型</param>
        /// <param name="idTmp">换装部件ID</param>
        internal void RefreshChangePropety(int typeTmp, int idTmp)
        {
            Dictionary<string, float> allPropetyNum = DataHelper.GetAllPropety(typeTmp, idTmp, false);
            List<float> propetyNums = new List<float>(6)
            {
                allPropetyNum["propetyZhongLiang"],
                allPropetyNum["propetyFuKong"],
                allPropetyNum["propetySuDu"],
                allPropetyNum["propetyKangZu"],
                allPropetyNum["propetyTuiJin"],
                allPropetyNum["propetyNengLiang"]
            };

            for (int i = 0; i < _propetyNumTexts.Length; i++)
            {
                _propetyNumTextsAdd[i].gameObject.SetActive(false);
                _propetyNumTextsSub[i].gameObject.SetActive(false);
                
                if (propetyNums[i] > _oldPropetyNums[i])
                {
                    _propetyNumTextsAdd[i].gameObject.SetActive(true);
                    _propetyNumTextsAdd[i].text = new StringBuilder("+" + (propetyNums[i] - _oldPropetyNums[i])).ToString();
                }

                if (propetyNums[i] < _oldPropetyNums[i])
                {
                    _propetyNumTextsSub[i].gameObject.SetActive(true);
                    _propetyNumTextsSub[i].text = new StringBuilder("-" + (_oldPropetyNums[i] - propetyNums[i])).ToString();
                }
            }
        }

        /** 暂存属性值(做数值滚动的初始值) */
        private List<float> _oldPropetyNums;
        /** 暂存属性进度值(做进度条滚动的初始值) */
        private List<float> _oldPropetyValues;
        /** UniTask异步信标 数值滚动 */
        private readonly CancellationTokenSource[] _cancellationTokenScrollNum = new CancellationTokenSource[6];
        /** UniTask异步信标 进度条滚动 */
        private readonly CancellationTokenSource[] _cancellationTokenScrollBar = new CancellationTokenSource[6];

        /// <summary>
        /// 数值滚动增加/减少
        /// </summary>
        /// <param name="index">属性列表索引</param>
        /// <param name="start">滚动开始值</param>
        /// <param name="target">滚动目标值</param>
        /// <param name="duration">滚动持续时间</param>
        async UniTask ScrollNumberToTargetAsync(int index, float start, float target, float duration)
        {
            _cancellationTokenScrollNum[index]?.Cancel();
            _cancellationTokenScrollNum[index]?.Dispose();
            _cancellationTokenScrollNum[index] = new CancellationTokenSource();
            
            float startTime = Time.time;
            float endTime = startTime + duration;
            int direction = target > start ? 1 : -1;

            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / duration;
                float newValue = Mathf.Lerp(start, target, t);

                // 避免因浮点运算误差而超过目标值
                if ((direction == 1 && newValue >= target) || (direction == -1 && newValue <= target))
                {
                    newValue = target;
                }

                // _propetyNumTexts[index].text = newValue.ToString("F1").TrimEnd('0').TrimEnd('.');
                _propetyNumTexts[index].text = Mathf.FloorToInt(newValue).ToString();

                await UniTask.Yield();
            }
            
            // _propetyNumTexts[index].text = target.ToString("F1").TrimEnd('0').TrimEnd('.');
            _propetyNumTexts[index].text = Mathf.FloorToInt(target).ToString();
            _oldPropetyNums[index] = target;
            _propetyNumTextsAdd[index].gameObject.SetActive(false);
            _propetyNumTextsSub[index].gameObject.SetActive(false);
        }

        /// <summary>
        /// 进度条滚动增加/减少
        /// </summary>
        /// <param name="index">属性列表索引</param>
        /// <param name="start">滚动开始值</param>
        /// <param name="target">滚动目标值</param>
        /// <param name="duration">滚动持续时间</param>
        async UniTask ScrollProgressToAsync(int index, float start, float target, float duration)
        {
            _cancellationTokenScrollBar[index]?.Cancel();
            _cancellationTokenScrollBar[index]?.Dispose();
            _cancellationTokenScrollBar[index] = new CancellationTokenSource();
            
            float startTime = Time.time;
            float endTime = startTime + duration;
            float direction = target > start ? 1f : -1f;

            while (Time.time < endTime)
            {
                float t = Mathf.Clamp01((Time.time - startTime) / duration);
                float newValue = Mathf.Lerp(start, target, t);

                // 避免因浮点运算误差而超过目标值或低于0
                if ((direction > 0 && newValue >= target) || (direction < 0 && newValue <= target))
                {
                    newValue = target;
                }
                
                _propetyBars[index].fillAmount = newValue;

                await UniTask.Yield();
            }

            _propetyBars[index].fillAmount = target;
            _oldPropetyValues[index] = target;
        }
        
        /// <summary>
        /// 加载涂装页面
        /// </summary>
        /// <param name="cb"></param>
        private void LoadBuildPaint(Action cb)
        {
            LoadResources.XXResourcesLoad("PaintingFrame", handleTmp =>
            {
                GameObject buildPaint = Instantiate(handleTmp, transform);
                _openBuildPaintUi = buildPaint.GetComponent<OpenBuildPaintUi>();
                _openBuildPaintUi.Initial();
                _openBuildPaintUi._openBuildMainUi = this;
                _openBuildPaintUi.gameObject.SetActive(false);
                cb();
            });
        }

        /// <summary>
        /// 刷新平衡
        /// </summary>
        private void RefreshBalance(int wingL, int wingR)
        {
            bool balance = DataHelper.GetPlaneWingBalance(wingL, wingR);
            // Debug.Log("飞机平衡 = " + balance);
            _balanceTip.SetActive(!balance);
        }

        // ----------------------------------------------- 按钮 -----------------------------------------------
        /// <summary>
        /// 按钮 标题栏
        /// </summary>
        /// <param name="index">标题栏列表索引</param>
        /// <param name="isClick">是否玩家点击</param>
        private void OnBtnTittle(int index, bool isClick)
        {
            if (isClick) AudioHandler._instance.PlayAudio(MainManager._instance.audioCamMove);
            if (_curSelectTittle == index) return;
            _curSelectTittle = index;
            
            _curSelectItem = null;
            for (int i = 0; i < _propetyNumTexts.Length; i++)
            {
                _propetyNumTextsAdd[i].gameObject.SetActive(false);
                _propetyNumTextsSub[i].gameObject.SetActive(false);
            }
            
            for (int i = 0; i < _tittleOnUis.Length; i++)
            {
                if (i == index)
                {
                    _tittleOnUis[i].SetActive(true);
                }
                else
                {
                    _tittleOnUis[i].SetActive(false);
                }
            }

            // 标题菜单层级切换
            // switch (index)
            // {
            //     case 2:
            //     case 0:
            //     case 3:
            //         _tittleLabelA.transform.SetAsLastSibling();
            //         break;
            //     case 1:
            //     case 4:
            //     case 5:
            //         _tittleLabelB.transform.SetAsLastSibling();
            //         break;
            // }

            _type = index;

            // 设置主摄像机动画
            MainManager._instance.SetMainCamAniTrigger(_type);
            
            // 刷新部件列表
            _ = RefreshList();
            // 暂存部件
            if (_oldEquipment[0] == -1 && _oldEquipment[1] == -1)
            {
                // 首次执行选择部件类型 直接暂存 不需要操作
                _oldEquipment = new[] { _type, DataHelper.CurUserInfoData.equipEquipments[_type] };
            }
            else
            {
                // 普通执行选择部件类型 判断是否需要还原部件
                int curEquipmentId = DataHelper.CurUserInfoData.equipEquipments[_oldEquipment[0]];
                if (curEquipmentId == _oldEquipment[1])
                    MainManager._instance.ChangePlaneEquipment(_oldEquipment[0], _oldEquipment[1]);
                // 刷新暂存部件
                _oldEquipment[0] = _type;
                _oldEquipment[1] = DataHelper.CurUserInfoData.equipEquipments[_type];
            }

            // 刷新平衡
            RefreshBalance(-1, -1);

            if (isClick)
            {
                for (int i = 0; i < _tittleRedPoints.Length; i++)
                {
                    _tittleRedPoints[i].SetActive(_showRedPoints[i] && _type != i);
                }
            }
        }

        /// <summary>
        /// 按钮 属性详情
        /// </summary>
        private void OnBtnDetails()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);
            _openBuildPageUi.OpenBuildDetails(true);
        }

        /// <summary>
        /// 按钮 涂装
        /// <param name="idTmp">装备ID</param>
        /// </summary>
        internal void OnBtnPainting(int idTmp)
        {
            void runOpenPaint()
            {
                // _propetyObj.SetActive(false);
                _openBuildPaintUi.gameObject.SetActive(true);
                _openBuildPaintUi.OpenBuildPaint(_type, idTmp);
            }

            if (_openBuildPaintUi) runOpenPaint();
            else LoadBuildPaint(runOpenPaint);
        }

        /// <summary>
        /// 关闭涂装页面
        /// </summary>
        internal void OnClosePainting()
        {
            _openBuildPaintUi.gameObject.SetActive(false);
            // _propetyObj.SetActive(true);
        }

        /// <summary>
        /// 选择部件
        /// </summary>
        /// <param name="itemBuildUi">部件</param>
        /// <param name="isClick">玩家点击</param>
        internal void OnSelectItem(ItemBuildUi itemBuildUi, bool isClick)
        {
            if (isClick)
            {
                if (_curSelectItem == itemBuildUi)
                {
                    // Debug.Log("当前选择即是当前选中");
                    return;
                }
            }
            
            _curSelectItem = itemBuildUi;

            _itemBuildSelect.SetSelect(itemBuildUi);
            _itemBuildSelect.gameObject.SetActive(true);

            if (isClick)
            {
                AudioHandler._instance.PlayAudio(MainManager._instance.audioEquip);
                // 玩家点击 换装 本地修改数据 待退出逻辑保存到服务器
                DataHelper.CurUserInfoData.equipEquipments[itemBuildUi._type] = itemBuildUi._id;
                MainManager._instance.ChangePlaneEquipment(itemBuildUi._type, itemBuildUi._id);
                RefreshChangePropety(itemBuildUi._type, itemBuildUi._id);
                RefreshPropety(false);
                RefreshBalance(-1, -1);
            }
        }
        
        /// <summary>
        /// 升级完成
        /// </summary>
        internal void UpGradeComplete(ItemBuildUi itemBuildUi)
        {
            // if (_curSelectItem)
            // {
            //     _curSelectItem = null;
            //     OnSelectItem(itemBuildUi, false);
            // }
            RefreshEquipments();
            _itemBuildSelect.RefrshRedPoint(-1);
        }

        /// <summary>
        /// 刷新提示红点
        /// </summary>
        private void RefreshRedPoint(int index)
        {
            bool[] showRedPoints = { false, false, false, false, false, false };
            // 可以升级
            foreach (KeyValuePair<int, int> data in DataHelper.CurUserInfoData.equipments)
            {
                int equipmentId = data.Key;
                int equipmentLv = data.Value;
                int chipNum = DataHelper.CurUserInfoData.equipmentChips.GetValueOrDefault(equipmentId, 0);
                float targetChipNumTmp = GlobalValueManager.EquipmentUpGradeChipNum;
                for (int j = 0; j < equipmentLv - 1; j++)
                {
                    targetChipNumTmp *= GlobalValueManager.EquipmentUpGradeChipUpGradeNum;
                }

                int targetChipNum = Mathf.CeilToInt(targetChipNumTmp);

                if (chipNum >= targetChipNum)
                {
                    ComponentConfig config = ConfigManager.Instance.ComponentConfigDict[equipmentId];
                    int typeIndex = config.Type;
                    if (!showRedPoints[typeIndex]) showRedPoints[typeIndex] = true;
                }
            }

            // 可以查看涂装
            if (DataHelper.CurUserInfoData.equipmentPaintNews.Count > 0)
            {
                foreach (KeyValuePair<int, int> data in DataHelper.CurUserInfoData.equipmentPaintNews)
                {
                    ComponentConfig config = ConfigManager.Instance.ComponentConfigDict[data.Key];
                    int typeIndex = config.Type;
                    if (!showRedPoints[typeIndex]) showRedPoints[typeIndex] = true;
                }
            }

            for (int i = 0; i < _tittleRedPoints.Length; i++)
            {
                _tittleRedPoints[i].SetActive(showRedPoints[i] && _type != i);
            }

            for (int i = 0; i < showRedPoints.Length; i++)
            {
                _showRedPoints[i] = showRedPoints[i];
            }
        }
    }
}