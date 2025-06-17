using System;
using System.Text;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using Data.ConfigData;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Globa.GlobaCanvas
{
    public class GetItem : MonoBehaviour
    {
        /** 图标 */
        private Image _image;
        /** 名称 */
        private TextMeshProUGUI _nameText;
        /** 转化碎片 */
        private GameObject _chipGet;
        /** 关闭按钮 */
        private Button _btnClose;
        /** 数量 */
        private TextMeshProUGUI _numText;
        
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _image = transform.Find("Frame/Image").GetComponent<Image>();
            _nameText = transform.Find("Frame/Name").GetComponent<TextMeshProUGUI>();
            _chipGet = transform.Find("Frame/Debris").gameObject;
            _numText = transform.Find("Frame/Num").GetComponent<TextMeshProUGUI>();
            _btnClose = gameObject.GetComponent<Button>();
            _btnClose.onClick.AddListener(OnBtnClose);
        }

        /// <summary>
        /// 打开 恭喜获得
        /// </summary>
        internal void OpenGetItem()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioGetItem);
            GameSdkManager._instance._sdkScript.LongVibrateControl();
            int type = DataHelper.CurGetItem[0];
            int id = DataHelper.CurGetItem[1];
            int num = DataHelper.CurGetItem[2];
            switch (type)
            {
                case 1:
                    _chipGet.SetActive(false);
                    _numText.gameObject.SetActive(true);
                    ShopConfig config_1 = ConfigManager.Instance.ShopConfigDict[id];
                    _nameText.text = config_1.Name;
                    _numText.text = num.ToString();
                    GameGlobalManager._instance.SetImage(_image, new StringBuilder("IconImage" + id).ToString());
                    break;
                case 2:
                    _numText.gameObject.SetActive(false);
                    ComponentConfig config_2 = ConfigManager.Instance.ComponentConfigDict[id];
                    _nameText.text = config_2.Name;
                    _chipGet.SetActive(num == 10);
                    GameGlobalManager._instance.SetImage(_image, new StringBuilder("IconImage" + config_2.ID).ToString());
                    break;
            }
            
            _btnClose.interactable = false;
            _ = AniPlayComplete();
        }

        /// <summary>
        /// 动画播放完成
        /// </summary>
        async UniTask AniPlayComplete()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            float timeTmp = (70 / 60f * 1000);
            await UniTask.Delay((int)timeTmp, cancellationToken: _cancellationToken.Token);

            _btnClose.interactable = true;
        }

        // ------------------------------------------------- 按钮 -------------------------------------------------
        /// <summary>
        /// 按钮 关闭
        /// </summary>
        private void OnBtnClose()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            GameGlobalManager._instance.OpenGetItem(false);
            EventManager.Send(CustomEventType.GetItemClose);
        }
    }
}