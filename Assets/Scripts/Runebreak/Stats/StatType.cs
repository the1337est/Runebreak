using UnityEngine;

public enum StatType
{
    None,
    MaxHP,
    Speed,
    Range,
    AttackSpeed,
    Damage,
    Armour,
    Dodge,
    HPRegen,
    LifeSteal,
    Collect,
    PickupRange,
    DamageOnCollect,
}

public enum StatChangeType
{
    Flat,
    Percent,
    Multiply,
}
