using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthWidget : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _hpText;

    private float _maxHPCache;
    private float _hpCache;
    
    protected bool IsFullHealth => Mathf.Approximately(_hpCache, _maxHPCache);

    protected virtual void Awake()
    {
        EventBus.Subscribe<PlayerGameValueChangeEvent<StatType>>(HandleStatChange);
        EventBus.Subscribe<PlayerGameValueChangeEvent<ResourceType>>(HandleResourceChange);
    }

    protected virtual void OnDestroy()
    {
        EventBus.Unsubscribe<PlayerGameValueChangeEvent<StatType>>(HandleStatChange);
        EventBus.Unsubscribe<PlayerGameValueChangeEvent<ResourceType>>(HandleResourceChange);
    }

    protected virtual void HandleResourceChange(PlayerGameValueChangeEvent<ResourceType> eventData)
    {
        if (eventData.ValueType != ResourceType.HP) return;
        _hpCache = eventData.Amount;
        RefreshText();
    }
    
    protected virtual void HandleStatChange(PlayerGameValueChangeEvent<StatType> eventData)
    {
        if (eventData.ValueType != StatType.MaxHP) return;
        _maxHPCache = eventData.Amount;
        RefreshText();
    }

    private void RefreshText()
    {
        if (_maxHPCache <= 0) return;
        if(_slider == null) return;
        _slider.value = Mathf.Clamp01(_hpCache / _maxHPCache);
        _hpText.text = $"{_hpCache:N0} / {_maxHPCache:N0}";
    }
}
