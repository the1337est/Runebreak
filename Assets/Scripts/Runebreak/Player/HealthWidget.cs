using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthWidget : MonoBehaviour
{
    private Slider _slider; 
    [SerializeField] private TextMeshProUGUI _hpText;

    private float _maxHPCache;
    private float _hpCache;
    
    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerGameValueChangeEvent<StatType>>(HandleStatChange);
        EventBus.Subscribe<PlayerGameValueChangeEvent<ResourceType>>(HandleResourceChange);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerGameValueChangeEvent<StatType>>(HandleStatChange);
        EventBus.Unsubscribe<PlayerGameValueChangeEvent<ResourceType>>(HandleResourceChange);
    }

    private void HandleResourceChange(PlayerGameValueChangeEvent<ResourceType> eventData)
    {
        if (eventData.ValueType != ResourceType.HP) return;
        _hpCache = eventData.Amount;
        RefreshText();
    }
    
    private void HandleStatChange(PlayerGameValueChangeEvent<StatType> eventData)
    {
        if (eventData.ValueType != StatType.MaxHP) return;
        _maxHPCache = eventData.Amount;
        RefreshText();
    }

    private void RefreshText()
    {
        if (_maxHPCache <= 0) return;
        _slider.value = Mathf.Clamp01(_hpCache / _maxHPCache);
        _hpText.text = $"{_hpCache:N0} / {_maxHPCache:N0}";
    }
}
