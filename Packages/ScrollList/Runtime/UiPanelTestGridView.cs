using System.Collections.Generic;
using UnityEngine;

namespace Plugins.ScrollList
{
    public class UiPanelTestGridView : MonoBehaviour
    {
        [SerializeField]
        private UIGridView m_UIGridView;

        [SerializeField]
        private RectTransform m_CellRTTemplate;

        private List<int> m_DataList;
        
        void Start()
        {
            //创建数据列表
            m_DataList = new List<int>();
            for (var i = 0; i < 10; i++)
            {
                m_DataList.Add(i);
            }

            StartShow();
        }

        void StartShow()
        {
            m_UIGridView.StartShow(m_CellRTTemplate, m_DataList.Count, OnCellCreated, OnCellAppear);
        }

        private void OnCellCreated(int index)
        {
            RectTransform cellRT = m_UIGridView.GetCellRT(index);

            // cellRT.GetComponent<GridCell>().Init((_clickedIndex) =>
            // {
            //     TestData testData = m_DataList[_clickedIndex];
            //
            //     Debug.Log(string.Format("当前点击，索引：{0}, 测试Id：{1}, 测试字符串：{2}", _clickedIndex, testData.id, testData.str));
            // });
        }

        private void OnCellAppear(int index)
        {
            RectTransform cellRT = m_UIGridView.GetCellRT(index);

            // cellRT.GetComponent<GridCell>().Refresh(index);
        }
    }
}
