using System;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class CoinsWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinsText;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerStatChangeEvent>(HandlePlayerCoinChange);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerStatChangeEvent>(HandlePlayerCoinChange);
    }

    private void Awake()
    {
        SetCoinsText(Player.Instance.Stats.Get(StatType.Coins));
    }

    private void HandlePlayerCoinChange(PlayerStatChangeEvent eventData)
    {
        if (eventData.Stat != StatType.Coins) return;
        SetCoinsText(eventData.Value);
    }

    private void SetCoinsText(float value)
    {
        _coinsText.text = value.ToString("N0");
    }

}
