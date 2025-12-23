using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PickupManager : MonoBehaviour
{
    private Transform _player;

    [Header("Prefabs")] [SerializeField] private Pickup _coinPrefab;

    private float _magnetRadius;
    private float _startFlySpeed;
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
        EventBus.Subscribe<PlayerGameValueChangeEvent<StatType>>(HandleStatChange);
        
        EventBus.Publish(new PlayerGameValueRequestEvent<StatType>(StatType.PickupRange));
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyDeathEvent>(HandleEnemyDeath);
        EventBus.Unsubscribe<PlayerGameValueChangeEvent<StatType>>(HandleStatChange);
    }

    private void HandleEnemyDeath(EnemyDeathEvent eventData)
    {
        SpawnCoin(eventData.Position, eventData.Coins);
    }

    private void HandleStatChange(PlayerGameValueChangeEvent<StatType> eventData)
    {
        if(eventData.ValueType == StatType.PickupRange) SetMagnetRadius(eventData.Amount);
        if(eventData.ValueType == StatType.Speed) SetStartFlySpeed(eventData.Amount);
    }

    private void SetMagnetRadius(float radius)
    {
        _magnetRadius = radius;
    }
    
    private void SetStartFlySpeed(float amount)
    {
        _startFlySpeed = amount;
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
        var coin = Instantiate(_coinPrefab, position, Quaternion.identity);
        coin.Init(count);
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
                pickup.BeginFlyTo(_player, _collectRadius, _startFlySpeed);
            }
        }
    }
}
