using UnityEngine;

namespace Common.GameRoot.TouchHandler
{
    public class Touch_Mobile : TouchHandler
    {
        void Update()
        {
            if (Input.touchCount <= 0) return;
            foreach (var touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        CallTouchBegan(touch.fingerId, touch.position);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        CallTouchMove(touch.fingerId, touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        CallTouchEnd(touch.fingerId, touch.position);
                        break;
                }
            }
        }
    }
}
