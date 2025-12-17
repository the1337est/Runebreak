using System;
using TMPro;
using UnityEngine;

public class StatEntry : MonoBehaviour
{
    [SerializeField] private StatType _statType;
    
    [SerializeField] private TextMeshProUGUI _statName;
    [SerializeField] private TextMeshProUGUI _statValue;

    private void Awake()
    {
        EventBus.Subscribe<PlayerStatChangeEvent>(HandlePlayerStatChange);
        _statName.text = _statType.ToString();
    }
    
    private void OnDestroy()
    {
        EventBus.Unsubscribe<PlayerStatChangeEvent>(HandlePlayerStatChange);
    }

    private void HandlePlayerStatChange(PlayerStatChangeEvent eventData)
    {
        if (eventData.Change.Stat != _statType) return;
        var amount = Player.Instance.Stats.Get(_statType);
        _statValue.text = amount.ToString("N0");
    }
}
