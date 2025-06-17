using System.Collections.Generic;
using Common.GameRoot.AudioHandler;
using Data;
using GamePlay.Globa;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage.PageBuild
{
    public class OpenBuildDetailsUi : MonoBehaviour
    {
        /** 组装总页面 */
        internal OpenBuildPageUi _openBuildPageUi;
        
        /** 滑动列表 */
        private ScrollRect _scrollRect;
        /** 滑动列表挂载容器 */
        private Transform _content;
        
        /** 属性值列表 */
        private readonly TextMeshProUGUI[] _propetyNumTexts = new TextMeshProUGUI[6];
        /** 属性条列表 */
        private readonly Image[] _propetyBars = new Image[6];
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(OnBtnClose);
            transform.Find("Frame/Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);

            _scrollRect = transform.Find("Frame/List").GetComponent<ScrollRect>();
            _content = _scrollRect.transform.Find("Viewport/Content");
            for (int i = 0; i < 6; i++)
            {
                Transform propety = _content.Find("Propety_" + (i + 1));
                if (propety.Find("Value")) _propetyBars[i] = propety.Find("Value").GetComponent<Image>();
                if (propety.Find("Num")) _propetyNumTexts[i] = propety.Find("Num").GetComponent<TextMeshProUGUI>();
            }
        }

        /// <summary>
        /// 打开属性详情
        /// </summary>
        internal void OpenDetails()
        {
            RefreshPropety();
        }
        
        /// <summary>
        /// 刷新属性
        /// </summary>
        private void RefreshPropety()
        {
            Dictionary<string, float> allPropetyNum = DataHelper.GetAllPropety(-1, -1, false);
            List<float> propetyNums = new List<float>(6)
            {
                allPropetyNum["propetyZhongLiang"],
                allPropetyNum["propetyFuKong"],
                allPropetyNum["propetySuDu"],
                allPropetyNum["propetyKangZu"],
                allPropetyNum["propetyTuiJin"],
                allPropetyNum["propetyNengLiang"]
            };

            for (int i = 0; i < _propetyNumTexts.Length; i++)
            {
                _propetyNumTexts[i].text = propetyNums[i].ToString("F1").TrimEnd('0').TrimEnd('.');
                if (_propetyBars[i] != null)
                {
                    float valueTmp = Mathf.Pow((propetyNums[i] / 5000f), 0.5f);
                    float value = Mathf.Clamp01(valueTmp);
                    _propetyBars[i].fillAmount = value;
                }
            }
        }
        
        // ------------------------------------------------ 按钮 ------------------------------------------------
        /// <summary>
        /// 按钮 关闭
        /// </summary>
        private void OnBtnClose()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            _openBuildPageUi.OpenBuildDetails(false);
        }
    }
}