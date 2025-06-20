using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using Data.ClassData;
using Data.ConfigData;
using GamePlay.Globa;
using GamePlay.Main;
using Newtonsoft.Json;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.OpenBox
{
    public class OpenBoxPageUi : MonoBehaviour
    {
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;
        
        /** 解锁宝箱 */
        private GameObject _unlockObj;
        /** 打开宝箱 */
        private GameObject _advanceObj;

        /** 解锁宝箱 标题 */
        private readonly GameObject[] _tittleTexts_Unlock = new GameObject[3];
        /** 解锁宝箱 详情 */
        private readonly GameObject[] _boxInfos_Unlock = new GameObject[3];
        /** 解锁宝箱 立刻打开消耗钻石 */
        private TextMeshProUGUI _btnOpenNowGemNumText_Unlock;
        /** 解锁宝箱 普通打开消耗时间 */
        private TextMeshProUGUI _btnOpenTimeText_Unlock;

        /** 打开宝箱 标题 */
        private readonly GameObject[] _tittleTexts_advance = new GameObject[3];
        /** 打开宝箱 详情 */
        private readonly GameObject[] _boxInfos_advance = new GameObject[3];
        /** 打开宝箱 提前打开消耗钻石 */
        private TextMeshProUGUI _btnOpenNowGemNumText_advance;
        /** 打开宝箱 普通打开剩余时间 */
        private TextMeshProUGUI _timeNumText;

        /** 打开宝箱 提前打开 */
        private GameObject _btnOpenNow_Advance;
        /** 打开宝箱 立即打开(倒计时结束) */
        private GameObject _btnOpen_Advance;

        /** 立刻打开消耗钻石 */
        private int _openNowGemNum;
        /** 立刻打开消耗时间 */
        private int _openNowTimeNum;
        
        /** 刷新倒计时 */
        private int _timeNum;
        
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
            transform.Find("Mask").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            _unlockObj = transform.Find("Unlock").gameObject;
            _advanceObj = transform.Find("Advance").gameObject;
            _unlockObj.transform.Find("Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            _advanceObj.transform.Find("Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);

            for (int i = 0; i < 3; i++)
            {
                _tittleTexts_Unlock[i] = _unlockObj.transform.Find("Tittle/Text" + (i + 1)).gameObject;
                _boxInfos_Unlock[i] = _unlockObj.transform.Find("Frame/BoxInfo" + (i + 1)).gameObject;
                _tittleTexts_advance[i] = _advanceObj.transform.Find("Tittle/Text" + (i + 1)).gameObject;
                _boxInfos_advance[i] = _advanceObj.transform.Find("Frame/BoxInfo" + (i + 1)).gameObject;
            }

            Transform btnOpenNow_Unlock = _unlockObj.transform.Find("BtnOpenNow");
            btnOpenNow_Unlock.GetComponent<Button>().onClick.AddListener(OnBtnOpenNow_Unlock);
            _btnOpenNowGemNumText_Unlock = btnOpenNow_Unlock.Find("Frame/Gem").GetComponent<TextMeshProUGUI>();
            
            Transform btnOpen_Unlock = _unlockObj.transform.Find("BtnOpen");
            btnOpen_Unlock.GetComponent<Button>().onClick.AddListener(OnBtnOpen_Unlock);
            _btnOpenTimeText_Unlock = btnOpen_Unlock.Find("Frame/Time").GetComponent<TextMeshProUGUI>();

            _btnOpenNow_Advance = _advanceObj.transform.Find("BtnOpenNow").gameObject;
            _btnOpenNow_Advance.GetComponent<Button>().onClick.AddListener(OnBtnOpenNow_Advance);
            _btnOpenNowGemNumText_advance = _btnOpenNow_Advance.transform.Find("Frame/Gem").GetComponent<TextMeshProUGUI>();

            _btnOpen_Advance = _advanceObj.transform.Find("BtnOpen").gameObject;
            _btnOpen_Advance.GetComponent<Button>().onClick.AddListener(OnBtnOpen_Advance);

            _timeNumText = _advanceObj.transform.Find("Frame/Time").GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// 打开 开箱
        /// </summary>
        internal void OpenOpenBox()
        {
            RefreshOpenBox();
        }

        /// <summary>
        /// 刷新开箱
        /// </summary>
        private void RefreshOpenBox()
        {
            int boxSlotIndex = DataHelper.CurOpenBoxInfo[0];
            int boxId = int.Parse(DataHelper.CurUserInfoData.boxList[boxSlotIndex][0]);
            long boxTime = long.Parse(DataHelper.CurUserInfoData.boxList[boxSlotIndex][1]);

            int boxLv = (boxId / 100) - 1;
            RewardBoxConfig rewardBoxConfig = ConfigManager.Instance.RewardBoxConfigDict[boxId];
            _openNowTimeNum = rewardBoxConfig.OpenTime;
            _openNowGemNum = rewardBoxConfig.OpenGem;

            for (int i = 0; i < _tittleTexts_Unlock.Length; i++)
            {
                _tittleTexts_Unlock[i].SetActive(i == boxLv);
                _boxInfos_Unlock[i].SetActive(i == boxLv);
                
                _tittleTexts_advance[i].SetActive(i == boxLv);
                _boxInfos_advance[i].SetActive(i == boxLv);
            }

            _btnOpenTimeText_Unlock.text = new StringBuilder(_openNowTimeNum + "Min").ToString();
            _btnOpenNowGemNumText_Unlock.text = _openNowGemNum.ToString();
            _btnOpenNowGemNumText_advance.text = _openNowGemNum.ToString();
            
            if (boxTime <= 0)
            {
                _unlockObj.SetActive(true);
                _advanceObj.SetActive(false);
            }
            else
            {
                _unlockObj.SetActive(false);
                _advanceObj.SetActive(true);
                _btnOpenNow_Advance.SetActive(true);
                _btnOpen_Advance.SetActive(false);

                StartRefreshTime(boxTime, _openNowTimeNum);
            }
        }

        /// <summary>
        /// 刷新时间
        /// <param name="boxTimeTmp">宝箱解锁启动时间</param>
        /// <param name="timeTmp">宝箱解锁需要时间</param>
        /// </summary>
        private void StartRefreshTime(long boxTimeTmp, int timeTmp)
        {
            long nextTime = boxTimeTmp + timeTmp * 60;
            long subTime = nextTime - ToolFunManager.GetCurrTime();
            int minute = (int)(subTime / 60 % 60);
            int second = (int)(subTime % 60);
            _timeNumText.text = new StringBuilder("Left Time:" + $"{minute:D2}:{second:D2}").ToString();
            
            _timeNum = (int)subTime;
            _ = RefreshTime();
        }

        /// <summary>
        /// 刷新倒计时
        /// </summary>
        async UniTask RefreshTime()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            int openNowGemNum = _openNowGemNum;
            while (_timeNum > 0)
            {
                int minute = _timeNum / 60 % 60;
                int second = _timeNum % 60;
                _timeNumText.text = new StringBuilder("Left Time:" + $"{minute:D2}:{second:D2}").ToString();
                _timeNum -= 1;
                _openNowGemNum = Mathf.FloorToInt(((float)_timeNum / (_openNowTimeNum * 60)) * openNowGemNum);
                _btnOpenNowGemNumText_advance.text = _openNowGemNum.ToString();
                
                await UniTask.Delay(1000, true, cancellationToken: _cancellationToken.Token);
            }
            
            _timeNum = 0;
            _timeNumText.text = "";
            _btnOpenNow_Advance.SetActive(false);
            _btnOpen_Advance.SetActive(true);
        }

        // ------------------------------------------------------ 按钮 ------------------------------------------------------
        /// <summary>
        /// 按钮 关闭
        /// </summary>
        private void OnBtnClose()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            MainManager._instance.OnOpenPop_OpenBox(false);
        }

        /// <summary>
        /// 按钮 解锁宝箱 立刻打开
        /// </summary>
        private void OnBtnOpenNow_Unlock()
        {
            if (DataHelper.CurUserInfoData.diamond < _openNowGemNum)
            {
                GameGlobalManager._instance.OpenNoMoney(true, 2);
                return;
            }
         
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            
            // 消耗钻石
            DataHelper.CurUserInfoData.diamond -= _openNowGemNum;
            // 消耗宝箱
            DataHelper.CurUserInfoData.boxList[DataHelper.CurOpenBoxInfo[0]] = null;
            
            // 完成日常任务 打开X个部件宝箱 TaskID:4
            DataHelper.CompleteDailyTask(4, 1, 0);
            // 完成成就任务 累计打开X个部件宝箱 TaskID:5
            DataHelper.CompleteGloalTask(5, 1);
            
            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(2) { "diamond", "boxsList", "taskInfo1", "taskInfo2" }, () => { });

            // 关闭弹窗
            MainManager._instance.OnOpenPop_OpenBox(false);
            // 打开宝箱
            GameGlobalManager._instance.OpenBox(DataHelper.CurOpenBoxInfo[1]);

            // 刷新宝箱
            MainManager._instance.RefreshBox();
            // 刷新货币
            EventManager.Send(CustomEventType.RefreshMoney);
            
            // 上报自定义分析数据 事件: 提前打开宝箱
            GameSdkManager._instance._sdkScript.ReportAnalytics("OpenBox_Advance", "boxId", DataHelper.CurOpenBoxInfo[1]);
        }

        /// <summary>
        /// 按钮 解锁宝箱 普通打开
        /// </summary>
        private void OnBtnOpen_Unlock()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            
            // 刷新宝箱
            DataHelper.CurUserInfoData.boxList[DataHelper.CurOpenBoxInfo[0]] = new[]
            {
                DataHelper.CurOpenBoxInfo[1].ToString(), JsonConvert.SerializeObject(ToolFunManager.GetCurrTime())
            };
            
            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(1) { "boxsList" }, () => { });
            
            // 刷新宝箱
            MainManager._instance.RefreshBox();
            
            // 关闭弹窗
            MainManager._instance.OnOpenPop_OpenBox(false);
        }

        /// <summary>
        /// 按钮 打开宝箱 提前打开
        /// </summary>
        private void OnBtnOpenNow_Advance()
        {
            if (DataHelper.CurUserInfoData.diamond < _openNowGemNum)
            {
                GameGlobalManager._instance.OpenNoMoney(true, 2);
                return;
            }
            
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            
            // 消耗钻石
            DataHelper.CurUserInfoData.diamond -= _openNowGemNum;
            // 消耗宝箱
            DataHelper.CurUserInfoData.boxList[DataHelper.CurOpenBoxInfo[0]] = null;
            
            // 完成日常任务 打开X个部件宝箱 TaskID:4
            DataHelper.CompleteDailyTask(4, 1, 0);
            // 完成成就任务 累计打开X个部件宝箱 TaskID:5
            DataHelper.CompleteGloalTask(5, 1);
            
            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(2) { "diamond", "boxsList", "taskInfo1", "taskInfo2" }, () => { });
            
            // 关闭弹窗
            MainManager._instance.OnOpenPop_OpenBox(false);
            // 打开宝箱
            GameGlobalManager._instance.OpenBox(DataHelper.CurOpenBoxInfo[1]);
            
            // 刷新宝箱
            MainManager._instance.RefreshBox();
            // 刷新货币
            EventManager.Send(CustomEventType.RefreshMoney);
            
            // 上报自定义分析数据 事件: 提前打开宝箱
            GameSdkManager._instance._sdkScript.ReportAnalytics("OpenBox_Advance", "boxId", DataHelper.CurOpenBoxInfo[1]);
        }

        /// <summary>
        /// 按钮 打开宝箱 立即打开(倒计时结束)
        /// </summary>
        private void OnBtnOpen_Advance()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            
            // 消耗宝箱
            DataHelper.CurUserInfoData.boxList[DataHelper.CurOpenBoxInfo[0]] = null;
            
            // 完成日常任务 打开X个部件宝箱 TaskID:4
            DataHelper.CompleteDailyTask(4, 1, 0);
            // 完成成就任务 累计打开X个部件宝箱 TaskID:5
            DataHelper.CompleteGloalTask(5, 1);
            
            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(1) { "boxsList", "taskInfo1", "taskInfo2" }, () => { });
            
            // 关闭弹窗
            MainManager._instance.OnOpenPop_OpenBox(false);
            // 打开宝箱
            GameGlobalManager._instance.OpenBox(DataHelper.CurOpenBoxInfo[1]);
            
            // 刷新宝箱
            MainManager._instance.RefreshBox();
            
            // 上报自定义分析数据 事件: 正常打开宝箱
            GameSdkManager._instance._sdkScript.ReportAnalytics("OpenBox_Common", "boxId", DataHelper.CurOpenBoxInfo[1]);
        }
    }
}