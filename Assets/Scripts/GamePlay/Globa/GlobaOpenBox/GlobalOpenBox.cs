using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.LoadRes;
using Cysharp.Threading.Tasks;
using Data;
using GamePlay.Main;
using Newtonsoft.Json;
using Platform;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Globa.GlobaOpenBox
{
    public class GlobalOpenBox : MonoBehaviour
    {
        /** 宝箱中部件预制 */
        public ItemOpenBoxRewardUi ItemOpenBoxUiPre;
        /** 部件品质框 */
        public Sprite[] qualityFrames;

        /** 音效 创建宝箱 */
        public AudioClip audioClipBron;
        /** 音效 获得一个部件 */
        public AudioClip audioClipItem;
        /** 音效 打开宝箱(首次打开) */
        public AudioClip audioClipOpen;
        /** 音效 获得额外部件 */
        public AudioClip audioClipOpenAdd;
        /** 音效 打开宝箱(第二次及以后打开) */
        public AudioClip audioClipOnce;
        
        /** 画布 */
        internal Canvas canvasMe;

        /** 宝箱位置点 */
        private Transform _boxPoint;

        /** 宝箱 */
        private Animator _boxAni;
        
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;

        private Animator _frameAnimation;
        private GameObject _btnSkip;
        private GameObject _btnOpen;
        private GameObject _btnContinue;
        private GameObject _rewardItemObj;
        private GameObject _rewardItemAllObj;
        private GameObject _additionalObj;
        private readonly Transform[] _rewardItemPointLists = new Transform[3];
        private readonly Transform[] _rewardItemPoints = new Transform[9];
        private Button _btnGet;
        private Button _btnLost;

        private ItemOpenBoxRewardUi _itemOpenBox;

        private int _totalNum;
        private int _openNum;
        private bool _extraGet;

        private List<int> _boxGetIds = new List<int>();
        private List<string> _boxGetIdIsNew = new List<string>();
        private int _extraGetId;

        private readonly List<int> _boxGetQualitys = new List<int>();
        private int _extraGetQuality;

        private readonly List<ItemOpenBoxRewardUi> _itemOpenBoxUis = new List<ItemOpenBoxRewardUi>();

        /** 开箱背景 */
        private readonly GameObject[] _openBoxBgUis = new GameObject[3];
        /** 箱子光效 */
        private readonly GameObject[] _boxLightEffects = new GameObject[5];

        /** 当前打开的宝箱ID */
        private int _curBoxId;
            
        private void Awake()
        {
            canvasMe = transform.Find("CanvasOpenBox").GetComponent<Canvas>();

            _boxPoint = transform.Find("Point_Box");

            _frameAnimation = canvasMe.transform.Find("Frame").GetComponent<Animator>();
            
            _btnSkip = canvasMe.transform.Find("Frame/FrameT/BtnSkip").gameObject;
            _btnSkip.GetComponent<Button>().onClick.AddListener(OnBtnSkip);
            
            _btnOpen = canvasMe.transform.Find("Frame/BtnOpen").gameObject;
            _btnOpen.GetComponent<Button>().onClick.AddListener(OnBtnOpen);
            
            _btnContinue = canvasMe.transform.Find("Frame/BtnContinue").gameObject;
            _btnContinue.GetComponent<Button>().onClick.AddListener(OnBtnContinue);
            
            _rewardItemObj = canvasMe.transform.Find("Frame/RewardItem").gameObject;
            _rewardItemAllObj = canvasMe.transform.Find("Frame/RewardItemAll").gameObject;
            _additionalObj = canvasMe.transform.Find("Frame/Additional").gameObject;

            _btnGet = _additionalObj.transform.Find("BtnGet").GetComponent<Button>();
            _btnGet.onClick.AddListener(OnBtnGetAdditional);
            _btnLost = _additionalObj.transform.Find("BtnLost").GetComponent<Button>();
            _btnLost.onClick.AddListener(OnbtnLostAdditional);

            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                Transform listTran = _rewardItemAllObj.transform.Find("Frame/List" + (i + 1));
                _rewardItemPointLists[i] = listTran;
                for (int j = 0; j < 3; j++)
                {
                    Transform itemPoint = listTran.Find("ItemPoint" + (j + 1));
                    _rewardItemPoints[index] = itemPoint;
                    index += 1;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                _openBoxBgUis[i] = _boxPoint.Find("OpenBoxBG" + (i + 1)).gameObject;
            }
        }

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }

        /// <summary>
        /// 按钮 获取额外奖励
        /// </summary>
        private void OnBtnGetAdditional()
        {
            DataHelper.CurReportDf_adScene = "GetBoxExtra";
            GameSdkManager._instance._sdkScript.VideoControl("开宝箱获得额外部件", () =>
            {
                _additionalObj.SetActive(false);
                _rewardItemObj.SetActive(false);
                _rewardItemAllObj.SetActive(true);
                _frameAnimation.Play("FrameRewardItemAll", -1, 0);
                AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioGetItem);

                _rewardItemPointLists[0].gameObject.SetActive(true);
                _rewardItemPointLists[1].gameObject.SetActive(false);
                _rewardItemPointLists[2].gameObject.SetActive(false);

                for (int i = 0; i < _rewardItemPoints.Length; i++)
                {
                    if (i == 0)
                    {
                        _rewardItemPoints[i].gameObject.SetActive(true);
                        ItemOpenBoxRewardUi itemOpenBox = Instantiate(ItemOpenBoxUiPre, _rewardItemPoints[i]);
                        itemOpenBox.Initial();
                        itemOpenBox.SetDataAndShow(_extraGetId, "");
                        _itemOpenBoxUis.Add(itemOpenBox);
                    }
                    else
                    {
                        _rewardItemPoints[i].gameObject.SetActive(false);
                    }
                }

                // 保存数据 额外奖励
                List<string> modifyKeys = DataHelper.ModifyEquipments(new List<int>(1) { _extraGetId })[0];

                // 完成日常任务 观看X次广告 TaskID:2
                DataHelper.CompleteDailyTask(2, 1, 0);
                modifyKeys.Add("taskInfo1");
                // 完成成就任务 累计观看X次广告 TaskID:3
                DataHelper.CompleteGloalTask(3, 1);
                modifyKeys.Add("taskInfo2");

                if (modifyKeys.Count > 0) DataHelper.ModifyLocalData(modifyKeys, () => { });

                _btnContinue.SetActive(true);

                // 上报自定义分析数据 事件: 获取宝箱额外奖励
                GameSdkManager._instance._sdkScript.ReportAnalytics("ExtraRewards", "boxId", _curBoxId);

            }, () => { });
        }

        /// <summary>
        /// 按钮 放弃额外奖励
        /// </summary>
        private void OnbtnLostAdditional()
        {
            OpenBoxEnd();
        }

        /// <summary>
        /// 按钮 跳过
        /// </summary>
        private void OnBtnSkip()
        {
            _btnSkip.SetActive(false);
            _btnOpen.SetActive(false);
            _btnContinue.SetActive(false);
            _rewardItemObj.SetActive(false);
            _rewardItemAllObj.SetActive(false);
            _additionalObj.SetActive(false);

            // 如果点击打开一次都没执行过 打开箱子
            if (_openNum == 0)
            {
                SetBoxLightEffect(-1);
                _boxAni.Play("Open", -1, 0);
                _rewardItemObj.SetActive(true);
                if (_itemOpenBox) Destroy(_itemOpenBox.gameObject);
                _itemOpenBox = Instantiate(ItemOpenBoxUiPre, _rewardItemObj.transform);
                _itemOpenBox.gameObject.SetActive(false);
                _itemOpenBox.Initial();
            }

            _openNum = _totalNum + 1;
            _rewardItemAllObj.SetActive(true);
            _frameAnimation.Play("FrameRewardItemAll", -1, 0);
            _ = OpenComplete();
        }

        /// <summary>
        /// 按钮 点击打开
        /// </summary>
        private void OnBtnOpen()
        {
            _btnOpen.SetActive(false);
            _ = OpenFrist();
        }

        /// <summary>
        /// 按钮 点击继续
        /// </summary>
        private void OnBtnContinue()
        {
            if (_openNum <= _totalNum)
            {
                _btnContinue.SetActive(false);
                if (_openNum == _totalNum)
                {
                    _openNum += 1;
                    _rewardItemObj.SetActive(false);
                    _rewardItemAllObj.SetActive(true);
                    _frameAnimation.Play("FrameRewardItemAll", -1, 0);
                    _ = OpenComplete();
                }
                else
                {
                    _ = OpenOne();
                }
            }
            else
            {
                if (!_extraGet)
                {
                    _extraGet = true;
                    if (MainManager._instance._guideMain1 != null)
                    {
                        // 新手引导 跳过额外获取 直接结束
                        OpenBoxEnd();
                    }
                    else
                    {
                        // 非新手引导 正常打开额外获取
                        _btnContinue.SetActive(false);
                        _additionalObj.SetActive(true);
                        _btnGet.interactable = false;
                        _btnLost.interactable = false;
                        _frameAnimation.Play("FrameAdditional", -1, 0);
                        _ = OpenGetExtra();
                    }
                }
                else
                {
                    OpenBoxEnd();
                }
            }
        }

        /// <summary>
        /// 初始化宝箱
        /// </summary>
        /// <param name="boxId">宝箱ID</param>
        internal void InitialOpenBox(int boxId)
        {
            // Debug.Log("开宝箱ID = " + boxId);
            
            _btnSkip.SetActive(false);
            _btnOpen.SetActive(false);
            _btnContinue.SetActive(false);
            _rewardItemObj.SetActive(true);
            _rewardItemAllObj.SetActive(false);
            _additionalObj.SetActive(false);

            _curBoxId = boxId;
            _openNum = 0;
            _extraGet = false;
            DataHelper.GetBoxOpenItems(boxId, (ids, isNews, extraId) =>
            {
                _boxGetIds = new List<int>(ids);
                _boxGetIdIsNew = new List<string>(isNews);
                _extraGetId = extraId;

                _totalNum = _boxGetIds.Count;

                _boxGetQualitys.Clear();
                for (int i = 0; i < _boxGetIds.Count; i++)
                {
                    _boxGetQualitys.Add(ConfigManager.Instance.ComponentConfigDict[_boxGetIds[i]].Quality);
                }

                _extraGetQuality = ConfigManager.Instance.ComponentConfigDict[_extraGetId].Quality;
            });
            
            // Debug.Log(JsonConvert.SerializeObject(_boxGetQualitys));
            // Debug.Log(_extraGetQuality);

            int boxResId = (boxId / 100) - 1;
            
            for (int i = 0; i < _openBoxBgUis.Length; i++)
            {
                _openBoxBgUis[i].SetActive(boxResId == i);
            }
            
            string assetsName = new StringBuilder("Box" + boxResId).ToString();
            LoadResources.XXResourcesLoad(assetsName, handleTmp =>
            {
                GameObject boxTmp = Instantiate(handleTmp, _boxPoint);
                boxTmp.transform.position = Vector3.zero;
                boxTmp.transform.rotation = Quaternion.identity;
                _boxAni = boxTmp.GetComponent<Animator>();
                for (int i = 0; i < 5; i++)
                {
                    _boxLightEffects[i] = boxTmp.transform.Find("Effects/Effect" + (i + 1)).gameObject;
                }
                _ = CreateBoxComplete();
            });
        }

        /// <summary>
        /// 宝箱创建完成
        /// </summary>
        async UniTask CreateBoxComplete()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();

            _boxAni.Play("Bron", -1, 0);
            AudioHandler._instance.PlayAudio(audioClipBron);
            
            float timeTmp = (40 / 60f * 1000);
            await UniTask.Delay((int)timeTmp, cancellationToken: _cancellationToken.Token);

            _btnOpen.SetActive(true);
            _btnSkip.SetActive(true);
        }

        /// <summary>
        /// 第一次打开
        /// </summary>
        async UniTask OpenFrist()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();

            SetBoxLightEffect(_boxGetQualitys[0]);
            _boxAni.Play("Open", -1, 0);
            AudioHandler._instance.PlayAudio(audioClipOpen);
            
            float timeTmp = (50 / 60f * 1000);
            await UniTask.Delay((int)timeTmp, cancellationToken: _cancellationToken.Token);

            if (_itemOpenBox) Destroy(_itemOpenBox.gameObject);
            _itemOpenBox = Instantiate(ItemOpenBoxUiPre, _rewardItemObj.transform);
            _itemOpenBox.gameObject.SetActive(true);
            _itemOpenBox.Initial();

            _itemOpenBox.SetDataAndOpen(_boxGetIds[0], _boxGetIdIsNew[0]);
            GameSdkManager._instance._sdkScript.ShortVibrateControl();
            
            _ = FristComplete();
        }

        /// <summary>
        /// 第一次打开完成
        /// </summary>
        async UniTask FristComplete()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            float timeTmp = (40 / 60f * 1000);
            await UniTask.Delay((int)timeTmp, cancellationToken: _cancellationToken.Token);
            
            _openNum += 1;
            _btnContinue.SetActive(true);
        }

        /// <summary>
        /// 打开一次宝箱 第二次开始及后续
        /// </summary>
        async UniTask OpenOne()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();

            SetBoxLightEffect(_boxGetQualitys[_openNum]);
            _itemOpenBox.gameObject.SetActive(false);
            _boxAni.Play("OpenOne", -1, 0);
            AudioHandler._instance.PlayAudio(audioClipOnce);
            
            float timeTmp = (10 / 60f * 1000);
            await UniTask.Delay((int)timeTmp, cancellationToken: _cancellationToken.Token);
            
            _itemOpenBox.gameObject.SetActive(true);
            _itemOpenBox.SetDataAndOpen(_boxGetIds[_openNum], _boxGetIdIsNew[_openNum]);
            GameSdkManager._instance._sdkScript.ShortVibrateControl();
            
            float timeTmp1 = (15 / 60f * 1000);
            await UniTask.Delay((int)timeTmp1, cancellationToken: _cancellationToken.Token);
            
            _openNum += 1;
            _btnContinue.SetActive(true);
        }

        /// <summary>
        /// 开箱点击动作完成
        /// </summary>
        async UniTask OpenComplete()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();

            _rewardItemPointLists[0].gameObject.SetActive(false);
            _rewardItemPointLists[1].gameObject.SetActive(false);
            _rewardItemPointLists[2].gameObject.SetActive(false);
            switch (_boxGetIds.Count)
            {
                case <= 3:
                    _rewardItemPointLists[0].gameObject.SetActive(true);
                    _rewardItemPointLists[1].gameObject.SetActive(false);
                    _rewardItemPointLists[2].gameObject.SetActive(false);
                    break;
                case <= 6:
                    _rewardItemPointLists[0].gameObject.SetActive(true);
                    _rewardItemPointLists[1].gameObject.SetActive(true);
                    _rewardItemPointLists[2].gameObject.SetActive(false);
                    break;
                case <= 9:
                    _rewardItemPointLists[0].gameObject.SetActive(true);
                    _rewardItemPointLists[1].gameObject.SetActive(true);
                    _rewardItemPointLists[2].gameObject.SetActive(true);
                    break;
            }

            for (int i = 0; i < _rewardItemPoints.Length; i++)
            {
                _rewardItemPoints[i].gameObject.SetActive(i < _boxGetIds.Count);
            }

            for (int i = 0; i < _boxGetIds.Count; i++)
            {
                float timeTmp = (10 / 60f * 1000);
                await UniTask.Delay((int)timeTmp, cancellationToken: _cancellationToken.Token);

                int iTmp = i;
                ItemOpenBoxRewardUi itemOpenBox = Instantiate(ItemOpenBoxUiPre, _rewardItemPoints[iTmp]);
                itemOpenBox.Initial();
                itemOpenBox.SetDataAndShow(_boxGetIds[iTmp], _boxGetIdIsNew[iTmp]);
                _itemOpenBoxUis.Add(itemOpenBox);
                AudioHandler._instance.PlayAudio(audioClipItem);
                GameSdkManager._instance._sdkScript.ShortVibrateControl();
            }
            
            float timeTmp1 = (30 / 60f * 1000);
            await UniTask.Delay((int)timeTmp1, cancellationToken: _cancellationToken.Token);

            _btnContinue.SetActive(true);
        }

        /// <summary>
        /// 打开额外获取部件
        /// </summary>
        async UniTask OpenGetExtra()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            
            float timeTmp = (10 / 60f * 1000);
            await UniTask.Delay((int)timeTmp, cancellationToken: _cancellationToken.Token);

            _rewardItemAllObj.SetActive(false);
            ClearItemOpenBox();
            _rewardItemObj.SetActive(false);
            _itemOpenBox.gameObject.SetActive(false);

            SetBoxLightEffect(_extraGetQuality);
            _boxAni.Play("OpenOne", -1, 0);
            AudioHandler._instance.PlayAudio(audioClipOpenAdd);
            
            float timeTmp0 = (10 / 60f * 1000);
            await UniTask.Delay((int)timeTmp0, cancellationToken: _cancellationToken.Token);
            
            _rewardItemObj.SetActive(true);
            _itemOpenBox.gameObject.SetActive(true);
            _itemOpenBox.SetDataAndOpen(_extraGetId, "");
            GameSdkManager._instance._sdkScript.LongVibrateControl();
            
            float timeTmp1 = (80 / 60f * 1000);
            await UniTask.Delay((int)timeTmp1, cancellationToken: _cancellationToken.Token);

            _btnGet.interactable = true;
            _btnLost.interactable = true;
        }

        /// <summary>
        /// 清空获取部件列表
        /// </summary>
        private void ClearItemOpenBox()
        {
            for (int i = 0; i < _itemOpenBoxUis.Count; i++)
            {
                Destroy(_itemOpenBoxUis[i].gameObject);
            }

            _itemOpenBoxUis.Clear();
        }

        /// <summary>
        /// 开箱流程结束
        /// </summary>
        private void OpenBoxEnd()
        {
            ClearItemOpenBox();
            Destroy(_boxAni.gameObject);
            Destroy(_itemOpenBox.gameObject);
            _boxAni = null;
            _itemOpenBox = null;

            gameObject.SetActive(false);

            // 当前是局内打开宝箱 派发事件通知开箱完成
            if (DataHelper.CurRunScene == "Battle") EventManager.Send(CustomEventType.BoxOpenDone);
            else
            {
                if (MainManager._instance._guideMain1 != null)
                {
                    _ = MainManager._instance._guideMain1.RunGuideStep_3();
                }
            }
        }

        /// <summary>
        /// 设置宝箱光效
        /// </summary>
        /// <param name="index">宝箱光效索引</param>
        private void SetBoxLightEffect(int index)
        {
            for (int i = 0; i < _boxLightEffects.Length; i++)
            {
                _boxLightEffects[i].SetActive(i == index);
            }
        }
    }
}