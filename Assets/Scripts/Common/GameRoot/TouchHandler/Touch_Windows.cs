using UnityEngine;

namespace Common.GameRoot.TouchHandler
{
    public class Touch_Windows : TouchHandler
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CallTouchBegan(0, Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                CallTouchMove(0, Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                CallTouchEnd(0, Input.mousePosition);
            }
        }
    }
}
