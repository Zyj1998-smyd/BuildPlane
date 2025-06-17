using UnityEngine.Events;

namespace Common.Event
{
    public class  EventHandler : UnityEvent
    {}

    public class EventHandler<TValue0> : UnityEvent<TValue0>
    {}

    public class EventHandler<TValue0, TValue1> : UnityEvent<TValue0, TValue1>
    {}

    public class EventHandler<TValue0, TValue1, TValue2> : UnityEvent<TValue0, TValue1, TValue2>
    {}

    public class EventHandler<TValue0, TValue1, TValue2, TValue3> : UnityEvent<TValue0, TValue1, TValue2, TValue3>
    {}
}