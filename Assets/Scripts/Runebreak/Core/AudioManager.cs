using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _sourcePrefab;
    [SerializeField] private int _poolSize = 16;

    private List<AudioSource> _availableSources;
    private List<AudioSource> _busySources;
    
    private AudioSource _mainSource;
    
    private void Awake()
    {
        // SetupAudioSoucePool();
        _mainSource = GetComponent<AudioSource>();
    }

    [SerializeField] private List<AudioClip> _coinCollectClips;
    [SerializeField] private List<AudioClip> _killClips;

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyDeathEvent>(HandleEnemyDeath);
        EventBus.Subscribe<PickupEvent>(HandlePickup);
    }

    private void HandlePickup(PickupEvent eventData)
    {
        var random = _coinCollectClips[Random.Range(0, _coinCollectClips.Count)];
        _mainSource.PlayOneShot(random);
    }

    private void HandleEnemyDeath(EnemyDeathEvent eventData)
    {
        var random = _killClips[Random.Range(0, _killClips.Count)];
        _mainSource.PlayOneShot(random);
    }

    private void SetupAudioSoucePool()
    {
        _availableSources = new List<AudioSource>();
        _busySources = new List<AudioSource>();
        
        for (int i = 0; i < _poolSize; i++)
        {
            var source = Instantiate(_sourcePrefab, transform);
            _availableSources.Add(source);
        }
    }

    private AudioSource GetAvailableSource()
    {
        AudioSource source = null;
        if (_availableSources.Count == 0)
        {
            source = Instantiate(_sourcePrefab, transform);
            _availableSources.Add(source);
        }
        
        return source;
    }

    private void PlayAudioOnSource(AudioSource audioSource, AudioClip clip)
    {
        if (_availableSources.Contains(audioSource))
        {
            _availableSources.Remove(audioSource);
            _busySources.Add(audioSource);
        }
        audioSource.PlayOneShot(clip);
    }
}
