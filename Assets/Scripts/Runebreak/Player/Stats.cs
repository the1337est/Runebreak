using System;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Stats
{
    private readonly Dictionary<StatType, float> _base = new();

    public float Get(StatType stat)
    {
        return _base.GetValueOrDefault(stat, 0);
    }
    
    public void Set(StatType stat, float value)
    {
        _base[stat] = value;
        EventBus.Publish(new PlayerStatChangeEvent(stat, value));
    }

    public void Update(PlayerStatChangeEvent change)
    {
        switch (change.ChangeType)
        {
            case StatChangeType.Flat:
                Set(change.Stat, change.Value); 
                break;
            
            case StatChangeType.Percent:
                
                break;
            
            case StatChangeType.Multiply:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
