using System.Collections.Generic;
using System.Text;
using Common.GameRoot.AudioHandler;
using Data;
using GamePlay.Globa;
using GamePlay.Main;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.ClockIn
{
    public class OpenClockInPageUi : MonoBehaviour
    {
        /** 城市预制 */
        public GameObject ItemCityPre;
        /** 地标预制 */
        public GameObject ItemLandMarkPre;

        /** 城市列表 */
        private ScrollRect _cityListSv;
        /** 城市列表项挂载容器 */
        private RectTransform _cityListContent;

        /** 地标列表 */
        private ScrollRect _landMarkListSv;
        /** 地标列表项挂载容器 */
        private RectTransform _landMarkListContent;

        /** 城市图片 */
        private Image _cityImage;
        /** 城市描述 */
        private Image _cityDesc;

        /** 城市列表 */
        private readonly List<ItemCityUi> _itemCityUis = new List<ItemCityUi>();

        /** 地标列表 */
        private readonly List<ItemLandMarkUi> _itemLandMarkUis = new List<ItemLandMarkUi>();

        /** 当前选中的城市ID */
        private int _curSelectCityId;
        /** 当前选中的地标ID */
        private int _curSelectLandMarkId;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            transform.Find("Mask").GetComponent<Button>().onClick.AddListener(OnBtnClose);
            transform.Find("ClockIn/Tittle/BtnClose").GetComponent<Button>().onClick.AddListener(OnBtnClose);

            _cityListSv = transform.Find("ClockIn/CityList").GetComponent<ScrollRect>();
            _cityListContent = _cityListSv.transform.Find("Viewport/Content").GetComponent<RectTransform>();

            _landMarkListSv = transform.Find("ClockIn/LandmarkList").GetComponent<ScrollRect>();
            _landMarkListContent = _landMarkListSv.transform.Find("Viewport/Content").GetComponent<RectTransform>();

            _cityImage = _landMarkListContent.transform.Find("CityImage/Image").GetComponent<Image>();
            _cityDesc = _cityImage.transform.Find("Mask/CityDoc").GetComponent<Image>();
        }

        /// <summary>
        /// 打开地标打开页面
        /// </summary>
        internal void OpenPop()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopOpen);

            RefreshCityList();
        }

        /// <summary>
        /// 清空城市列表
        /// </summary>
        private void ClearCityList()
        {
            for (int i = 0; i < _itemCityUis.Count; i++)
            {
                Destroy(_itemCityUis[i].gameObject);
            }

            _itemCityUis.Clear();
        }

        /// <summary>
        /// 清空地标列表
        /// </summary>
        private void ClearLandMarkList()
        {
            for (int i = 0; i < _itemLandMarkUis.Count; i++)
            {
                Destroy(_itemLandMarkUis[i].gameObject);
            }

            _itemLandMarkUis.Clear();
        }

        /// <summary>
        /// 刷新城市列表
        /// </summary>
        private void RefreshCityList()
        {
            // 清空上次刷新的城市列表
            ClearCityList();
            // 刷新城市列表
            int cityNum = 15;
            for (int i = 0; i < cityNum; i++)
            {
                ItemCityUi itemCityUi = Instantiate(ItemCityPre, _cityListContent).GetComponent<ItemCityUi>();
                itemCityUi.Initial();
                itemCityUi._openClockInPageUi = this;
                itemCityUi.SetData(i + 1);
                _itemCityUis.Add(itemCityUi);
            }

            // 默认选择
            OnSelectCity(1);
        }

        /// <summary>
        /// 刷新地标列表
        /// </summary>
        private void RefreshLandMarkList()
        {
            // 清空上次刷新的地标列表
            ClearLandMarkList();
            // 选出已打卡地标列表中属于当前选择城市的地标ID
            List<int> landMarkIds = new List<int>();
            foreach (KeyValuePair<int, int> data in DataHelper.CurUserInfoData.landMarkInfo)
            {
                if (GlobalValueManager._landMarkIds[_curSelectCityId - 1].Contains(data.Key))
                {
                    landMarkIds.Add(data.Key);
                }
            }
            // 地标ID列表排序 按ID编号先后
            int idTmp;
            for (int i = 0; i < landMarkIds.Count; i++)
            {
                for (int j = 0; j < landMarkIds.Count - i - 1; j++)
                {
                    if (landMarkIds[j] > landMarkIds[j + 1])
                    {
                        idTmp = landMarkIds[j];
                        landMarkIds[j] = landMarkIds[j + 1];
                        landMarkIds[j + 1] = idTmp;
                    }
                }
            }
            // 刷新地标列表
            int landMarkNum = GlobalValueManager._landMarkIds[_curSelectCityId - 1].Count;
            for (int i = 0; i < landMarkNum; i++)
            {
                int landMarkId = GlobalValueManager._landMarkIds[_curSelectCityId - 1][i];
                bool isMark = landMarkIds.Contains(landMarkId);
                
                ItemLandMarkUi itemLandMarkUi = Instantiate(ItemLandMarkPre, _landMarkListContent).GetComponent<ItemLandMarkUi>();
                itemLandMarkUi.Initial();
                itemLandMarkUi._openClockInPageUi = this;
                itemLandMarkUi.SetData(landMarkId, isMark);
                _itemLandMarkUis.Add(itemLandMarkUi);
            }

            // 默认选择
            OnSelectLandMark(-1);
        }

        // --------------------------------------------------- 按钮 ---------------------------------------------------
        /// <summary>
        /// 按钮 关闭
        /// </summary>
        private void OnBtnClose()
        {
            AudioHandler._instance.PlayAudio(GameGlobalManager._instance.audioPopClose);
            MainManager._instance.OnOpenPop_ClockIn(false);
        }

        /// <summary>
        /// 按钮响应 选择城市
        /// </summary>
        /// <param name="cityId">城市ID</param>
        internal void OnSelectCity(int cityId)
        {
            _curSelectCityId = cityId;
            // 城市选中
            for (int i = 0; i < _itemCityUis.Count; i++)
            {
                if (_itemCityUis[i]._cityId == cityId)
                {
                    _itemCityUis[i].SetSelect(true);
                }
                else
                {
                    _itemCityUis[i].SetSelect(false);
                }
            }

            // 城市图片
            string cityImageName = new StringBuilder("City" + cityId).ToString();
            GameGlobalManager._instance.SetImage(_cityImage, cityImageName);
            // 城市描述
            string cityDescName = new StringBuilder("CityDoc" + cityId).ToString();
            GameGlobalManager._instance.SetImage(_cityDesc, cityDescName);

            // 刷新地标列表
            RefreshLandMarkList();
        }

        /// <summary>
        /// 按钮响应 选择地标
        /// </summary>
        /// <param name="landMarkId">地标ID</param>
        internal void OnSelectLandMark(int landMarkId)
        {
            if (landMarkId == -1)
            {
                // 列表刷新完 默认全部归为待机动画
                for (int i = 0; i < _itemLandMarkUis.Count; i++)
                {
                    _itemLandMarkUis[i].SetAnimation(0);
                }

                _curSelectLandMarkId = landMarkId;
            }
            else
            {
                if (_curSelectLandMarkId == -1)
                {
                    // 没有上次选择的地标 直接选择
                    for (int i = 0; i < _itemLandMarkUis.Count; i++)
                    {
                        if (_itemLandMarkUis[i]._landMarkId == landMarkId)
                        {
                            _itemLandMarkUis[i].SetAnimation(1);
                            break;
                        }
                    }

                    _curSelectLandMarkId = landMarkId;
                }
                else
                {
                    // 有上次选择的地标
                    if (_curSelectLandMarkId == landMarkId)
                    {
                        // 本次选择的地标就是上次选择的地标 关闭选择的地标
                        for (int i = 0; i < _itemLandMarkUis.Count; i++)
                        {
                            if (_itemLandMarkUis[i]._landMarkId == landMarkId)
                            {
                                _itemLandMarkUis[i].SetAnimation(2);
                                break;
                            }
                        }

                        _curSelectLandMarkId = -1;
                    }
                    else
                    {
                        // 本次选择的地标不是上次选择的地标 关闭上次的打开本次的
                        for (int i = 0; i < _itemLandMarkUis.Count; i++)
                        {
                            if (_itemLandMarkUis[i]._landMarkId == _curSelectLandMarkId)
                            {
                                _itemLandMarkUis[i].SetAnimation(2);
                            }

                            if (_itemLandMarkUis[i]._landMarkId == landMarkId)
                            {
                                _itemLandMarkUis[i].SetAnimation(1);
                            }
                        }

                        _curSelectLandMarkId = landMarkId;
                    }
                }
            }
        }

        /// <summary>
        /// 刷新城市列表提示红点
        /// </summary>
        internal void RefrehCityListRedpoint()
        {
            for (int i = 0; i < _itemCityUis.Count; i++)
            {
                _itemCityUis[i].RefreshRedPoint();
            }
        }
    }
}