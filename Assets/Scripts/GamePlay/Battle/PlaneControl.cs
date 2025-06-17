using System.Collections.Generic;
using System.Threading;
using Common.GameRoot.AudioHandler;
using Common.LoadRes;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using GamePlay.Globa;
using Platform;
using UnityEngine;

namespace GamePlay.Battle
{
    public class PlaneControl : MonoBehaviour
    {
        private float propetySuDuNow;
        private float propetySuDuFactor;
        private float propetyGravityNow;
        private float propetyGravityFactor;

        private float propetySpurtTuiJinNow;
        private float propetyItemTuiJinNow;

        private Vector3 nowSpeed;

        private float thrusterSpurtTimeNow;

        private  Rigidbody _rigidbody;
        internal Transform launcherPoint;

        private  bool onFly;
        internal bool onLauncher;

        private bool onThrusterSpurt;
        private bool onThrusterItem;

        internal List<GameObject> effectThrusterSpurts;
        internal List<GameObject> effectThrusterItems;
        internal List<GameObject> effectTrails;

        internal List<GameObject> _planeSubs;

        private AudioSource audioSourceThrusterLoop;

        private GameObject targetObj;

        private CancellationTokenSource _cancellationToken;
        private CancellationTokenSource _cancellationTokenThrusterSpurt;
        private CancellationTokenSource _cancellationTokenThrusterItem;
        private CancellationTokenSource _cancellationTokenThrusterSpurtCancel;

        private void Awake()
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();

            audioSourceThrusterLoop = gameObject.AddComponent<AudioSource>();
            audioSourceThrusterLoop.clip = BattleManager._instance.audioThrusterLoop;
            audioSourceThrusterLoop.loop = true;
            audioSourceThrusterLoop.Stop();
        }

        private void Start()
        {
            _ = InitializePhysics();
            CreateTarget();
        }

        void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
            _cancellationTokenThrusterSpurt?.Cancel();
            _cancellationTokenThrusterSpurt?.Dispose();
            _cancellationTokenThrusterSpurt = null;
            _cancellationTokenThrusterItem?.Cancel();
            _cancellationTokenThrusterItem?.Dispose();
            _cancellationTokenThrusterItem = null;
            _cancellationTokenThrusterSpurtCancel?.Cancel();
            _cancellationTokenThrusterSpurtCancel?.Dispose();
            _cancellationTokenThrusterSpurtCancel = null;
        }

        private void FixedUpdate()
        {
            PlayerMove();
        }


        internal void StartFly(float tanLiSpeed)
        {
            propetyGravityNow = -3;
            propetySuDuNow = tanLiSpeed;

            propetySuDuFactor = BattleManager._instance.propetySuDu / (BattleManager._instance.propetyKangZu + 1) * 0.004f;

            propetyGravityFactor = 30 / (BattleManager._instance.propetyFuKong + 1) * 0.015f;

            thrusterSpurtTimeNow = BattleManager._instance.propetyNengLiang;

            _rigidbody.isKinematic = false;
            _rigidbody.velocity = transform.forward * propetySuDuNow;

            for (int i = 0; i < effectTrails.Count; i++) effectTrails[i].SetActive(true);

            onLauncher = false;
            onFly = true;

            if (targetObj) targetObj.SetActive(true);

            if (BattleManager._instance.speedBoardLevel > 1)
            {
                ThrusterItemStart(BattleManager._instance.propetyThrusterLauncher);
            }

            _cancellationToken = new CancellationTokenSource();

            _ = TanLiSpeed(tanLiSpeed);
        }

        void PlayerMove()
        {
            if (!onFly) return;

            if (propetySuDuNow > 10) propetySuDuNow -= propetySuDuFactor;

            Vector3 velocityLocal = transform.forward * (propetySuDuNow + propetySpurtTuiJinNow + propetyItemTuiJinNow);
            Vector3 worldVelocity = transform.TransformDirection(velocityLocal); // 局部空间速度转换世界空间速度

            // //===============================================左应用右====================================================
            // worldVelocity = Quaternion.Euler(0, -transform.rotation.eulerAngles.z, 0) * worldVelocity;
            // worldVelocity = transform.TransformDirection(worldVelocity);

            //===============================================衰减====================================================
            worldVelocity.x *= 0.6f;
            worldVelocity.y *= 0.4f;

            //===============================================处理重力====================================================
            propetyGravityNow += propetyGravityFactor;
            worldVelocity.y -= propetyGravityNow;

            //=============================================处理地图限制==================================================
            // if (worldVelocity.y > 0) worldVelocity.y *= Mathf.Min(10, 40 - BattleManager._instance.bodyCenter.position.y) * 0.1f;
            float clamp = Mathf.Exp(-Mathf.Pow(BattleManager._instance.bodyCenter.position.y / 30f, 3));
            if (worldVelocity.y > 0) worldVelocity.y *= clamp;

            if (BattleManager._instance.bodyCenter.position.x > 0 && worldVelocity.x > 0)
                worldVelocity.x *= Mathf.Min(10, 30 - BattleManager._instance.bodyCenter.position.x) * 0.1f;
            if (BattleManager._instance.bodyCenter.position.x < 0 && worldVelocity.x < 0)
                worldVelocity.x *= Mathf.Min(10, 30 + BattleManager._instance.bodyCenter.position.x) * 0.1f;


            _rigidbody.velocity = worldVelocity;

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            nowSpeed = worldVelocity;
            if (nowSpeed.z < 5)
                for (int i = 0; i < effectTrails.Count; i++)
                    effectTrails[i].SetActive(false);

            BattleManager._instance.RefreshSpeed((int)nowSpeed.z);
            BattleManager._instance.RefreshHeight();
        }

        internal void PlayerRot(Vector2 joyMoveVector)
        {
            if (!onFly) return;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(joyMoveVector.y * -30, joyMoveVector.x * 20, joyMoveVector.x * -30),
                // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(joyMoveVector.y * -30, 0, joyMoveVector.x * -30),
                Time.deltaTime * 3);
        }

        async UniTask TanLiSpeed(float tanLiSpeed)
        {
            float propetyTanLiFactor = (tanLiSpeed - BattleManager._instance.propetySuDu) / 250; // 5/0.02

            float moveTimeMax = 5f;
            float moveTime = 0;
            while (moveTime < moveTimeMax)
            {
                propetySuDuNow -= propetyTanLiFactor;

                moveTime += 0.02f;
                await UniTask.Delay(20, cancellationToken: _cancellationToken.Token);
            }
        }


#region 推进器
        internal void ThrusterSpurtStart()
        {
            if (!onFly) return;

            if (thrusterSpurtTimeNow <= 0)
            {
                GameGlobalManager._instance.ShowTips("能量不足");
                return;
            }

            _cancellationTokenThrusterSpurtCancel?.Cancel();

            onThrusterSpurt = true;

            BattleManager._instance._uiBattle.RefreshSpurt(1, thrusterSpurtTimeNow / BattleManager._instance.propetyNengLiang);
            BattleManager._instance._camControl.FollowSpeedRatio(0.5f);

            for (int i = 0; i < effectThrusterSpurts.Count; i++) effectThrusterSpurts[i].SetActive(true);
            for (int i = 0; i < effectTrails.Count; i++) effectTrails[i].SetActive(true);

            _cancellationTokenThrusterSpurt?.Cancel();
            _cancellationTokenThrusterSpurt = new CancellationTokenSource();
            _ = ThrusterSpurtIng();

            AudioHandler._instance.PlayAudio(BattleManager._instance.audioThruster);
            GameSdkManager._instance._sdkScript.LongVibrateControl();
            if (DataHelper.CurUserInfoData.settings[1] == 1) audioSourceThrusterLoop.Play();
        }

        async UniTask ThrusterSpurtIng()
        {
            while (onThrusterSpurt)
            {
                BattleManager._instance._uiBattle.RefreshSpurt(2, thrusterSpurtTimeNow / BattleManager._instance.propetyNengLiang);

                thrusterSpurtTimeNow -= 0.02f;
                if (thrusterSpurtTimeNow <= 0)
                {
                    GameGlobalManager._instance.ShowTips("能量耗尽");

                    _ = ThrusterSpurtCancel();
                }
                else
                {
                    if (propetySpurtTuiJinNow < BattleManager._instance.propetyTuiJin)
                    {
                        propetySpurtTuiJinNow += 1f;
                    }
                }

                await UniTask.Delay(20, cancellationToken: _cancellationTokenThrusterSpurt.Token);
            }
        }

        internal async UniTask ThrusterSpurtCancel()
        {
            if (!onThrusterSpurt) return;

            onThrusterSpurt = false;

            BattleManager._instance._uiBattle.RefreshSpurt(0, thrusterSpurtTimeNow / BattleManager._instance.propetyNengLiang);

            if (!onThrusterItem && !BattleManager._instance.gameWin)
                BattleManager._instance._camControl.FollowSpeedRatio(1);

            for (int i = 0; i < effectThrusterSpurts.Count; i++) effectThrusterSpurts[i].SetActive(false);

            audioSourceThrusterLoop.Stop();


            _cancellationTokenThrusterSpurtCancel?.Cancel();
            _cancellationTokenThrusterSpurtCancel = new CancellationTokenSource();
            float propetySpurtTuiJinOld = propetySpurtTuiJinNow;
            float moveTimeMax = 0.5f;
            float moveTime = 0;
            while (moveTime < moveTimeMax)
            {
                thrusterSpurtTimeNow -= 0.02f;
                BattleManager._instance._uiBattle.RefreshSpurt(2, thrusterSpurtTimeNow / BattleManager._instance.propetyNengLiang);

                propetySpurtTuiJinNow = Mathf.Lerp(propetySpurtTuiJinOld, 0, moveTime / moveTimeMax);

                moveTime += 0.02f;
                await UniTask.Delay(20, cancellationToken: _cancellationTokenThrusterSpurtCancel.Token);
            }

            propetySpurtTuiJinNow = 0;
        }
#endregion 推进器


#region 道具推进
        internal void ThrusterItemStart(float thrusterSpeedTmp)
        {
            if (!onFly) return;

            BattleManager._instance.scoreGetRing++;
            onThrusterItem = true;
            propetyItemTuiJinNow = thrusterSpeedTmp;

            BattleManager._instance._camControl.FollowSpeedRatio(0.5f);

            for (int i = 0; i < effectThrusterItems.Count; i++) effectThrusterItems[i].SetActive(true);
            for (int i = 0; i < effectTrails.Count; i++) effectTrails[i].SetActive(true);

            _cancellationTokenThrusterItem?.Cancel();
            _cancellationTokenThrusterItem = new CancellationTokenSource();
            _ = ThrusterItemEnd();

            AudioHandler._instance.PlayAudio(BattleManager._instance.audioThruster);
            GameSdkManager._instance._sdkScript.LongVibrateControl();
        }

        async UniTask ThrusterItemEnd()
        {
            await UniTask.Delay(1500, cancellationToken: _cancellationTokenThrusterItem.Token);

            onThrusterItem = false;

            if (!onThrusterSpurt && !BattleManager._instance.gameWin)
                BattleManager._instance._camControl.FollowSpeedRatio(1);

            for (int i = 0; i < effectThrusterItems.Count; i++) effectThrusterItems[i].SetActive(false);

            float propetyItemTuiJinOld = propetyItemTuiJinNow;
            float moveTimeMax = 0.5f;
            float moveTime = 0;
            while (moveTime < moveTimeMax)
            {
                propetyItemTuiJinNow = Mathf.Lerp(propetyItemTuiJinOld, 0, moveTime / moveTimeMax);

                moveTime += 0.02f;
                await UniTask.Delay(20, cancellationToken: _cancellationTokenThrusterItem.Token);
            }

            propetyItemTuiJinNow = 0;
        }
#endregion 道具推进


        private void OnCollisionEnter(Collision other)
        {
            if (!onFly) return;
            if (other.gameObject.layer != 7) return;

            _ = BattleEnd();
        }

        internal async UniTask BattleEnd()
        {
            onFly = false;

            AudioHandler._instance.PlayAudio(BattleManager._instance.audioBroken);
            GameSdkManager._instance._sdkScript.LongVibrateControl();
            BattleManager._instance.nowSpeed = 0;

            _cancellationToken = new CancellationTokenSource();

            for (int i = 0; i < _planeSubs.Count; i++)
            {
                _planeSubs[i].transform.SetParent(null);
                Rigidbody rigidbodySub = _planeSubs[i].AddComponent<Rigidbody>();
                // rigidbodySub.drag = 0.1f;
                // rigidbodySub.angularDrag = 0.1f;
                rigidbodySub.velocity = nowSpeed;
                await UniTask.Yield(cancellationToken: _cancellationToken.Token);
            }

            BattleManager._instance.EndFly();

            gameObject.SetActive(false);
        }

        internal void BattleWin()
        {
            _ = ThrusterSpurtCancel();
            BattleManager._instance._camControl.FollowSpeedRatio(0.1f);
        }


        async UniTask InitializePhysics()
        {
            _cancellationToken = new CancellationTokenSource();

            await UniTask.Delay(500, cancellationToken: _cancellationToken.Token);
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;

            BattleManager._instance._launcherControl.InitPoint();
            onLauncher = true;
        }

        void CreateTarget()
        {
            LoadResources.XXResourcesLoad("Effect_Target", handleTmp =>
            {
                targetObj = Instantiate(handleTmp, transform);
                targetObj.transform.localPosition = new Vector3(0, 0, 3);
                targetObj.transform.localRotation = Quaternion.identity;
                targetObj.transform.localScale = Vector3.one;
                targetObj.SetActive(false);
            });
        }




    }
}
