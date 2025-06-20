using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GamePlay.Battle
{
    public class CamControl : MonoBehaviour
    {
        private const float     followSpeed      = 10;
        private       float     followSpeedRatio = 1;
        private       Vector3   posTmp;
        private       Camera subCamTram;


        internal Transform targatTram;

        private CancellationTokenSource _cancellationToken;

        private void Awake()
        {
            subCamTram = transform.Find("CamMain").GetComponent<Camera>();
        }

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }

        internal async UniTask ResetCam(Transform targatTramTmp)
        {
            targatTram = targatTramTmp;

            _cancellationToken = new CancellationTokenSource();

            float moveTimeMax = 0.5f;
            float moveTime = 0;
            while (moveTime < moveTimeMax)
            {
                moveTime += Time.deltaTime;
                
                transform.position = Vector3.Lerp(Vector3.zero, targatTram.position, moveTime / moveTimeMax);

                await UniTask.Yield(cancellationToken: _cancellationToken.Token);
            }
        }

        internal async UniTask AdjustAngle()
        {
            _cancellationToken = new CancellationTokenSource();

            float moveTimeMax = 2;
            float moveTime = 0;
            Vector3 posOldTmp = subCamTram.transform.localPosition;
            Vector3 posNewTmp = new Vector3(0f, 1.5f, -2.5f);
            Quaternion rotOldTmp = subCamTram.transform.localRotation;
            Quaternion rotNewTmp = Quaternion.Euler(new Vector3(20, 0, 0));
            while (moveTime < moveTimeMax)
            {
                moveTime += Time.deltaTime;
                subCamTram.transform.localPosition = Vector3.Lerp(posOldTmp, posNewTmp, moveTime    / moveTimeMax);
                subCamTram.transform.localRotation = Quaternion.Lerp(rotOldTmp, rotNewTmp, moveTime / moveTimeMax);
                subCamTram.fieldOfView = Mathf.Lerp(60, 90, moveTime                                / moveTimeMax);

                await UniTask.Yield(cancellationToken: _cancellationToken.Token);
            }
        }

        internal void FollowSpeedRatio(float ratioTmp)
        {
            followSpeedRatio = ratioTmp;
        }

        private void LateUpdate()
        {
            if (!BattleManager._instance._planeControl.onLauncher) return;

            Vector3 smoothPos = Vector3.Lerp(transform.position, targatTram.position, followSpeed * Time.deltaTime);
            transform.position = new Vector3(smoothPos.x, targatTram.position.y, smoothPos.z);
        }

        void FixedUpdate()
        {
            if (BattleManager._instance._planeControl.onLauncher) return;
            if (!targatTram) return;

            Vector3 smoothPos = Vector3.Lerp(transform.position, targatTram.position, followSpeed * followSpeedRatio * Time.fixedDeltaTime);
            transform.position = new Vector3(smoothPos.x, targatTram.position.y, smoothPos.z);
        }


    }
}
