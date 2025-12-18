using UnityEngine;

public enum StatType
{
    None,
    MaxHP,
    Speed,
    AttackSpeed,
    Damage,
    Armour,
    Dodge,
    HPRegen,
    LifeSteal,
    Collect,
    CollectRadius,
    DamageOnCollect,
}

public enum StatChangeType
{
    Flat,
    Percent,
    Multiply,
}
