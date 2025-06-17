using UnityEngine;

namespace Common.Tool
{
    public class GetOrthographicSize : MonoBehaviour
    {
        public static float OrthographicSize(float oldSize)
        {
            //正交相机自适应宽 公式： 实际视口 = 初始化视口大小 * 初始宽高比 / 实际宽高比
            return (oldSize * (720 / 1440f)) / (Screen.width / (float)Screen.height);
        }
    }
}