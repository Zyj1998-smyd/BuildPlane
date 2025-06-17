using System.Text;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.LoadRes;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using GamePlay.Battle.Guide;
using Platform;
using UnityEngine;

namespace GamePlay.Battle
{
    public class LauncherControl : MonoBehaviour
    {
        private Transform    linePoint;
        private LineRenderer _lineRendererR, _lineRendererL;
        private Transform    flyPoint;
        
        private bool    selsctPlane;
        private Vector3 initialPos;
        private Vector3 planeOffset;
        
        private bool onAni;
        private bool onFly;

        private float tanliScale;

        private AudioSource audioSourceDrag;
        
        private CancellationTokenSource _cancellationToken;
        private CancellationTokenSource _cancellationUpdateToken;
        
        private GuideBattle1 _guideBattle1;
        private GuideBattle2 _guideBattle2;
        
        private void Start()
        {
            audioSourceDrag = gameObject.AddComponent<AudioSource>();
            audioSourceDrag.clip = BattleManager._instance.audioDrag;
            audioSourceDrag.Stop();
            
            linePoint = transform.Find("LinePoint");
            flyPoint = transform.Find("FlyPoint");

            int launcherId = Mathf.Clamp(((BattleManager._instance.launcherLevel / 10) + 1), 1, 5);
            LoadResources.XXResourcesLoad(new StringBuilder("Launcher" + launcherId).ToString(),
                handleTmp =>
                {
                    Transform objTmp = Instantiate(handleTmp,transform).transform;
                    
                    _lineRendererR = objTmp.Find("Line/LineR").GetComponent<LineRenderer>();
                    _lineRendererL = objTmp.Find("Line/LineL").GetComponent<LineRenderer>();

                    _ = LauncherUpdate();
                });
            
            if (BattleManager._instance.speedBoardLevel > 1)
            {
                int speedBoardId = Mathf.Clamp(((BattleManager._instance.speedBoardLevel / 10) + 1), 1, 5);
                LoadResources.XXResourcesLoad(new StringBuilder("SpeedBoard" + speedBoardId).ToString(),
                    handleTmp =>
                    {
                        Instantiate(handleTmp.gameObject, transform);
                        EventManager.Send(CustomEventType.ResLoadDone);
                    });
            }
            else
            {
                EventManager.Send(CustomEventType.ResLoadDone);
            }
            
            Guide();
        }

        internal void InitPoint()
        {
            initialPos = BattleManager._instance._planeControl.transform.position;
            
            flyPoint.position = new Vector3(flyPoint.position.x, flyPoint.position.y + initialPos.y, flyPoint.position.z);
        }

        private void OnEnable()
        {
            EventManager<EnumButtonSign,Vector2>.Add(EnumButtonType.TouchScreenDown, TouchDown);
            EventManager<EnumButtonSign,Vector2>.Add(EnumButtonType.TouchScreenDrag, TouchDrag);
            EventManager<EnumButtonSign,Vector2>.Add(EnumButtonType.TouchScreenUp, TouchUp);
        }

        private void OnDisable()
        {
            EventManager<EnumButtonSign,Vector2>.Remove(EnumButtonType.TouchScreenDown, TouchDown);
            EventManager<EnumButtonSign,Vector2>.Remove(EnumButtonType.TouchScreenDrag, TouchDrag);
            EventManager<EnumButtonSign,Vector2>.Remove(EnumButtonType.TouchScreenUp, TouchUp);
            
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
            _cancellationUpdateToken?.Cancel();
            _cancellationUpdateToken?.Dispose();
            _cancellationUpdateToken = null;
        }

        async UniTask LauncherUpdate()
        {
            _cancellationUpdateToken = new CancellationTokenSource();
            while (!_cancellationUpdateToken.IsCancellationRequested)
            {
                _lineRendererR.SetPosition(1, linePoint.position - _lineRendererR.transform.position);
                _lineRendererL.SetPosition(1, linePoint.position - _lineRendererL.transform.position);

                if (!onFly)
                {
                    if (BattleManager._instance._planeControl)
                    {
                        if (BattleManager._instance._planeControl.launcherPoint)
                            linePoint.position = BattleManager._instance._planeControl.launcherPoint.position;

                        PlaneLook();
                    }
                }

                await UniTask.Yield(cancellationToken: _cancellationUpdateToken.Token);
            }
        }

        void PlaneLook()
        {
            Quaternion rotation = Quaternion.LookRotation(flyPoint.position - BattleManager._instance._planeControl.transform.position,
                BattleManager._instance._planeControl.transform.up);
            float angle = rotation.eulerAngles.y;

            BattleManager._instance._planeControl.transform.rotation = Quaternion.Euler(
                BattleManager._instance._planeControl.transform.rotation.eulerAngles.x,
                angle,
                BattleManager._instance._planeControl.transform.rotation.eulerAngles.z);
        }

        void HoldPlaneStart(Vector3 touchPoint)
        {
            selsctPlane = true;

            planeOffset = BattleManager._instance._planeControl.transform.position - touchPoint;

            if (DataHelper.CurUserInfoData.settings[1] == 1) audioSourceDrag.Play();
            GameSdkManager._instance._sdkScript.ShortVibrateControl();

            if (_guideBattle1) _guideBattle1.GuideAniStep(1);
        }

        void HoldPlaneIng(Vector3 touchPoint)
        {
            BattleManager._instance._planeControl.transform.position = touchPoint + planeOffset;

            if (BattleManager._instance._planeControl.transform.position.x < initialPos.x - 1f)
                BattleManager._instance._planeControl.transform.position =
                    new Vector3(initialPos.x - 1f, BattleManager._instance._planeControl.transform.position.y, BattleManager._instance._planeControl.transform.position.z);
            if (BattleManager._instance._planeControl.transform.position.x > initialPos.x + 1f)
                BattleManager._instance._planeControl.transform.position =
                    new Vector3(initialPos.x + 1f, BattleManager._instance._planeControl.transform.position.y, BattleManager._instance._planeControl.transform.position.z);
            if (BattleManager._instance._planeControl.transform.position.z < initialPos.z - 1.5f)
                BattleManager._instance._planeControl.transform.position =
                    new Vector3(BattleManager._instance._planeControl.transform.position.x, BattleManager._instance._planeControl.transform.position.y, initialPos.z - 1.5f);
            if (BattleManager._instance._planeControl.transform.position.z > initialPos.z)
                BattleManager._instance._planeControl.transform.position =
                    new Vector3(BattleManager._instance._planeControl.transform.position.x, BattleManager._instance._planeControl.transform.position.y, initialPos.z);
        }

        void HoldPlaneEnd()
        {
            selsctPlane = false;

            if (BattleManager._instance._planeControl.transform.position.z > initialPos.z - 0.5f)
            {
                _ = PlaneReset();
                
                if (_guideBattle1) _guideBattle1.GuideAniStep(0);
            }
            else
            {
                tanliScale = (-BattleManager._instance._planeControl.transform.position.z + initialPos.z) * 0.5f;
                _ = PlaneFly();
                
                AudioHandler._instance.PlayAudio(BattleManager._instance.audioStartFly);
            }
            
            audioSourceDrag.Stop();
        }

        async UniTask PlaneReset()
        {
            onAni = true;
            _cancellationToken = new CancellationTokenSource();

            float moveTimeMax = 10 / 60f;
            float moveTime = 0;
            Vector3 posOldTmp = BattleManager._instance._planeControl.transform.position;
            Vector3 posNewTmp = initialPos;
            while (moveTime < moveTimeMax)
            {
                moveTime += Time.deltaTime;
                BattleManager._instance._planeControl.transform.position = Vector3.Lerp(posOldTmp, posNewTmp, moveTime / moveTimeMax);

                await UniTask.Yield(cancellationToken: _cancellationToken.Token);
            }
            onAni = false;
        }

        async UniTask PlaneFly()
        {
            bool activateCam = false;

            onFly = true;

            _cancellationToken = new CancellationTokenSource();

            float tanLiSpeed = BattleManager._instance.propetyTanLi * tanliScale * (4f - GetPropety.GetZhongLiang(BattleManager._instance.propetyZhongLiang) * 3);

            float moveTimeMax = Vector3.Distance(BattleManager._instance._planeControl.transform.position, flyPoint.position) / tanLiSpeed;
            float moveTime = 0;

            Vector3 posOldTmp = BattleManager._instance._planeControl.transform.position;
            Vector3 posNewTmp = flyPoint.position;

            flyPoint.LookAt(BattleManager._instance._planeControl.transform.position);
            flyPoint.Rotate(Vector3.up, 180);
            Quaternion rotOldTmp = BattleManager._instance._planeControl.transform.rotation;
            Quaternion rotNewTmp = flyPoint.rotation;

            while (moveTime < moveTimeMax)
            {
                moveTime += Time.deltaTime;
                BattleManager._instance._planeControl.transform.position = Vector3.Lerp(posOldTmp, posNewTmp, moveTime    / moveTimeMax);
                BattleManager._instance._planeControl.transform.rotation = Quaternion.Lerp(rotOldTmp, rotNewTmp, moveTime / moveTimeMax);

                if (BattleManager._instance._planeControl.launcherPoint.position.z <= 0)
                    linePoint.position = BattleManager._instance._planeControl.launcherPoint.position;
                else if (!activateCam)
                {
                    activateCam = true;
                    BattleManager._instance._camControl.enabled = true;
                    _ = BattleManager._instance._camControl.AdjustAngle();
                }

                await UniTask.Yield(cancellationToken: _cancellationToken.Token);
            }

            BattleManager._instance.StartFly(tanLiSpeed);

            if (_guideBattle1) _guideBattle1.GuideAniStep(2);
            if (_guideBattle2)
            {
                _guideBattle2.gameObject.SetActive(true);
                _guideBattle2.GuideAniStep(0);
            }

            Destroy(this);
        }


        void TouchDown(EnumButtonSign buttonSign, Vector2 touchPos)
        {
            if (buttonSign != EnumButtonSign.LauncherTouch) return;

            if (!BattleManager._instance.cityLogoShowComplete) return;
            if (!BattleManager._instance._planeControl.onLauncher) return;
            if (onFly || onAni) return;
            if (selsctPlane) return;

            var ray = BattleManager._instance.mainCam.ScreenPointToRay(touchPos);
            // ReSharper disable once Unity.PreferNonAllocApi
            RaycastHit[] raycastHit = Physics.RaycastAll(ray, 100, 1 << 6 | 1 << 9);

            if (raycastHit.Length<2) return;
            
            for (int i = 0; i < raycastHit.Length; i++)
            {
                if (raycastHit[i].collider.gameObject.layer == 9)
                {
                    HoldPlaneStart(raycastHit[i].point);
                }
            }
        }

        void TouchDrag(EnumButtonSign buttonSign, Vector2 touchPos)
        {
            if (buttonSign != EnumButtonSign.LauncherTouch) return;
            
            if (!BattleManager._instance.cityLogoShowComplete) return;
            if (!BattleManager._instance._planeControl.onLauncher) return;
            if (!selsctPlane) return;
            
            var ray = BattleManager._instance.mainCam.ScreenPointToRay(touchPos);
            if (!Physics.Raycast(ray, out RaycastHit hit, 10, 1 << 9)) return;

            HoldPlaneIng(hit.point);
        }

        void TouchUp(EnumButtonSign buttonSign, Vector2 touchPos)
        {
            if (buttonSign != EnumButtonSign.LauncherTouch) return;
            
            if (!BattleManager._instance.cityLogoShowComplete) return;
            if (!BattleManager._instance._planeControl.onLauncher) return;
            if (selsctPlane) HoldPlaneEnd();
        }



        void Guide()
        {
            switch (DataHelper.CurUserInfoData.isNewUser)
            {
                case 0:
                    LoadResources.XXResourcesLoad("GuideBattle1",
                        handleTmp =>
                        {
                            GameObject objTmp = Instantiate(handleTmp);
                            _guideBattle1 = objTmp.GetComponent<GuideBattle1>();
                            _guideBattle1.Initial();
                    
                            EventManager.Send(CustomEventType.ResLoadDone);
                        });
                    break;
                case 1:
                    LoadResources.XXResourcesLoad("GuideBattle2",
                        handleTmp =>
                        {
                            GameObject objTmp = Instantiate(handleTmp);
                            _guideBattle2 = objTmp.GetComponent<GuideBattle2>();
                            _guideBattle2.Initial();
                            _guideBattle2.gameObject.SetActive(false);
                    
                            EventManager.Send(CustomEventType.ResLoadDone);
                        });
                    break;
                default:
                    if (PlayerPrefs.GetInt("QualitySwitch", 0) == 0) FPSMonitor.Instance.StartTracking();
                    EventManager.Send(CustomEventType.ResLoadDone);
                    break;
            }
        }
        
        
        
        
    }
}
