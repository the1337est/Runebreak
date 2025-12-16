using System.Collections.Generic;
using UnityEngine;

public class ShopOpenEvent : IGameEvent { }

public class ShopCloseEvent : IGameEvent { }

public class ShopBuyEvent : IGameEvent
{
    public UpgradeSO Item;

    public ShopBuyEvent(UpgradeSO item)
    {
        Item = item;
    }
}

public class ShopCardDisposeEvent : IGameEvent
{
    public ShopCard Card;

    public ShopCardDisposeEvent(ShopCard card)
    {
        Card = card;
    }
}


public class ShopItemUpdateEvent : IGameEvent
{
    public List<UpgradeSO> Items;

    public ShopItemUpdateEvent(List<UpgradeSO> items)
    {
        Items = items;
    }
}

public class ShopRerollRequestEvent : IGameEvent
{
}

public class ShopCoinsSpentEvent : IGameEvent
{
    public int Coins;

    public ShopCoinsSpentEvent(int coins)
    {
        Coins = coins;
    }
}