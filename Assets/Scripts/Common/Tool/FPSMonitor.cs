using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace Common.Tool
{
    public class FPSMonitor : MonoBehaviour
    {
        public static FPSMonitor Instance { get; private set; }

        private int   _totalFrames;
        private float _totalTime;
        private bool  _isTracking;

        private CancellationTokenSource _trackingCts;

        void Awake()
        {
            // 单例初始化
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void OnDisable()
        {
            _trackingCts?.Cancel();
            _trackingCts?.Dispose();
            _trackingCts = null;
        }

        // 开始统计（在游戏开始时调用）
        public void StartTracking()
        {
            _totalFrames = 0;
            _totalTime = 0f;
            _isTracking = true;

            // 取消之前的循环（如果存在）
            _trackingCts?.Cancel();
            _trackingCts?.Dispose();
            _trackingCts = new CancellationTokenSource();

            // 启动 UniTask 每帧更新
            UniTaskAsyncEnumerable.EveryUpdate()
                .ForEachAsync(_ =>
                {
                    if (!_isTracking) return;

                    _totalFrames++;
                    _totalTime += Time.deltaTime;
                }, _trackingCts.Token)
                .Forget(); // 忽略未处理的 Task
        }

        // 结束统计并计算（在游戏结束时调用）
        public float StopTracking()
        {
            if (!_isTracking) return -1;

            _trackingCts?.Cancel();
            _trackingCts?.Dispose();
            _trackingCts = null;

            _isTracking = false;
            float averageFPS = _totalFrames / _totalTime;
            return averageFPS;
        }

        public void PauseTracking()
        {
            _isTracking = false;
        }

        public void ResumeTracking()
        {
            _isTracking = true;
        }




    }
}
