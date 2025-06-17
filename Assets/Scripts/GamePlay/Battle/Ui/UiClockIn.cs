using System.Text;
using System.Threading;
using Common.GameRoot.AudioHandler;
using Common.LoadRes;
using Cysharp.Threading.Tasks;
using Platform;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Ui
{
    public class UiClockIn : MonoBehaviour
    {
        public  AudioClip AudioClockIn;
        private Animation uiAni;
        private Image     clockInName;
        
        private CancellationTokenSource _cancellationToken;
        
        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }
        
        internal void Initial()
        {
            uiAni = GetComponent<Animation>();
            clockInName = transform.Find("ClockInName").GetComponent<Image>();
            gameObject.SetActive(false);
        }

        internal void ShowClockIn(int clockInIdTmp)
        {
            LoadResources.XXResourcesLoad(new StringBuilder("ClockIn"+clockInIdTmp).ToString(), handleTmp =>
            {
                clockInName.sprite = handleTmp;
                
                gameObject.SetActive(true);
                uiAni["ClockIn"].time = 0;
                uiAni.Play("ClockIn");
                AudioHandler._instance.PlayAudio(AudioClockIn);
                GameSdkManager._instance._sdkScript.ShortVibrateControl();

                _ = waitEnd();
            });
        }

        async UniTask waitEnd()
        {
            _cancellationToken = new CancellationTokenSource();
            await UniTask.Delay(2000,cancellationToken: _cancellationToken.Token);
            gameObject.SetActive(false);
        }
    }
}
