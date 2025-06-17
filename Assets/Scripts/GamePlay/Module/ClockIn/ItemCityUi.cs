using System.Collections.Generic;
using System.Text;
using Common.GameRoot.AudioHandler;
using Data;
using GamePlay.Globa;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.ClockIn
{
    public class ItemCityUi : MonoBehaviour
    {
        /** 地标打卡页面 */
        internal OpenClockInPageUi _openClockInPageUi;

        /** 城市名称 */
        private Image _nameImage;
        /** 非选中状态 */
        private GameObject _cityOff;
        /** 选中状态 */
        private GameObject _cityOn;
        /** 提示红点 */
        private GameObject _redPoint;

        /** 城市ID */
        internal int _cityId;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _cityOff = transform.Find("CityOff").gameObject;
            _cityOn = transform.Find("CityOn").gameObject;
            _nameImage = transform.Find("Name").GetComponent<Image>();
            _redPoint = _cityOff.transform.Find("RedPoint").gameObject;

            gameObject.GetComponent<Button>().onClick.AddListener(OnBtnItem);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="cityId">城市ID</param>
        internal void SetData(int cityId)
        {
            _cityId = cityId;

            string imageName = new StringBuilder("CityName" + _cityId).ToString();
            GameGlobalManager._instance.SetImage(_nameImage, imageName);

            RefreshRedPoint();
        }

        /// <summary>
        /// 设置选中状态
        /// </summary>
        /// <param name="isSelect">是否选中</param>
        internal void SetSelect(bool isSelect)
        {
            _cityOff.SetActive(!isSelect);
            _cityOn.SetActive(isSelect);
        }

        /// <summary>
        /// 刷新提示红点
        /// </summary>
        internal void RefreshRedPoint()
        {
            // 选出已打卡地标列表中属于当前选择城市的地标ID
            List<int> landMarkIds = new List<int>();
            foreach (KeyValuePair<int, int> data in DataHelper.CurUserInfoData.landMarkInfo)
            {
                if (GlobalValueManager._landMarkIds[_cityId - 1].Contains(data.Key))
                {
                    landMarkIds.Add(data.Key);
                }
            }
            
            int landMarkNum = GlobalValueManager._landMarkIds[_cityId - 1].Count;
            bool isMarked = false;
            for (int i = 0; i < landMarkNum; i++)
            {
                int landMarkId = GlobalValueManager._landMarkIds[_cityId - 1][i];
                if (landMarkIds.Contains(landMarkId))
                {
                    int rewardGetNum = DataHelper.CurUserInfoData.landMarkInfo.GetValueOrDefault(landMarkId, 0);
                    if (rewardGetNum == 0)
                    {
                        isMarked = true;
                        break;
                    }
                }
            }

            _redPoint.SetActive(isMarked);
        }

        /// <summary>
        /// 按钮 Item
        /// </summary>
        private void OnBtnItem()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioBtnClick);
            _openClockInPageUi.OnSelectCity(_cityId);
        }
    }
}