using System.Collections.Generic;
using System.Threading;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Guide
{
    public class GuideBattle2 : MonoBehaviour
    {
        /** 画布组件 */
        private Canvas _canvasMe;

        /** 对话 */
        private TextMeshProUGUI _talkText;

        /** 音频组件 */
        private AudioSource _audio;

        /** 引导音效 */
        public AudioClip[] audioGuideStep;

        private Animator guiAni, talkAni;

        private GameObject spurtObj;
        private GameObject spurtGuideObj;
        private GameObject touchMaskObj, touchObj;


        private CancellationTokenSource _cancellationToken;

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }

        internal void Initial()
        {
            _canvasMe = gameObject.GetComponent<Canvas>();
            _canvasMe.worldCamera = GameObject.Find("/CamUi2D").GetComponent<Camera>();

            _talkText = transform.Find("GuideTalk/TalkFrame/TalkDoc").GetComponent<TextMeshProUGUI>();
            _audio = gameObject.GetComponent<AudioSource>();
            guiAni = transform.Find("Guide").GetComponent<Animator>();
            talkAni = transform.Find("GuideTalk").GetComponent<Animator>();
            spurtGuideObj = transform.Find("Guide/BtnSpurt").gameObject;
            spurtGuideObj.SetActive(false);
            touchMaskObj = transform.Find("TouchMask").gameObject;
            touchMaskObj.SetActive(false);
            touchObj = touchMaskObj.transform.Find("Touch").gameObject;
            touchObj.GetComponent<Button>().onClick.AddListener(ButtonNext);
            touchObj.SetActive(false);

            spurtObj = GameObject.Find("Canvas2D").transform.Find("Main/BtnSpurt").gameObject;
        }

        internal void GuideAniStep(int step)
        {
            switch (step)
            {
                case 0:
                    _cancellationToken = new CancellationTokenSource();
                    _ = GuiStep0();
                    break;
                case 1:
                    _cancellationToken = new CancellationTokenSource();
                    _ = GuiStep1();
                    break;
            }

        }

        async UniTask GuiStep0()
        {
            spurtObj.SetActive(false);

            await UniTask.Delay(300, cancellationToken: _cancellationToken.Token);

            Time.timeScale = 0;

            _talkText.text = "Long press—Press and hold the propel button to propel.";
            // _audio.clip = audioGuideStep[0];
            // _audio.Play();

            touchMaskObj.SetActive(true);
            spurtObj.SetActive(true);

            guiAni.gameObject.SetActive(true);
            guiAni.Play("GuideBattle2Hand1");

            await UniTask.Delay(1500, true, cancellationToken: _cancellationToken.Token);
            touchObj.SetActive(true);
        }

        async UniTask GuiStep1()
        {
            DataHelper.CurUserInfoData.isNewUser = 2;
            DataHelper.ModifyLocalData(new List<string>(1) { "isNewUser" }, () => { });
            
            guiAni.gameObject.SetActive(false);
            
            Time.timeScale = 1;
            if (PlayerPrefs.GetInt("QualitySwitch", 0) == 0) FPSMonitor.Instance.StartTracking();
            
            touchMaskObj.SetActive(false);
            touchObj.SetActive(false);
            spurtObj.SetActive(true);
            
            _talkText.text = "Please note that if the energy is exhausted, you will not be able to propel.";
            // _audio.clip = audioGuideStep[1];
            // _audio.Play();
            await UniTask.Delay(2500, true, cancellationToken: _cancellationToken.Token);
            
            _talkText.text = "If you want stronger propulsion, you need to equip better thrusters.";
            // _audio.clip = audioGuideStep[2];
            // _audio.Play();
            await UniTask.Delay(3000, true, cancellationToken: _cancellationToken.Token);
            
            talkAni.Play("GuideBattleTalkEnd");
        }
        
        void ButtonNext()
        {
            GuideAniStep(1);
        }

    }
}
