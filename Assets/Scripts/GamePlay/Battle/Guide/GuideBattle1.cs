using System.Threading;
using Common.Tool;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Guide
{
    public class GuideBattle1 : MonoBehaviour
    {
        /** 画布组件 */
        private Canvas _canvasMe;

        /** 对话 */
        private TextMeshProUGUI _talkText;

        /** 音频组件 */
        private AudioSource _audio;

        /** 引导音效 */
        public AudioClip[] audioGuideStep;

        private Animator guiAni,talkAni;

        private GameObject joystickObj, spurtObj;
        private GameObject joystickGuideObj;
        private GameObject touchMaskObj, touchObj;

        private int setpNow;

        
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
            joystickGuideObj = transform.Find("Guide/Joystick").gameObject;
            joystickGuideObj.SetActive(false);
            touchMaskObj = transform.Find("TouchMask").gameObject;
            touchMaskObj.SetActive(false);
            touchObj = touchMaskObj.transform.Find("Touch").gameObject;
            touchObj.GetComponent<Button>().onClick.AddListener(ButtonNext);
            touchObj.SetActive(false);
            
            joystickObj = GameObject.Find("Canvas2D").transform.Find("Main/JoystickTouch").gameObject;
            spurtObj = GameObject.Find("Canvas2D").transform.Find("Main/BtnSpurt").gameObject;

            // _audio.clip = audioGuideStep[0];
            // _audio.Play();
            GuideAniStep(0);
        }

        internal void GuideAniStep(int step)
        {
            switch (step)
            {
                case 0:
                    guiAni.gameObject.SetActive(true);
                    guiAni.Play("GuideBattle1Hand1");
                    
                    _talkText.text = "Drag the plane down to pull the elastic cord.";
                    break;
                case 1:
                    guiAni.gameObject.SetActive(false);
                    
                    _talkText.text = "Release your finger to launch and take off!";
                    break;
                case 2:
                    _cancellationToken = new CancellationTokenSource();
                    _=GuiStep2();
                    break;
                case 3:
                    _cancellationToken = new CancellationTokenSource();
                    _=GuiStep3();
                    
                    break;
                case 4:
                    guiAni.gameObject.SetActive(false);
                    
                    Time.timeScale = 1;
                    if (PlayerPrefs.GetInt("QualitySwitch", 0) == 0) FPSMonitor.Instance.StartTracking();
                    
                    touchMaskObj.SetActive(false);
                    touchObj.SetActive(false);
                    joystickObj.SetActive(true);
                    talkAni.Play("GuideBattleTalkEnd");
                    break;
            }
        }

        async UniTask GuiStep2()
        {
            joystickObj.SetActive(false);
            spurtObj.SetActive(false);
            
            await UniTask.Delay(300, cancellationToken: _cancellationToken.Token);

            Time.timeScale = 0;
            
            _talkText.text = "Slide the joystick left and right to control the direction of the plane.";
            // _audio.clip = audioGuideStep[1];
            // _audio.Play();
            
            touchMaskObj.SetActive(true);
            joystickGuideObj.SetActive(true);
            
            guiAni.gameObject.SetActive(true);
            guiAni.Play("GuideBattle1Hand2");
            
            await UniTask.Delay(1500, true,cancellationToken: _cancellationToken.Token);
            touchObj.SetActive(true);
        }
        async UniTask GuiStep3()
        {
            touchObj.SetActive(false);
            
            _talkText.text = "Push the joystick up to pull up the plane and delay the landing time.";
            // _audio.clip = audioGuideStep[2];
            // _audio.Play();
            
            guiAni.Play("GuideBattle1Hand3");
            
            await UniTask.Delay(1500, true,cancellationToken: _cancellationToken.Token);
            touchObj.SetActive(true);
        }

        void ButtonNext()
        {
            switch (setpNow)
            {
                case 0:
                    setpNow++;
                    GuideAniStep(3);
                    break;
                case 1:
                    setpNow++;
                    GuideAniStep(4);
                    break;
            }
        }

    }
}
