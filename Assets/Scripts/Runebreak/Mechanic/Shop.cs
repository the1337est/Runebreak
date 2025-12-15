
using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Shop : Interactable
{
    [SerializeField] private int _itemCount = 4;
    [SerializeField] private GameObject _shopUI;
    
    [Header("Items")]
    [SerializeField] private List<UpgradeSO> _allItems;
    
    private List<UpgradeSO> _activeItems = new();
    private List<UpgradeSO> _commonItems = new();
    private List<UpgradeSO> _uncommonItems = new();
    private List<UpgradeSO> _rareItems = new();
    private List<UpgradeSO> _epicItems = new();
    private List<UpgradeSO> _legendaryItems = new();
    
    private void Start()
    {
        ProcessItems();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Subscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Subscribe<WaveStartEvent>(HandleWaveStart);
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Unsubscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Unsubscribe<WaveStartEvent>(HandleWaveStart);
    }

    private void HandleWaveEnd(WaveEndEvent obj)
    {
        InteractionAllowed = true;
    }
    
    private void HandleWaveStart(WaveStartEvent obj)
    {
        InteractionAllowed = false;
    }
    
    private void ProcessItems()
    {
        foreach (var item in _allItems)
        {
            var list = GetList(item.Rarity);
            list.Add(item);
        }
    }

    private UpgradeSO GetRandomItem()
    {
        var index = Random.Range(0, _allItems.Count);
        return _allItems[index];
    }
    
    private UpgradeSO GetRandomItem(Rarity rarity)
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
    
    public override void Interact()
    {
        FetchShopItems();
    }

    private void FetchShopItems()
    {
        _activeItems.Clear();
        for (int i = 0; i < _itemCount; i++)
        {
            var item = GetRandomItem();
            _activeItems.Add(item);
        }
        _shopUI.SetActive(true);
        EventBus.Publish(new ShopItemUpdateEvent(_activeItems));
    }
}
