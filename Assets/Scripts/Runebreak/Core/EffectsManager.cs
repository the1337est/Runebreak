using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EffectsManager : MonoBehaviour
{
    [SerializeField] private Image _hitImage;

    private bool _active; 
    
    private void OnEnable()
    {
        EventBus.Subscribe<PlayerHitEvent>(HandlePlayerHit);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerHitEvent>(HandlePlayerHit);
    }

    private void HandlePlayerHit(PlayerHitEvent eventData)
    {
        if(_active) return;
        _active = true;
        _hitImage.DOFade(0f, 0f);
        _hitImage.DOFade(0.1f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _hitImage.DOFade(0f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                _active = false;
            });
        });
    }
}
