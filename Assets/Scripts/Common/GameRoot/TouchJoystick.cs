using System.Collections.Generic;
using Common.Event;
using Common.Event.CustomEnum;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.GameRoot
{
	public class TouchJoystick : MonoBehaviour
	{
		int mRadius;
		RectTransform QuanBGRectT, QuanRectT;

		Vector3 targetPos;
		Vector3 initialPos;

		private long fingerId;
		private RectTransform buttonRect;

		private Camera uiCam;

		void Awake()
		{
			Transform canvasTmp = transform.parent.parent;
			uiCam = canvasTmp.GetComponent<Canvas>().worldCamera;

			GetComponent<RectTransform>().offsetMax = new Vector2(0, canvasTmp.GetComponent<RectTransform>().sizeDelta.y * -0.5f);

			QuanBGRectT = transform.Find("QuanBG").GetComponent<RectTransform>();
			QuanRectT = QuanBGRectT.transform.Find("Quan").GetComponent<RectTransform>();
			initialPos = QuanBGRectT.anchoredPosition;
			mRadius = (int) ((QuanBGRectT.rect.width - QuanRectT.rect.width) * 0.6);

			fingerId = -1;
			buttonRect = GetComponent<RectTransform>();
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

		private void OnDrag(long fingerIdTmp, Vector2 position)
		{
			if (fingerId != fingerIdTmp) return;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(buttonRect, position, uiCam, out var screenPoint);

			Vector2 delta = screenPoint - new Vector2(QuanBGRectT.localPosition.x, QuanBGRectT.localPosition.y);
			if (delta.magnitude > mRadius) delta = delta.normalized * mRadius;

			targetPos = new Vector3(delta.x, delta.y, 0);
		}

		private void OnPointerUp(long fingerIdTmp, Vector2 position)
		{
			if (fingerId != fingerIdTmp) return;
			
			fingerId = -1;
			QuanBGRectT.anchoredPosition = initialPos;
			targetPos = Vector3.zero;
		}

		private void OnPointerDown(long fingerIdTmp, Vector2 position)
		{
			if (fingerId != -1) return;
			if (!buttonRect.gameObject.activeInHierarchy) return;
			if (!RectTransformUtility.RectangleContainsScreenPoint(buttonRect, position, uiCam)) return;
			if (!IsPointerOverGameObject(position)) return;
			

			fingerId = fingerIdTmp;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(buttonRect, position, uiCam, out var screenPoint);

			QuanBGRectT.localPosition = screenPoint;
		}

		void Update()
		{
			QuanRectT.localPosition = Vector3.Lerp(QuanRectT.localPosition, targetPos, 0.5f);
			EventManager<Vector2>.Send(EnumButtonType.TouchJoystick,QuanRectT.anchoredPosition / mRadius);
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