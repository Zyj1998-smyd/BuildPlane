using UnityEngine;

namespace Common.GameRoot.TouchHandler
{
    public abstract class TouchHandler : MonoBehaviour
    {
        public delegate void TouchAction(long fingerId, Vector2 position);

        public static event TouchAction callTouchBegan;
        public static event TouchAction callTouchEnd;
        public static event TouchAction callTouchMove;

        protected static void CallTouchBegan(long fingerId, Vector2 position)
        {
            callTouchBegan?.Invoke(fingerId, position);
        }

        protected static void CallTouchMove(long fingerId, Vector2 position)
        {
            callTouchMove?.Invoke(fingerId, position);
        }

        protected static void CallTouchEnd(long fingerId, Vector2 position)
        {
            callTouchEnd?.Invoke(fingerId, position);
        }
    }
}
