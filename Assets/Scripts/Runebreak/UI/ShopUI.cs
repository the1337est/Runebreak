using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopCard _shopCardPrefab;
    [SerializeField] private Transform _cardContainer;
    [SerializeField] private Button _rerollButton;
    
    private List<ShopCard> _shopCards = new ();
    
    private GridLayoutGroup _gridLayoutGroup;
    
    [SerializeField] private TextMeshProUGUI _rerollCostText;

    private void Awake()
    {
        _gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
    }

    private void OnEnable()
    {
        _rerollButton.onClick.AddListener(HandleRerollClick);
        EventBus.Subscribe<ShopItemUpdateEvent>(HandleShopItemUpdate);
        EventBus.Subscribe<ShopCardDisposeEvent>(HandleShopCardDispose);
    }


    private void OnDisable()
    {
        EventBus.Unsubscribe<ShopItemUpdateEvent>(HandleShopItemUpdate);
        EventBus.Unsubscribe<ShopCardDisposeEvent>(HandleShopCardDispose);
    }
    
    private void HandleRerollClick()
    {
        EventBus.Publish(new ShopRerollRequestEvent());
    }

    private void HandleShopCardDispose(ShopCardDisposeEvent eventdata)
    {
        if (eventdata.Card != null && _shopCards.Contains(eventdata.Card))
        {
            _shopCards.Remove(eventdata.Card);
            Destroy(eventdata.Card.gameObject);
        }
    }

    private void HandleShopItemUpdate(ShopItemUpdateEvent eventData)
    {
        var items = eventData.Items;
        if (_shopCards.Count < items.Count)
        {
            var diff = items.Count - _shopCards.Count;
            for (int i = 0; i < diff; i++)
            {
                var card = Instantiate(_shopCardPrefab, _cardContainer);
                _shopCards.Add(card);
            }
        }

        for (int i = 0; i < _shopCards.Count; i++)
        {
            if (i < items.Count)
            {
                _shopCards[i].Set(items[i]);
            }
            else
            {
                Destroy(_shopCards[i].gameObject);
                _shopCards.RemoveAt(i);
            }
        }

        _gridLayoutGroup.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_gridLayoutGroup.transform as RectTransform);
        StartCoroutine(SelectGameObjectNextFrame(_shopCards[0]));
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Debug.Log(_shopCards.Count);
        if (_shopCards.Count > 0)
        {
            StartCoroutine(SelectGameObjectNextFrame(_shopCards[0]));
        }
    }

    private IEnumerator SelectGameObjectNextFrame(ShopCard card)
    {
        yield return null;
        if (card != null)
        {
            card.Select();
        }

        if (_gridLayoutGroup)
        {
            _gridLayoutGroup.enabled = false;
        }
    }

    public void SetRerollCost(int cost)
    {
        var coins = Player.Instance.Stats.Get(StatType.Coins);
        _rerollButton.interactable = coins >= cost;
        _rerollCostText.text = cost.ToString("N0");
    }

    public void SyncCards()
    {
        var coins = Player.Instance.Stats.Get(StatType.Coins);
        foreach (var card in _shopCards)
        {
            card.UpdateInteractability(coins);
        }
    }
}
