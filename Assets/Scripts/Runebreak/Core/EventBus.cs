using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    // Key: message type, Value: multicast delegate (Action<T> boxed as Delegate)
    private static readonly Dictionary<Type, Delegate> _handlers = new();

    public static void Subscribe<T>(Action<T> handler) where T: IGameEvent
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var existing))
            _handlers[type] = (Action<T>)existing + handler;
        else
            _handlers[type] = handler;
    }

    public static void Unsubscribe<T>(Action<T> handler) where T: IGameEvent
    {
        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var existing)) return;

        var updated = (Action<T>)existing - handler;
        if (updated == null) _handlers.Remove(type);
        else _handlers[type] = updated;
    }

    public static void Publish<T>(T message) where T: IGameEvent
    {
        if (message is not TimerUpdateEvent)
        {
            Debug.Log($"[EventBus] Published: {typeof(T).Name}, message: {message}");
        }
        if (_handlers.TryGetValue(typeof(T), out var del))
            ((Action<T>)del)?.Invoke(message);
    }

    public static void ClearAll() => _handlers.Clear();
}
