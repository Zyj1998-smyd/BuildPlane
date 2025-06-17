using System.Collections.Generic;
using Common.Event;
using Common.Event.CustomEnum;
using Common.GameRoot.AudioHandler;
using Common.Tool;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Battle.Ui
{
    public class UiGuide : MonoBehaviour
    {
        /** 音效 新手引导步骤1 */
        public AudioClip GuideAudio1;
        /** 音效 新手引导步骤2 */
        public AudioClip GuideAudio2;
        /** 音效 新手引导步骤3-1 */
        public AudioClip GuideAudio3_1;
        /** 音效 新手引导步骤3-2 */
        public AudioClip GuideAudio3_2;
        /** 音效 新手引导步骤4 */
        public AudioClip GuideAudio4;
        /** 音效 新手引导步骤5 */
        public AudioClip GuideAudio5;
        
        /** 引导步骤 */
        private List<GameObject> _guideObjs;

        /** 新手引导步骤3 子步骤1/2/3 */
        private GameObject _guideStep3_1, _guideStep3_2, _guideStep3_3;
        /** 新手引导步骤4 子步骤1/2 */
        private GameObject _guideStep4_1, _guideStep4_2;
        /** 新手引导步骤5 子步骤1/2 */
        private GameObject _guideStep5_1, _guideStep5_2;

        /** 引导音效2时长 单位秒 */
        private readonly int _guideAudioTime2 = 3;
        /** 引导音效3_1时长 单位秒 */
        private readonly int _guideAudioTime3_1 = 6;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initial()
        {
            _guideObjs = new List<GameObject>(5);
            for (int i = 0; i < 5; i++)
            {
                var guideObj = transform.Find("Guide" + (i + 1)).gameObject;
                guideObj.SetActive(false);
                _guideObjs.Add(guideObj);
            }

            _guideStep3_1 = _guideObjs[2].transform.Find("Setp1").gameObject;
            _guideStep3_1.SetActive(false);
            _guideStep3_2 = _guideObjs[2].transform.Find("Setp2").gameObject;
            _guideStep3_2.SetActive(false);
            _guideStep3_3 = _guideObjs[2].transform.Find("Setp3").gameObject;
            _guideStep3_3.SetActive(false);
            _guideStep3_2.transform.Find("GuideBtn").GetComponent<Button>().onClick.AddListener(OnBtnGuideStep3_2);
            _guideStep3_3.transform.Find("GuideBtn").GetComponent<Button>().onClick.AddListener(OnBtnGuideStep3_3);

            _guideStep4_1 = _guideObjs[3].transform.Find("Setp1").gameObject;
            _guideStep4_1.SetActive(false);
            _guideStep4_2 = _guideObjs[3].transform.Find("Setp2").gameObject;
            _guideStep4_2.SetActive(false);
            _guideStep4_1.transform.Find("GuideBtn").GetComponent<Button>().onClick.AddListener(OnBtnGuideStep4_1);
            _guideStep4_2.transform.Find("GuideBtn").GetComponent<Button>().onClick.AddListener(OnBtnGuideStep4_2);

            _guideStep5_1 = _guideObjs[4].transform.Find("Setp1").gameObject;
            _guideStep5_1.SetActive(false);
            _guideStep5_2 = _guideObjs[4].transform.Find("Setp2").gameObject;
            _guideStep5_2.SetActive(false);
            _guideStep5_1.transform.Find("GuideBtn").GetComponent<Button>().onClick.AddListener(OnBtnGuideStep5_1);
            _guideStep5_2.transform.Find("GuideBtn").GetComponent<Button>().onClick.AddListener(OnBtnGuideStep5_2);
        }

        // -------------------------------------------------------- 引导按钮 --------------------------------------------------------
        /// <summary>
        /// 引导按钮 点击道具1 清空备料杯
        /// </summary>
        private void OnBtnGuideStep3_2()
        {
            // 隐藏引导节点3_2
            _guideStep3_2.SetActive(false);
            // 打开清空备料杯道具使用页
            UiBattle._instance._isClickOpenUseItem = false;
            UiBattle._instance.OnBtnOpenOperation(true, 0);
            // 延迟1帧 展示引导节点3_3
            DelayTime.DelayFrame(() =>
            {
                _guideStep3_3.SetActive(true);
            }, 1, this.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 引导按钮 点击道具1 清空备料杯 确定使用
        /// </summary>
        private void OnBtnGuideStep3_3()
        {
            // 隐藏引导节点3_3
            _guideStep3_3.SetActive(false);
            // 隐藏引导节点
            ShowGuideObj(-1);
            // 派发全局事件 通知使用道具
            EventManager<int>.Send(CustomEventType.RunGuideUseItem, 0);
            // 记录新手引导完成状态
            DataHelper.CurUserInfoData.guideCompleteList[2] = 1;
            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(1) { "GuideCompleteList" }, () => { });
            
            // 隐藏引导总节点
            UiBattle._instance.RunGuideStep(true);
        }

        /// <summary>
        /// 引导按钮 点击道具2 刷新订单
        /// </summary>
        private void OnBtnGuideStep4_1()
        {
            // 隐藏引导节点4_1
            _guideStep4_1.SetActive(false);
            // 打开刷新订单杯道具使用页
            UiBattle._instance._isClickOpenUseItem = false;
            UiBattle._instance.OnBtnOpenOperation(true, 1);
            // 延迟1帧 展示引导节点4_2
            DelayTime.DelayFrame(() =>
            {
                _guideStep4_2.SetActive(true);
            }, 1, this.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 引导按钮 点击道具2 刷新订单 确定使用
        /// </summary>
        private void OnBtnGuideStep4_2()
        {
            // 隐藏引导节点4_2
            _guideStep4_2.SetActive(false);
            // 隐藏引导节点
            ShowGuideObj(-1);
            // 派发全局事件 通知使用道具
            EventManager<int>.Send(CustomEventType.RunGuideUseItem, 1);
            // 记录新手引导完成状态
            DataHelper.CurUserInfoData.guideCompleteList[3] = 1;
            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(1) { "GuideCompleteList" }, () => { });
            
            // 隐藏引导总节点
            UiBattle._instance.RunGuideStep(true);
        }

        /// <summary>
        /// 引导按钮 点击道具3 刷新原料
        /// </summary>
        private void OnBtnGuideStep5_1()
        {
            // 隐藏引导节点5_1
            _guideStep5_1.SetActive(false);
            // 打开刷新原料瓶道具使用页
            UiBattle._instance._isClickOpenUseItem = false;
            UiBattle._instance.OnBtnOpenOperation(true, 2);
            // 延迟1帧 展示引导节点5_2
            DelayTime.DelayFrame(() =>
            {
                _guideStep5_2.SetActive(true);
            }, 1, this.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 引导按钮 点击道具3 刷新原料 确定使用
        /// </summary>
        private void OnBtnGuideStep5_2()
        {
            // 隐藏引导节点4_2
            _guideStep5_2.SetActive(false);
            // 隐藏引导节点
            ShowGuideObj(-1);
            // 派发全局事件 通知使用道具
            EventManager<int>.Send(CustomEventType.RunGuideUseItem, 2);
            // 记录新手引导完成状态
            DataHelper.CurUserInfoData.guideCompleteList[4] = 1;
            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(1) { "GuideCompleteList" }, () => { });
            
            // 隐藏引导总节点
            UiBattle._instance.RunGuideStep(true);
        }

        // -------------------------------------------------------- 引导步骤 --------------------------------------------------------
        /// <summary>
        /// 展示/隐藏引导节点
        /// </summary>
        /// <param name="idTmp">列表索引 -1: 全部隐藏 0~N: 展示对应的引导节点</param>
        private void ShowGuideObj(int idTmp)
        {
            if (idTmp == -1)
            {
                for (int i = 0; i < _guideObjs.Count; i++)
                {
                    _guideObjs[i].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < _guideObjs.Count; i++)
                {
                    _guideObjs[i].SetActive(i == idTmp);
                }
            }
        }

        /// <summary>
        /// 执行新手引导步骤1
        /// </summary>
        public void RunGuideStep_1()
        {
            // 展示引导节点1
            ShowGuideObj(0);
            // 播放引导音效1
            AudioHandler._instance.PlayAudio(GuideAudio1);
        }

        /// <summary>
        /// 完成新手引导步骤1
        /// </summary>
        public void CompleteGuideStep_1()
        {
            // 隐藏引导节点
            ShowGuideObj(-1);
            // 记录新手引导完成状态
            DataHelper.CurUserInfoData.guideCompleteList[0] = 1;
            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(1) { "GuideCompleteList" }, () => { });
        }

        /// <summary>
        /// 执行新手引导步骤2
        /// </summary>
        public void RunGuideStep_2()
        {
            // 展示引导节点2
            ShowGuideObj(1);
            // 播放引导音效2
            AudioHandler._instance.PlayAudio(GuideAudio2);
            // 音效播放完成
            DelayTime.DelaySecondsNoTimeScale(CompleteGuideStep_2, _guideAudioTime2, this.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 完成新手引导步骤2
        /// </summary>
        private void CompleteGuideStep_2()
        {
            // 隐藏引导节点
            ShowGuideObj(-1);
            // 记录新手引导完成状态
            DataHelper.CurUserInfoData.guideCompleteList[1] = 1;
            // 保存数据
            DataHelper.ModifyLocalData(new List<string>(1) { "GuideCompleteList" }, () => { });

            // 隐藏引导总节点
            UiBattle._instance.RunGuideStep(true);
        }

        /// <summary>
        /// 执行新手引导步骤3_1
        /// </summary>
        public void RunGuideStep_3_1()
        {
            // 展示引导节点3
            ShowGuideObj(2);
            // 展示引导节点3_1
            _guideStep3_1.SetActive(true);
            // 播放引导音效3_1
            AudioHandler._instance.PlayAudio(GuideAudio3_1);
            // 音效播放完成
            DelayTime.DelaySecondsNoTimeScale(() =>
            {
                // 隐藏引导节点3_1
                _guideStep3_1.SetActive(false);
                // 展示引导节点3_2
                _guideStep3_2.SetActive(true);
                // 播放引导音效3_2
                AudioHandler._instance.PlayAudio(GuideAudio3_2);
            }, _guideAudioTime3_1, this.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 执行新手引导步骤4_1
        /// </summary>
        public void RunGuideStep_4_1()
        {
            // 展示引导节点4
            ShowGuideObj(3);
            // 展示引导节点4_1
            _guideStep4_1.SetActive(true);
            // 播放引导音效4
            AudioHandler._instance.PlayAudio(GuideAudio4);
        }

        /// <summary>
        /// 执行新手引导步骤5_1
        /// </summary>
        public void RunGuideStep_5_1()
        {
            // 展示引导节点5
            ShowGuideObj(4);
            // 展示引导节点5_1
            _guideStep5_1.SetActive(true);
            // 播放引导音效5
            AudioHandler._instance.PlayAudio(GuideAudio5);
        }
    }
}