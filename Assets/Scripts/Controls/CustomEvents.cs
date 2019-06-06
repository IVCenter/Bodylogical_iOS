using UnityEngine.Events;
using System;

public class CustomEvents {
    [Serializable]
    public class IntEvent : UnityEvent<int> { }

    [Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    [Serializable]
    public class FloatEvent : UnityEvent<float> { }
}