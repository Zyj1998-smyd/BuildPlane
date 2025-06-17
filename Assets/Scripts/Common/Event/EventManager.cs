using System;
using UnityEngine;
using UnityEngine.Events;

namespace Common.Event
{
    public class EventManager : MonoBehaviour
    {
        private static readonly EventSender<Enum> _sender = new EventSender<Enum>();


        public static void Add(Enum type, UnityAction action)
        {
            _sender.Add(type, action);
        }

        public static void Remove(Enum type, UnityAction action)
        {
            _sender.Remove(type, action);
        }

        public static void Send(Enum type)
        {
            _sender.Send(type);
        }
    }

    public class EventManager<TValue0> : MonoBehaviour
    {
        private static readonly EventSender<Enum, TValue0> _sender = new EventSender<Enum, TValue0>();

        public static void Add(Enum type, UnityAction<TValue0> action)
        {
            _sender.Add(type, action);
        }

        public static void Remove(Enum type, UnityAction<TValue0> action)
        {
            _sender.Remove(type, action);
        }

        public static void Send(Enum type, TValue0 arg0)
        {
            _sender.Send(type, arg0);
        }
    }

    public class EventManager<TValue0, TValue1> : MonoBehaviour
    {
        private static readonly EventSender<Enum, TValue0, TValue1> _sender = new EventSender<Enum, TValue0, TValue1>();

        public static void Add(Enum type, UnityAction<TValue0, TValue1> action)
        {
            _sender.Add(type, action);
        }

        public static void Remove(Enum type, UnityAction<TValue0, TValue1> action)
        {
            _sender.Remove(type, action);
        }

        public static void Send(Enum type, TValue0 arg0, TValue1 arg1)
        {
            _sender.Send(type, arg0, arg1);
        }
    }

    public class EventManager<TValue0, TValue1, TValue2> : MonoBehaviour
    {
        private static readonly EventSender<Enum, TValue0, TValue1, TValue2> _sender =
            new EventSender<Enum, TValue0, TValue1, TValue2>();

        public static void Add(Enum type, UnityAction<TValue0, TValue1, TValue2> action)
        {
            _sender.Add(type, action);
        }

        public static void Remove(Enum type, UnityAction<TValue0, TValue1, TValue2> action)
        {
            _sender.Remove(type, action);
        }

        public static void Send(Enum type, TValue0 arg0, TValue1 arg1, TValue2 arg2)
        {
            _sender.Send(type, arg0, arg1, arg2);
        }
    }

    public class EventManager<TValue0, TValue1, TValue2, TValue3> : MonoBehaviour
    {
        private static readonly EventSender<Enum, TValue0, TValue1, TValue2, TValue3> _sender =
            new EventSender<Enum, TValue0, TValue1, TValue2, TValue3>();

        public static void Add(Enum type, UnityAction<TValue0, TValue1, TValue2, TValue3> action)
        {
            _sender.Add(type, action);
        }

        public static void Remove(Enum type, UnityAction<TValue0, TValue1, TValue2, TValue3> action)
        {
            _sender.Remove(type, action);
        }

        public static void Send(Enum type, TValue0 arg0, TValue1 arg1, TValue2 arg2, TValue3 arg3)
        {
            _sender.Send(type, arg0, arg1, arg2, arg3);
        }
    }
}