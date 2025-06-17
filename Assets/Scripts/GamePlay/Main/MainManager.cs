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
using Data.ClassData;
using GamePlay.Globa;
using GamePlay.Main.Guide;
using GamePlay.Module.Call;
using GamePlay.Module.ClockIn;
using GamePlay.Module.Follow;
using GamePlay.Module.InternalPage.PageRank;
using GamePlay.Module.OpenBox;
using GamePlay.Module.PopMassage;
using GamePlay.Module.Round.Raffle;
using GamePlay.Module.Round.Sign;
using GamePlay.Module.Round.Task;
using GamePlay.Module.Set;
using Newtonsoft.Json;
using Platform;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Main
{
    public class MainManager : MonoBehaviour
    {
        /** 游戏主管理类 */
        public static MainManager _instance;

        /** 音效 摄像机移动 */
        public AudioClip audioCamMove;
        /** 音效 装备部件 */
        public AudioClip audioEquip;
        /** 音效 属性升级 */
        public AudioClip audioTrainLvUp;
        /** 音效 打开页面(仅主页面) */
        public AudioClip audioPageClose;
        /** 音效 打开页面(除主页面外) */
        public AudioClip audioPageOpen;
        /** 音效 升级部件 */
        public AudioClip audioEquipmentLvUp;
        /** 音效 转盘抽奖 */
        public AudioClip audioRaffle;
        /** 音效 涂装 */
        public AudioClip audioPaint;

        /** 提示红点管理类 */
        internal RedPointManager _redPointManager;
        
        /** 主摄像机父节点 */
        private Transform _mainCameraTran;
        /** 主摄像机动画组件 */
        private Animator mainCamAni;
        /** UI摄像机 */
        internal Camera uiCamera;
        /** UICanvas */
        internal RectTransform _uiCanvasRect;
        /** 宝箱摄像机 */
        internal Camera mainBoxCam;
        /** 宝箱3DUI摄像机 */
        internal Camera mainBoxUi3DCam;

        /** 排行榜渲染Canvas */
        private GameObject _canvasRank;
        /** 排行榜渲染节点 */
        internal RawImage rankBody;
        /** 排行榜Canvas缩放组件 */
        internal CanvasScaler scaler;

        /** 页面名称 */
        private readonly string[] _internalPageNames = { "PageTrain", "PageShop", "PageMain", "PageBuild", "PageRank" };
        /** 菜单栏未选中状态列表 */
        private readonly GameObject[] _menuOffUis = new GameObject[5];
        /** 菜单栏选中状态列表 */
        private readonly GameObject[] _menuOnUis = new GameObject[5];
        /** 页面列表 */
        private readonly InternalPageScript[] _internalPageUis = new InternalPageScript[5];
        /** 提示红点列表 */
        private readonly GameObject[] _redPoints = new GameObject[5];

        /** 弹窗 设置 */
        private OpenSetPageUi _openSetPageUi;
        /** 弹窗 收藏 */
        private OpenFollowPageUi _openFollowPageUi;
        /** 弹窗 邀请 */
        private OpenCallPageUi _openCallPageUi;
        /** 弹窗 开箱 */
        private OpenBoxPageUi _openBoxPageUi;
        /** 弹窗 签到 */
        private OpenSignPageUi _openSignPageUi;
        /** 弹窗 转盘 */
        private OpenRafflePageUi _openRafflePageUi;
        /** 弹窗 任务 */
        private OpenTaskPageUi _openTaskPageUi;
        /** 弹窗 地标打卡 */
        private OpenClockInPageUi _openClockInPageUi;
        /** 弹窗 订阅 */
        private OpenPopMassageUi _openPopMassageUi;
        /** 弹窗 GM */
        private OpenPopGm _openPopGmUi;

        /** 飞机挂载节点 */
        private Transform _planePoint;
        /** 飞机挂载节点动画组件 */
        private Animation _planePointAni;

        /** 飞机组装控制器 */
        private MainPlaneCreate _mainPlaneCreate;

        /** 排行榜渲染摄像机总节点 */
        private GameObject _rankCamObj;
        /** 排行榜渲染飞机挂载节点 */
        internal readonly Transform[] _rankPlaneTrans = new Transform[3];
        /** 排行榜飞机控制器 */
        internal readonly RankPlaneMgr[] _rankPlaneMgr = new RankPlaneMgr[3];

        /** 宝箱渲染摄像机总节点 */
        private GameObject _boxCamObj;
        /** 宝箱渲染挂载节点 */
        private readonly Transform[] _boxTrans = new Transform[5];

        /** 宝箱列表 */
        private readonly Animator[] _boxAnis = new Animator[5];

        /** 页面挂载节点 */
        private Transform _pageTran;
        /** 弹窗挂载节点 */
        private Transform _popTran;
        
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;
        
        private static readonly int Trigger_Main = Animator.StringToHash("Main");
        private static readonly int Trigger_Body = Animator.StringToHash("Body");
        private static readonly int Trigger_WingL = Animator.StringToHash("WingL");
        private static readonly int Trigger_WingR = Animator.StringToHash("WingR");
        private static readonly int Trigger_Propeller = Animator.StringToHash("Propeller");
        private static readonly int Trigger_Fin = Animator.StringToHash("Fin");
        private static readonly int Trigger_Spurt = Animator.StringToHash("Spurt");
        private static readonly int Trigger_Color = Animator.StringToHash("Color");

        /** 一阶段新手引导组件 */
        internal GuideMain1 _guideMain1;
        
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
            
            GameObject.Find("/Scene").GetComponent<GlobaMaterialSet>().SetBend(true);
            
            // 提示红点管理类
            _redPointManager = gameObject.AddComponent<RedPointManager>();
            _redPointManager.Initial();

            Transform uiCanvasRect = GameObject.Find("/CanvasUi2D").transform;
            _uiCanvasRect = uiCanvasRect.GetComponent<RectTransform>();

            // 主摄像机
            _mainCameraTran = GameObject.Find("/CamMain").transform;
            mainCamAni = _mainCameraTran.GetComponent<Animator>();
            
            // UI摄像机
            uiCamera = GameObject.Find("/CamUi2D").GetComponent<Camera>();
            
            // 飞机挂载节点
            _planePoint = GameObject.Find("/Plane_Me").transform;
            _planePointAni = _planePoint.Find("PlaneModle").GetComponent<Animation>();
            _mainPlaneCreate = gameObject.AddComponent<MainPlaneCreate>();
            
            // 排行榜渲染摄像机总节点
            _rankCamObj = GameObject.Find("/RankCam");
            _rankCamObj.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                _rankPlaneTrans[i] = _rankCamObj.transform.Find("CamTop" + (i + 1) + "/PlaneTmp/PlaneModle");
                _rankPlaneMgr[i] = _rankPlaneTrans[i].gameObject.AddComponent<RankPlaneMgr>();
            }
            
            // 宝箱渲染摄像机总节点
            _boxCamObj = GameObject.Find("/Box");
            mainBoxCam = _boxCamObj.transform.Find("CamBox").GetComponent<Camera>();
            mainBoxUi3DCam = mainBoxCam.transform.Find("CamUi3D_MainBox").GetComponent<Camera>();
            _boxCamObj.SetActive(false);
            for (int i = 0; i < 5; i++)
            {
                _boxTrans[i] = _boxCamObj.transform.Find("Point_Box" + (i + 1));
            }
            
            // 菜单栏/页面
            for (int i = 0; i < 5; i++)
            {
                // 菜单栏
                GameObject menuOff = uiCanvasRect.Find("Menu/Label" + (i + 1) + "Off").gameObject;
                GameObject menuOn = uiCanvasRect.Find("Menu/Label" + (i + 1) + "On").gameObject;
                _menuOffUis[i] = menuOff;
                _menuOnUis[i] = menuOn;
                int index = i;
                Button btnMenu = menuOff.AddComponent<Button>();
                btnMenu.transition = Selectable.Transition.None;
                btnMenu.onClick.AddListener(() =>
                {
                    AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
                    OnBtnMenu(index, 0);
                });
                // 提示红点
                GameObject redPoint = menuOff.transform.Find("RedPoint").gameObject;
                _redPoints[i] = redPoint;
            }

            _pageTran = uiCanvasRect.Find("Page");
            _popTran = uiCanvasRect.Find("Pop");
            
            // 场景中的页面 主页面
            GameObject mainPage = _pageTran.Find("Main").gameObject;
            InternalPageScript mainPageUi = mainPage.GetComponent<InternalPageScript>();
            mainPageUi.Initial();
            _internalPageUis[2] = mainPageUi;
        }

        private void Start()
        {
            LoadPage(_internalPageNames[3], 3, () => { }); // 加载组装页面
            LoadPage(_internalPageNames[4], 4, () => { });  // 加载排行页面
            LoadPage(_internalPageNames[1], 1, () => { });  // 加载商店页面
            LoadPage(_internalPageNames[0], 0, () => { }); // 加载升级页面

            if (DataHelper.CurUserInfoData.isNewUser == 0)
            {
                // 需要执行新手引导 弹窗不可能被打开 无需加载弹窗
                // 新手引导预先给一个宝箱 宝箱打开时间设置为当前时间 + 宝箱打开持续时间 + 额外一分钟
                long boxTime = ToolFunManager.GetCurrTime() - ConfigManager.Instance.RewardBoxConfigDict[100].OpenTime * 60 - 60;
                List<string> modifyKeys = new List<string>();
                DataHelper.CurUserInfoData.boxList[0] = new[] { "100", boxTime.ToString() };
                modifyKeys.Add("boxsList");
                DataHelper.CurUserInfoData.equipEquipments = new List<int>(GlobalValueManager.InitEquipments);
                modifyKeys.Add("equipEquipments");
                DataHelper.CurUserInfoData.equipments = new Dictionary<int, int>();
                for (int i = 0; i < GlobalValueManager.InitEquipments.Count; i++)
                {
                    DataHelper.CurUserInfoData.equipments.Add(GlobalValueManager.InitEquipments[i], 1);
                }
                modifyKeys.Add("equipments");
                DataHelper.ModifyLocalData(modifyKeys, () => { });
                // 加载新手引导
                LoadGuideMain_1();
            }
            else
            {
                // 不需要执行新手引导 正常游戏模式 加载弹窗
                _guideMain1 = null;
                LoadPop("PopSet", () => { });     // 加载弹窗 设置
                LoadPop("PopOpenBox", () => { }); // 加载弹窗 开箱
                LoadPop("PopCall", () => { });    // 加载弹窗 邀请
                LoadPop("PopFollow", () => { });  // 加载弹窗 收藏
                LoadPop("PopTask", () => { });    // 加载弹窗 任务
                LoadPop("PopClockIn", () => { }); // 加载弹窗 地标打卡
                if (DataHelper.CurUserInfoData.feedSubGet != 2) LoadPop("PopMassage", () => { }); // 加载弹窗 订阅

                // 自动弹窗 签到
                void AutoPopSign()
                {
                    SignInfoData signInfoData = JsonConvert.DeserializeObject<SignInfoData>(DataHelper.CurUserInfoData.signInfo);
                    if (signInfoData.day <= 7)
                    {
                        LoadPop("PopSign", () =>
                        {
                            if (!DataHelper.SignAutoPop)
                            {
                                DataHelper.SignAutoPop = true;
                                if (_redPointManager.GetRedPoint_Sign()) { OnOpenPop_Sign(true); }
                            }
                        });
                    }
                }

                if (DataHelper.isRootLoad)
                {
                    // 本次进入主场景是冷启动
                    switch (DataHelper.CurLaunchSceneId)
                    {
                        case -1: // 默认(非抖音直玩)
                            AutoPopSign();
                            break;
                        case 1: // 离线收益场景 转盘
                            LoadPop("PopRaffle", () => { OnOpenPop_Raffle(true); });
                            break;
                        case 2: // 体力恢复场景 签到
                            SignInfoData signInfoData = JsonConvert.DeserializeObject<SignInfoData>(DataHelper.CurUserInfoData.signInfo);
                            if (signInfoData.day <= 7)
                            {
                                // 签到未全部签完
                                LoadPop("PopSign", () => { OnOpenPop_Sign(true); });
                            }
                            break;
                        case 3: // 重要事件场景 开箱
                            break;
                    }
                }
                else
                {
                    // 本次进入主场景不是冷启动
                    LoadPop("PopRaffle", () => { });  // 加载弹窗 转盘
                    AutoPopSign();
                }
            }

            _cancellationToken = new CancellationTokenSource();
            _mainPlaneCreate.CreatePlane(_planePointAni.transform);
            
            DataHelper.CurRunScene = "Main";
            GameGlobalManager._instance.SetCanvasUiMain();
            GameGlobalManager._instance.SetCanvasUiMainOpenBox();
            if (DataHelper.isRootLoad)
            {
                DataHelper.isRootLoad = false;
                GameRootLoad.Instance.EndLoad(0, () => { });
            }
            else
            {
                GameGlobalManager._instance.ShowLoadSceneEnd(0, () => { });
            }
            
            AudioHandler._instance.PlayBGM(DataHelper.GetCurSceneBgmName());

            RefreshInfo();

            SetMainCamAniTrigger(-1);

            _startRefreshBox = false;
            _initPlaneRot = _planePoint.transform.localRotation;

            OnBtnMenu(2, 1);
            RefreshRedPoint(-1);
            
            RefreshBox();
            
            // 获取用户信息
            GetUserInfo(() => { }, false);
        }

        private void OnEnable()
        {
            EventManager<int>.Add(CustomEventType.RefreshRedPoint, RefreshRedPoint);
            EventManager<EnumButtonSign, Vector2>.Add(EnumButtonType.TouchScreenDown, TouchDown_RotPlane);
            EventManager<EnumButtonSign, Vector2>.Add(EnumButtonType.TouchScreenDrag, TouchDrag_RotPlane);
            EventManager<EnumButtonSign, Vector2>.Add(EnumButtonType.TouchScreenUp, TouchUp_RotPlane);
        }

        private void OnDisable()
        {
            EventManager<int>.Remove(CustomEventType.RefreshRedPoint, RefreshRedPoint);
            EventManager<EnumButtonSign, Vector2>.Remove(EnumButtonType.TouchScreenDown, TouchDown_RotPlane);
            EventManager<EnumButtonSign, Vector2>.Remove(EnumButtonType.TouchScreenDrag, TouchDrag_RotPlane);
            EventManager<EnumButtonSign, Vector2>.Remove(EnumButtonType.TouchScreenUp, TouchUp_RotPlane);
            
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
            _cancellationTokenRot?.Cancel();
            _cancellationTokenRot?.Dispose();
            _cancellationTokenRot = null;
        }
        
        private void Update()
        {
            GMPlayer();
        }

        #region 作弊

        private void GMPlayer()
        {
            if (Input.GetKeyDown(KeyCode.F11))
            {
                DataHelper.CurUserInfoData.gold += 100000;
                DataHelper.ModifyLocalData(new List<string>(1) { "gold" }, () => { });
                EventManager.Send(CustomEventType.RefreshMoney);
            }
            
            if (Input.GetKeyDown(KeyCode.F12))
            {
                DataHelper.CurUserInfoData.diamond += 100;
                DataHelper.ModifyLocalData(new List<string>(1) { "diamond" }, () => { });
                EventManager.Send(CustomEventType.RefreshMoney);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                DataHelper.CurUserInfoData.landMarkInfo.Add(2, 0);
                DataHelper.ModifyLocalData(new List<string>(1) { "landMarkInfo" }, () => { });
                EventManager<int>.Send(CustomEventType.RefreshRedPoint, 2);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                DataHelper.CurUserInfoData.curLevelNum = 2;
            }
        }

        #endregion

        /// <summary>
        /// 刷新页面常驻信息
        /// </summary>
        private void RefreshInfo()
        {
        }
        
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="cb">回调</param>
        /// <param name="isClick">用户主动点击触发</param>
        private void GetUserInfo(Action cb, bool isClick)
        {
            DataHelper.GetUserInfoClick = isClick;
            if (DataHelper.CurUserInfoData.userName == GameSdkManager._instance._serverScript.GetUserInfoJudgeKey() || 
                DataHelper.CurUserInfoData.userAvatar == GameSdkManager._instance._serverScript.GetUserInfoJudgeKey())
            {
                // 用户信息未获取 获取用户信息
                GameSdkManager._instance._sdkScript.RequirePrivacyAuthorize(cb, () => { });
            }
            else
            {
                // 用户信息已获取 直接回调成功
                cb();
            }
        }

        /// <summary>
        /// 刷新提示红点
        /// <param name="index">提示红点索引</param>
        /// </summary>
        private void RefreshRedPoint(int index)
        {
            bool[] redPoints = _redPointManager.GetMenuRedPoint();
            for (int i = 0; i < _redPoints.Length; i++)
            {
                _redPoints[i].SetActive(redPoints[i]);
            }

            // if (!redPoints[2])
            // {
            //     // 主页面提示红点不显示 判断直玩订阅奖励是否需要领取
            //     GameSdkManager._instance._sdkScript.CheckFeedSubscribeStatus(() => { _redPoints[2].SetActive(true); }, () => { });
            // }

            // if (index == -1)
            // {
            //     bool[] redPoints = _redPointManager.GetMenuRedPoint();
            //     for (int i = 0; i < _redPoints.Length; i++)
            //     {
            //         _redPoints[i].SetActive(redPoints[i]);
            //     }
            // }
            // else
            // {
            //     switch (index)
            //     {
            //         case 0:
            //             break;
            //         case 1:
            //             _redPoints[index].SetActive(_redPointManager.GetRedPoint_Shop());
            //             break;
            //         case 2:
            //             _redPoints[index].SetActive(_redPointManager.GetRedPoint_Main());
            //             break;
            //         case 3:
            //             _redPoints[index].SetActive(_redPointManager.GetRedPoint_Build());
            //             break;
            //         case 4:
            //             break;
            //     }
            // }
        }

        #region 摄像机动画

        /// <summary>
        /// 设置主摄像机动画
        /// </summary>
        /// <param name="type">类型</param>
        internal void SetMainCamAniTrigger(int type)
        {
            switch (type)
            {
                case 0:
                    mainCamAni.SetTrigger(Trigger_Body);
                    break;
                case 1:
                    mainCamAni.SetTrigger(Trigger_Propeller);
                    break;
                case 2:
                    mainCamAni.SetTrigger(Trigger_WingL);
                    break;
                case 3:
                    mainCamAni.SetTrigger(Trigger_WingR);
                    break;
                case 4:
                    mainCamAni.SetTrigger(Trigger_Fin);
                    break;
                case 5:
                    mainCamAni.SetTrigger(Trigger_Spurt);
                    break;
                case 6:
                    mainCamAni.SetTrigger(Trigger_Color);
                    break;
                default:
                    mainCamAni.SetTrigger(Trigger_Main);
                    break;
            }
        }

        #endregion

        #region 页面加载

        /// <summary>
        /// 加载页面
        /// <param name="pageName">页面名称</param>
        /// <param name="index">页面列表索引</param>
        /// <param name="cb">回调</param>
        /// </summary>
        private void LoadPage(string pageName, int index, Action cb)
        {
            LoadResources.XXResourcesLoad(pageName, handleTmp =>
            {
                GameObject pageTmp = Instantiate(handleTmp, _pageTran);
                InternalPageScript pageUi = pageTmp.GetComponent<InternalPageScript>();
                pageUi.Initial();
                pageUi.gameObject.SetActive(false);
                _internalPageUis[index] = pageUi;
                cb();
            });
        }

        /// <summary>
        /// 加载弹窗
        /// <param name="popName">弹窗名称</param>
        /// <param name="cb">回调</param>
        /// </summary>
        private void LoadPop(string popName, Action cb)
        {
            switch (popName)
            {
                case "PopSet":
                    LoadResources.XXResourcesLoad(popName, handleTmp =>
                    {
                        GameObject popTmp = Instantiate(handleTmp, _popTran);
                        _openSetPageUi = popTmp.AddComponent<OpenSetPageUi>();
                        _openSetPageUi.Initial();
                        _openSetPageUi.gameObject.SetActive(false);
                        cb();
                    });
                    break;
                case "PopOpenBox":
                    LoadResources.XXResourcesLoad(popName, handleTmp =>
                    {
                        GameObject popTmp = Instantiate(handleTmp, _popTran);
                        _openBoxPageUi = popTmp.AddComponent<OpenBoxPageUi>();
                        _openBoxPageUi.Initial();
                        _openBoxPageUi.gameObject.SetActive(false);
                        cb();
                    });
                    break;
                case "PopCall":
                    LoadResources.XXResourcesLoad(popName, handleTmp =>
                    {
                        GameObject popTmp = Instantiate(handleTmp, _popTran);
                        _openCallPageUi = popTmp.AddComponent<OpenCallPageUi>();
                        _openCallPageUi.Initial();
                        _openCallPageUi.gameObject.SetActive(false);
                        cb();
                    });
                    break;
                case "PopFollow":
                    LoadResources.XXResourcesLoad(popName, handleTmp =>
                    {
                        GameObject popTmp = Instantiate(handleTmp, _popTran);
                        _openFollowPageUi = popTmp.AddComponent<OpenFollowPageUi>();
                        _openFollowPageUi.Initial();
                        _openFollowPageUi.gameObject.SetActive(false);
                        cb();
                    });
                    break;
                case "PopSign":
                    LoadResources.XXResourcesLoad(popName, handleTmp =>
                    {
                        GameObject popTmp = Instantiate(handleTmp, _popTran);
                        _openSignPageUi = popTmp.GetComponent<OpenSignPageUi>();
                        _openSignPageUi.Initial();
                        _openSignPageUi.gameObject.SetActive(false);
                        cb();
                    });
                    break;
                case "PopRaffle":
                    LoadResources.XXResourcesLoad(popName, handleTmp =>
                    {
                        GameObject popTmp = Instantiate(handleTmp, _popTran);
                        _openRafflePageUi = popTmp.AddComponent<OpenRafflePageUi>();
                        _openRafflePageUi.Initial();
                        _openRafflePageUi.gameObject.SetActive(false);
                        cb();
                    });
                    break;
                case "PopTask":
                    LoadResources.XXResourcesLoad(popName, handleTmp =>
                    {
                        GameObject popTmp = Instantiate(handleTmp, _popTran);
                        _openTaskPageUi = popTmp.AddComponent<OpenTaskPageUi>();
                        _openTaskPageUi.Initial();
                        _openTaskPageUi.gameObject.SetActive(false);
                        cb();
                    });
                    break;
                case "PopClockIn":
                    LoadResources.XXResourcesLoad(popName, handleTmp =>
                    {
                        GameObject popTmp = Instantiate(handleTmp, _popTran);
                        _openClockInPageUi = popTmp.GetComponent<OpenClockInPageUi>();
                        _openClockInPageUi.Initial();
                        _openClockInPageUi.gameObject.SetActive(false);
                        cb();
                    });
                    break;
                case "PopMassage":
                    LoadResources.XXResourcesLoad(popName, handleTmp =>
                    {
                        GameObject popTmp = Instantiate(handleTmp, _popTran);
                        _openPopMassageUi = popTmp.AddComponent<OpenPopMassageUi>();
                        _openPopMassageUi.Initial();
                        _openPopMassageUi.gameObject.SetActive(false);
                        cb();
                    });
                    break;
                case "PopGM":
                    LoadResources.XXResourcesLoad(popName, handleTmp =>
                    {
                        GameObject popTmp = Instantiate(handleTmp, _popTran);
                        _openPopGmUi = popTmp.AddComponent<OpenPopGm>();
                        _openPopGmUi.Initial();
                        _openPopGmUi.gameObject.SetActive(false);
                        cb();
                    });
                    break;
            }
        }

        /// <summary>
        /// 加载主页新手引导 阶段 1
        /// </summary>
        private void LoadGuideMain_1()
        {
            LoadResources.XXResourcesLoad("GuideMain1", handleTmp =>
            {
                GameObject guideMain = Instantiate(handleTmp);
                _guideMain1 = guideMain.GetComponent<GuideMain1>();
                _guideMain1.Initial();
            });
        }

        #endregion

        #region 菜单栏

        /// <summary>
        /// 按钮 菜单栏
        /// </summary>
        /// <param name="index">菜单编号</param>
        /// <param name="runType">执行方式 0: 点击 1: 调用 2: 事件</param>
        internal void OnBtnMenu(int index, int runType)
        {
            if (index != 4)
            {
                // 其他页签
                for (int i = 0; i < _menuOffUis.Length; i++)
                {
                    int iTmp = i;
                    if (i == index)
                    {
                        if (index == 2)
                        {
                            _startRefreshBox = runType == 1;
                        }
                        _menuOffUis[i].SetActive(false);
                        _menuOnUis[i].SetActive(true);
                        if (_internalPageUis[i]) _internalPageUis[i].OpenInternalPage();
                        else LoadPage(_internalPageNames[index], index, () => { _internalPageUis[iTmp].OpenInternalPage(); });
                    }
                    else
                    {
                        _menuOffUis[i].SetActive(true);
                        _menuOnUis[i].SetActive(false);
                        if (_internalPageUis[i]) _internalPageUis[i].CloseInternalPage();
                    }
                }
            }
            else
            {
                // 排行榜页签
                GetUserInfo(() =>
                {
                    GameSdkManager._instance._serverScript.GetRankAll(2, () =>
                    {
                        for (int i = 0; i < _menuOffUis.Length; i++)
                        {
                            int iTmp = i;
                            if (i == index)
                            {
                                _menuOffUis[i].SetActive(false);
                                _menuOnUis[i].SetActive(true);
                                if (_internalPageUis[i]) _internalPageUis[i].OpenInternalPage();
                                else LoadPage(_internalPageNames[index], index, () => { _internalPageUis[iTmp].OpenInternalPage(); });
                            }
                            else
                            {
                                _menuOffUis[i].SetActive(true);
                                _menuOnUis[i].SetActive(false);
                                if (_internalPageUis[i]) _internalPageUis[i].CloseInternalPage();
                            }
                        }
                    });
                }, true);
            }
        }

        #endregion

        #region 弹窗

        /// <summary>
        /// 弹窗 设置
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OnOpenPop_Set(bool isOpen)
        {
            void runPopSet()
            {
                _openSetPageUi.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    _openSetPageUi.OpenTanChuang();
                }
            }

            if (_openSetPageUi) runPopSet();
            else LoadPop("PopSet", runPopSet);
        }

        /// <summary>
        /// 弹窗 收藏
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OnOpenPop_Follow(bool isOpen)
        {
            void runPopFollow()
            {
                _openFollowPageUi.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    _openFollowPageUi.OpenTanChuang();
                }
            }

            if (_openFollowPageUi) runPopFollow();
            else LoadPop("PopFollow", runPopFollow);
        }

        /// <summary>
        /// 弹窗 邀请
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OnOpenPop_Call(bool isOpen)
        {
            void runPopCall()
            {
                _openCallPageUi.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    _openCallPageUi.OpenTanChuang();
                }
            }

            if (_openCallPageUi) runPopCall();
            else LoadPop("PopCall", runPopCall);
        }

        /// <summary>
        /// 弹窗 开箱
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OnOpenPop_OpenBox(bool isOpen)
        {
            void runPopOpenBox()
            {
                _openBoxPageUi.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    _openBoxPageUi.OpenOpenBox();
                }
            }

            if (_openBoxPageUi) runPopOpenBox();
            else LoadPop("PopOpenBox", runPopOpenBox);
        }

        /// <summary>
        /// 弹窗 签到
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OnOpenPop_Sign(bool isOpen)
        {
            void runPopSign()
            {
                _openSignPageUi.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    _openSignPageUi.OpenPop();
                }
            }

            if (_openSignPageUi) runPopSign();
            else LoadPop("PopSign", runPopSign);
        }

        /// <summary>
        /// 弹窗 转盘
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OnOpenPop_Raffle(bool isOpen)
        {
            void runPopRaffle()
            {
                _openRafflePageUi.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    _openRafflePageUi.OpenPop();
                }
            }

            if (_openRafflePageUi) runPopRaffle();
            else LoadPop("PopRaffle", runPopRaffle);
        }

        /// <summary>
        /// 弹窗 任务
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OnOpenPop_Task(bool isOpen)
        {
            void runPopTask()
            {
                _openTaskPageUi.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    _openTaskPageUi.OpenPop();
                }
            }

            if (_openTaskPageUi) runPopTask();
            else LoadPop("PopTask", runPopTask);
        }

        /// <summary>
        /// 弹窗 地标打卡
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OnOpenPop_ClockIn(bool isOpen)
        {
            void runPopClockIn()
            {
                _openClockInPageUi.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    _openClockInPageUi.OpenPop();
                }
            }

            if (_openClockInPageUi) runPopClockIn();
            else LoadPop("PopClockIn", runPopClockIn);
        }

        /// <summary>
        /// 弹窗 订阅
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OnOpenPop_Massage(bool isOpen)
        {
            void runPopMassage()
            {
                _openPopMassageUi.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    _openPopMassageUi.OpenPop();
                }
            }

            if (_openPopMassageUi) runPopMassage();
            else LoadPop("PopMassage", runPopMassage);
        }

        /// <summary>
        /// 弹窗 GM
        /// </summary>
        /// <param name="isOpen">打开/关闭</param>
        internal void OnOpenPop_Gm(bool isOpen)
        {
            void runPopGm()
            {
                _openPopGmUi.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    _openPopGmUi.OnOpenPop();
                }
            }

            if (_openPopGmUi) runPopGm();
            else LoadPop("PopGM", runPopGm);
        }

        #endregion

        #region 飞机

        /// <summary>
        /// 更换飞机配件
        /// </summary>
        /// <param name="type">配件类型</param>
        /// <param name="id">配件ID</param>
        internal void ChangePlaneEquipment(int type, int id)
        {
            switch (type)
            {
                case 0: // 机身
                    _mainPlaneCreate.CreatePlaneBody(id, () => { });
                    break;
                case 1: // 机头
                    _mainPlaneCreate.CreatePlanePropeller(id);
                    break;
                case 2: // 机翼 左
                    if (id != -1)
                    {
                        _mainPlaneCreate.CreatePlaneWingL(id, () =>
                        {
                            if (DataHelper.CurUserInfoData.equipEquipments[5] != -1)
                            {
                                ChangePlaneEquipment(5, DataHelper.CurUserInfoData.equipEquipments[5]);
                            }
                        });
                    }
                    else
                    {
                        _mainPlaneCreate.CreatePlaneWingL(-1, () => { });
                        _mainPlaneCreate.HidePlaneSpurt(false, true);
                    }
                    break;
                case 3: // 机翼 右
                    if (id != -1)
                    {
                        _mainPlaneCreate.CreatePlaneWingR(id, () =>
                        {
                            if (DataHelper.CurUserInfoData.equipEquipments[5] != -1)
                            {
                                ChangePlaneEquipment(5, DataHelper.CurUserInfoData.equipEquipments[5]);
                            }
                        });
                    }
                    else
                    {
                        _mainPlaneCreate.CreatePlaneWingR(-1, () => { });
                        _mainPlaneCreate.HidePlaneSpurt(true, true);
                    }
                    break;
                case 4: // 机尾
                    _mainPlaneCreate.CreatePlaneFin(id);
                    break;
                case 5: // 推进器
                    // 左右推进器同创建同回收
                    if (DataHelper.CurUserInfoData.equipEquipments[2] != -1)
                    {
                        _mainPlaneCreate.CreatePlaneSpurtL(id, () =>
                        {
                            // 机翼(左) 已安装显示 未安装隐藏
                            _mainPlaneCreate.HidePlaneSpurt(false, DataHelper.CurUserInfoData.equipEquipments[2] == -1);
                        });
                    }

                    if (DataHelper.CurUserInfoData.equipEquipments[3] != -1)
                    {
                        _mainPlaneCreate.CreatePlaneSpurtR(id, () =>
                        {
                            // 机翼(右) 已安装显示 未安装隐藏
                            _mainPlaneCreate.HidePlaneSpurt(true, DataHelper.CurUserInfoData.equipEquipments[3] == -1);
                        });
                    }
                    
                    break;
            }
        }

        /// <summary>
        /// 重置飞机部件
        /// </summary>
        internal void ReSetPlanceEquipment()
        {
            _mainPlaneCreate.CreatePlane(_planePointAni.transform);
        }

        /// <summary>
        /// 刷新飞机涂装
        /// </summary>
        internal void RefreshPlaneColor()
        {
            _mainPlaneCreate.RefreshPlaneColor();
        }

        /** 触摸开始坐标 */
        private Vector2 _initTouchPoint;
        /** 上次触摸坐标 */
        private Vector2 _lastTouchPoint;
        /** 初始化飞机角度 */
        private Quaternion _initPlaneRot;
        /** UniTask异步信标 重置飞机旋转 */
        private CancellationTokenSource _cancellationTokenRot;
        
        /// <summary>
        /// 触摸开始 旋转飞机
        /// </summary>
        private void TouchDown_RotPlane(EnumButtonSign buttonSign, Vector2 touchPos)
        {
            if (buttonSign != EnumButtonSign.RotPlane) return;

            _cancellationTokenRot?.Cancel();
            _cancellationTokenRot?.Dispose();
            _cancellationTokenRot = null;
            
            _initTouchPoint = touchPos;
            _lastTouchPoint = touchPos;
        }

        /// <summary>
        /// 触摸拖拽 旋转飞机
        /// </summary>
        private void TouchDrag_RotPlane(EnumButtonSign buttonSign, Vector2 touchPos)
        {
            if (buttonSign != EnumButtonSign.RotPlane) return;

            if (Mathf.Approximately(touchPos.x, _lastTouchPoint.x) &&
                Mathf.Approximately(touchPos.y, _lastTouchPoint.y)) return;
            
            Vector2 touchDirection = (touchPos - _lastTouchPoint).normalized;
            float angle = Vector3.Angle(touchDirection, _planePoint.up);
            float cross = Vector3.Cross(touchDirection, _planePoint.up).y;
            if (cross > 0) angle = -angle;

            _planePoint.Rotate(0, angle * 0.05f, 0);
            _lastTouchPoint = touchPos;
        }

        /// <summary>
        /// 触摸结束 旋转飞机
        /// </summary>
        private void TouchUp_RotPlane(EnumButtonSign buttonSign, Vector2 touchPos)
        {
            if (buttonSign != EnumButtonSign.RotPlane) return;

            _ = ResetPlaneRot(15);
        }

        /// <summary>
        /// 重置飞机旋转 回弹
        /// <param name="frameNum">执行回弹需要的帧数</param>
        /// </summary>
        async UniTask ResetPlaneRot(int frameNum)
        {
            _cancellationTokenRot = new CancellationTokenSource();

            Quaternion oldRot = _planePoint.localRotation;
            
            float moveTimeMax = frameNum / 60f;
            float moveTime = 0;
            while (moveTime < moveTimeMax)
            {
                moveTime += Time.deltaTime;
                _planePoint.localRotation = Quaternion.Lerp(oldRot, _initPlaneRot, moveTime / moveTimeMax);
                await UniTask.Yield(cancellationToken: _cancellationToken.Token);
            }

            _planePoint.localRotation = _initPlaneRot;
        }

        /// <summary>
        /// 设置主场景飞机动画
        /// </summary>
        /// <param name="aniName">动画名称</param>
        internal void SetMainPlaneAni(string aniName)
        {
            _planePointAni.Play(aniName);
        }

        #endregion

        #region 排行榜渲染摄像机/飞机

        /// <summary>
        /// 设置排行榜渲染摄像机
        /// </summary>
        /// <param name="isActive">显示/隐藏</param>
        internal void SetRankCamActive(bool isActive)
        {
            _rankCamObj.SetActive(isActive);
        }

        #endregion

        #region 宝箱渲染摄像机/宝箱

        /// <summary>
        /// 设置宝箱渲染摄像机
        /// </summary>
        /// <param name="isActive">显示/隐藏</param>
        internal void SetBoxCamActive(bool isActive)
        {
            _boxCamObj.SetActive(isActive);
        }

        /// <summary>
        /// 获取宝箱点位
        /// </summary>
        /// <param name="index">宝箱列表索引</param>
        /// <returns>宝箱点位</returns>
        internal Vector3 GetBoxPoint(int index)
        {
            return _boxTrans[index].position;
        }

        /** 刷新宝箱执行步骤 */
        private int _refreshBoxNum;
        /** 启动场景刷新宝箱 */
        internal bool _startRefreshBox;

        /// <summary>
        /// 刷新宝箱
        /// </summary>
        internal void RefreshBox()
        {
            _refreshBoxNum = 0;
            int index = 0;
            foreach (KeyValuePair<int, string[]> boxData in DataHelper.CurUserInfoData.boxList)
            {
                if (boxData.Value != null)
                {
                    // 槽位宝箱数据不为空 宝箱已经创建 播放动画 未创建就创建宝箱
                    int boxId = int.Parse(boxData.Value[0]);
                    long boxTime = long.Parse(boxData.Value[1]);

                    long boxTimeTmp = boxTime == 0 ? ToolFunManager.GetCurrTime() : boxTime;
                    long nextTime = boxTimeTmp + ConfigManager.Instance.RewardBoxConfigDict[boxId].OpenTime * 60;
                    int subTime = (int)(nextTime - ToolFunManager.GetCurrTime());
                    subTime = subTime <= 0 ? 0 : subTime;

                    if (boxTime == 0)
                    {
                        // 未启动倒计时
                        CreateBox(index, boxId, "Stand1", _boxAnis[index] != null);
                    }
                    else
                    {
                        // 已启动倒计时
                        if (subTime <= 0)
                        {
                            // 倒计时已结束
                            CreateBox(index, boxId, "Stand3", _boxAnis[index] != null);
                        }
                        else
                        {
                            // 倒计时未结束
                            CreateBox(index, boxId, "Stand2", _boxAnis[index] != null);
                        }
                    }
                }
                else
                {
                    // 槽位宝箱数据为空 宝箱已经创建 移除 
                    if (_boxAnis[index] != null)
                    {
                        Destroy(_boxAnis[index].gameObject);
                        _boxAnis[index] = null;
                    }

                    _refreshBoxNum += 1;
                }

                index += 1;
            }

            if (_refreshBoxNum >= index)
            {
                EventManager.Send(CustomEventType.BoxOpenDone);
            }
        }

        /// <summary>
        /// 创建宝箱
        /// </summary>
        /// <param name="index">宝箱槽位</param>
        /// <param name="boxIdTmp">宝箱ID</param>
        /// <param name="aniName">动画名称</param>
        /// <param name="createOk">已经创建</param>
        private void CreateBox(int index, int boxIdTmp, string aniName, bool createOk)
        {
            if (!createOk)
            {
                int boxId = (boxIdTmp / 100) - 1;
                var assetsPath = new StringBuilder("Box" + boxId).ToString();
                LoadResources.XXResourcesLoad(assetsPath, handleTmp =>
                {
                    GameObject boxTmp = Instantiate(handleTmp, _boxTrans[index]);
                    boxTmp.transform.position = Vector3.zero;
                    boxTmp.transform.rotation = Quaternion.identity;
                    _boxAnis[index] = boxTmp.GetComponent<Animator>();
                    _boxAnis[index].Play(aniName, -1, 0);
                    _refreshBoxNum += 1;
                    if (_refreshBoxNum >= DataHelper.CurUserInfoData.boxList.Count)
                    {
                        EventManager.Send(CustomEventType.BoxOpenDone);
                    }
                });
            }
            else
            {
                _boxAnis[index].Play(aniName, -1, 0);
                _refreshBoxNum += 1;
                if (_refreshBoxNum >= DataHelper.CurUserInfoData.boxList.Count)
                {
                    EventManager.Send(CustomEventType.BoxOpenDone);
                }
            }
        }

        #endregion
    }
}