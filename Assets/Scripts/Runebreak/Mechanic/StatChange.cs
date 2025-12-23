using System;

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
        string prefix = Amount >= 0 ? "Gain" : "Lose";
        string suffix = string.Empty;
        if (Type == StatChangeType.Percent)
        {
            suffix = "%";
        }
        else if (Type == StatChangeType.Multiply)
        {
            suffix = "x";
        }

        return $"{prefix} <color=green>{Amount:N0}{suffix}</color> {Stat}";
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
        string suffix = string.Empty;
        if (ChangeType == StatChangeType.Percent)
        {
            suffix = "%";
        }
        else if (ChangeType == StatChangeType.Multiply)
        {
            suffix = "x";
        }

        return $"{prefix} <color=green>{Amount:N0}{suffix}</color> {ValueType}";
    }
}