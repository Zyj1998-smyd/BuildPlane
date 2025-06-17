using UnityEngine;

namespace GamePlay.Battle
{
    public class PathSampler : MonoBehaviour
    {
        public Transform[] controlPoints;

        void OnDrawGizmos()
        {
            if (controlPoints == null || controlPoints.Length < 2) return;

            // 自动按Z轴排序控制点
            System.Array.Sort(controlPoints, (a, b) => a.position.z.CompareTo(b.position.z));

            // 绘制控制点
            Gizmos.color = Color.green;
            foreach (var point in controlPoints)
            {
                if (point == null) continue;
                Gizmos.DrawSphere(point.position, 0.3f);
            }

            // 绘制路径曲线
            Gizmos.color = Color.green;
            Vector3 previousPoint = controlPoints[0].position;
            for (int i = 1; i < controlPoints.Length; i++)
            {
                Vector3 currentPoint = controlPoints[i].position;
                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
        }

        // 根据Z坐标获取X（运行时使用）
        public float GetXAtZ(float z)
        {
            if (controlPoints == null || controlPoints.Length < 2) return 0;

            // 找到最近的区间
            for (int i = 1; i < controlPoints.Length; i++)
            {
                float z1 = controlPoints[i - 1].position.z;
                float z2 = controlPoints[i].position.z;
                if (z >= z1 && z <= z2)
                {
                    // 线性插值
                    float t = Mathf.InverseLerp(z1, z2, z);
                    return Mathf.Lerp(
                        controlPoints[i - 1].position.x,
                        controlPoints[i].position.x,
                        t
                    );
                }
            }

            // 超出范围时返回第一个/最后一个点
            return z < controlPoints[0].position.z
                ? controlPoints[0].position.x
                : controlPoints[^1].position.x;
        }

    }
}
