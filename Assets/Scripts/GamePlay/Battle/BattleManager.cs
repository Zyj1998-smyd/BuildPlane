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
using Platform;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePlay.Battle
{
    public class BattleManager : MonoBehaviour
    {
        internal static BattleManager _instance;

        internal Camera mainCam;

        private  GameObject joystickTouch;
        private  GameObject spurtTouch;
        internal GameObject launcherTouch;

        internal float propetyZhongLiang; // 重量
        internal float propetyFuKong;     // 浮空
        internal float propetySuDu;       // 速度
        internal float propetyKangZu;     // 抗阻
        internal float propetyTuiJin;     // 推进
        internal float propetyNengLiang;  // 能量

        internal float propetyTanLi;            //弹力
        internal float propetyThrusterLauncher; // 推进板
        internal float propetyThrusterRing;     // 推进环

        internal int launcherLevel;
        internal int speedBoardLevel;

        private  int   landmarkNum;
        private  int   LevelLong;
        private  int   LevelLongTmp;
        internal float endDis = -1;
        private  int   sceneArtId;

        public int sceneItemSCount;
        public int sceneItemMCount;

        internal bool gameWin;
        private  bool accountWinIng;
        internal bool cityLogoShowComplete;

        private Transform sceneParentTram;

        private readonly List<GameObject> sceneArtsA = new List<GameObject>();
        private readonly List<GameObject> sceneArtsB = new List<GameObject>();
        private          SubSceneArt      landmarkNow;

        private          float                          createScenePoint = -150;
        private          List<bool>                     landmarkSwitch   = new List<bool>();
        private readonly Dictionary<SubSceneArt, float> sceneArtSubs     = new Dictionary<SubSceneArt, float>();

        private Transform           sceneItemsTram;
        List<AssetPool<SceneItems>> pool_sceneItemsPools;

        internal PlaneControl      _planeControl;
        internal CamControl        _camControl;
        internal BattlePlaneCreate BattlePlaneCreate;
        internal LauncherControl   _launcherControl;

        internal Transform bodyCenter;
        internal int       nowSpeed,  maxSpeed;
        internal int       nowHeight, maxHeight;

        private Transform        EffectTram;
        AssetPool<EffectControl> pool_effectGetGoldPool;

        private CancellationTokenSource _cancellationToken_Layer;
        private CancellationTokenSource _cancellationToken;

        private Color[] sceneFogColor =
        {
            new Color(0.474f, 0.839f, 0.969f), new Color(0.165f, 0.753f, 1f), new Color(0.651f, 0.863f, 1f), new Color(0.137f, 0.808f, 0.879f),
            new Color(0f, 0.698f, 0.984f), new Color(0.992f, 0.741f, 0.608f), new Color(0.137f, 0.808f, 0.879f), new Color(0.165f, 0.753f, 1f),
            new Color(0.651f, 0.863f, 1f), new Color(0.992f, 0.741f, 0.608f)
        };
        
        public AudioClip audioAccount;
        public AudioClip audioAccountNext;
        public AudioClip audioGetGold;
        public AudioClip audioStartFly;
        public AudioClip audioThruster;
        public AudioClip audioThrusterLoop;
        public AudioClip audioBroken;
        public AudioClip audioDrag;
        public AudioClip audioQiZi;

        /** UI */
        internal UiBattle _uiBattle;

        /** 飞行距离 */
        internal float scoreDistance;

        /** 拾取金币 */
        internal float scoreGetGold;

        /** 拾取加速环 */
        internal float scoreGetRing;

        /** 打卡地标列表 */
        internal readonly List<int> getLandmarks = new List<int>();

        private GameObject qiZi;

        internal readonly string[] cityNames = { "Area1", "Area2", "Area3", "Area4", "Area5", "Area6", "Area7", "Area8", "Area9", "Area10", "Area11", "Area12", "Area13", "Area14", "Area15" };

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            Transform canvasTram = GameObject.Find("/Canvas2D").transform;
            joystickTouch = canvasTram.Find("Main/JoystickTouch").gameObject;
            spurtTouch = canvasTram.Find("Main/BtnSpurt").gameObject;
            launcherTouch = canvasTram.Find("Main/LauncherTouch").gameObject;

            _camControl = GameObject.Find("/CamMain").GetComponent<CamControl>();
            mainCam = _camControl.transform.Find("CamMain").GetComponent<Camera>();

            sceneParentTram = GameObject.Find("/Scene/SceneArt").transform;
            sceneItemsTram = GameObject.Find("/Scene/SceneItem").transform;

            GameObject.Find("/Scene").GetComponent<GlobaMaterialSet>().SetBend(true);

            _uiBattle = gameObject.AddComponent<UiBattle>();
            _uiBattle.Initial();
        }

        void Start()
        {
            AudioHandler._instance.PlayBGM("BgmBattle");

            int loadResNum = 15; // 加载步数
            DataHelper.CurRunScene = "Battle";
            GameGlobalManager._instance.SetCanvasUiMain();
            //GameGlobalManager._instance.SetCanvasUiMainOpenBox();
            if (DataHelper.isRootLoad)
            {
                DataHelper.isRootLoad = false;
                GameRootLoad.Instance.EndLoad(loadResNum, () => { });
            }
            else
            {
                GameGlobalManager._instance.ShowLoadSceneEnd(loadResNum, () => { });
            }

            joystickTouch.SetActive(false);
            spurtTouch.SetActive(false);
            _camControl.enabled = false;

            sceneArtId = Mathf.Min(DataHelper.CurLevelNum, 10);
            SetPropety();

            LevelLong = 4;
            LevelLongTmp = LevelLong;
            // endDis = LevelLong * 200 * 5; //间隔数 * 每段长度200
            endDis = 4000;
            landmarkNum = 1;

            CreateScene();

            Transform skyTramTmp = GameObject.Find("/CamMain/CamMain/Sky").transform;
            LoadResources.XXResourcesLoad(new StringBuilder("SceneArt" + sceneArtId + "_Sky").ToString(), handleTmp =>
            {
                GameObject skyObjTmp = Instantiate(handleTmp, skyTramTmp);
                skyObjTmp.transform.localPosition = Vector3.zero;
                skyObjTmp.transform.localRotation = Quaternion.identity;

                RenderSettings.fogColor = sceneFogColor[sceneArtId - 1];

                EventManager.Send(CustomEventType.ResLoadDone);
            });

            GameObject planeObjTmp = new GameObject
            {
                transform =
                {
                    position = new Vector3(0, 0.5f, 0)
                },
                name = "Plane"
            };

            gameObject.AddComponent<BattlePlaneCreate>().CreatePlane(planeObjTmp.transform);
            _launcherControl = GameObject.Find("/Scene/Launcher").AddComponent<LauncherControl>();

            EffectTram = GameObject.Find("/Scene/Effect").transform;
            InitSceneAsset();

            LoadResources.XXResourcesLoad("QiZi", handleTmp =>
            {
                qiZi = handleTmp;
                EventManager.Send(CustomEventType.ResLoadDone);
            });

            _uiBattle.RefreshDistance();

            landmarkSwitch = DataHelper.GetLandMarks();

            // // 需要录屏 字节系 安卓 开始录屏
            // if (GameSdkManager._instance._sdkScript._isNeedRecord)
            // {
            //     GameSdkManager._instance._sdkScript.OnStartRecord();
            // }

            // 上报自定义分析数据 事件: 开始游戏
            GameSdkManager._instance._sdkScript.ReportAnalytics("StartGame", "levelId", DataHelper.CurLevelNum);
        }

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
            _cancellationToken_Layer?.Cancel();
            _cancellationToken_Layer?.Dispose();
            _cancellationToken_Layer = null;
        }

        void SetPropety()
        {
            Dictionary<string, float> allPropetyNum = DataHelper.GetAllPropety();
            propetyZhongLiang = allPropetyNum["propetyZhongLiang"];             // 重量
            propetyFuKong = allPropetyNum["propetyFuKong"];                     // 浮空
            propetySuDu = allPropetyNum["propetySuDu"];                         // 速度
            propetyKangZu = allPropetyNum["propetyKangZu"];                     // 抗阻
            propetyTuiJin = allPropetyNum["propetyTuiJin"];                     // 推进
            propetyNengLiang = allPropetyNum["propetyNengLiang"];               // 能量
            propetyTanLi = allPropetyNum["propetyTanLi"];                       // 弹力
            propetyThrusterLauncher = allPropetyNum["propetyThrusterLauncher"]; // 推进板
            propetyThrusterRing = allPropetyNum["propetyThrusterRing"];         // 推进环

            // sceneArtId = 5;
            // propetyZhongLiang = 300;       // 重量
            // propetyFuKong = 10000;          // 浮空
            // propetySuDu = 10000;            // 速度
            // propetyKangZu = 10000;          // 抗阻
            // propetyTuiJin = 10000;          // 推进
            // propetyNengLiang = 10000;       // 能量
            // propetyTanLi = 1000;            // 弹力
            // propetyThrusterLauncher = 1000; // 推进板
            // propetyThrusterRing = 1000;     // 推进环

            // Debug.Log("浮空：" + propetyFuKong);
            propetyFuKong = GetPropety.GetFuKong(propetyFuKong, propetyZhongLiang); // 浮空(浮空，重量)
            // Debug.Log("浮空：" + propetyFuKong);
            // Debug.Log("速度：" + propetySuDu);
            propetySuDu = GetPropety.GetSuDu(propetySuDu); // 速度
            // Debug.Log("速度：" + propetySuDu);
            // Debug.Log("抗阻：" + propetyKangZu);
            propetyKangZu = GetPropety.GetKangZu(propetyKangZu); // 抗阻
            // Debug.Log("抗阻：" + propetyKangZu);
            // Debug.Log("推进：" + propetyTuiJin);
            propetyTuiJin = GetPropety.GetTuiJin(propetyTuiJin); // 推进
            // Debug.Log("推进：" + propetyTuiJin);
            // Debug.Log("能量：" + propetyNengLiang);
            propetyNengLiang = GetPropety.GetNengLiang(propetyNengLiang); // 能量
            // Debug.Log("能量：" + propetyNengLiang);
            // Debug.Log("弹力：" + propetyTanLi);
            propetyTanLi = GetPropety.GetTanLi(propetyTanLi); // 弹力
            // Debug.Log("弹力："  + propetyTanLi);
            // Debug.Log("推进板：" + propetyThrusterLauncher);
            propetyThrusterLauncher = GetPropety.GetTuiJinItem(propetyThrusterLauncher) * 0.8f; // 推进板，0.8修正
            // Debug.Log("推进板：" + propetyThrusterLauncher);
            // Debug.Log("推进环：" + propetyThrusterRing);
            propetyThrusterRing = GetPropety.GetTuiJinItem(propetyThrusterRing); // 推进环
            // Debug.Log("推进环：" + propetyThrusterRing);

            launcherLevel = DataHelper.CurUserInfoData.additions[1];
            speedBoardLevel = DataHelper.CurUserInfoData.additions[2];

            //平衡处理
            float wingBalance = DataHelper.GetPlaneWingBalance() ? 1f : 0.7f;
            propetyFuKong *= wingBalance;
            propetySuDu *= wingBalance;
            propetyKangZu *= wingBalance;
        }

        internal void ModleDone(GameObject planeObj,     Transform        launcherPointTmp, List<GameObject> effectThrusterSpurts, List<GameObject> effectThrusterItems,
            List<GameObject>               effectTrails, List<GameObject> planeSubsTmp)
        {
            _planeControl = planeObj.AddComponent<PlaneControl>();
            _planeControl.launcherPoint = launcherPointTmp;
            _planeControl.effectThrusterSpurts = new List<GameObject>(effectThrusterSpurts);
            _planeControl.effectThrusterItems = new List<GameObject>(effectThrusterItems);
            _planeControl.effectTrails = new List<GameObject>(effectTrails);
            _planeControl._planeSubs = new List<GameObject>(planeSubsTmp);

            // 判断进入与上次不同的关卡
            // 条件1 不在新手引导中
            bool isNewGuide = DataHelper.CurUserInfoData.isNewUser >= 2;
            // 条件2 当前选择的关卡 不是上一局选择的关卡
            bool isNewLevel = DataHelper.CurLevelNum != DataHelper.LastLevelNum;

            DataHelper.LastLevelNum = DataHelper.CurLevelNum;

            if (isNewGuide && isNewLevel)
            {
                cityLogoShowComplete = false;
                _uiBattle.GreateCityLogo(() =>
                {
                    _uiBattle._uiCityLogo.ShowCityLogo(sceneArtId);

                    cityLogoShowComplete = true;
                    EventManager.Send(CustomEventType.ResLoadDone);

                    // _launcherControl = GameObject.Find("/Scene/Launcher").AddComponent<LauncherControl>();
                });
            }
            else
            {
                cityLogoShowComplete = true;
                EventManager.Send(CustomEventType.ResLoadDone);

                // _launcherControl = GameObject.Find("/Scene/Launcher").AddComponent<LauncherControl>();
            }
        }


        internal void StartFly(float tanLiSpeed)
        {
            _planeControl.StartFly(tanLiSpeed);
            Destroy(launcherTouch);
            joystickTouch.SetActive(true);
            spurtTouch.SetActive(true);

            _cancellationToken = new CancellationTokenSource();
            _ = OnFly();

            _cancellationToken_Layer = new CancellationTokenSource();
            LayerActivate().Forget();
        }

        internal void EndFly()
        {
            if (accountWinIng) return;

            joystickTouch.SetActive(false);
            spurtTouch.SetActive(false);

            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
            _cancellationToken_Layer?.Cancel();
            _cancellationToken_Layer?.Dispose();
            _cancellationToken_Layer = null;

            _cancellationToken = new CancellationTokenSource();
            if (gameWin)
                _ = AccountWin();
            else
                _ = BattleEnd();
        }

        async UniTask BattleEnd()
        {
            Rigidbody bodyCenterRB = bodyCenter.GetComponent<Rigidbody>();
            bodyCenterRB.drag = 0.3f;
            bodyCenterRB.angularDrag = 0.3f;

            float moveTimeMax = 3f;
            float moveTime = 0;
            while (bodyCenterRB.velocity.z > 0.1f && moveTime < moveTimeMax)
            {
                if (scoreDistance < bodyCenter.position.z)
                {
                    scoreDistance = bodyCenter.position.z;
                    _uiBattle.RefreshDistance();

                    if (bodyCenter.position.z > endDis)
                    {
                        _cancellationToken?.Cancel();
                        _cancellationToken?.Dispose();
                        _cancellationToken = null;
                        _cancellationToken_Layer?.Cancel();
                        _cancellationToken_Layer?.Dispose();
                        _cancellationToken_Layer = null;

                        _cancellationToken = new CancellationTokenSource();
                        _ = AccountWin();
                    }
                }

                moveTime += 0.02f;
                await UniTask.Delay(20, cancellationToken: _cancellationToken.Token);
            }

            if (!accountWinIng)
            {
                Instantiate(qiZi, bodyCenter.position, quaternion.identity);
                _camControl.targatTram = null;
                AudioHandler._instance.PlayAudio(audioQiZi);
                await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
                _uiBattle.OpenAccount();
            }
        }

        async UniTask AccountWin()
        {
            accountWinIng = true;

            await UniTask.Delay(20, cancellationToken: _cancellationToken.Token);

            _uiBattle.OpenAccount();
        }

        async UniTask OnFly()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                _uiBattle.RefreshSpeed();
                _uiBattle.RefreshHeight();

                if (scoreDistance < bodyCenter.position.z)
                {
                    scoreDistance = bodyCenter.position.z;
                    _uiBattle.RefreshDistance();
                }

                if (bodyCenter.position.z > createScenePoint)
                {
                    CreateScene();
                }

                if (bodyCenter.position.z > endDis)
                {
                    gameWin = true;
                    EndFly();
                    _planeControl.BattleWin();
                }

                if (bodyCenter.position.y < -5)
                {
                    _ = _planeControl.BattleEnd();
                }

                await UniTask.Delay(20, cancellationToken: _cancellationToken.Token);
            }
        }

        void CreateScene()
        {
            createScenePoint += 200;
            float createScenePointTmp = createScenePoint;

            if (sceneArtsA.Count >= 2)
            {
                GameObject objTmp = sceneArtsA[0];

                objTmp.transform.position = new Vector3(0, 0, createScenePointTmp);

                Transform objTmpSub1 = objTmp.transform.Find("Scene1");
                Transform objTmpSub2 = objTmp.transform.Find("Scene2");
                Transform objTmpSub3 = objTmp.transform.Find("Scene3");
                sceneArtSubs[objTmpSub1.GetComponent<SubSceneArt>()] = objTmpSub1.position.z;
                sceneArtSubs[objTmpSub2.GetComponent<SubSceneArt>()] = objTmpSub2.position.z;
                sceneArtSubs[objTmpSub3.GetComponent<SubSceneArt>()] = objTmpSub3.position.z;

                sceneArtsA.RemoveAt(0);
                sceneArtsA.Add(objTmp);
            }
            else
            {
                LoadResources.XXResourcesLoad(new StringBuilder("SceneArt" + sceneArtId).ToString(),
                    handleTmp =>
                    {
                        GameObject objTmp = Instantiate(handleTmp, new Vector3(0, 0, createScenePointTmp), Quaternion.identity, sceneParentTram);

                        sceneArtsA.Add(objTmp);

                        Transform objTmpSub1 = objTmp.transform.Find("Scene1");
                        Transform objTmpSub2 = objTmp.transform.Find("Scene2");
                        Transform objTmpSub3 = objTmp.transform.Find("Scene3");
                        sceneArtSubs.Add(objTmpSub1.gameObject.AddComponent<SubSceneArt>(), objTmpSub1.position.z);
                        sceneArtSubs.Add(objTmpSub2.gameObject.AddComponent<SubSceneArt>(), objTmpSub2.position.z);
                        sceneArtSubs.Add(objTmpSub3.gameObject.AddComponent<SubSceneArt>(), objTmpSub3.position.z);
                    });
            }

            CreateSceneLandmark(createScenePointTmp);
        }

        void CreateSceneLandmark(float createScenePointTmp)
        {
            if (landmarkNow != null && landmarkNow.transform.position.z + 125 < BattleManager._instance.bodyCenter.position.z)
            {
                sceneArtSubs.Remove(landmarkNow);
                Destroy(landmarkNow);
            }

            LevelLongTmp--;
            if (LevelLongTmp == 0)
            {
                LevelLongTmp = LevelLong;
                int landmarkNumTmp = landmarkNum;

                LoadResources.XXResourcesLoad(new StringBuilder("SceneArt" + sceneArtId + "_Landmark" + (landmarkNum > 5 ? 0 : landmarkNum)).ToString(),
                    handleTmp =>
                    {
                        GameObject objTmp = Instantiate(handleTmp, new Vector3(0, 0, createScenePointTmp), Quaternion.identity, sceneParentTram);
                        landmarkNow = objTmp.AddComponent<SubSceneArt>();
                        sceneArtSubs.Add(landmarkNow, landmarkNow.transform.position.z + 125);

                        List<GameObject> clockInPintsTmp = new List<GameObject>();
                        if (objTmp.transform.Find("ClockIn1")) clockInPintsTmp.Add(objTmp.transform.Find("ClockIn1").gameObject);
                        if (objTmp.transform.Find("ClockIn2")) clockInPintsTmp.Add(objTmp.transform.Find("ClockIn2").gameObject);
                        if (landmarkSwitch[landmarkNumTmp - 1])
                        {
                            for (int i = 0; i < clockInPintsTmp.Count; i++)
                                clockInPintsTmp[i]?.SetActive(false);
                        }
                        else
                        {
                            if (clockInPintsTmp.Count == 2)
                            {
                                if (Random.Range(0, 10) < 5) clockInPintsTmp[0].SetActive(false);
                                else clockInPintsTmp[1].SetActive(false);
                            }
                        }
                    });

                landmarkNum++;
            }
            else
            {
                if (sceneArtsB.Count >= 2)
                {
                    GameObject objTmp = sceneArtsB[0];

                    objTmp.transform.position = new Vector3(0, 0, createScenePointTmp);

                    sceneArtsB.RemoveAt(0);
                    sceneArtsB.Add(objTmp);

                    sceneArtSubs[objTmp.GetComponent<SubSceneArt>()] = objTmp.transform.position.z + 125;
                }
                else
                {
                    LoadResources.XXResourcesLoad(new StringBuilder("SceneArt" + sceneArtId + "_Landmark" + 0).ToString(),
                        handleTmp =>
                        {
                            GameObject objTmp = Instantiate(handleTmp, new Vector3(0, 0, createScenePointTmp), Quaternion.identity, sceneParentTram);

                            sceneArtsB.Add(objTmp);
                            sceneArtSubs.Add(objTmp.AddComponent<SubSceneArt>(), objTmp.transform.position.z + 125);
                        });
                }
            }
        }


        private float lastSceneitem;
        internal void CreateSceneItem(Vector3 createPoint)
        {
            lastSceneitem = createPoint.z + 100 * Random.Range(1f, 1.5f);
            
            int ranTmp = Random.Range(0, sceneItemSCount + sceneItemMCount);
            
            Vector3 planePointTmp = new Vector3(createPoint.x, Mathf.Clamp(bodyCenter.transform.position.y *Random.Range(0.8f,1.2f), 10, 35), createPoint.z);

            SceneItems sceneItemsTmp = pool_sceneItemsPools[ranTmp].GetAsset();
            sceneItemsTmp.transform.position = planePointTmp;
        }

        internal void GetGold(int goldNumTmp)
        {
            scoreGetGold += goldNumTmp;
            _uiBattle.RefreshGold();

            EffectControl effectTmp = pool_effectGetGoldPool.GetAsset();
            effectTmp.SetFollow(_planeControl.transform);
            effectTmp.transform.position = _planeControl.transform.position;

            AudioHandler._instance.PlayAudio(audioGetGold);
            GameSdkManager._instance._sdkScript.ShortVibrateControl();
        }

        internal void ClockIn(int clockInIdTmp)
        {
            if (!_uiBattle._uiClockIn) return;

            getLandmarks.Add(clockInIdTmp);

            _uiBattle._uiClockIn.ShowClockIn(clockInIdTmp);
        }


        void InitSceneAsset()
        {
            LoadResources.XXResourcesLoad("Effect_GetGold",
                handleTmp => { pool_effectGetGoldPool = new AssetPool<EffectControl>(handleTmp.GetComponent<EffectControl>(), EffectTram); });

            pool_sceneItemsPools = new List<AssetPool<SceneItems>>();
            for (int i = 0; i < sceneItemSCount; i++)
            {
                LoadResources.XXResourcesLoad(new StringBuilder("SceneItemS" + (i + 1)).ToString(),
                    handleTmp => { pool_sceneItemsPools.Add(new AssetPool<SceneItems>(handleTmp.GetComponent<SceneItems>(), sceneItemsTram)); });
                if (i == sceneItemSCount - 1)
                {
                    for (int j = 0; j < sceneItemMCount; j++)
                    {
                        LoadResources.XXResourcesLoad(new StringBuilder("SceneItemM" + (j + 1)).ToString(),
                            handleTmp => { pool_sceneItemsPools.Add(new AssetPool<SceneItems>(handleTmp.GetComponent<SceneItems>(), sceneItemsTram)); });
                    }
                }
            }
        }


        internal void RefreshSpeed(int speedTmp)
        {
            nowSpeed = speedTmp;
            if (maxSpeed < speedTmp)
            {
                maxSpeed = speedTmp;
            }
        }

        internal void RefreshHeight()
        {
            int heightTmp = (int)bodyCenter.position.y;

            nowHeight = heightTmp;
            if (maxHeight < heightTmp)
            {
                maxHeight = heightTmp;
            }
        }

        async UniTask LayerActivate()
        {
            while (!_cancellationToken_Layer.IsCancellationRequested)
            {
                await UniTask.Delay(500, cancellationToken: _cancellationToken_Layer.Token);
                foreach (var VARIABLE in sceneArtSubs)
                {
                    if (VARIABLE.Value < bodyCenter.position.z - 50)
                    {
                        VARIABLE.Key.SetLayer(11);
                        continue;
                    }

                    if (VARIABLE.Value > bodyCenter.position.z + 150)
                    {
                        VARIABLE.Key.SetLayer(11);
                        continue;
                    }

                    VARIABLE.Key.SetLayer(7);
                    
                    if (bodyCenter.position.z > lastSceneitem)
                        VARIABLE.Key.CreateSceneItem();
                }
            }
        }


#if UNITY_EDITOR
        private void Update()
        {
            GMPlayer();
        }

        void GMPlayer()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                // 打开宝箱
                int levelNumTmp = DataHelper.CurUserInfoData.curLevelNum;
                if (levelNumTmp >= 5) levelNumTmp = 5;
                int _newBoxGet = 100 + (levelNumTmp - 1);
                GameGlobalManager._instance.OpenBox(_newBoxGet);
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                // 打开宝箱
                int levelNumTmp = DataHelper.CurUserInfoData.curLevelNum;
                if (levelNumTmp >= 5) levelNumTmp = 5;
                int _newBoxGet = 200 + (levelNumTmp - 1);
                GameGlobalManager._instance.OpenBox(_newBoxGet);
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                // 打开宝箱
                int levelNumTmp = DataHelper.CurUserInfoData.curLevelNum;
                if (levelNumTmp >= 5) levelNumTmp = 5;
                int _newBoxGet = 300 + (levelNumTmp - 1);
                GameGlobalManager._instance.OpenBox(_newBoxGet);
            }
        }
#endif
    }
}
