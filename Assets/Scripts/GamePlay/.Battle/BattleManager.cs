using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot;
using Common.GameRoot.AudioHandler;
using Common.LoadRes;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using GamePlay.Battle.Ui;
using GamePlay.Globa;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace GamePlay.Battle
{
    public class BattleManager : MonoBehaviour
    {
        internal static BattleManager _instance;
        
        /** 移动音效 */
        public AudioClip MoveAudio;
        /** 刷新原料瓶闪光音效 */
        public AudioClip ChangeStarAudio;
        /** 杯身Logo */
        public Texture[] OrderCupBodyLogos;

        /** 订单完成 */
        public AudioClip OrderCompleteAudio;
        /** 胜利结算音效 */
        public AudioClip AccountWinAudio;
        /** 失败结算音效 */
        public AudioClip AccountFailAudio;
        /** 按钮点击音效 */
        public AudioClip BtnClickAudio;

        /** 统一结算订单超过3杯(>=3)音效 */
        public AudioClip UnifyOrderWaOAudio;
        /** 统一结算订单超过2杯(==2)音效 */
        public AudioClip UnifyOrderGoodAudio;

        /** 订单杯子位置列表 */
        private List<Transform> _orderCupPoints = new List<Transform>(5);
        /** 订单杯子列表 */
        private readonly CupControl[] _orderCups = new CupControl[5];

        /** 备料杯子位置列表 */
        private List<Transform> _prepareCupPoints = new List<Transform>(6);
        /** 备料杯子列表 */
        private List<CupControl> _prepareCups = new List<CupControl>(6);
        /** 备料杯子(动画使用节点)列表 */
        private List<GameObject> _prepareCupsB = new List<GameObject>(6);

        /** 原料瓶位置列表 */
        private List<List<Transform>> _vesselPoints = new List<List<Transform>>(5);
        
        /** 备料杯子清空动画 */
        private Animation _prepareCupObjAni;
        /** 原料瓶切换闪光特效 */
        private GameObject _vesselsChangeStar;

        /** 4号/5号订单杯 5号/6号备料杯 UI节点名称 */
        private readonly List<string> _unlockCup3DUiNames = new List<string>(4) { "CupNull1", "CupNull2", "CupSNull1", "CupSNull2" };
        /** 4号/5号订单杯 5号/6号备料杯 解锁 */
        private List<Ui3DCupUnlock> _unlockCup3DUis;
        
        /** 已完成订单数量 */
        [HideInInspector] public int orderCompleteNum;
        /** 目标订单数量 */
        [HideInInspector] public int orderCompleteNumMax;
        
        /** 订单杯子数量 */
        [HideInInspector] public int orderCupNum;
        /** 备料杯子数量 */
        [HideInInspector] public int prepareCupNum;
        
        /** 游戏暂停 */
        [HideInInspector] public bool gamePause;
        /** 游戏结束 */
        [HideInInspector] public bool gameEnd;

        /** 道具使用中 */
        [HideInInspector] public bool itemUsing;

        /** 原料瓶不可使用动画播放中 */
        [HideInInspector] public bool vesselErrorAniPlaying;
        
        /** 订单号 */
        private int _orderNum;

        /** 复活次数 */
        [HideInInspector] public int reviveNum;

        /** 备料杯清空类型 0: 使用道具 1: 复活 */
        [HideInInspector] public int prepareCupClearType;

        /** 解锁杯子分享使用状态 */
        [HideInInspector] public bool unlockShareUsed;

        #region 生命周期

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            
            Transform orderCupObj = GameObject.Find("Order").transform;     // 订单杯子节点
            Transform prepareCupObj = GameObject.Find("Prepare").transform; // 备料杯子节点
            Transform vesselObj = GameObject.Find("Vessels").transform;     // 原料瓶节点
            
            _prepareCupObjAni = prepareCupObj.GetComponent<Animation>();
            _vesselsChangeStar = GameObject.Find("Scene").transform.Find("Star").gameObject;
            _vesselsChangeStar.SetActive(false);
            
            // 订单杯子
            _orderCupPoints = new List<Transform>(5);
            for (int i = 0; i < 5; i++)
            {
                var orderCupPoint = orderCupObj.Find("CupPoint_" + (i + 1));
                _orderCupPoints.Add(orderCupPoint);
            }
            
            // 备料杯子
            _prepareCupPoints = new List<Transform>(6);
            _prepareCups = new List<CupControl>(6);
            _prepareCupsB = new List<GameObject>(6);
            for (int i = 0; i < 6; i++)
            {
                var prepareCupPoint = prepareCupObj.Find("CupSPoint_" + (i + 1));
                var prepareCup = prepareCupPoint.Find("CupS").GetComponent<CupControl>();
                _prepareCupPoints.Add(prepareCupPoint);
                _prepareCups.Add(prepareCup);
                _prepareCupsB.Add(prepareCupObj.Find("CupSPointB_" + (i + 1) + "/CupS").gameObject);
            }

            // 原料瓶
            _vesselPoints = new List<List<Transform>>(5);
            for (int i = 0; i < 5; i++)
            {
                var vesselPoints = new List<Transform>(7);
                for (int j = 0; j < 7; j++)
                {
                    var vesselPoint = vesselObj.Find("VesselPoint_" + i + "_" + (j + 1));
                    vesselPoints.Add(vesselPoint);
                }
                _vesselPoints.Add(vesselPoints);
            }
            
            // 解锁新的订单杯/备料杯
            _unlockCup3DUis = new List<Ui3DCupUnlock>(_unlockCup3DUiNames.Count);
            for (int i = 0; i < _unlockCup3DUiNames.Count; i++)
            {
                Ui3DCupUnlock cupUnlockUiTmp;
                if (i == 0 || i == 1)
                {
                    cupUnlockUiTmp = orderCupObj.Find(_unlockCup3DUiNames[i]).GetComponent<Ui3DCupUnlock>();
                }
                else
                {
                    cupUnlockUiTmp = prepareCupObj.Find(_unlockCup3DUiNames[i]).GetComponent<Ui3DCupUnlock>();
                }

                cupUnlockUiTmp.AwakeOnUi();
                _unlockCup3DUis.Add(cupUnlockUiTmp);
            }
        }
        
        private void Start()
        {
            GameGlobalManager._instance.SetCanvasUiMain();
            if (DataHelper.isRootLoad)
            {
                DataHelper.isRootLoad = false;
                GameRootLoad.Instance.EndLoad(0, () => { });
            }
            else
            {
                GameGlobalManager._instance.ShowLoadSceneEnd(0, () => { });
            }
            
            // 获取游戏数据
            DataHelper.GetData();
            
            // 初始化局内数据
            InitData();
            // 初始化未解锁的新杯子UI
            InitUnlockCupsUi();
            
            // 初始化备料杯子
            InitPrepareCups();
            
            // 初始化订单杯子
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            _ = InitOrderCup();
            
            // 初始化原料瓶
            InitVessels();
            
            AudioHandler._instance.PlayBGM("Bgm1");
        }
        
        private void Update()
        {
            if (gamePause) return;
            if (itemUsing) return;
            if (gameEnd) return;
            if (Input.GetMouseButtonDown(0))
            {
                // 新手引导
                if (DataHelper.CurUserInfoData.guideCompleteList[0] == 0)
                {
                    if (Camera.main != null)
                    {
                        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out RaycastHit hit))
                        {
                            var obj = hit.collider.gameObject;
                            var objName = obj.name;
                            if (objName == "Vessel_0_3")
                            {
                                // 隐藏引导节点
                                UiBattle._instance._uiGuide.CompleteGuideStep_1();
                                UiBattle._instance.RunGuideStep(true);
                                // 正常倒水逻辑
                                ConfigManager.Instance.ConsoleLog(0, new StringBuilder("新手引导 使用原料瓶 [0,3]").ToString());
                                _curUseVessel = obj.GetComponent<VesselControl>();
                                _curStepIndex = _curUseVessel.type;
                                _unifyOrderCups = new List<CupControl>();
                                _unifyOrderCupIndexs = new List<int>();
                                RunStep();
                            }
                        }
                    }

                    return;
                }

                if (!IsOnUI(Input.mousePosition))
                {
                    if (Camera.main != null)
                    {
                        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out RaycastHit hit))
                        {
                            var obj = hit.collider.gameObject;
                            var objName = obj.name;
                            ConfigManager.Instance.ConsoleLog(0, objName);
                            if (objName == _unlockCup3DUiNames[0])
                            {
                                // 解锁4号订单杯
                                ConfigManager.Instance.ConsoleLog(0, "解锁4号订单杯");
                                AudioHandler._instance.PlayAudio(BtnClickAudio);
                                _unlockCup3DUis[0].UnLockCup(0, 0);
                            }
                            else if (objName == _unlockCup3DUiNames[1])
                            {
                                // 解锁5号订单杯
                                ConfigManager.Instance.ConsoleLog(0, "解锁5号订单杯");
                                AudioHandler._instance.PlayAudio(BtnClickAudio);
                                _unlockCup3DUis[1].UnLockCup(0, 1);
                            }
                            else if (objName == _unlockCup3DUiNames[2])
                            {
                                // 解锁5号备料杯
                                ConfigManager.Instance.ConsoleLog(0, "解锁5号备料杯");
                                AudioHandler._instance.PlayAudio(BtnClickAudio);
                                _unlockCup3DUis[2].UnLockCup(1, 2);
                            }
                            else if (objName == _unlockCup3DUiNames[3])
                            {
                                // 解锁6号备料杯
                                ConfigManager.Instance.ConsoleLog(0, "解锁6号备料杯");
                                AudioHandler._instance.PlayAudio(BtnClickAudio);
                                _unlockCup3DUis[3].UnLockCup(1, 3);
                            }
                            else
                            {
                                // 点击原料瓶
                                if (objName.Contains("Vessel"))
                                {
                                    var indexTmp = objName.Split('_');
                                    var index_i = int.Parse(indexTmp[1]);
                                    var index_j = int.Parse(indexTmp[2]);

                                    ConfigManager.Instance.ConsoleLog(0, new StringBuilder("使用原料瓶 [" + index_i + "," + index_j + "]").ToString());
                                    if (_curStepIndex != -1) return;
                                    if (_unifyOrderCups.Count > 0) return;
                                    if (_isStepPrepareCupRunning) return;
                                    
                                    if (index_i == 0)
                                    {
                                        // 第一排
                                        _curUseVessel = obj.GetComponent<VesselControl>();
                                        _curStepIndex = _curUseVessel.type;
                                        _unifyOrderCups = new List<CupControl>();
                                        _unifyOrderCupIndexs = new List<int>();
                                        RunStep();
                                    }
                                    else
                                    {
                                        // 不是第一排 找出当前实时的第一排原料瓶节点名称 拆分出坐标
                                        if (_vessels[0][index_j] != null)
                                        {
                                            var indexsTmp = _vessels[0][index_j].gameObject.name.Split('_');
                                            var indexTmpI = int.Parse(indexsTmp[1]);
                                            var indexTmpJ = int.Parse(indexsTmp[2]);
                                            if (index_i == indexTmpI)
                                            {
                                                // 能正常执行倒水逻辑
                                                _curUseVessel = obj.GetComponent<VesselControl>();
                                                _curStepIndex = _curUseVessel.type;
                                                _unifyOrderCups = new List<CupControl>();
                                                _unifyOrderCupIndexs = new List<int>();
                                                RunStep();
                                            }
                                            else
                                            {
                                                // 不可使用
                                                if (DataHelper.CurUserInfoData.guideCompleteList[1] == 0)
                                                {
                                                    UiBattle._instance.RunGuideStep(false);
                                                    obj.GetComponent<VesselControl>().NoCanUseError(true, () =>
                                                    {
                                                        UiBattle._instance._uiGuide.RunGuideStep_2();
                                                    });
                                                }
                                                else
                                                {
                                                    obj.GetComponent<VesselControl>().NoCanUseError(false, () => { });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
        }

        private void OnEnable()
        {
            EventManager<int>.Add(CustomEventType.VesselAdd, EventAddVessel);
        }

        private void OnDisable()
        {
            EventManager<int>.Remove(CustomEventType.VesselAdd, EventAddVessel);
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化局内数据
        /// </summary>
        private void InitData()
        {
            // 解锁的订单杯数量
            orderCupNum = 3;
            // if (DataHelper.CurUserInfoData.cupUnlockStatus[0] == 1) orderCupNum += 1; // 持久化数据 不需要
            // if (DataHelper.CurUserInfoData.cupUnlockStatus[1] == 1) orderCupNum += 1; // 持久化数据 不需要
            DataHelper.CurUserInfoData.cupUnlockStatus[0] = 0;
            DataHelper.CurUserInfoData.cupUnlockStatus[1] = 0;
            
            // 解锁的备料杯数量
            prepareCupNum = 4;
            // if (DataHelper.CurUserInfoData.cupUnlockStatus[2] == 1) prepareCupNum += 1; // 持久化数据 不需要
            // if (DataHelper.CurUserInfoData.cupUnlockStatus[3] == 1) prepareCupNum += 1; // 持久化数据 不需要
            DataHelper.CurUserInfoData.cupUnlockStatus[2] = 0;
            DataHelper.CurUserInfoData.cupUnlockStatus[3] = 0;
            
            _curStepIndex = -1;

            orderCompleteNumMax = DataHelper.CurOrderNum;
            orderCompleteNum = 0;

            _orderNum = 0;

            reviveNum = GlobalValueManager.ReviveNumMax;

            unlockShareUsed = false;
        }

        #endregion

        #region 流程

        /** 当前使用的原料瓶 */
        private VesselControl _curUseVessel;
        /** 当前执行的步骤 */
        private int _curStepIndex;
        
        /** 当前倒入的杯子类型 0: 订单杯 1: 备料杯 */
        private int _cupType;
        /** 当前倒入的订单杯子索引 */
        private int _curOrderCupIndex;
        /** 当前倒入的备料杯子索引 */
        private int _curPrepareCupIndex;

        /** 统一结算订单杯列表 */
        private List<CupControl> _unifyOrderCups = new List<CupControl>();
        /** 统一结算订单杯索引列表 */
        private List<int> _unifyOrderCupIndexs = new List<int>();
        /** 统一结算订单杯计数器 */
        private int _unifyOrderCount;

        /// <summary>
        /// 执行步骤
        /// </summary>
        private void RunStep()
        {
            int orderCupIndex = -1;
            float orderCupHeightTmp = 0;
            for (int i = 0; i < _orderCups.Length; i++)
            {
                if (_orderCups[i] == null) continue;
                if (_curUseVessel.colorIds[_curStepIndex] == _orderCups[i].colorId)
                {
                    if (_orderCups[i].nowHight < 1f)
                    {
                        if (orderCupIndex == -1)
                        {
                            orderCupIndex = i;
                            orderCupHeightTmp = _orderCups[i].nowHight;
                        }
                        else
                        {
                            if (_orderCups[i].nowHight > orderCupHeightTmp)
                            {
                                orderCupIndex = i;
                                orderCupHeightTmp = _orderCups[i].nowHight;
                            }
                        }
                    }
                }
            }
            
            ConfigManager.Instance.ConsoleLog(0, new StringBuilder("符合条件的订单杯子 = " + orderCupIndex).ToString());

            if (orderCupIndex == -1)
            {
                // 没有符合条件的订单杯子
                var prepareCupIndex = GetPrepareCupIndex();
                ConfigManager.Instance.ConsoleLog(0, new StringBuilder("符合条件的备料杯子 = " + prepareCupIndex).ToString());
                if (prepareCupIndex == -1)
                {
                    // 没有符合条件的备料杯子 游戏结束
                    if (reviveNum > 0)
                    {
                        // 单局复活次数未用完 打开复活
                        UiBattle._instance.OnBtnOpenRevive(true);
                    }
                    else
                    {
                        // 单局复活次数已用完 游戏结束
                        UiBattle._instance.OnOpenAccount(false);
                    }
                }
                else
                {
                    // 找到符合条件的备料杯子 将原料倒进备料杯子
                    var targetPointTmp = _prepareCups[prepareCupIndex].transform.position;
                    var target = new Vector3(targetPointTmp.x, (targetPointTmp.y + 1f), targetPointTmp.z);
                    DelayTime.DelayFrame(() =>
                    {
                        if (_curStepIndex == 0) AudioHandler._instance.PlayAudio(MoveAudio);
                        TransformMove(_curUseVessel.transform, target, 10, () =>
                        {
                            _cupType = 1;
                            _curUseVessel.PourOutStart();
                            _curPrepareCupIndex = prepareCupIndex;
                        });
                    }, 1, this.GetCancellationTokenOnDestroy());
                }
            }
            else
            {
                // 找到符合条件的订单杯子 将原料倒进订单杯子
                var targetPointTmp = _orderCupPoints[orderCupIndex].transform.position;
                var target = new Vector3(targetPointTmp.x, (targetPointTmp.y + 1.5f), targetPointTmp.z);
                DelayTime.DelayFrame(() =>
                {
                    if (_curStepIndex == 0) AudioHandler._instance.PlayAudio(MoveAudio);
                    TransformMove(_curUseVessel.transform, target, 10, () =>
                    {
                        _cupType = 0;
                        _curUseVessel.PourOutStart();
                        _curOrderCupIndex = orderCupIndex;
                    });
                }, 1, this.GetCancellationTokenOnDestroy());
            }
        }
        
        /** 寻找符合条件的备料杯子 */
        private int GetPrepareCupIndex()
        {
            for (int i = 0; i < _prepareCups.Count; i++)
            {
                if (i < prepareCupNum)
                {
                    if (_prepareCups[i].nowHight <= 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 统一结算订单杯
        /// <param name="type">调用类型 0: 原料瓶用完 1: 备料杯倒完</param>
        /// <param name="cb">结算完成回调</param>
        /// </summary>
        private void UnifyOrderCupAccount(int type, Action cb)
        {
            if (_unifyOrderCups.Count <= 0)
            {
                // 没有需要结算的订单杯 直接执行回调(执行步骤 备料杯倒到订单杯)
                if (type == 0)
                    RunStepPrepareCup(cb);
                else
                    cb();
            }
            else
            {
                // 有需要结算的订单杯
                _unifyOrderCount = 0;
                if (_unifyOrderCups.Count == 2) AudioHandler._instance.PlayAudio(UnifyOrderGoodAudio); // 1次结算2杯
                if (_unifyOrderCups.Count >= 3) AudioHandler._instance.PlayAudio(UnifyOrderWaOAudio);  // 1次结算3杯及以上
                for (int i = 0; i < _unifyOrderCups.Count; i++)
                {
                    var iTmp = i;
                    var oldOrderCup = _unifyOrderCups[iTmp].gameObject;
                    var oldOrderCupIndex = _unifyOrderCupIndexs[iTmp];
                    AudioHandler._instance.PlayAudio(OrderCompleteAudio);
                    orderCompleteNum += 1;
                    UiBattle._instance.OrderComplete();
                    AudioHandler._instance.PlayAudio(MoveAudio);
                    DataHelper.CupDatas[_unifyOrderCups[i].orderNum][1] = 3;
                    DelayTime.DelayFrame(() =>
                    {
                        _unifyOrderCups[iTmp].OrderComplete(() =>
                        {
                            Destroy(oldOrderCup); // 订单完成回收订单杯子
                            if (orderCompleteNum >= orderCompleteNumMax) UiBattle._instance.OnOpenAccount(true);
                        }, () =>
                        {
                            CreateNewOrderCup(oldOrderCupIndex, () =>
                            {
                                _unifyOrderCount += 1;
                                if (_unifyOrderCount >= _unifyOrderCups.Count)
                                {
                                    // if (type == 0)
                                    // {
                                    //     _unifyOrderCups = new List<CupControl>();
                                    //     _unifyOrderCupIndexs = new List<int>();
                                    //     RunStepPrepareCup(cb);
                                    // }
                                    // else
                                    // {
                                    //     cb();
                                    // }
                                    _unifyOrderCups = new List<CupControl>();
                                    _unifyOrderCupIndexs = new List<int>();
                                    RunStepPrepareCup(cb);
                                }
                            });
                        });
                    }, i * GlobalValueManager.OrderUnifyAccountTime, this.GetCancellationTokenOnDestroy());
                }
            }
        }

        #endregion
        
        #region 移动

        /// <summary>
        /// 移动到指定目标点(不需要动画同步)
        /// </summary>
        /// <param name="transTmp"></param>
        /// <param name="target"></param>
        /// <param name="maxDisDelta"></param>
        /// <param name="cb"></param>
        private void TransformMoveNoAni(Transform transTmp, Vector3 target, float maxDisDelta, Action cb)
        {
            var direction = target - transTmp.position;
            transTmp.position = Vector3.MoveTowards(transTmp.position, target, maxDisDelta);
            if (direction != Vector3.zero)
            {
                DelayTime.DelayFrame(() =>
                {
                    TransformMoveNoAni(transTmp, target, maxDisDelta, cb);
                }, 1, this.GetCancellationTokenOnDestroy());
            }
            else
            {
                cb();
            }
        }

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// 移动到指定目标点(动画同步)
        /// </summary>
        /// <param name="transTmp"></param>
        /// <param name="target"></param>
        /// <param name="frameNum"></param>
        /// <param name="cb"></param>
        private void TransformMove(Transform transTmp, Vector3 target, int frameNum, Action cb)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            _ = TransformMoveFun(transTmp, target, frameNum, cb);
        }
        
        async UniTask TransformMoveFun(Transform transTmp, Vector3 target, int frameNum, Action cb)
        {
            float speed = Vector3.Distance(transTmp.position, target) / frameNum;
            
            var num = 0;
            while (num < frameNum)
            {
                transTmp.Translate((target - transTmp.position).normalized * speed);
                num += 1;
                await UniTask.DelayFrame(1, cancellationToken: _cancellationTokenSource.Token);
            }

            transTmp.position = target;
            cb();
        }

        #endregion

        #region 使用道具

        /** 刷新订单杯计数器 */
        private int _refreshOrderCupCount;

        /// <summary>
        /// 使用道具 清空备料杯 播放清空重置动画
        /// </summary>
        public void OnClearPrepareCups()
        {
            _prepareCupObjAni.Play("PrepareOut");
        }

        /// <summary>
        /// 使用道具 刷新订单
        /// </summary>
        public void OnRefreshOrderCup()
        {
            _refreshOrderCupCount = 0;
            for (int i = 0; i < _orderCups.Length; i++)
            {
                var iTmp = i;
                DelayTime.DelayFrame(() =>
                {
                    if (_orderCups[iTmp])
                    {
                        _orderCups[iTmp].OrderCupChange(() =>
                        {
                            var orderCupPoint = _orderCupPoints[iTmp].position;
                            // 重置订单杯数据
                            _orderCups[iTmp].cupLogoId = Random.Range(0, OrderCupBodyLogos.Length);
                            _orderCups[iTmp].colorId = DataHelper.CupDatas[_orderCups[iTmp].orderNum][0];
                            _orderCups[iTmp].color = ToolFunManager.HexToColor(GlobalValueManager.ColorList[_orderCups[iTmp].colorId]);
                            _orderCups[iTmp].Init();
                            // 播放订单杯移进动画
                            _orderCups[iTmp].OrderCupIn(() =>
                            {
                                _refreshOrderCupCount += 1;
                                if (_refreshOrderCupCount >= orderCupNum)
                                {
                                    ConfigManager.Instance.ConsoleLog(0, "退出刷新订单道具使用状态...");
                                    itemUsing = false;
                                    // 执行备用杯检测是否可以倒进订单杯
                                    _unifyOrderCups = new List<CupControl>();
                                    _unifyOrderCupIndexs = new List<int>();
                                    RunStepPrepareCup(() => { });
                                }
                            });
                            // 延迟1帧 设置位置
                            DelayTime.DelayFrame(() =>
                            {
                                _orderCups[iTmp].transform.position = orderCupPoint;
                            }, 1, this.GetCancellationTokenOnDestroy());
                        });
                    }
                }, i * GlobalValueManager.OrderCupChangeTime, this.GetCancellationTokenOnDestroy());
            }
        }

        /// <summary>
        /// 使用道具 刷新原料瓶 播放刷新动画
        /// </summary>
        public void OnRefreshVessels()
        {
            // 播放闪光音特效
            _vesselsChangeStar.SetActive(false);
            _vesselsChangeStar.SetActive(true);
            AudioHandler._instance.PlayAudio(ChangeStarAudio);
            
            // 播放原料瓶动画
            for (int i = 0; i < _vessels.Count; i++)
            {
                for (int j = 0; j < _vessels[i].Count; j++)
                {
                    if (_vessels[i][j] != null)
                    {
                        _vessels[i][j].ChangeVessel();
                    }
                }
            }
        }

        #endregion

        #region 订单杯子

        /** 初始化订单杯子数量 */
        private int _initOrderNum;
        
        /// <summary>
        /// 初始化订单杯子
        /// </summary>
        async UniTask InitOrderCup()
        {
            _initOrderNum = 0;
            for (int i = 0; i < _orderCupPoints.Count; i++)
            {
                if (i < orderCupNum)
                {
                    await UniTask.DelayFrame(10, cancellationToken: _cancellationTokenSource.Token);
                    CreateNewOrderCup(i, () =>
                    {
                        _initOrderNum += 1;
                        if (_initOrderNum >= orderCupNum)
                        {
                            if (DataHelper.CurUserInfoData.guideCompleteList[0] == 0)
                            {
                                UiBattle._instance.RunGuideStep(false);
                                UiBattle._instance._uiGuide.RunGuideStep_1();
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 创建一个新的订单杯子
        /// </summary>
        /// <param name="index">订单杯所在的列表索引</param>
        /// <param name="cb">创建订单杯完成回调</param>
        private void CreateNewOrderCup(int index, Action cb)
        {
            if (_orderNum >= orderCompleteNumMax)
            {
                ConfigManager.Instance.ConsoleLog(1, "没有下一份订单了...");
                cb();
                return;
            }
            
            var type = 1;
            var cupAssetName = "CupL";
            LoadResources.XXResourcesLoad(cupAssetName, handleTmp =>
            {
                GameObject cupTmp = Instantiate(handleTmp, _orderCupPoints[index], false);
                cupTmp.name = "Cup";
                var orderCupPoint = _orderCupPoints[index].position;
                cupTmp.transform.position = Vector3.one * 10;
                var cupControl = cupTmp.GetComponent<CupControl>();

                cupControl.cupLogoId = Random.Range(0, OrderCupBodyLogos.Length);
                cupControl.type = type;
                cupControl.orderNum = _orderNum;
                cupControl.colorId = DataHelper.CupDatas[_orderNum][0];
                cupControl.color = ToolFunManager.HexToColor(GlobalValueManager.ColorList[cupControl.colorId]);
                cupControl.Init();
                _orderCups[index] = cupControl;

                // 播放订单杯移进动画
                cupControl.OrderCupIn(cb);
                // 延迟1帧 设置位置
                DelayTime.DelayFrame(() =>
                {
                    cupControl.transform.position = orderCupPoint;
                }, 1, this.GetCancellationTokenOnDestroy());
                
                _orderNum += 1;

            }, LoadResources.AssetsGroup.cup);
        }

        /// <summary>
        /// 杯子开始增加进度
        /// </summary>
        public void CupPourStart()
        {
            if (_cupType == 0)
            {
                var height = _orderCups[_curOrderCupIndex].nowHight;
                if (_orderCups[_curOrderCupIndex].type == 1)
                    height = height <= 0 ? 0.33f : height <= 0.33f ? 0.66f : height <= 0.66f ? 1f : height;
                else
                    height = height <= 0 ? 0.385f : height <= 0.385f ? 0.77f : height;
                _orderCups[_curOrderCupIndex].PourAddStart(height);
                DataHelper.CupDatas[_orderCups[_curOrderCupIndex].orderNum][1] += 1;
            }
            else
            {
                _prepareCups[_curPrepareCupIndex].SetColor(_curUseVessel.colorIds[_curStepIndex]);
                _prepareCups[_curPrepareCupIndex].PourAddStart(0.3f);
                DataHelper.PrepareCupDatas[_curPrepareCupIndex] = _curUseVessel.colorIds[_curStepIndex];
            }
        }

        /// <summary>
        /// 杯子进度增加完成
        /// </summary>
        public void CupPourEnd()
        {
            if (_cupType == 0)
            {
                var targetPointTmp = _orderCupPoints[_curOrderCupIndex].transform.position;
                var target = new Vector3(targetPointTmp.x, (targetPointTmp.y + 1.5f), targetPointTmp.z);
                _curStepIndex += 1;
                var nowHightTmp = _curUseVessel.nowHight;
                var targetHight = _orderCups[_curOrderCupIndex].type == 0 ? 0.77f : 1f;
                if (_orderCups[_curOrderCupIndex].nowHight >= targetHight)
                {
                    // 订单杯子倒满
                    // 订单完成 完成的订单杯子暂存到统一结算列表
                    _unifyOrderCups.Add(_orderCups[_curOrderCupIndex]);
                    _unifyOrderCupIndexs.Add(_curOrderCupIndex);
                    // 执行下一步骤
                    if (nowHightTmp > 0) RunStep(); // 当前原料瓶没用完 继续执行下一次
                    else VesselUseEnd(target, () => { }); // 当前原料瓶已经用完 移走空原料瓶子 空原料瓶移走完成后执行备料杯倒入订单杯
                }
                else
                {
                    // 订单杯子未倒满
                    if (nowHightTmp > 0) RunStep(); // 当前原料瓶没用完 继续执行下一次
                    else VesselUseEnd(target, () => { }); // 当前原料瓶已经用完 移走空原料瓶子 空原料瓶移走完成后执行备料杯倒入订单杯
                }
            }
            else
            {
                var targetPointTmp = _prepareCups[_curPrepareCupIndex].transform.position;
                var target = new Vector3(targetPointTmp.x, (targetPointTmp.y + 1f), targetPointTmp.z);
                _curStepIndex += 1;
                if (_curUseVessel.nowHight > 0) RunStep(); // 当前原料瓶没用完 继续执行下一次
                else VesselUseEnd(target, () => { }); // 当前原料瓶已经用完 移走空原料瓶子 空原料瓶移走完成后执行备料杯倒入订单杯
            }
        }

        #endregion

        #region 备料杯子
        
        /** 当前备料杯子索引坐标(i) */
        private int _curPrepareCupIndex_i;
        /** 当前备料杯子索引坐标(j) */
        private int _curPrepareCupIndex_j;
        /** 当前备料杯子倒完回调 */
        private Action _curCallBack;
        /** 备料杯步骤正在进行中 */
        private bool _isStepPrepareCupRunning;

        /// <summary>
        /// 初始化备料杯子
        /// </summary>
        public void InitPrepareCups()
        {
            for (int i = 0; i < _prepareCups.Count; i++)
            {
                if (i < prepareCupNum)
                {
                    _prepareCupsB[i].SetActive(true);
                    _prepareCups[i].gameObject.SetActive(true);
                    _prepareCups[i].Init();
                }
                else
                {
                    _prepareCupsB[i].SetActive(false);
                    _prepareCups[i].gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 执行步骤 将备料杯中的倒入订单杯
        /// </summary>
        /// <param name="cb">步骤执行完成回调</param>
        private void RunStepPrepareCup(Action cb)
        {
            _isStepPrepareCupRunning = true;
            var iTmp = -1;
            var jTmp = -1;
            for (int i = 0; i < _prepareCups.Count; i++)
            {
                if (i < prepareCupNum)
                {
                    if (_prepareCups[i].nowHight >= 0.3f)
                    {
                        var isBreak = false;
                        for (int j = 0; j < _orderCups.Length; j++)
                        {
                            if (_orderCups[j] == null) continue;
                            if (_prepareCups[i].colorId == _orderCups[j].colorId && _orderCups[j].nowHight < 1)
                            {
                                iTmp = i;
                                jTmp = j;
                                isBreak = true;
                                break;
                            }
                        }

                        if (isBreak) break;
                    }
                }
            }

            if (iTmp != -1 && jTmp != -1)
            {
                var targetPointTmp = _orderCupPoints[jTmp].transform.position;
                var target = new Vector3((targetPointTmp.x), (targetPointTmp.y + 1.5f), targetPointTmp.z);
                TransformMove(_prepareCups[iTmp].transform, target, 10, () =>
                {
                    _prepareCups[iTmp].PourOutStart(0);
                    _curPrepareCupIndex_i = iTmp;
                    _curPrepareCupIndex_j = jTmp;
                    _curCallBack = cb;
                });
            }
            else
            {
                UnifyOrderCupAccount(1, () =>
                {
                    cb();
                    _isStepPrepareCupRunning = false;
                    // 新手引导
                    RunGuideStepUseItem();
                });
            }
        }

        /// <summary>
        /// 备料杯子开始倒
        /// </summary>
        public void PrepareCupPourStart()
        {
            var height = _orderCups[_curPrepareCupIndex_j].nowHight;
            if (_orderCups[_curPrepareCupIndex_j].type == 1)
                height = height <= 0 ? 0.33f : height <= 0.33f ? 0.66f : height <= 0.66f ? 1f : height;
            else
                height = height <= 0 ? 0.385f : height <= 0.385f ? 0.77f : height;
            _orderCups[_curPrepareCupIndex_j].PourAddStart(height);
        }

        /// <summary>
        /// 备料杯子重置
        /// </summary>
        public void PrepareCupPourReSet()
        {
            var targetPoint = _prepareCupPoints[_curPrepareCupIndex_i].position;
            TransformMoveNoAni(_prepareCups[_curPrepareCupIndex_i].transform, targetPoint, 0.2f, () =>
            {
                _prepareCups[_curPrepareCupIndex_i].Init();
                // 重置备料杯数据
                DataHelper.PrepareCupDatas[_curPrepareCupIndex_i] = 0;
                // 订单数据刷新
                DataHelper.CupDatas[_orderCups[_curPrepareCupIndex_j].orderNum][1] += 1;
                
                var targetHight = _orderCups[_curPrepareCupIndex_j].type == 0 ? 0.77f : 1f;
                if (_orderCups[_curPrepareCupIndex_j].nowHight >= targetHight)
                {
                    // 订单杯子倒满
                    // 订单完成 完成的订单杯子暂存到统一结算列表
                    _unifyOrderCups.Add(_orderCups[_curPrepareCupIndex_j]);
                    _unifyOrderCupIndexs.Add(_curPrepareCupIndex_j);
                    // 执行下一步骤
                    RunStepPrepareCup(_curCallBack);
                }
                else
                {
                    // 订单杯子未倒满
                    RunStepPrepareCup(_curCallBack);
                }
            });
        }

        #endregion

        #region 原料瓶

        /** 原料瓶列表 */
        private List<List<VesselControl>> _vessels;
        /** 自动递补的原料瓶列表 */
        private List<VesselControl> _supplyLineVessels;
        /** 自动递补的原料瓶所在的列 追加计数器 */
        private List<int> _supplyLineNums;
        /** 回收增加的原料瓶数量 */
        private int _addVesselNum;
        /** 复活状态码 */
        private bool _reviveStatus;
        
        /// <summary>
        /// 初始化原料瓶
        /// </summary>
        private void InitVessels()
        {
            _vessels = new List<List<VesselControl>>();
            for (int i = 0; i < 5; i++)
            {
                var vessels = new List<VesselControl>();
                for (int j = 0; j < 7; j++)
                {
                    vessels.Add(null);
                }

                _vessels.Add(vessels);
            }
            
            _supplyLineNums = new List<int>(7) { 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < DataHelper.VesselDatas.Count; i++)
            {
                if (i >= 28) continue;
                var iTmp = i / 7;
                var jTmp = i % 7;
                DataHelper.VesselStatus[i] = true;
                CreateNewVessel(_vesselPoints[iTmp][jTmp], DataHelper.VesselDatas[i], new[] { iTmp, jTmp }, i);
                _supplyLineNums[jTmp] += 1;
            }
        }

        /// <summary>
        /// 创建一个新的原料瓶
        /// <param name="root">挂载节点</param>
        /// </summary>
        private void CreateNewVessel(Transform root, List<int> colors, int[] indexs, int vesselNum)
        {
            LoadResources.XXResourcesLoad("Vessel", handleTmp =>
            {
                GameObject vesselTmp = Instantiate(handleTmp, root, false);
                if (indexs.Length == 3)
                    vesselTmp.name = new StringBuilder("Vessel_" + indexs[2] + "_" + indexs[1]).ToString();
                else
                    vesselTmp.name = new StringBuilder("Vessel_" + indexs[0] + "_" + indexs[1]).ToString();

                var vesselControl = vesselTmp.GetComponent<VesselControl>();
                vesselControl.colorIds = new List<int>(3) { colors[0], colors[1], colors[2] };
                var colorTmpA = colors[0] == 0 ? Color.white : ToolFunManager.HexToColor(GlobalValueManager.ColorList[vesselControl.colorIds[0]]);
                var colorTmpB = colors[1] == 0 ? Color.white : ToolFunManager.HexToColor(GlobalValueManager.ColorList[vesselControl.colorIds[1]]);
                var colorTmpC = colors[2] == 0 ? Color.white : ToolFunManager.HexToColor(GlobalValueManager.ColorList[vesselControl.colorIds[2]]);
                vesselControl.colorA = colorTmpA;
                vesselControl.colorB = colorTmpB;
                vesselControl.colorC = colorTmpC;
                vesselControl.vesselId = vesselNum;
                var zeroNum = 0;
                for (int i = 0; i < colors.Count; i++)
                {
                    if (colors[i] == 0) zeroNum += 1;
                }

                vesselControl.Init(zeroNum);
                
                _vessels[indexs[0]][indexs[1]] = vesselControl;

            }, LoadResources.AssetsGroup.cup);
        }

        /// <summary>
        /// 原料瓶用完
        /// </summary>
        /// <param name="point">用完时所在的位置</param>
        /// <param name="cb">回调</param>
        private void VesselUseEnd(Vector3 point, Action cb)
        {
            // 拆分出当前使用的原料瓶列表坐标
            var indexsTmp = _curUseVessel.gameObject.name.Split('_');
            var index_i = int.Parse(indexsTmp[1]);
            var index_j = int.Parse(indexsTmp[2]);
            ConfigManager.Instance.ConsoleLog(0, "i = " + index_i + " , j = " + index_j);
            // 刷新当前使用的原料瓶数据
            DataHelper.VesselDatas[_curUseVessel.vesselId][0] = 0;
            DataHelper.VesselDatas[_curUseVessel.vesselId][1] = 0;
            DataHelper.VesselDatas[_curUseVessel.vesselId][2] = 0;

            // 判断是否在队列尾追加原料瓶
            var noUseEndNum = DataHelper.GetNoUseEndVesselNum();
            ConfigManager.Instance.ConsoleLog(0, new StringBuilder("未用完的原料瓶 = " + noUseEndNum).ToString());
            if (noUseEndNum >= 28)
            {
                // 未用完的原料瓶还有28个 需要在队列尾部追加1个原料瓶
                var canUseVesselData = DataHelper.GetCanUseVesselData();
                var canUseVessel = new List<int>(3) { canUseVesselData[1], canUseVesselData[2], canUseVesselData[3] };
                DataHelper.VesselStatus[canUseVesselData[0]] = true;
                var indexI = _supplyLineNums[index_j];
                CreateNewVessel(_vesselPoints[4][index_j], canUseVessel, new[] { 4, index_j, indexI }, canUseVesselData[0]);
                _supplyLineNums[index_j] += 1;
            }

            // 找出当前使用的原料瓶所在列的所有原料瓶
            _supplyLineVessels = new List<VesselControl>();
            for (int i = 0; i < _vessels.Count; i++)
            {
                if (_vessels[i][index_j] == null) continue;
                _supplyLineVessels.Add(_vessels[i][index_j]);
            }

            ConfigManager.Instance.ConsoleLog(0, _supplyLineVessels.Count.ToString());
            
            // 用完的原料瓶移走
            var target = new Vector3(point.x + 10, point.y, point.z);
            DelayTime.DelayFrame(() =>
            {
                AudioHandler._instance.PlayAudio(MoveAudio);
                TransformMoveNoAni(_curUseVessel.transform, target, 0.5f, () =>
                {
                    Destroy(_curUseVessel.gameObject); // 回收用完的原料瓶
                    _curUseVessel = null; // 用完的原料瓶记录置空

                    // 空原料瓶移走以后延迟1帧 刷新原料瓶列表
                    DelayTime.DelayFrame(() =>
                    {
                        for (int i = 0; i < _vessels.Count; i++)
                        {
                            for (int j = 0; j < _vessels[i].Count; j++)
                            {
                                if (_vessels[i][j] == null)
                                {
                                    if (i != _vessels.Count - 1)
                                    {
                                        _vessels[i][j] = _vessels[i + 1][j];
                                        _vessels[i + 1][j] = null;
                                    }
                                }
                            }
                        }
                        
                        // 重置流程控制器
                        _curStepIndex = -1;
                        
                        // 复活时的那瓶原料用完
                        if (_reviveStatus)
                        {
                            _reviveStatus = false;
                            EventAddVesselFun();
                        }
                        
                    }, 1, this.GetCancellationTokenOnDestroy());
                });
            }, 1, this.GetCancellationTokenOnDestroy());

            // 自动递补
            for (int i = 1; i < _supplyLineVessels.Count; i++)
            {
                var iTmp = i;
                TransformMoveNoAni(_supplyLineVessels[i].transform, _vesselPoints[i - 1][index_j].position, 0.2f,
                    () =>
                    {
                        // 播放原料瓶移动完成抖动
                        _supplyLineVessels[iTmp].MoveSupplyComplete();
                    });
            }

            // 统一结算
            UnifyOrderCupAccount(0, () => { });
        }

        /// <summary>
        /// 全局事件增加原料瓶回调
        /// </summary>
        /// <param name="addNum"></param>
        private void EventAddVessel(int addNum)
        {
            if (addNum <= 0) return;
            _addVesselNum = addNum;
            if (prepareCupClearType == 0)
            {
                EventAddVesselFun();
            }
        }

        /// <summary>
        /// 全局事件增加原料瓶回调执行方法
        /// </summary>
        private void EventAddVesselFun()
        {
            var addNumTmp = _addVesselNum;
            for (int i = 0; i < _vessels.Count; i++)
            {
                if (i == _vessels.Count - 1) break;
                for (int j = 0; j < _vessels[i].Count; j++)
                {
                    if (_vessels[i][j] == null)
                    {
                        var canUseVesselData = DataHelper.GetCanUseVesselData();
                        var canUseVessel = new List<int>(3) { canUseVesselData[1], canUseVesselData[2], canUseVesselData[3] };
                        DataHelper.VesselStatus[canUseVesselData[0]] = true;
                        var indexI = _supplyLineNums[j];
                        CreateNewVessel(_vesselPoints[i][j], canUseVessel, new[] { i, j, indexI }, canUseVesselData[0]);
                        _supplyLineNums[j] += 1;

                        addNumTmp -= 1;
                        if (addNumTmp <= 0) break;
                    }
                }

                if (addNumTmp <= 0) break;
            }
        }

        /** 执行新手引导使用道具 */
        private void RunGuideStepUseItem()
        {
            if (DataHelper.CurUserInfoData.guideCompleteList[2] == 0)
            {
                // 引导使用道具 清空备料杯
                var num = GetPrepareCupFullNum();
                if (num >= prepareCupNum)
                {
                    UiBattle._instance.RunGuideStep(false);
                    UiBattle._instance._uiGuide.RunGuideStep_3_1();
                }
            }
            else
            {
                if (DataHelper.CurUserInfoData.guideCompleteList[3] == 0)
                {
                    // 引导使用道具 刷新订单杯
                    var num = GetPrepareCupFullNum();
                    if (num >= prepareCupNum)
                    {
                        UiBattle._instance.RunGuideStep(false);
                        UiBattle._instance._uiGuide.RunGuideStep_4_1();
                    }
                }
                else
                {
                    if (DataHelper.CurUserInfoData.guideCompleteList[4] == 0)
                    {
                        // 引导使用道具 刷新原料瓶
                        var num = GetPrepareCupFullNum();
                        if (num >= prepareCupNum)
                        {
                            UiBattle._instance.RunGuideStep(false);
                            UiBattle._instance._uiGuide.RunGuideStep_5_1();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取满备料杯的数量
        /// </summary>
        private int GetPrepareCupFullNum()
        {
            var num = 0;
            for (int i = 0; i < DataHelper.PrepareCupDatas.Count; i++)
            {
                if (DataHelper.PrepareCupDatas[i] != 0)
                {
                    num += 1;
                }
            }

            return num;
        }

        #endregion

        #region 解锁新的订单/备料杯子

        /// <summary>
        /// 刷新未解锁的新杯子UI
        /// </summary>
        private void InitUnlockCupsUi()
        {
            // 订单杯
            // 4号订单杯
            if (DataHelper.CurUserInfoData.cupUnlockStatus[0] == 0)
            {
                _unlockCup3DUis[0].gameObject.SetActive(true);
                _unlockCup3DUis[0].SetUnLockType(unlockShareUsed ? 0 : 1);
            }
            else
                _unlockCup3DUis[0].gameObject.SetActive(false);
            // 5号订单杯
            if (DataHelper.CurUserInfoData.cupUnlockStatus[0] == 1 && DataHelper.CurUserInfoData.cupUnlockStatus[1] == 0)
            {
                _unlockCup3DUis[1].gameObject.SetActive(true);
                _unlockCup3DUis[1].SetUnLockType(unlockShareUsed ? 0 : 1);
            }
            else
                _unlockCup3DUis[1].gameObject.SetActive(false);

            // 备料杯
            // 5号备料杯
            if (DataHelper.CurUserInfoData.cupUnlockStatus[2] == 0)
            {
                _unlockCup3DUis[2].gameObject.SetActive(true);
                _unlockCup3DUis[2].SetUnLockType(unlockShareUsed ? 0 : 1);
            }
            else
                _unlockCup3DUis[2].gameObject.SetActive(false);

            // 6号备料杯
            if (DataHelper.CurUserInfoData.cupUnlockStatus[2] == 1 && DataHelper.CurUserInfoData.cupUnlockStatus[3] == 0)
            {
                _unlockCup3DUis[3].gameObject.SetActive(true);
                _unlockCup3DUis[3].SetUnLockType(unlockShareUsed ? 0 : 1);
            }
            else
                _unlockCup3DUis[3].gameObject.SetActive(false);
        }

        /// <summary>
        /// 刷新未解锁的杯子
        /// </summary>
        /// <param name="typeTmp">解锁杯子类型 0: 订单杯 1: 备料杯</param>
        /// <param name="idTmp">解锁杯子列表索引</param>
        public void RefreshUnlockCups(int typeTmp, int idTmp)
        {
            if (typeTmp == 0)
            {
                orderCupNum += 1;
                var indexTmp = idTmp == 0 ? 3 : 4;
                CreateNewOrderCup(indexTmp, () =>
                {
                    // 执行备用杯检测是否可以倒进订单杯
                    _unifyOrderCups = new List<CupControl>();
                    _unifyOrderCupIndexs = new List<int>();
                    RunStepPrepareCup(() => { });
                });
            }
            else
            {
                prepareCupNum += 1;
                var indexTmp = idTmp == 2 ? 4 : 5;
                _prepareCupsB[indexTmp].SetActive(true);

                var positionTmp = _prepareCups[indexTmp].transform.position;
                _prepareCups[indexTmp].transform.position = new Vector3(positionTmp.x + 3, positionTmp.y, positionTmp.z);
                _prepareCups[indexTmp].gameObject.SetActive(true);
                _prepareCups[indexTmp].Init();
                DelayTime.DelayFrame(() =>
                {
                    TransformMoveNoAni(_prepareCups[indexTmp].transform, positionTmp, 0.125f, () => { });
                }, 1, this.GetCancellationTokenOnDestroy());
            }

            // 刷新未解锁的新杯子UI
            InitUnlockCupsUi();
        }

        #endregion

        #region 复活

        /// <summary>
        /// 复活
        /// </summary>
        public void OnRevive()
        {
            _reviveStatus = true;
            DelayTime.DelayFrame(RunStep, 2, this.GetCancellationTokenOnDestroy());
        }

        #endregion

        #region 补丁

        /// <summary>
        /// 检测是否点击在UI上
        /// </summary>
        /// <param name="pos">点击位置</param>
        private bool IsOnUI(Vector2 pos)
        {
            // 通过当前场景中活跃的EventSystem实例 获取输入事件的数据
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                // 设置检测的点坐标
                position = pos
            };
            // 检测到的对象列表
            List<RaycastResult> results = new List<RaycastResult>();
            // 传入的点坐标检测到的所有物体填充到列表
            EventSystem.current.RaycastAll(pointerData, results);
            // 如果没检测到任何物体则返回false
            if (results.Count < 1) return false;
            // 检测到了的第一个物体的layer为UI 则代表检测到了UI 返回true
            if (results[0].gameObject.layer == LayerMask.NameToLayer("UI")) return true;
            // 检测不是UI 返回false
            return false;
        }

        #endregion
    }
}