using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Shop : Interactable
{
    [SerializeField] private int _itemCount = 4;
    [SerializeField] private ShopUI _shopUI;
    
    [Header("Items")]
    [SerializeField] private List<UpgradeSO> _allItems;
    
    private List<UpgradeSO> _activeItems = new();
    private List<UpgradeSO> _commonItems = new();
    private List<UpgradeSO> _uncommonItems = new();
    private List<UpgradeSO> _rareItems = new();
    private List<UpgradeSO> _epicItems = new();
    private List<UpgradeSO> _legendaryItems = new();

    private int _rerollCost = 1;
    private bool _nextRerollFree = false;

    private void Start()
    {
        ProcessItems();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Subscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Subscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Subscribe<ShopRerollRequestEvent>(HandleShopReroll);
        EventBus.Subscribe<ShopBuyEvent>(HandleShopBuy);
        EventBus.Subscribe<PlayerGameValueChangeEvent<ResourceType>>(HandleResourceChange);
    }

    

    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Unsubscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Unsubscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Unsubscribe<ShopRerollRequestEvent>(HandleShopReroll);
        EventBus.Unsubscribe<PlayerGameValueChangeEvent<ResourceType>>(HandleResourceChange);
    }

    private void HandleWaveEnd(WaveEndEvent obj)
    {
        InteractionAllowed = true;
    }
    
    private void HandleWaveStart(WaveStartEvent eventData)
    {
        InteractionAllowed = false;
        _rerollCost = eventData.WaveIndex;
        GenerateNewItems();
    }
    
    private void HandleResourceChange(PlayerGameValueChangeEvent<ResourceType> eventData)
    {
        if (eventData.ValueType != ResourceType.Coins) return;
        _shopUI.Refresh((int)eventData.Amount, _rerollCost);
    }

    private void ProcessItems()
    {
        foreach (var item in _allItems)
        {
            var list = GetList(item.Rarity);
            list.Add(item);
        }
    }
    
    private void HandleShopBuy(ShopBuyEvent eventData)
    {
        StartCoroutine(NextFrameShopCheck());
    }

    private IEnumerator NextFrameShopCheck()
    {
        yield return new WaitForEndOfFrame();
        if (_shopUI == null) yield break;
        if (_shopUI.Count <= 0)
        {
            _nextRerollFree = true;
            var coins = Player.Instance.Resources.Get(ResourceType.Coins);
            _shopUI.Refresh((int)coins, 0);
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
        OpenShop(); 
    }
    
    public override void UIBack()
    {
        if(!IsActive) return;
        IsActive = false;
        EventSystem.current.SetSelectedGameObject(null);
        _shopUI.gameObject.SetActive(false);
    }

    private void HandleShopReroll(ShopRerollRequestEvent eventData)
    {
        if (!_nextRerollFree)
        {
            EventBus.Publish(new ShopCoinsSpentEvent(_rerollCost));
            _rerollCost += _rerollCost;
        }
        else
        {
            _nextRerollFree = false;
            EventBus.Publish(new ShopCoinsSpentEvent(0));
        }
        GenerateNewItems();
    }
    
    private void GenerateNewItems()
    {
        _activeItems.Clear();
        for (int i = 0; i < _itemCount; i++)
        {
            var item = GetRandomItem();
            _activeItems.Add(item);
        }
        EventBus.Publish(new ShopItemUpdateEvent(_activeItems));
    }

    private void OpenShop()
    {
        IsActive = true;
        _shopUI.Open();
        var cost = _nextRerollFree ? 0 : _rerollCost;
        _shopUI.Refresh((int)Player.Instance.Resources.Get(ResourceType.Coins), cost);
    }
}
