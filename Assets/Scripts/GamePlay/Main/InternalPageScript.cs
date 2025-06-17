using UnityEngine;

namespace GamePlay.Main
{
    /// <summary>
    /// 子页面基类
    /// </summary>
    public class InternalPageScript : MonoBehaviour
    {
        /** 打开内部子页面 */
        public virtual void OpenInternalPage()
        {
            gameObject.SetActive(true);
        }

        /** 关闭内部子页面 */
        public virtual void CloseInternalPage()
        {
            gameObject.SetActive(false);
        }

        /** 初始化内部子页面 */
        public virtual void Initial()
        {
        }
    }
}