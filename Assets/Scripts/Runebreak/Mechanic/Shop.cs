
using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<UpgradeSO> _allItems;
    [SerializeField] private float _activationRange = 4f;

    private Transform _target;

    [SerializeField] private GameObject _enabledShop;
    [SerializeField] private GameObject _disabledShop;
    [SerializeField] private GameObject _shopUI;

    private bool _shopIsOpen;
    private bool _withinRange;
    
    private InputActions _inputActions => GameManager.Instance.InputActions;
    private InputAction _interactInput => _inputActions.Player.Interact;


    private int _itemCount = 4;
    private List<UpgradeSO> _activeItems = new();
    
    private List<UpgradeSO> _commonItems = new();
    private List<UpgradeSO> _uncommonItems = new();
    private List<UpgradeSO> _rareItems = new();
    private List<UpgradeSO> _epicItems = new();
    private List<UpgradeSO> _legendaryItems = new();
    
    private void Awake()
    {
        ProcessItems();
        _target = Player.Instance.transform;
    }
    
    private void GenerateShopItems()
    {
        _activeItems.Clear();
        for (int i = 0; i < 3; i++)
        {
            var item = GetRandomItem();
            _activeItems.Add(item);
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Subscribe<WaveStartEvent>(HandleWaveStart);
        
        _interactInput.performed += HandleInteract;
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Unsubscribe<WaveStartEvent>(HandleWaveStart);
        
        _interactInput.performed -= HandleInteract;
    }

    private void HandleWaveEnd(WaveEndEvent obj)
    {
        _shopIsOpen = true;
    }
    
    private void HandleWaveStart(WaveStartEvent obj)
    {
        _shopIsOpen = false;
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
    
    private void Update()
    {
        if (!_shopIsOpen) return;
        ProximityUpdate();
    }
    
    private void ProximityUpdate()
    {
        var pos = transform.position;
        var toTarget = _target.position - pos;
        float dist = toTarget.magnitude;
        
        _withinRange = dist <= _activationRange;
        
        _enabledShop.SetActive(_withinRange);
        _disabledShop.SetActive(!_withinRange);
    }

    private void HandleInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("Performed");
        if (!_withinRange) return;
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
