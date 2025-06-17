using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Common.Event
{
    public class EventSender<TKey>
    {
        private readonly Dictionary<TKey, EventHandler> _dict = new Dictionary<TKey, EventHandler>();

        public void Add(TKey eventType, UnityAction action)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                unityEvent = new EventHandler();
                _dict.Add(eventType, unityEvent);
            }

            unityEvent.AddListener(action);
        }

        public void Remove(TKey eventType, UnityAction action)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                Debug.Log("未找到事件类型"+eventType.ToString());
                return;
            }

            unityEvent.RemoveListener(action);
        }

        public void Send(TKey eventType)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                Debug.Log("事件类型没有监听"+eventType.ToString());
                return;
            }

            unityEvent.Invoke();
        }
    }

    public class EventSender<TKey, TValue0>
    {
        private readonly Dictionary<TKey, EventHandler<TValue0>> _dict = new Dictionary<TKey, EventHandler<TValue0>>();

        public void Add(TKey eventType, UnityAction<TValue0> action)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                unityEvent = new EventHandler<TValue0>();
                _dict.Add(eventType, unityEvent);
            }

            unityEvent.AddListener(action);
        }

        public void Remove(TKey eventType, UnityAction<TValue0> action)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                Debug.Log("未找到事件类型"+eventType.ToString());
                return;
            }

            unityEvent.RemoveListener(action);
        }

        public void Send(TKey eventType, TValue0 arg0)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                Debug.Log("事件类型没有监听"+eventType.ToString());
                return;
            }

            unityEvent.Invoke(arg0);
        }
    }

    public class EventSender<TKey, TValue0, TValue1>
    {
        private readonly Dictionary<TKey, EventHandler<TValue0, TValue1>> _dict =
            new Dictionary<TKey, EventHandler<TValue0, TValue1>>();

        public void Add(TKey eventType, UnityAction<TValue0, TValue1> action)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                unityEvent = new EventHandler<TValue0, TValue1>();
                _dict.Add(eventType, unityEvent);
            }

            unityEvent.AddListener(action);
        }

        public void Remove(TKey eventType, UnityAction<TValue0, TValue1> action)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                Debug.Log("未找到事件类型"+eventType.ToString());
                return;
            }

            unityEvent.RemoveListener(action);
        }

        public void Send(TKey eventType, TValue0 arg0, TValue1 arg1)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                Debug.Log("事件类型没有监听"+eventType.ToString());
                return;
            }

            unityEvent.Invoke(arg0, arg1);
        }
    }

    public class EventSender<TKey, TValue0, TValue1, TValue2>
    {
        private readonly Dictionary<TKey, EventHandler<TValue0, TValue1, TValue2>> _dict =
            new Dictionary<TKey, EventHandler<TValue0, TValue1, TValue2>>();

        public void Add(TKey eventType, UnityAction<TValue0, TValue1, TValue2> action)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                unityEvent = new EventHandler<TValue0, TValue1, TValue2>();
                _dict.Add(eventType, unityEvent);
            }

            unityEvent.AddListener(action);
        }

        public void Remove(TKey eventType, UnityAction<TValue0, TValue1, TValue2> action)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                Debug.Log("未找到事件类型"+eventType.ToString());
                return;
            }

            unityEvent.RemoveListener(action);
        }

        public void Send(TKey eventType, TValue0 arg0, TValue1 arg1, TValue2 arg2)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                Debug.Log("事件类型没有监听"+eventType.ToString());
                return;
            }

            unityEvent.Invoke(arg0, arg1, arg2);
        }
    }

    public class EventSender<TKey, TValue0, TValue1, TValue2, TValue3>
    {
        private readonly Dictionary<TKey, EventHandler<TValue0, TValue1, TValue2, TValue3>> _dict =
            new Dictionary<TKey, EventHandler<TValue0, TValue1, TValue2, TValue3>>();

        public void Add(TKey eventType, UnityAction<TValue0, TValue1, TValue2, TValue3> action)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                unityEvent = new EventHandler<TValue0, TValue1, TValue2, TValue3>();
                _dict.Add(eventType, unityEvent);
            }

            unityEvent.AddListener(action);
        }

        public void Remove(TKey eventType, UnityAction<TValue0, TValue1, TValue2, TValue3> action)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                Debug.Log("未找到事件类型"+eventType.ToString());
                return;
            }

            unityEvent.RemoveListener(action);
        }

        public void Send(TKey eventType, TValue0 arg0, TValue1 arg1, TValue2 arg2, TValue3 arg3)
        {
            _dict.TryGetValue(eventType, out var unityEvent);
            if (unityEvent == null)
            {
                Debug.Log("事件类型没有监听"+eventType.ToString());
                return;
            }

            unityEvent.Invoke(arg0, arg1, arg2, arg3);
        }
    }
}