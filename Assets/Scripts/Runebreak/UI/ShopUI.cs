using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopCard _shopCardPrefab;
    [SerializeField] private Transform _cardContainer;
    
    private List<ShopCard> _shopCards = new ();
    
    private void OnEnable()
    {
        EventBus.Subscribe<ShopItemUpdateEvent>(HandleShopItemUpdate);
    }
    
    private void OnDisable()
    {
        EventBus.Subscribe<ShopItemUpdateEvent>(HandleShopItemUpdate);
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
    }
}
