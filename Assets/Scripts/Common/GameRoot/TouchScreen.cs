using System.Collections.Generic;
using Common.Event;
using Common.Event.CustomEnum;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.GameRoot
{
	public class TouchScreen : MonoBehaviour
	{
		public EnumButtonSign buttonSign;

		private                 long          fingerId;
		private                 RectTransform buttonRect;
		private                 Camera        uiCam;
		
		private                 Animator btnAni;
		private static readonly int      Pressed = Animator.StringToHash("Pressed");
		private static readonly int      Normal  = Animator.StringToHash("Normal");

		void Awake()
		{
			Transform canvasTmp = transform.parent.parent;
			uiCam = canvasTmp.GetComponent<Canvas>().worldCamera;

			fingerId = -1;
			buttonRect = GetComponent<RectTransform>();

			btnAni = GetComponent<Animator>();
		}

		public void OnEnable()
		{
			TouchHandler.TouchHandler.callTouchBegan += OnPointerDown;
			TouchHandler.TouchHandler.callTouchMove += OnDrag;
			TouchHandler.TouchHandler.callTouchEnd += OnPointerUp;
		}

		public void OnDisable()
		{
			TouchHandler.TouchHandler.callTouchBegan -= OnPointerDown;
			TouchHandler.TouchHandler.callTouchMove -= OnDrag;
			TouchHandler.TouchHandler.callTouchEnd -= OnPointerUp;
		}


		private void OnPointerDown(long fingerIdTmp, Vector2 position)
		{
			if (fingerId != -1) return;
			if (!buttonRect.gameObject.activeInHierarchy) return;
			if (!RectTransformUtility.RectangleContainsScreenPoint(buttonRect, position, uiCam)) return;
			if (!IsPointerOverGameObject(position)) return;

			fingerId = fingerIdTmp;

			// RectTransformUtility.ScreenPointToLocalPointInRectangle(buttonRect, position, uiCam, out var screenPoint);

			EventManager<EnumButtonSign, Vector2>.Send(EnumButtonType.TouchScreenDown, buttonSign, position);

			if (btnAni) btnAni.SetTrigger(Pressed);
		}

		private void OnDrag(long fingerIdTmp, Vector2 position)
		{
			if (fingerId != fingerIdTmp) return;

			// RectTransformUtility.ScreenPointToLocalPointInRectangle(buttonRect, position, uiCam, out var screenPoint);

			EventManager<EnumButtonSign, Vector2>.Send(EnumButtonType.TouchScreenDrag, buttonSign, position);
		}

		private void OnPointerUp(long fingerIdTmp, Vector2 position)
		{
			if (fingerId != fingerIdTmp) return;

			fingerId = -1;

			EventManager<EnumButtonSign, Vector2>.Send(EnumButtonType.TouchScreenUp, buttonSign, position);

			if (btnAni) btnAni.SetTrigger(Normal);
		}


		private bool IsPointerOverGameObject(Vector2 screenPosition)
		{
			bool isMe = false;
			//实例化点击事件
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
			{
				//将点击位置的屏幕坐标赋值给点击事件
				position = new Vector2(screenPosition.x, screenPosition.y)
			};

			List<RaycastResult> results = new List<RaycastResult>();
			//向点击处发射射线
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

			if (results.Count > 0)
			{
				if (results[0].gameObject == gameObject)
				{
					isMe = true;
				}
			}

			return isMe;
		}
	}
}