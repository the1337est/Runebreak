using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDeathEvent : IGameEvent { }

public class PlayerHitEvent : IGameEvent { }

public class PlayerStatChangeEvent : IGameEvent
{
    public StatChange Change;
    public StatType Stat => Change.Stat;
    public StatChangeType Type => Change.Type;
    public float Amount => Change.Amount;
    
    public PlayerStatChangeEvent(StatChange change)
    {
        Change = change;
    }

    public override string ToString()
    {
        return $"{Change.Stat} {Change.Type} {Change.Amount}";
    }
}

