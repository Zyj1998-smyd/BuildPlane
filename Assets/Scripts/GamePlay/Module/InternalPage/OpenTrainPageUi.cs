using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Data;
using Data.ConfigData;
using GamePlay.Globa;
using GamePlay.Main;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage
{
    public class OpenTrainPageUi : InternalPageScript
    {
        /** UniTask异步信标 */
        private CancellationTokenSource _cancellationToken;
        
        /** 金币 */
        private TextMeshProUGUI _goldNumText;
        /** 钻石 */
        private TextMeshProUGUI _diamondNumText;

        /** 滑动列表 */
        private ScrollRect _scrollRect;
        /** 滑动列表挂载容器 */
        private Transform _content;

        /** 等级 弹弓强化/喷射推进板/推进环/金币增幅 */
        private readonly TextMeshProUGUI[] _levelNumTexts = new TextMeshProUGUI[4];
        /** 当前等级属性 弹弓强化/喷射推进板/推进环/金币增幅 */
        private readonly TextMeshProUGUI[] _curNumTexts = new TextMeshProUGUI[4];
        /** 下一等级属性 弹弓强化/喷射推进板/推进环/金币增幅 */
        private readonly TextMeshProUGUI[] _nextNumTexts = new TextMeshProUGUI[4];
        /** 属性提升箭头 弹弓强化/喷射推进板/推进环/金币增幅 */
        private readonly GameObject[] _arrows = new GameObject[4];
        /** 升级消耗货币 弹弓强化/喷射推进板/推进环/金币增幅 */
        private readonly TextMeshProUGUI[] _btnUpGradeNumTexts = new TextMeshProUGUI[4];
        /** 升级消耗金币图标 弹弓强化/喷射推进板/推进环/金币增幅 */
        private readonly GameObject[] _btnUpGradeIcons_Gold = new GameObject[4];
        /** 升级消耗钻石图标 弹弓强化/喷射推进板/推进环/金币增幅 */
        private readonly GameObject[] _btnUpGradeIcons_Diamond = new GameObject[4];

        /** 图标 发射器/喷射推进板 */
        private Image _image1_1, _image1_2;
        /** 图标 推进环 */
        private Image _image2;
        /** 图标 金币增幅 */
        private Image _image3;

        /** 升级消耗货币数量 弹弓强化/喷射推进板/推进环/金币增幅 */
        private readonly int[] _upGradeNums = new int[4];
        /** 升级消耗货币类型 弹弓强化/喷射推进板/推进环/金币增幅 */
        private readonly int[] _upGradeTypes = new int[4];

        /** 待修改的数据Key列表 */
        private readonly List<string> _modifyKeys = new List<string>();
        /** 打开弹窗凭证 */
        private bool _isOpenPage;
        
        private void OnEnable()
        {
            EventManager.Add(CustomEventType.RefreshMoney, RefreshMoney);
        }

        private void OnDisable()
        {
            EventManager.Remove(CustomEventType.RefreshMoney, RefreshMoney);
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = null;
        }
        
        public override void Initial()
        {
            base.Initial();

            _scrollRect = transform.Find("ScrollView").GetComponent<ScrollRect>();
            _content = _scrollRect.transform.Find("Viewport/Content");
            
            _goldNumText = _scrollRect.transform.Find("Frame/Money/Gold/Num").GetComponent<TextMeshProUGUI>();
            _diamondNumText = _scrollRect.transform.Find("Frame/Money/Gem/Num").GetComponent<TextMeshProUGUI>();

            Transform trainItem_1 = _content.Find("Item_1");
            _image1_1 = trainItem_1.Find("ImageMask/Image1").GetComponent<Image>();
            _image1_2 = trainItem_1.Find("ImageMask/Image2").GetComponent<Image>();
            Transform trainItem_1_1 = trainItem_1.Find("ItemSub1");
            Transform trainItem_1_2 = trainItem_1.Find("ItemSub2");
            Initial_TrainItem(trainItem_1_1, 0);
            Initial_TrainItem(trainItem_1_2, 1);

            Transform trainItem_2 = _content.Find("Item_2");
            _image2 = trainItem_2.Find("ImageMask/Image").GetComponent<Image>();
            Transform trainItem_2_1 = trainItem_2.Find("ItemSub1");
            Initial_TrainItem(trainItem_2_1, 2);

            Transform trainItem_3 = _content.Find("Item_3");
            _image3 = trainItem_3.Find("ImageMask/Image").GetComponent<Image>();
            Transform trainItem_3_1 = trainItem_3.Find("ItemSub1");
            Initial_TrainItem(trainItem_3_1, 3);
        }

        /// <summary>
        /// 初始化 训练科目
        /// </summary>
        /// <param name="trainItem">训练科目</param>
        /// <param name="index">列表索引</param>
        private void Initial_TrainItem(Transform trainItem, int index)
        {
            _levelNumTexts[index] = trainItem.Find("Tittle/Name/LevelNum").GetComponent<TextMeshProUGUI>();
            _curNumTexts[index] = trainItem.Find("Info/Propety/Num").GetComponent<TextMeshProUGUI>();
            _nextNumTexts[index] = trainItem.Find("Info/Propety/Num/Arrow/Num").GetComponent<TextMeshProUGUI>();
            _arrows[index] = trainItem.Find("Info/Propety/Num/Arrow").gameObject;
            Transform btnUpGrade = trainItem.Find("Info/BtnEquip");
            _btnUpGradeNumTexts[index] = btnUpGrade.Find("Num").GetComponent<TextMeshProUGUI>();
            _btnUpGradeIcons_Gold[index] = btnUpGrade.Find("Num/Icon1").gameObject;
            _btnUpGradeIcons_Diamond[index] = btnUpGrade.Find("Num/Icon2").gameObject;
            btnUpGrade.GetComponent<Button>().onClick.AddListener(() => { OnBtnUpGrade(index); });
        }

        public override void OpenInternalPage()
        {
            base.OpenInternalPage();
            
            AudioHandler._instance.PlayAudio(MainManager._instance.audioPageOpen);
            
            _scrollRect.verticalNormalizedPosition = 1f;
            _isOpenPage = true;
            _modifyKeys.Clear();
            
            RefreshMoney();

            RefreshTrain();
        }

        public override void CloseInternalPage()
        {
            if (_isOpenPage)
            {
                _isOpenPage = false;
                if (_modifyKeys.Count > 0)
                {
                    DataHelper.ModifyLocalData(_modifyKeys, () => { });
                }
            }

            base.CloseInternalPage();
        }
        
        /// <summary>
        /// 刷新货币
        /// </summary>
        private void RefreshMoney()
        {
            _goldNumText.text = ToolFunManager.GetText(DataHelper.CurUserInfoData.gold, false);
            _diamondNumText.text = ToolFunManager.GetText(DataHelper.CurUserInfoData.diamond, false);
        }

        /// <summary>
        /// 刷新训练
        /// </summary>
        private void RefreshTrain()
        {
            for (int i = 0; i < _levelNumTexts.Length; i++)
            {
                int id = (i + 1);
                int additionLv = DataHelper.CurUserInfoData.additions[id];
                int baseNum = GlobalValueManager.TrainBaseNums[i];
                int curAddNum = baseNum + (additionLv - 1);
                int nextAddNum = baseNum + additionLv;
                string curAddNumStr = GlobalValueManager.TrainNumTypes[i] == 1
                    ? curAddNum.ToString()
                    : new StringBuilder(curAddNum + "%").ToString();
                string nextAddNumStr = GlobalValueManager.TrainNumTypes[i] == 1
                    ? nextAddNum.ToString()
                    : new StringBuilder(nextAddNum + "%").ToString();

                int diamondUpGrade = id == 4 ? 10 : 100;
                if ((additionLv + 1) % diamondUpGrade == 0)
                {
                    // 逢10或100升级
                    _btnUpGradeIcons_Gold[i].SetActive(false);
                    _btnUpGradeIcons_Diamond[i].SetActive(true);
                    float upGradeNumTmp = DataHelper.GetTrainUpGradeNum(id, additionLv) / 10f;
                    int upGradeNum = (int)(Math.Ceiling((double)upGradeNumTmp / 10) * 10);
                    _btnUpGradeNumTexts[i].text = ToolFunManager.GetText(upGradeNum, false);
                    _upGradeNums[i] = upGradeNum;
                    _upGradeTypes[i] = 2;
                }
                else
                {
                    // 普通升级
                    _btnUpGradeIcons_Gold[i].SetActive(true);
                    _btnUpGradeIcons_Diamond[i].SetActive(false);
                    int upGradeNum = DataHelper.GetTrainUpGradeNum(id, additionLv);
                    _btnUpGradeNumTexts[i].text = ToolFunManager.GetText(upGradeNum, false);
                    _upGradeNums[i] = upGradeNum;
                    _upGradeTypes[i] = 1;
                }

                _levelNumTexts[i].text = new StringBuilder("Lv." + additionLv).ToString();
                _curNumTexts[i].text = curAddNumStr;
                _nextNumTexts[i].text = nextAddNumStr;

                switch (id)
                {
                    case 1: // 发射器
                        int imageIndex_1 = additionLv / 100 + 1;
                        imageIndex_1 = imageIndex_1 >= 5 ? 5 : imageIndex_1;
                        GameGlobalManager._instance.SetImage(_image1_1, new StringBuilder("IconImageTrain1_" + imageIndex_1).ToString());
                        break;
                    case 2: // 喷射推进板
                        if (additionLv > 1)
                        {
                            int imageIndex_2 = additionLv / 100 + 1;
                            imageIndex_2 = imageIndex_2 >= 5 ? 5 : imageIndex_2;
                            GameGlobalManager._instance.SetImage(_image1_2, new StringBuilder("IconImageTrain2_" + imageIndex_2).ToString());
                        }
                        else GameGlobalManager._instance.SetImage(_image1_2, "Null0");
                        break;
                    case 3: // 推进环
                        GameGlobalManager._instance.SetImage(_image2, "IconImageTrain3_1");
                        break;
                    case 4: // 金币增幅
                        GameGlobalManager._instance.SetImage(_image3, "IconImageTrain4_1");
                        break;
                }
            }
        }

        // ------------------------------------------- 按钮 -------------------------------------------
        /// <summary>
        /// 按钮 升级
        /// </summary>
        /// <param name="index">列表索引</param>
        private void OnBtnUpGrade(int index)
        {
            int curMoneyNum = _upGradeTypes[index] == 1
                ? DataHelper.CurUserInfoData.gold
                : DataHelper.CurUserInfoData.diamond;
            if (curMoneyNum < _upGradeNums[index])
            {
                GameGlobalManager._instance.OpenNoMoney(true, _upGradeTypes[index]);
                return;
            }

            AudioHandler._instance.PlayAudio(MainManager._instance.audioTrainLvUp);
            GameSdkManager._instance._sdkScript.ShortVibrateControl();
            // 消耗货币
            if (_upGradeTypes[index] == 1)
            {
                // 消耗金币
                DataHelper.CurUserInfoData.gold -= _upGradeNums[index];
                _modifyKeys.Add("gold");
            }
            else
            {
                // 消耗钻石
                DataHelper.CurUserInfoData.diamond -= _upGradeNums[index];
                _modifyKeys.Add("diamond");
            }
            // 升级
            DataHelper.CurUserInfoData.additions[(index + 1)] += 1;
            _modifyKeys.Add("additions");
            // 刷新页面
            RefreshTrain();
            EventManager.Send(CustomEventType.RefreshMoney);
            
            // 完成日常任务 升级任意一项强化X次 TaskID:9
            DataHelper.CompleteDailyTask(9, 1, 0);
            _modifyKeys.Add("taskInfo1");
            // 完成成就任务 发射器强化到X级(8)/推进板强化到X级(9)/推进环强化到X级(10)/金币增幅强化到X级(11)
            int taskId = new[] { 8, 9, 10, 11 }[index];
            DataHelper.CompleteGloalTask(taskId, 1);
            _modifyKeys.Add("taskInfo2");
            
            // 上报自定义分析数据 事件: 弹弓强化/喷射推进板强化/推进环强化/金币增幅
            string trainName = new List<string>(4) { "DanGong", "PenShe", "TuiJin", "Gold" }[index];
            string eventName = new StringBuilder("Train_" + trainName).ToString();
            GameSdkManager._instance._sdkScript.ReportAnalytics(eventName, "trainLv", DataHelper.CurUserInfoData.additions[(index + 1)]);
        }
    }
}