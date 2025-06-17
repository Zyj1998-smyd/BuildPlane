using System.Text;
using Data;
using Data.ConfigData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Globa.GlobaOpenBox
{
    public class ItemOpenBoxRewardUi : MonoBehaviour
    {
        /** 动画组件 */
        private Animator _animation;
        /** 图标 */
        private Image _image;
        /** 名称 */
        private TextMeshProUGUI _nameText;
        /** 碎片+1 */
        private GameObject _chipGet;
        /** 新部件 */
        private GameObject _newEquipGet;
        /** 品质框 */
        private Image _qulityFrame;
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _animation = gameObject.GetComponent<Animator>();
            _image = transform.Find("Image").GetComponent<Image>();
            _nameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            _chipGet = transform.Find("Frame/Debris").gameObject;
            _newEquipGet = transform.Find("Frame/New").gameObject;
            _qulityFrame = gameObject.GetComponent<Image>();
        }

        /// <summary>
        /// 设置数据 执行Open
        /// </summary>
        /// <param name="id">部件ID</param>
        /// <param name="isNew">新获取</param>
        internal void SetDataAndOpen(int id, string isNew)
        {
            SetData(id, isNew);
            _animation.Play("RewardItemOpen", -1, 0);
        }

        /// <summary>
        /// 设置数据 执行Show
        /// </summary>
        /// <param name="id">部件ID</param>
        /// <param name="isNew">新获取</param>
        internal void SetDataAndShow(int id, string isNew)
        {
            SetData(id, isNew);
            _animation.Play("RewardItemShow", -1, 0);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="id">部件ID</param>
        /// <param name="isNew">新获取</param>
        private void SetData(int id, string isNew)
        {
            ComponentConfig config = ConfigManager.Instance.ComponentConfigDict[id];

            _nameText.text = config.Name;
            _qulityFrame.sprite = GameGlobalManager._instance._globalOpenBox.qualityFrames[config.Quality];

            if (isNew == "")
            {
                _chipGet.SetActive(DataHelper.CurUserInfoData.equipments.ContainsKey(id));
                _newEquipGet.SetActive(!DataHelper.CurUserInfoData.equipments.ContainsKey(id));
            }
            else
            {
                _chipGet.SetActive(isNew == "0");
                _newEquipGet.SetActive(isNew == "1");
            }

            GameGlobalManager._instance.SetImage(_image, new StringBuilder("IconImage" + config.ID).ToString());
        }
    }
}