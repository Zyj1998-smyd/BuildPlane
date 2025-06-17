using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.LoadRes;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Ui
{
    public class UiCityLogo : MonoBehaviour
    {
        private Animation uiAni;
        private Image     logoImage;

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
            logoImage = transform.Find("Logo/Image").GetComponent<Image>();
            gameObject.SetActive(false);
        }

        internal void ShowCityLogo(int sceneArtIdTmp)
        {
            LoadResources.XXResourcesLoad(new StringBuilder("City" + sceneArtIdTmp).ToString(), handleTmp =>
            {
                logoImage.sprite = handleTmp;

                gameObject.SetActive(true);
                uiAni["CityLogo"].time = 0;
                uiAni.Play("CityLogo");
                
                _ = waitEnd();
            });
        }

        async UniTask waitEnd()
        {
            _cancellationToken = new CancellationTokenSource();
            await UniTask.Delay(2000, cancellationToken: _cancellationToken.Token);
            
            gameObject.SetActive(false);
        }
        
    }
}