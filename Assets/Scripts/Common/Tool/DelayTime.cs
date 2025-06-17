using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.Tool
{
    public class DelayTime : MonoBehaviour
    {
        /// <summary>
        /// 延迟指定秒数执行 受时间缩放影响
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delaySeconds"></param>
        /// <param name="cancellationToken"></param>
        async public static void DelaySeconds(Action action, float delaySeconds, CancellationToken cancellationToken)
        {
            bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), ignoreTimeScale: false, cancellationToken: cancellationToken).SuppressCancellationThrow();
            if (isCanceled) return;

            action();
        }

        /// <summary>
        /// 延迟指定帧数执行
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayFrame"></param>
        /// <param name="cancellationToken"></param>
        async public static void DelayFrame(Action action, int delayFrame, CancellationToken cancellationToken)
        {
            bool isCanceled = await UniTask.DelayFrame(delayFrame, cancellationToken: cancellationToken).SuppressCancellationThrow();
            if (isCanceled) return;

            action();
        }
        
        /// <summary>
        /// 延迟指定秒数执行 不受时间缩放影响
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delaySeconds"></param>
        /// <param name="cancellationToken"></param>
        async public static void DelaySecondsNoTimeScale(Action action, float delaySeconds, CancellationToken cancellationToken)
        {
            bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), ignoreTimeScale: true, cancellationToken: cancellationToken).SuppressCancellationThrow();
            if (isCanceled) return;

            action();
        }
    }
}