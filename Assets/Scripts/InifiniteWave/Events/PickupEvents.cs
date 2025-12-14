using UnityEngine;

public class PickupEvent : IGameEvent
{
    public PickupType PickupType;
    public int BaseAmount;

    public PickupEvent(PickupType pickupType, int baseAmount)
    {
        PickupType = pickupType;
        BaseAmount = baseAmount;
    }
}

public enum PickupType
{
    Coin,
    Health,
    LootBox
}