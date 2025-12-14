using UnityEngine;

public class PlayerDeathEvent : IGameEvent { }

public class PlayerStatChangeEvent : IGameEvent
{
    public StatType Stat;
    public StatChangeType ChangeType;

    public float Value;
    
    public PlayerStatChangeEvent(StatType stat, float value, StatChangeType changeType = StatChangeType.Flat)
    {
        Stat = stat;
        ChangeType = changeType;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Stat} {ChangeType} {Value}";
    }
}

