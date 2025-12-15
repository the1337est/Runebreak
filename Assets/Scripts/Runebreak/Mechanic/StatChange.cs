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
