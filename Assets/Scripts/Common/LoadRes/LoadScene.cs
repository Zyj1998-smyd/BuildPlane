using System;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Common.LoadRes
{
    public class LoadScene : MonoBehaviour
    {
        // private AsyncOperation              operation;
        AsyncOperationHandle<SceneInstance> scenehandle;

        private float currProgressA, currProgressB;
        private float currProgressTmp;

        private Animator loadFrameAni;
        // private RectTransform progressSlider;
        private Image progressSlider;
        
        private CancellationTokenSource _cancellationToken;

        private void Awake()
        {
            loadFrameAni = GetComponent<Animator>();
            // progressSlider = transform.Find("ProgressBG/ProgressSlider").GetComponent<RectTransform>();
            // progressSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            progressSlider = transform.Find("ProgressBG/ProgressSlider").GetComponent<Image>();
            progressSlider.fillAmount = 0;
        }

        public void StartLoad()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            // _ = ReleaseAsset();
            // LoadResources.ReleaseAllAsset();
            
            currProgressA = 0;

            progressSlider.fillAmount = 0;

            _ = LoadSceneUi();
            _ = DownloadScene();
        }

        async UniTask DownloadScene()
        {
            var handle = Addressables.LoadSceneAsync(DataHelper.nextSceneName, LoadSceneMode.Single, false);
            handle.Completed += (XXObj) =>
            {
                if (XXObj.Status == AsyncOperationStatus.Succeeded)
                {
                    scenehandle = handle;
                    currProgressTmp = 1;
                }
                else
                {
                    Debug.LogWarning($"场景{DataHelper.nextSceneName}下载{XXObj.Status}");
                }
            };

            while (!handle.IsDone)
            {
                currProgressTmp = handle.PercentComplete;
                await UniTask.Yield(_cancellationToken.Token);
            }
        }

        async UniTask LoadSceneUi()
        {
            loadFrameAni.Play("Open", -1, 0);
            
            while (currProgressA < 2)
            {
                if (currProgressA >= 1)
                {
                    currProgressA = 2;
                    
                    await scenehandle.Result.ActivateAsync();
                }

                if (currProgressA < currProgressTmp)
                {
                    currProgressA += (currProgressTmp - currProgressA) * 0.5f + 0.01f;
                    // progressSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (currProgressA * 0.8f) * 600);
                    progressSlider.fillAmount = currProgressA * 0.5f;
                }

                await UniTask.Yield(_cancellationToken.Token);
            }
        }



        private int loadIngNum;
        private int loadIngNumMax; //总加载步数

        private void OnEnable()
        {
            EventManager.Add(CustomEventType.ResLoadDone, LoadDone);
        }

        private void OnDisable()
        {
            EventManager.Remove(CustomEventType.ResLoadDone, LoadDone);
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }

        public void EndLoad(int loadIngNumMaxTmp, Action callback)
        {
            loadIngNumMax = loadIngNumMaxTmp;
            loadIngNum = 0;
            currProgressB = 0;
            currProgressTmp = 0;

            if (loadIngNumMax == 0)
                currProgressTmp = 1;

            _ = EndLoad(callback);
        }

        async UniTask EndLoad(Action callback)
        {
            while (currProgressB < 2)
            {
                if (currProgressB >= 1 && loadIngNum >= loadIngNumMax)
                {
                    currProgressB = 2;
                    loadFrameAni.Play("Close", -1, 0);
                    _ = ReleaseAsset();
                    callback();
                    await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
                    gameObject.SetActive(false);
                    break;
                }

                if (currProgressB < currProgressTmp)
                {
                    currProgressB += (currProgressTmp - currProgressB) * 0.5f + 0.01f;
                    // progressSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (0.8f + currProgressB * 0.2f) * 600);
                    progressSlider.fillAmount = 0.5f + currProgressB * 0.5f;
                }

                await UniTask.Yield(cancellationToken: _cancellationToken.Token);
            }

        }

        private void LoadDone()
        {
            if (loadIngNum < loadIngNumMax)
            {
                loadIngNum++;
                currProgressTmp = loadIngNum / (float) loadIngNumMax;
            }
        }

        async UniTask ReleaseAsset()
        {
            await Resources.UnloadUnusedAssets(); //卸载未占用的asset资源
            GC.Collect(); //回收内存 
        }
    }
}