using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

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
        Debug.Log("Shop Item Update received");
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
