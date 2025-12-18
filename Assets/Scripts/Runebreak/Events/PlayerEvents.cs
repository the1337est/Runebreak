using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDeathEvent : IGameEvent { }

public class PlayerHitEvent : IGameEvent { }
public class PlayerGameValueChangeEvent<T> : IGameEvent where T : Enum
{
    private GameValueChange<T> _change;
    public T ValueType => _change.ValueType;
    public StatChangeType Type => _change.ChangeType;
    public float Amount => _change.Amount;
    
    public PlayerGameValueChangeEvent(GameValueChange<T> change)
    {
        _change = change;
    }

    public override string ToString()
    {
        return $"{ValueType} {Type} {Amount}";
    }
}

public class PlayerGameValueRequestEvent<T> : IGameEvent where T : Enum
{
    public T ValueType;

    public PlayerGameValueRequestEvent(T valueType)
    {
        ValueType = valueType;
    }
}
        

