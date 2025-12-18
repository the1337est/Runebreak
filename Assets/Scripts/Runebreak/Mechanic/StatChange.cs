using System;
using UnityEditor.UIElements;
using UnityEngine;

[System.Serializable]
public class StatChange
{
    public StatType Stat;
    public StatChangeType Type;
    public float Amount;

    public StatChange(StatType stat, float amount, StatChangeType type = StatChangeType.Flat)
    {
        Stat = stat;
        Type = type;
        Amount = amount;
    }

    public override string ToString()
    {
        return $"Gain <color=green>{Amount:N0}</color> {Stat}";
    }
}

public class GameValueChange<T> where T : Enum
{
    public T ValueType;
    public float Amount;
    public StatChangeType ChangeType;
    
    public GameValueChange(T valueType, float amount, StatChangeType type = StatChangeType.Flat)
    {
        ValueType = valueType;
        ChangeType = type;
        Amount = amount;
    }

    public override string ToString()
    {
        string prefix = Amount >= 0 ? "Gain" : "Lose";
        return $"{prefix} <color=green>{Amount:N0}</color> {ValueType}";
    }
}