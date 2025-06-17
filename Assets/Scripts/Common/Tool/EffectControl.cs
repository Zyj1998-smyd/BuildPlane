using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Common.Tool
{
    public class EffectControl : MonoBehaviour
    {
        [LabelText("延迟等待关闭"), SerializeField] public float delayTime;         

        private Transform followTarget;
        private CancellationTokenSource  cancellationToken = new CancellationTokenSource();

        private void OnEnable()
        {
            if (delayTime<=0) return;
            
            cancellationToken.Cancel();
            cancellationToken = new CancellationTokenSource();
            _ = EffectWait();
        }

        private void OnDestroy()
        {
            cancellationToken.Cancel();
        }

        async UniTask EffectWait()
        {
            await UniTask.Delay((int)(delayTime * 1000), cancellationToken: cancellationToken.Token);
            followTarget = null;
            gameObject.SetActive(false);
        }

        public void SetFollow(Transform followTargetTmp)
        {
            followTarget = followTargetTmp;
        }

        private void Update()
        {
            if (!followTarget || !gameObject.activeSelf) return;
            transform.position = followTarget.position;
            transform.rotation = followTarget.rotation;
        }
    }
}
