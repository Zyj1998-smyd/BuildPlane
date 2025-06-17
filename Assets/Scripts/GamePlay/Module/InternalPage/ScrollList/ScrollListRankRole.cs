using System.Collections.Generic;
using GamePlay.Module.InternalPage.ItemPrefabs;
using Plugins.ScrollList;
using UnityEngine;

namespace GamePlay.Module.InternalPage.ScrollList
{
    /// <summary>
    /// 个人榜
    /// </summary>
    public class ScrollListRankRole : MonoBehaviour
    {
        [SerializeField]
        private UIGridView m_UIGridView;
        [SerializeField]
        private RectTransform m_CellRTTemplate;
        
        private List<string[]> m_DataList;
        
        /** 全国排行榜页面 */
        [HideInInspector] public OpenRankPageUi OpenRankPageUi;
        
        /** 设置列表 */
        public void SetList(List<string[]> list)
        {
            var width = OpenRankPageUi._rankRoleContent.rect.width;
            var sizeTmp = m_CellRTTemplate.sizeDelta;
            m_CellRTTemplate.sizeDelta = new Vector2(width, sizeTmp.y);
            
            m_DataList = new List<string[]>();
            for (int i = 0; i < list.Count; i++)
            {
                m_DataList.Add(list[i]);
            }

            StartShow();
        }

        private void StartShow()
        {
            m_UIGridView.StartShow(m_CellRTTemplate, m_DataList.Count, OnCellCreated, OnCellAppear);
        }

        private void OnCellCreated(int index)
        {
            RectTransform cellRt = m_UIGridView.GetCellRT(index);
        }

        private void OnCellAppear(int index)
        {
            RectTransform cellRt = m_UIGridView.GetCellRT(index);
            ItemRankRoleUi cellRtUi = cellRt.GetComponent<ItemRankRoleUi>();
            cellRtUi.SetData(index, m_DataList[index]);
        }
    }
}