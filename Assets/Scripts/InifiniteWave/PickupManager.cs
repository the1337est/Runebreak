using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PickupManager : MonoBehaviour
{
    private Transform _player;
    
    [Header("Prefabs")]
    [SerializeField] private Pickup _coinPrefab;

    [SerializeField] private float _magnetRadius = 2f;
    [SerializeField] private float _collectRadius = 0.2f;
    [SerializeField] float _checkInterval = 0.1f;
    
    [SerializeField] private List<Pickup> _pickups;
    private float _nextCheckTime;

    public static PickupManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _player = FindFirstObjectByType<Player>().transform;
        _pickups = new List<Pickup>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyDeathEvent>(HandleEnemyDeath);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyDeathEvent>(HandleEnemyDeath);
    }

    private void HandleEnemyDeath(EnemyDeathEvent eventData)
    {
        SpawnCoin(eventData.Position, eventData.Coins);
    }

    public void Register(Pickup pickup)
    {
        _pickups.Add(pickup);
    }
    
    public void Unregister(Pickup pickup)
    {
        if(_pickups.Contains(pickup)) _pickups.Remove(pickup);
    }

    private void SpawnCoin(Vector3 position, int count)
    {
        //todo: do something with count: either spread multiple pickups, or spawn a bigger pickup
        var coin = Instantiate(_coinPrefab, position, Quaternion.identity);
        coin.transform.SetParent(transform);
        Register(coin);
    }

    private void Update()
    {
        if (!_player) return;
        if (Time.time < _nextCheckTime) return;
        _nextCheckTime = Time.time + _checkInterval;

        Vector3 playerPos = _player.position;
        float magnetRadiusSqr = _magnetRadius * _magnetRadius;
        
        for (int i = 0; i < _pickups.Count; i++)
        {
            var pickup = _pickups[i];
            if (!pickup || pickup.IsFlying) continue;

            Vector3 diff = pickup.Position - playerPos;
            if (diff.sqrMagnitude <= magnetRadiusSqr)
            {
                pickup.BeginFlyTo(_player, _collectRadius);
            }
        }
    }
}
