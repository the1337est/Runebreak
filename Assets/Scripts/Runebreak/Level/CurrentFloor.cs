using System;
using UnityEngine;

public class CurrentFloor : Floor
{
    private bool _allowFade;
    
    private void OnEnable()
    {
        EventBus.Subscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Subscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Subscribe<PlayerSpawnEvent>(HandlePlayerSpawn);
    }

    private void HandleWaveStart(WaveStartEvent obj)
    {
        _allowFade = false;
    }
    
    private void HandleWaveEnd(WaveEndEvent obj)
    {
        _allowFade = true;
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerSpawnEvent>(HandlePlayerSpawn);
    }

    private void HandlePlayerSpawn(PlayerSpawnEvent eventData)
    {
        Target = eventData.Player.transform;
    }

    private void Update()
    {
        if (Target == null) return;
        float halfWidth = WorldSize.x / 2f;
        float quarterWidth = WorldSize.x / 4f; 
        
        float fadeStartX = transform.position.x + halfWidth;
        float fadeEndX = fadeStartX + quarterWidth;
        
        float t = Mathf.InverseLerp(fadeStartX, fadeEndX, Target.position.x);
        float alpha = 1f - t;

        Color color = FloorRenderer.color;
        color.a = alpha;
        SetColor(color);
    }
}
