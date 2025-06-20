using System;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Cysharp.Threading.Tasks;
using GamePlay.Module.InternalPage;
using Platform;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Common.GameRoot
{
    public class GameRootLoad : MonoBehaviour
    {
        public static GameRootLoad Instance;
        
        // private AsyncOperation              operation;
        AsyncOperationHandle<SceneInstance> scenehandle;
        
        private float currProgressA, currProgressB;
        private float currProgressTmp;
        
        private Animator loadFrameAni;
        private Image progressSlider;
        
        private int loadIngNum;
        private int loadIngNumMax; //总加载步数
        
        private OpenNamePageUi openNamePageUi;

        private CancellationTokenSource _cancellationToken;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
            
            loadFrameAni = GetComponent<Animator>();
            progressSlider = transform.Find("ProgressBG/ProgressSlider").GetComponent<Image>();
            // progressSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            openNamePageUi = transform.Find("PageName").GetComponent<OpenNamePageUi>();
            openNamePageUi.Initial();
            progressSlider.fillAmount = 0;
        }

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
        
        public void StartLoad(string nextSceneName)
        {
            _cancellationToken = new CancellationTokenSource();
            
            currProgressA = 0;
            progressSlider.fillAmount = 0;

            _ = LoadSceneUi(nextSceneName);
        }
        
        async UniTask DownloadScene(string nextSceneName)
        {
            // operation = SceneManager.LoadSceneAsync(nextSceneName);
            // operation.allowSceneActivation = false;
            //
            // while (operation.progress < 0.9f)
            // {
            //     currProgressTmp = operation.progress;
            //     await UniTask.Yield();
            // }
            // currProgressTmp = 1;
            
            var handle = Addressables.LoadSceneAsync(nextSceneName, LoadSceneMode.Single, false);
            handle.Completed += (XXObj) =>
            {
                if (XXObj.Status == AsyncOperationStatus.Succeeded)
                {
                    scenehandle = handle;
                    currProgressTmp = 1;
                }
                else
                {
                    Debug.LogWarning($"场景{nextSceneName}下载{XXObj.Status}");
                }
            };

            while (!handle.IsDone)
            {
                currProgressTmp = handle.PercentComplete;
                await UniTask.Yield(_cancellationToken.Token);
            }
        }
        
        async UniTask LoadSceneUi(string nextSceneName)
        {
            _ = UniTask.DelayFrame(60,cancellationToken: _cancellationToken.Token);
            
            _ = DownloadScene(nextSceneName);
            
            while (currProgressA < 2)
            {
                if (currProgressA >= 1)
                {
                    currProgressA = 2;
                    // operation.allowSceneActivation = true;
                    await scenehandle.Result.ActivateAsync();
                }

                if (currProgressA < currProgressTmp)
                {
                    currProgressA += (currProgressTmp - currProgressA) * 0.1f + 0.01f;
                    // progressSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (currProgressA * 0.1f) * 600);
                    progressSlider.fillAmount = currProgressA * 0.5f;
                }
                await UniTask.Yield(cancellationToken: _cancellationToken.Token);
            }
        }
        
        public void EndLoad(int loadIngNumMaxTmp, Action callback)
        {
            currProgressTmp = 0;
            loadIngNumMax = loadIngNumMaxTmp;
            loadIngNum = 0;
            currProgressB = 0;

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
                    // StartCoroutine(ReleaseAsset());
                    loadFrameAni.Play("Close", -1, 0);
                    callback();
                    await UniTask.DelayFrame((int)(30 / 60f * 1000), cancellationToken: _cancellationToken.Token);
                    Destroy(gameObject);
                    break;
                }

                if (currProgressB < currProgressTmp)
                {
                    currProgressB += (currProgressTmp - currProgressB) * 0.2f + 0.01f;
                    // progressSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (0.5f + currProgressB * 0.2f) * 600);
                    progressSlider.fillAmount = 0.5f + currProgressB * 0.5f;
                }

                await UniTask.Yield(_cancellationToken.Token);
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
        
        public void ShowCreateNameUI()
        {
            openNamePageUi.gameObject.SetActive(true);
        }
    }
}