using System;
using System.Collections.Generic;

public abstract class GameValue<T> where T : Enum
{
    private readonly Dictionary<T, float> _map = new();
    
    public float Get(T valueType)
    {
        return _map.GetValueOrDefault(valueType, 0);
    }
    
    public void Set(T valueType, float value)
    {
        _map[valueType] = value;
        var data = new GameValueChange<T>(valueType, value);
        EventBus.Publish(new PlayerGameValueChangeEvent<T>(data));
    }
}
