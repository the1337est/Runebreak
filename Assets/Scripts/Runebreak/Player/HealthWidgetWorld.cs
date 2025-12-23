using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthWidgetWorld : HealthWidget
{

    private bool _show = false;

    protected override void Awake()
    {
        base.Awake();
        EventBus.Subscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Subscribe<WaveStartEvent>(HandleWaveStart);
        gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.Unsubscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Unsubscribe<WaveStartEvent>(HandleWaveStart);
    }

    private void HandleWaveEnd(WaveEndEvent obj)
    {
        _show = false;
        gameObject.SetActive(false);
    }
    
    private void HandleWaveStart(WaveStartEvent obj)
    {
        _show = true;
    }

    protected override void HandleResourceChange(PlayerGameValueChangeEvent<ResourceType> eventData)
    {
        base.HandleResourceChange(eventData);
        if (!_show) return;
        if (eventData.ValueType != ResourceType.HP) return;
        gameObject.SetActive(true);
    }
}