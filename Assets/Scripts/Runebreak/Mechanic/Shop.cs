
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private List<UpgradeSO> _allItems;

    private void Awake()
    {
        ProcessItems();
    }

    private List<UpgradeSO> _commonItems = new();
    private List<UpgradeSO> _uncommonItems = new();
    private List<UpgradeSO> _rareItems = new();
    private List<UpgradeSO> _epicItems = new();
    private List<UpgradeSO> _legendaryItems = new();
    private void ProcessItems()
    {
        foreach (var item in _allItems)
        {
            var list = GetList(item.Rarity);
            list.Add(item);
        }
    }

    private UpgradeSO GetOneItem(Rarity rarity)
    {
        var list = GetList(rarity);
        var index = Random.Range(0, list.Count);
        return list[index];
    }

    private List<UpgradeSO> GetList(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return _commonItems;
                
            case Rarity.Uncommon:
                return _uncommonItems;
                
            case Rarity.Rare:
                return _rareItems;
                
            case Rarity.Epic:
                return _epicItems;
                
            case Rarity.Legendary:
                return _legendaryItems;
                
        }

        return null;
    }
}
