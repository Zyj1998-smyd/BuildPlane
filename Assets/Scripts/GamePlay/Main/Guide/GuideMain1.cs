using System;
using System.Collections.Generic;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Cysharp.Threading.Tasks;
using Data;
using GamePlay.Globa;
using GamePlay.Module.InternalPage.ItemPrefabs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Main.Guide
{
    public class GuideMain1 : MonoBehaviour
    {
        /** 引导音效 */
        public AudioClip[] audioGuideStep = new AudioClip[18];
        
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;

        /** 画布组件 */
        private Canvas _canvasMe;

        /** 音频组件 */
        private AudioSource _audio;
        
        /** UI摄像机 */
        private Camera _cameraUi;
        /** 组件画布 */
        private RectTransform _canvasTran;

        /** 对话节点 */
        private RectTransform _guideTalkFrame;
        /** 对话未完提示标 */
        private GameObject _nextTip;
        /** 对话 */
        private TextMeshProUGUI _talkText;
        /** 引导手指 */
        private RectTransform _handTran;
        /** 引导手指点击区域 */
        private RectTransform _handTouchRect;

        /** 背景遮罩按钮 */
        private Button _btnMask;

        /** 引导步骤 */
        private int _guideStep;

        private Transform _btnMenuBuild;
        private Transform _btnMenuMain;
        private Transform _btnStart;
        private Transform _btnBoxTouch;

        private ItemBuildUi _curItemBuildUi;

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
            _canvasMe = gameObject.GetComponent<Canvas>();
            _canvasMe.worldCamera = GameObject.Find("/CamUi2D").GetComponent<Camera>();

            _audio = gameObject.GetComponent<AudioSource>();
            
            _cameraUi = GameObject.Find("/CamUi2D").GetComponent<Camera>();
            _canvasTran = gameObject.GetComponent<RectTransform>();

            _guideTalkFrame = transform.Find("GuideTalk/Frame").GetComponent<RectTransform>();
            _nextTip = transform.Find("GuideTalk/Frame/TalkFrame/Next").gameObject;
            _talkText = transform.Find("GuideTalk/Frame/TalkFrame/TalkDoc").GetComponent<TextMeshProUGUI>();
            _handTran = transform.Find("GuideHand").GetComponent<RectTransform>();
            _handTouchRect = _handTran.transform.Find("Touch").GetComponent<RectTransform>();
            _handTouchRect.gameObject.AddComponent<Button>().onClick.AddListener(GuideClick);

            _btnMask = transform.Find("Mask").gameObject.AddComponent<Button>();
            _btnMask.onClick.AddListener(OnBtnMask);

            Transform canvasTran = GameObject.Find("/CanvasUi2D/").transform;

            _btnMenuBuild = canvasTran.Find("Menu/Label4Off");
            _btnMenuMain = canvasTran.Find("Menu/Label3Off");
            _btnStart = canvasTran.Find("Page/Main/BtnStart");
            _btnBoxTouch = canvasTran.Find("Page/Main/MainBox/Frame/BoxTouchsB/BoxTouch1");

            GuideStepStart();
        }
        
        /// <summary>
        /// 引导点击
        /// </summary>
        private void GuideClick()
        {
            _handTran.gameObject.SetActive(false);
            int guideStep = _guideStep;
            _guideStep = -1;
            switch (guideStep)
            {
                case 2:
                    // 消耗宝箱
                    DataHelper.CurUserInfoData.boxList[0] = null;
                    // 打开宝箱
                    GameGlobalManager._instance.OpenBox(100);
                    // 保存数据
                    DataHelper.ModifyLocalData(new List<string>(1) { "boxsList" }, () => { });
                    // 刷新宝箱
                    MainManager._instance.RefreshBox();
                    break;
                case 4:
                    MainManager._instance.OnBtnMenu(3, 1);
                    _ = DelayRunFun(() => { _ = RunGuideStep_5(); });
                    break;
                case 6:
                    EventManager<ItemBuildUi, bool>.Send(CustomEventType.GuideClickBuild, _curItemBuildUi, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_7(); });
                    break;
                case 7:
                    EventManager<int, bool>.Send(CustomEventType.GuideClickBuild, 2, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_8(); });
                    break;
                case 8:
                    EventManager<ItemBuildUi, bool>.Send(CustomEventType.GuideClickBuild, _curItemBuildUi, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_9(); });
                    break;
                case 9:
                    EventManager<int, bool>.Send(CustomEventType.GuideClickBuild, 3, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_10(); });
                    break;
                case 10:
                    EventManager<ItemBuildUi, bool>.Send(CustomEventType.GuideClickBuild, _curItemBuildUi, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_11(); });
                    break;
                case 11:
                    EventManager<int, bool>.Send(CustomEventType.GuideClickBuild, 1, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_12(); });
                    break;
                case 12:
                    EventManager<ItemBuildUi, bool>.Send(CustomEventType.GuideClickBuild, _curItemBuildUi, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_13(); });
                    break;
                case 13:
                    EventManager<int, bool>.Send(CustomEventType.GuideClickBuild, 4, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_14(); });
                    break;
                case 14:
                    EventManager<ItemBuildUi, bool>.Send(CustomEventType.GuideClickBuild, _curItemBuildUi, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_15(); });
                    break;
                case 15:
                    EventManager<int, bool>.Send(CustomEventType.GuideClickBuild, 5, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_16(); });
                    break;
                case 16:
                    EventManager<ItemBuildUi, bool>.Send(CustomEventType.GuideClickBuild, _curItemBuildUi, true);
                    _ = DelayRunFun(() => { _ = RunGuideStep_17(); });
                    break;
                case 18:
                    MainManager._instance.OnBtnMenu(2, 1);
                    _ = RunGuideStep_19();
                    break;
                case 19:
                    GuideEnd();
                    break;
            }
        }

        /// <summary>
        /// 引导结束
        /// </summary>
        private void GuideEnd()
        {
            // 新手引导完成
            DataHelper.CurUserInfoData.isNewUser = 1;
            DataHelper.ModifyLocalData(new List<string>(1) { "isNewUser" }, () => { });

            EventManager<int>.Send(CustomEventType.GuideComplete, 0);
            
            // Destroy(gameObject);
        }

        /// <summary>
        /// 设置引导手指位置
        /// </summary>
        /// <param name="targetTran">目标位置</param>
        /// <param name="offsetY">Y轴偏移</param>
        private void SetHandPoint(Transform targetTran, float offsetY = 0)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_cameraUi, targetTran.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTran, screenPos, _cameraUi, out var localPositionTmp);
            Vector3 newPos = new Vector3(localPositionTmp.x, localPositionTmp.y + offsetY, 0);
            _handTran.localPosition = newPos;
        }

        /// <summary>
        /// 按钮 背景遮罩
        /// </summary>
        private void OnBtnMask()
        {
            _btnMask.interactable = false;
            switch (_guideStep)
            {
                case 1:
                    _ = RunGuideStep_2();
                    break;
                case 3:
                    _ = RunGuideStep_4();
                    break;
                case 5:
                    _ = RunGuideStep_6();
                    break;
                case 17:
                    _ = RunGuideStep_18();
                    break;
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="index">引导步骤</param>
        private void PlayAudio(int index)
        {
            _audio.Stop();
            // _audio.clip = audioGuideStep[index];
            // _audio.Play();
        }

        /// <summary>
        /// 延迟执行方法
        /// </summary>
        /// <param name="cb">待执行的方法</param>
        async UniTask DelayRunFun(Action cb)
        {
            _cancellationToken = new CancellationTokenSource();
            await UniTask.Delay(100, cancellationToken: _cancellationToken.Token);
            cb();
        }

        #region 步骤

        /// <summary>
        /// 引导开始
        /// </summary>
        internal void GuideStepStart()
        {
            _guideStep = -1;
            _handTran.gameObject.SetActive(false);
            _guideTalkFrame.gameObject.SetActive(true);
            
            _cancellationToken = new CancellationTokenSource();
            _ = RunGuideStep_1();
        }

        /// <summary>
        /// 引导步骤 1
        /// </summary>
        async UniTask RunGuideStep_1()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();

            Vector2 pos = _guideTalkFrame.anchoredPosition;
            pos.y = -330;
            _guideTalkFrame.anchoredPosition = pos;
            PlayAudio(0);
            _talkText.text = "The first flight doesn't seem to be very far, but that's okay;";
            _nextTip.SetActive(true);
            _btnMask.interactable = true;
            _guideStep = 1;

            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        /// <summary>
        /// 引导步骤 2
        /// </summary>
        async UniTask RunGuideStep_2()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(1);
            _talkText.text = "Let's get some new aircraft parts.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            SetHandPoint(_btnBoxTouch, _handTouchRect.rect.height / 2);
            _guideStep = 2;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        /// <summary>
        /// 引导步骤 3
        /// </summary>
        internal async UniTask RunGuideStep_3()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(2);
            _talkText.text = "Great! It seems that we got some good parts. Let's assemble them!";
            _nextTip.SetActive(true);
            _btnMask.interactable = true;
            _guideStep = 3;

            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        /// <summary>
        /// 引导步骤 4
        /// </summary>
        async UniTask RunGuideStep_4()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(3);
            _talkText.text = "Yes, it doesn't matter if you get duplicate parts, they will automatically be converted into part upgrade fragments.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            SetHandPoint(_btnMenuBuild);
            _guideStep = 4;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        /// <summary>
        /// 引导步骤 5
        /// </summary>
        async UniTask RunGuideStep_5()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            Vector2 pos = _guideTalkFrame.anchoredPosition;
            pos.y = -20;
            _guideTalkFrame.anchoredPosition = pos;
            
            await UniTask.Delay(500, cancellationToken: _cancellationToken.Token);

            PlayAudio(4);
            _talkText.text = "First, determine the fuselage parts.";
            _nextTip.SetActive(true);
            _btnMask.interactable = true;
            _guideStep = 5;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        /// <summary>
        /// 引导步骤 6
        /// </summary>
        async UniTask RunGuideStep_6()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(5);
            _talkText.text = "Select the newly obtained fuselage.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform itemTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/ListFrame/List/Viewport/Content/ItemBuildList1/Point_2").transform;
            _curItemBuildUi = itemTran.Find("ItemBuild2").GetComponent<ItemBuildUi>();
            SetHandPoint(itemTran);
            _guideStep = 6;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        /// <summary>
        /// 引导步骤 7
        /// </summary>
        async UniTask RunGuideStep_7()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(6);
            _talkText.text = "Next, select the left wing.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform tittleTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/Label/LabelA/Label3").transform;
            SetHandPoint(tittleTran);
            _guideStep = 7;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        /// <summary>
        /// 引导步骤 8
        /// </summary>
        async UniTask RunGuideStep_8()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            await UniTask.Delay(100, cancellationToken: _cancellationToken.Token);
            
            PlayAudio(7);
            _talkText.text = "Select the newly obtained left wing.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform itemTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/ListFrame/List/Viewport/Content/ItemBuildList1/Point_2").transform;
            _curItemBuildUi = itemTran.Find("ItemBuild2").GetComponent<ItemBuildUi>();
            SetHandPoint(itemTran);
            _guideStep = 8;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }
        
        /// <summary>
        /// 引导步骤 9
        /// </summary>
        async UniTask RunGuideStep_9()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(8);
            _talkText.text = "Next, select the right wing.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform tittleTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/Label/LabelA/Label4").transform;
            SetHandPoint(tittleTran);
            _guideStep = 9;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }
        
        /// <summary>
        /// 引导步骤 10
        /// </summary>
        async UniTask RunGuideStep_10()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            await UniTask.Delay(100, cancellationToken: _cancellationToken.Token);
            
            PlayAudio(9);
            _talkText.text = "Select the newly obtained right wing. Keep the left and right wings consistent so that you ";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform itemTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/ListFrame/List/Viewport/Content/ItemBuildList1/Point_2").transform;
            _curItemBuildUi = itemTran.Find("ItemBuild2").GetComponent<ItemBuildUi>();
            SetHandPoint(itemTran);
            _guideStep = 10;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }
        
        /// <summary>
        /// 引导步骤 11
        /// </summary>
        async UniTask RunGuideStep_11()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(10);
            _talkText.text = "Next, select the nose part.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform tittleTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/Label/LabelB/Label2").transform;
            SetHandPoint(tittleTran);
            _guideStep = 11;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }
        
        /// <summary>
        /// 引导步骤 12
        /// </summary>
        async UniTask RunGuideStep_12()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            await UniTask.Delay(100, cancellationToken: _cancellationToken.Token);
            
            PlayAudio(11);
            _talkText.text = "Select the newly obtained nose.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform itemTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/ListFrame/List/Viewport/Content/ItemBuildList1/Point_2").transform;
            _curItemBuildUi = itemTran.Find("ItemBuild2").GetComponent<ItemBuildUi>();
            SetHandPoint(itemTran);
            _guideStep = 12;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }
        
        /// <summary>
        /// 引导步骤 13
        /// </summary>
        async UniTask RunGuideStep_13()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(12);
            _talkText.text = "Also select the tail part.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform tittleTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/Label/LabelB/Label5").transform;
            SetHandPoint(tittleTran);
            _guideStep = 13;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }
        
        /// <summary>
        /// 引导步骤 14
        /// </summary>
        async UniTask RunGuideStep_14()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            await UniTask.Delay(100, cancellationToken: _cancellationToken.Token);
            
            PlayAudio(13);
            _talkText.text = "Select the newly obtained tail.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform itemTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/ListFrame/List/Viewport/Content/ItemBuildList1/Point_2").transform;
            _curItemBuildUi = itemTran.Find("ItemBuild2").GetComponent<ItemBuildUi>();
            SetHandPoint(itemTran);
            _guideStep = 14;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }
        
        /// <summary>
        /// 引导步骤 15
        /// </summary>
        async UniTask RunGuideStep_15()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(14);
            _talkText.text = "Finally, select the propeller part.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform tittleTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/Label/LabelB/Label6").transform;
            SetHandPoint(tittleTran);
            _guideStep = 15;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }
        
        /// <summary>
        /// 引导步骤 16
        /// </summary>
        async UniTask RunGuideStep_16()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            await UniTask.Delay(100, cancellationToken: _cancellationToken.Token);
            
            PlayAudio(15);
            _talkText.text = "Select the newly obtained propeller.";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            Transform itemTran = GameObject.Find("/CanvasUi2D/Page/PageBuild(Clone)/BuildList/ListFrame/List/Viewport/Content/ItemBuildList1/Point_2").transform;
            _curItemBuildUi = itemTran.Find("ItemBuild2").GetComponent<ItemBuildUi>();
            SetHandPoint(itemTran);
            _guideStep = 16;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        /// <summary>
        /// 引导步骤 17
        /// </summary>
        async UniTask RunGuideStep_17()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(16);
            _talkText.text = "Assembly completed! Looks great!";
            _nextTip.SetActive(true);
            _btnMask.interactable = true;
            _guideStep = 17;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        /// <summary>
        /// 引导步骤 18
        /// </summary>
        async UniTask RunGuideStep_18()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            PlayAudio(17);
            _talkText.text = "Quick, let's fly again!";
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            SetHandPoint(_btnMenuMain);
            _guideStep = 18;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        /// <summary>
        /// 引导步骤 19
        /// </summary>
        async UniTask RunGuideStep_19()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();

            _guideTalkFrame.gameObject.SetActive(false);
            
            _nextTip.SetActive(false);
            _handTran.gameObject.SetActive(true);
            SetHandPoint(_btnStart);
            _guideStep = 19;
            
            await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token);
        }

        #endregion
    }
}