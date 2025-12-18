using System;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class CoinsWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinsText;

    private void Awake()
    {
        EventBus.Subscribe<PlayerGameValueChangeEvent<ResourceType>>(HandleResourceChange);
        SetCoinsText(Player.Instance.Resources.Get(ResourceType.Coins));
    }

    private void HandleResourceChange(PlayerGameValueChangeEvent<ResourceType> eventData)
    {
        if (eventData.ValueType != ResourceType.Coins) return;
        SetCoinsText(eventData.Amount);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<PlayerGameValueChangeEvent<ResourceType>>(HandleResourceChange);
    }

    private void SetCoinsText(float value)
    {
        _coinsText.text = value.ToString("N0");
    }
}
