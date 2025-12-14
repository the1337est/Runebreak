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
        EventBus.Subscribe<PlayerStatChangeEvent>(HandleStatChange);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerStatChangeEvent>(HandleStatChange);
    }

    private void HandleStatChange(PlayerStatChangeEvent eventData)
    {
        if (eventData.Stat == StatType.HP)
        {
            _hpCache = eventData.Value;
        }
        else if (eventData.Stat == StatType.MaxHP)
        {
            _maxHPCache = eventData.Value;
        }
        else
        {
            return;
        }

        if (_maxHPCache <= 0) return;
        _slider.value = Mathf.Clamp01(_hpCache / _maxHPCache);
        _hpText.text = $"{_hpCache:N0} / {_maxHPCache:N0}";
    }
}
