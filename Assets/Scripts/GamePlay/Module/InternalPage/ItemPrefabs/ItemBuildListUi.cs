using UnityEngine;

namespace GamePlay.Module.InternalPage.ItemPrefabs
{
    /// <summary>
    /// 飞机部件列表 水平一行
    /// </summary>
    public class ItemBuildListUi : MonoBehaviour
    {
        /** 挂载节点 */
        internal readonly Transform[] itemPoints = new Transform[5]; 
        
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            for (int i = 0; i < 5; i++)
            {
                Transform point = transform.Find("Point_" + (i + 1));
                itemPoints[i] = point;
            }
        }
    }
}