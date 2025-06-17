using Common.GameRoot.AudioHandler;
using GamePlay.Globa;
using GamePlay.Module.InternalPage.PageBuild;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Module.InternalPage.ItemPrefabs
{
    public class ItemBuildColorUi : MonoBehaviour
    {
        /** 当前装备中 */
        private GameObject _nowEquip;

        /** 涂装页面 */
        internal OpenBuildPaintUi _openBuildPaintUi;

        /** 列表索引 */
        internal int _index;
        /** 类型 */
        internal int _type;
        /** ID */
        internal int _id;
        
        internal RectTransform _parentRect;

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            _parentRect = transform.GetComponent<RectTransform>();
            // _nowEquip = transform.Find("Select").gameObject;
            gameObject.GetComponent<Button>().onClick.AddListener(OnBtnItem);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="index">列表索引</param>
        /// <param name="type">类型</param>
        /// <param name="id">ID</param>
        internal void SetData(int index, int type, int id)
        {
            _index = index;
            _type = type;
            _id = id;
        }

        /// <summary>
        /// 设置装备中
        /// </summary>
        /// <param name="nowEquip">是否装备中</param>
        internal void SetNowEquip(bool nowEquip)
        {
            // _nowEquip.SetActive(nowEquip);
        }

        /// <summary>
        /// 按钮 选中
        /// </summary>
        private void OnBtnItem()
        {
            _openBuildPaintUi.OnSelectColorItem(this, true);
        }
    }
}