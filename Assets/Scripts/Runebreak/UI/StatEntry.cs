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
        EventBus.Subscribe<PlayerGameValueChangeEvent<StatType>>(HandlePlayerStatChange);
        EventBus.Publish(new PlayerGameValueRequestEvent<StatType>(_statType));
        _statName.text = _statType.ToString();
    }
    
    private void OnDestroy()
    {
        EventBus.Unsubscribe<PlayerGameValueChangeEvent<StatType>>(HandlePlayerStatChange);
    }

    private void HandlePlayerStatChange(PlayerGameValueChangeEvent<StatType> eventData)
    {
        if (eventData.ValueType != _statType) return;
        _statValue.text = eventData.Amount.ToString("N0");
    }
}
